using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using gD = gestureData;

namespace Softwareprojekt
{
    class Program
    {
        static void noMain(string[] args) {
            String s =" 1 ";
            int a = Int32.Parse(s);
            Console.WriteLine(a);
        }
        static void Main(string[] args)
        {
            //tabKlein.csv oder KinectDaten_Pascal.csv
            String path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/tabKlein.csv";
            //shapeID: sortiert
            //gestureID: sortiert

            trainDataReader TDReader = new trainDataReader(path);
            TDReader.readData();
            
        }
    }


        class trainDataReader {

        private String _path;

        public trainDataReader(String path) => _path = path;

        public void readData() {
            using(var reader = new StreamReader(@_path))
             {
                 //ab hier nochmal ändern -> für jede spalter eine Liste
                 //FrameID;X;Y;Z;GestureID;SqlTime;Alpha;Beta;Velocity;ShapeId;UID
                 // wichtig:ShapeID -> GestureID -> (FrameID, X, Y)
                 //Gestures sind alle zusammen, Punkte nie verteilt
                gestureData.trainingData td = new gestureData.trainingData();

                String head = reader.ReadLine();

                List<gD.trainingData.Shape> currentShape = new List<gD.trainingData.Shape>();
                List<gD.trainingData.Shape.Gesture> currentGesture = new List<gD.trainingData.Shape.Gesture>();

                int currentPointID;
                float currentPointX;
                float currentPointY;
                float currentPointZ;
                int currentGestureID;
                int currentShapeID;

                Boolean[] shapesCreated = new Boolean[20+1]; //merken welche shapes es schon gibt
                Boolean[] isShape3D = new Boolean[20+1]; //alle mit false initialisiert,scheiss auf die 0
                isShape3D[14] = true; 
                isShape3D[15] = true; //die Spiralen

                int prevGestureShapeID = int.MinValue; //in prevSHapeID umbenennen?
                int prevGestureID = int.MinValue;
                gD.trainingData.Shape.Gesture prevGesture = new gD.trainingData.Shape.Gesture();

                do {
                    String line = reader.ReadLine();
                    string[] lineArray = line.Split(';');

                    currentPointID = Int32.Parse(lineArray[0]);
                    currentPointX = float.Parse(lineArray[1]);
                    currentPointY = float.Parse(lineArray[2]);
                    currentPointZ = float.Parse(lineArray[3]);
                    currentGestureID = Int32.Parse(lineArray[4]);
                    currentShapeID = Int32.Parse(lineArray[9]);

                    //falls neue Geste != der bisherigen und nicht 
                    if(prevGestureID != currentGestureID && prevGestureID!=int.MinValue) {
                        //prevGesture in zugehöriges shape einfügen
                        //neue Gesture anlegen
                        //prevGesture durch neue Gesture ersetzen

                        if(!shapesCreated[currentShapeID]) {
                            //shape gibts noch nicht -> erstellen
                            td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, isShape3D[currentShapeID]));
                        }

                        Console.WriteLine(prevGestureID);
                        td.getShape(prevGestureShapeID).addGesture(prevGesture);

                        prevGesture = new gD.trainingData.Shape.Gesture();
                    }

                    //Add Point to current Gesture
                    if(isShape3D[currentShapeID]) {       //Shape is 3D
                        prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY, currentPointZ));
                    } else {                                //Shape is 2D
                        prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY));
                    }

                    //update pevVariablen
                    prevGestureShapeID = currentShapeID; //was muss passieren wenn anderes shape?
                    prevGestureID = currentGestureID;

            
                }while (!reader.EndOfStream);

                //add last Gesture
                //td.getShape(prevGestureShapeID).addGesture(prevGesture);



                
             }
        }
    }

}