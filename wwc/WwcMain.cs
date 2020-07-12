using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace wwc
{
    class WwcMain
    {

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
        }
    }
}
