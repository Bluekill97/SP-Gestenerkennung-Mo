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
            //String path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/tabKlein.csv";
            String path;
            if(false) path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/tabKlein.csv"; else 
                path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/KinectDaten_Pascal.csv";
            trainDataReader TDReader = new trainDataReader(path);
            gestureData.trainingData trainData = TDReader.readData();

            //nur der Test, ob alle Punkte eingelesen wurde
            //unwichtig, kann man löschen
            /*Console.WriteLine("Daten eingelesen, beginne checken");
            Boolean correct = true; //checkTD(trainData);
            //Boolean correct = false;
            if(correct) Console.WriteLine("Daten korrekt");
            else Console.WriteLine("Nöp");*/
            
        }

        
        static private Boolean checkTD(gestureData.trainingData td) {
            Boolean isCorrect = true;
            String _path = "C:/Users/Asus/Documents/VS Code/Softwareprojekt/data/tabKlein.csv";
            using(var reader = new StreamReader(@_path)) {
                 String head = reader.ReadLine();

                int currentPointID;
                float currentPointX;
                float currentPointY;
                float currentPointZ;
                int currentGestureID;
                int currentShapeID;

                //count points
                int q=0;
                for(int k=0; k<23; k++) {
                    if(td.getShape(k)!=null) {
                    List<gD.trainingData.Shape.Gesture> a = td.getShape(k).getGestures();
                    foreach(gD.trainingData.Shape.Gesture l in a) {
                        List<gD.trainingData.Shape.Gesture.Point> b = l.getPoints();
                        foreach(gD.trainingData.Shape.Gesture.Point p in b) {
                         //Console.WriteLine(p.ToString());
                         q++;
                        }
                    }
                }
                }
                Console.WriteLine(q+" Punkte");

                while (!reader.EndOfStream){    
                    String line = reader.ReadLine();
                    string[] lineArray = line.Split(';');
                    currentPointID = Int32.Parse(lineArray[0]);
                    currentPointX = float.Parse(lineArray[1]);
                    currentPointY = float.Parse(lineArray[2]);
                    currentPointZ = float.Parse(lineArray[3]);
                    currentGestureID = Int32.Parse(lineArray[4]);
                    currentShapeID = Int32.Parse(lineArray[9]);

                    gD.trainingData.Shape s = td.getShape(currentShapeID);
                    List<gD.trainingData.Shape.Gesture> gestures = s.getGestures();

                    for (int i = 0; i < gestures.Count; i++) {
                        gD.trainingData.Shape.Gesture g = gestures[i];
                        if(g.getGestureID() == currentGestureID) {
                            //punkt checken
                            List<gD.trainingData.Shape.Gesture.Point> points = g.getPoints();
                            for(int j = 0; j < points.Count; j++) {
                                gD.trainingData.Shape.Gesture.Point p = points[j];
                                if(p.ID == currentPointID) break;
                                if(j == points.Count-1) isCorrect = false;
                            }
                            break;
                        }
                    }
                }


                 return isCorrect;
            }
            
        } 
    }


        class trainDataReader {

        private String _path;

        public trainDataReader(String path) => _path = path;

        public gestureData.trainingData readData() {
            using(var reader = new StreamReader(@_path))
             {
                 //ab hier nochmal ändern -> für jede spalte eine Liste
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

                Boolean[] shapesCreated = new Boolean[22+1]; //merken welche shapes es schon gibt, eig unnötig
                Boolean[] isShape3D = new Boolean[22+1]; //alle mit false initialisiert,scheiss auf die 0
                //isShape3D[14] = true; 
                //isShape3D[15] = true; //die Spiralen
                //Problem: in trainingData.cs variable is2D, hier aber is3D übergeben

                int prevShapeID = int.MinValue; //in prevSHapeID umbenennen?
                int prevGestureID = int.MinValue;
                gD.trainingData.Shape.Gesture prevGesture = new gD.trainingData.Shape.Gesture(prevGestureID);

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

                        //falls noch kein neues Shape besteht -> anlegen
                        if(!shapesCreated[prevShapeID]) {//if(!shapesCreated[currentShapeID]) {
                            //Console.WriteLine(currentShapeID + " Shape hinzugefügt");
                            //td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, isShape3D[currentShapeID])); //für 3D, unfertig
                            td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, true));
                            shapesCreated[prevShapeID] = true;
                        }

                        td.getShape(prevShapeID).addGesture(prevGesture);

                        //prevGesture durch neue, leere gesture ersetzen
                        prevGesture = new gD.trainingData.Shape.Gesture(currentGestureID);
                        prevGestureID = currentGestureID;
                    } else if(prevGestureID==int.MinValue) {    //nur beim ersten durchlauf
                        //td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, isShape3D[currentShapeID])); //für 3D, unfertig
                        td.setShape(currentShapeID, new gD.trainingData.Shape(currentShapeID, true));
                        prevGesture = new gD.trainingData.Shape.Gesture(currentGestureID);
                        prevGestureID = currentGestureID;
                        Console.WriteLine("Erste Geste erstellt " + currentShapeID);
                        shapesCreated[currentShapeID] = true;
                        prevShapeID = currentShapeID;
                    }

                    //Add Point to current Gesture
                    /*if(isShape3D[currentShapeID]) {       //Shape is 3D
                        prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY, currentPointZ));
                    } else {                                //Shape is 2D
                        prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY));
                    }*/
                    //nur 2D
                    prevGesture.addPoint(new gD.trainingData.Shape.Gesture.Point(currentPointID, currentPointX, currentPointY));
                    

                    //update prevVariable
                    prevShapeID = currentShapeID;
                    

            
                }while (!reader.EndOfStream);

                //add last Gesture
                td.getShape(prevShapeID).addGesture(prevGesture);

                return td;
             }
        }
    }

}