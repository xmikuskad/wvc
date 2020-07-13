using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiimoteLib;



namespace wwc
{
    class WwcMain
    {
        Wiimote remote = new Wiimote();
        Wiimote remote2 = new Wiimote();
        Point2D[] wiimotePointsNormalized = new Point2D[4];
        




        private TextBox printBox;
        public WwcMain(TextBox printBoxInc)
        {
            this.printBox = printBoxInc;
        }

        public void test()
        {
            printMsg("Hello there");
            printMsg("Added msg");
        }


        //Tuto funkciu budeme pouzivat ako print funkciu :)
        public void printMsg(String msg)
        {
            printBox.AppendText(msg+Environment.NewLine);
        }



        
        public void startProgram()
        {
            float dotDistanceInMM = 8.5f * 25.4f;//width of the wii sensor bar
            float movementScaling = 1.0f;
            float screenHeightinMM = 20 * 25.4f;
            float radiansPerPixel = (float)(Math.PI / 4) / 1024.0f; //45 degree field of view with a 1024x768 camera
                                                                    //toto sa bude este menit na zaklade realneho modelu co postavim
            //remote.WiimoteState.AccelCalibrationInfo.X0
        }

       

        /*

        public void ParseWiimoteData()
        {
            if (remote.WiimoteState == null)
                return;

            Point2D firstPoint = new Point2D();
            Point2D secondPoint = new Point2D();
            int numvisible = 0;
            
            if (remote.WiimoteState.IRState.)
            {
                wiimotePointsNormalized[0].x = 1.0f - remote.WiimoteState.IRState.RawX1 / 768.0f;
                wiimotePointsNormalized[0].y = remote.WiimoteState.IRState.RawY1 / 768.0f;
                firstPoint.x = remote.WiimoteState.IRState.RawX1;
                firstPoint.y = remote.WiimoteState.IRState.RawY1;
                numvisible = 1;
            }
            else
            {//not visible
            }
            if (remote.WiimoteState.IRState.Found2)
            {
                wiimotePointsNormalized[1].x = 1.0f - remote.WiimoteState.IRState.RawX2 / 768.0f;
                wiimotePointsNormalized[1].y = remote.WiimoteState.IRState.RawY2 / 768.0f;
                wiiCursor2.isDown = true;
                if (numvisible == 0)
                {
                    firstPoint.x = remote.WiimoteState.IRState.RawX2;
                    firstPoint.y = remote.WiimoteState.IRState.RawY2;
                    numvisible = 1;
                }
                else
                {
                    secondPoint.x = remote.WiimoteState.IRState.RawX2;
                    secondPoint.y = remote.WiimoteState.IRState.RawY2;
                    numvisible = 2;
                }
            }
            else
            {//not visible
                wiiCursor2.isDown = false;
            }
            if (remote.WiimoteState.IRState.Found3)
            {
                wiimotePointsNormalized[2].x = 1.0f - remote.WiimoteState.IRState.RawX3 / 768.0f;
                wiimotePointsNormalized[2].y = remote.WiimoteState.IRState.RawY3 / 768.0f;
                wiiCursor3.isDown = true;
                if (numvisible == 0)
                {
                    firstPoint.x = remote.WiimoteState.IRState.RawX3;
                    firstPoint.y = remote.WiimoteState.IRState.RawY3;
                    numvisible = 1;
                }
                else if (numvisible == 1)
                {
                    secondPoint.x = remote.WiimoteState.IRState.RawX3;
                    secondPoint.y = remote.WiimoteState.IRState.RawY3;
                    numvisible = 2;
                }
            }
            else
            {//not visible
                wiiCursor3.isDown = false;
            }
            if (remote.WiimoteState.IRState.Found4)
            {
                wiimotePointsNormalized[3].x = 1.0f - remote.WiimoteState.IRState.RawX4 / 768.0f;
                wiimotePointsNormalized[3].y = remote.WiimoteState.IRState.RawY4 / 768.0f;
                wiiCursor4.isDown = true;
                if (numvisible == 1)
                {
                    secondPoint.x = remote.WiimoteState.IRState.RawX4;
                    secondPoint.y = remote.WiimoteState.IRState.RawY4;
                    numvisible = 2;
                }
            }
            else
            {//not visible
                wiiCursor4.isDown = false;
            }

            if (numvisible == 2)
            {


                float dx = firstPoint.x - secondPoint.x;
                float dy = firstPoint.y - secondPoint.y;
                float pointDist = (float)Math.Sqrt(dx * dx + dy * dy);

                float angle = radiansPerPixel * pointDist / 2;
                //in units of screen hieght since the box is a unit cube and box hieght is 1
                headDist = movementScaling * (float)((dotDistanceInMM / 2) / Math.Tan(angle)) / screenHeightinMM;


                float avgX = (firstPoint.x + secondPoint.x) / 2.0f;
                float avgY = (firstPoint.y + secondPoint.y) / 2.0f;


                //should  calaculate based on distance

                headX = (float)(movementScaling * Math.Sin(radiansPerPixel * (avgX - 512)) * headDist);

                relativeVerticalAngle = (avgY - 384) * radiansPerPixel;//relative angle to camera axis

                if (cameraIsAboveScreen)
                    headY = .5f + (float)(movementScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
                else
                    headY = -.5f + (float)(movementScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
            }



            //position the graphical cursors at the 3rd and 4th ir points if they exist
            if (wiiCursor1.isDown)
                wiiCursor1.setDown(wiimotePointsNormalized[0].x, wiimotePointsNormalized[0].y);
            if (wiiCursor2.isDown)
                wiiCursor2.setDown(wiimotePointsNormalized[1].x, wiimotePointsNormalized[1].y);
            if (wiiCursor3.isDown)
                wiiCursor3.setDown(wiimotePointsNormalized[2].x, wiimotePointsNormalized[2].y);
            if (wiiCursor4.isDown)
                wiiCursor4.setDown(wiimotePointsNormalized[3].x, wiimotePointsNormalized[3].y);


            wiiCursor1.wasDown = wiiCursor1.isDown;
            wiiCursor2.wasDown = wiiCursor2.isDown;
            wiiCursor3.wasDown = wiiCursor3.isDown;
            wiiCursor4.wasDown = wiiCursor4.isDown;

        }



        //Missuje firstPoint 
        //float dx = firstPoint.x - secondPoint.x;
        //float dy = firstPoint.y - secondPoint.y;
        //float pointDist = (float)Math.Sqrt(dx * dx + dy * dy);
        //float angle = radiansPerPixel * pointDist / 2;

        //headDist = movementScaling * (float)((dotDistanceInMM / 2) / Math.Tan(angle)) / screenHeightinMM;
        //headX = (float)(movementScaling * Math.Sin(radiansPerPixel * (avgX - 512)) * headDist);
        //headY = -.5f + (float)(movementScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
        //txtHelper.DrawTextLine("Est Head X-Y (mm): " + headX * screenHeightinMM + ", " + headY * screenHeightinMM); //toto je pozicia X a Y
        //txtHelper.DrawTextLine("Est Head Dist (mm): " + headDist * screenHeightinMM); //toto je pozicia Z
        */
    }
    class Point2D
    {
        public float x = 0.0f;
        public float y = 0.0f;
        public void set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
