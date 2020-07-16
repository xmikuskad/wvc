using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using vJoyInterfaceWrap;
using WiimoteLib;



namespace wwc
{
    class WwcMain
    {
        Wiimote remote1 = new Wiimote();
        Wiimote remote2 = new Wiimote();
        Point2D firstPoint = new Point2D();
        Point2D secondPoint = new Point2D();
        int vypis = 0;
        vJoy controller1 = new vJoy();
        vJoy controller2 = new vJoy();
        Point2D[] wiimotePointsNormalized = new Point2D[4];
        private TextBox printBox;

        //------------------------bude sa to menit podla modelu
        float dotDistanceInMM = 8.5f * 25.4f;//width of the wii sensor bar
        float screenHeightinMM = 20 * 25.4f;
        float radiansPerPixel = (float)(Math.PI / 4) / 1024.0f; //45 degree field of view with a 1024x768 camera
        float movementScaling = 1.0f;
        //-----------------------------------
        float headX = 0;
        float headY = 0;
        float headDist = 2;
        float cameraVerticaleAngle = 0; //begins assuming the camera is point straight forward
        float relativeVerticalAngle = 0; //current head position view angle
        bool cameraIsAboveScreen = false;//has no affect until zeroing and then is set automatically.



        public WwcMain(TextBox printBoxInc)
        {
            this.printBox = printBoxInc;
        }



        private void connectController(Wiimote con, vJoy vCon, uint id)
        {
            //Netreba??
            //vJoy.vConState iReport;
            //iReport = new vJoy.vConState();

            // Device ID can only be in the range 1-16
            if (id <= 0 || id > 16)
            {
                printMsg("Illegal device ID " +id.ToString()+"\nExit!");
                return;
            }
            
            // Get the driver attributes (Vendor ID, Product ID, Version Number)
            if (!vCon.vJoyEnabled())
            {
                printMsg("vJoy driver not enabled: Failed Getting vJoy attributes.\n");
                return;
            }
            else
                printMsg("Vendor: " + vCon.GetvJoyManufacturerString() + "\nProduct :"+ vCon.GetvJoyProductString() + "\nVersion Number:" +
                    vCon.GetvJoySerialNumberString() + "\n"); 
                  

            // Get the state of the requested device
            VjdStat status = vCon.GetVJDStatus(id);
            switch (status)
            {
                case VjdStat.VJD_STAT_OWN:
                    printMsg("vJoy Device "+id.ToString()+" is already owned by this feeder\n");
                    break;
                case VjdStat.VJD_STAT_FREE:
                    printMsg("vJoy Device "+id.ToString()+" is free\n");
                    break;
                case VjdStat.VJD_STAT_BUSY:
                    printMsg("vJoy Device "+id.ToString()+" is already owned by another feeder\nCannot continue\n");
                    return;
                case VjdStat.VJD_STAT_MISS:
                    printMsg("vJoy Device "+id.ToString()+" is not installed or disabled\nCannot continue\n");
                    return;
                default:
                    printMsg("vJoy Device "+id.ToString()+" general error\nCannot continue\n");
                    return;
            };
            
            // Check which axes are supported
            bool AxisX = vCon.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_X);
            bool AxisY = vCon.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Y);
            bool AxisZ = vCon.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_Z);
            bool AxisRX = vCon.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RX);
            bool AxisRZ = vCon.GetVJDAxisExist(id, HID_USAGES.HID_USAGE_RZ);
            // Get the number of buttons and POV Hat switchessupported by this vJoy device
            int nButtons = vCon.GetVJDButtonNumber(id);
            int ContPovNumber = vCon.GetVJDContPovNumber(id);
            int DiscPovNumber = vCon.GetVJDDiscPovNumber(id);

            // Print results
            printMsg("\nvJoy Device" + id.ToString() + " capabilities:\n");
            printMsg("Numner of buttons\t\t" + nButtons.ToString() + "\n");
            printMsg("Numner of Continuous POVs\t" + ContPovNumber.ToString() + "\n");
            printMsg("Numner of Descrete POVs\t\t" + DiscPovNumber.ToString() + "\n");

            if (AxisX)
            {
                printMsg("Axis X\t\tYES\n");
                printMsg("Axis Y\t\tYES\n");
                printMsg("Axis Z\t\tYES\n");
            }
            else
            {
                printMsg("Axis X\t\tNO\n");
                printMsg("Axis Y\t\tNO\n");
                printMsg("Axis Z\t\tNO\n");
            }
            if(AxisRX)
            {
                printMsg("Axis Rx\t\tYES\n");
            }
            else
            {
                printMsg("Axis Rx\t\tNO\n");
            }

            if(AxisRZ)
            {
                printMsg("Axis Rz\t\tYES\n");
            }
            else
            {
                printMsg("Axis Rz\t\tYES\n");
            }


            // Test if DLL matches the driver
            UInt32 DllVer = 0, DrvVer = 0;
            bool match = vCon.DriverMatch(ref DllVer, ref DrvVer);
            if (match)
                printMsg("Version of Driver Matches DLL Version "+ DllVer.ToString() + ")\n");
            else
                printMsg("Version of Driver "+ DrvVer.ToString() + " does NOT match DLL Version "+DllVer.ToString()+"\n");


            // Acquire the target
            if ((status == VjdStat.VJD_STAT_OWN) || ((status == VjdStat.VJD_STAT_FREE) && (!vCon.AcquireVJD(id))))
            {
                printMsg("Failed to acquire vJoy device number "+id.ToString()+".\n");
                return;
            }
            else
                printMsg("Acquired: vJoy device number "+id.ToString()+".\n");

            //Tuto treba dat nieco rozumnejsie
            /*
            for(int i=5; i>0;i--)
            {
                printMsg("SLEEPING " + i.ToString());
                Thread.Sleep(1000);
            }
            */
            int X, Y, Z, ZR, XR;
            uint count = 0;
            long maxval = 0;

            X = 20;
            Y = 30;
            Z = 40;
            XR = 60;
            ZR = 80;

            vCon.GetVJDAxisMax(id, HID_USAGES.HID_USAGE_X, ref maxval);

            bool res;
            // Reset this device to default values
            vCon.ResetVJD(id);


            bool done = false;


            while (!done)
            {
                try
                {
                    printMsg("3");
                    Wiimote test = connectController(con);
                    if (test == null)
                        printMsg("ERROR CONENCTION!");
                    else
                    {
                       
                        printMsg("conenction fine");
                        con = test;
                        
                        
                    }

                    done = true;
                }
                catch (Exception e)
                {
                    done = false;
                    printMsg("Not found!");
                }

            }


            var T = con.WiimoteState.Extension;

            //con.WiimoteState.ExtensionType = ExtensionType.MotionPlus;
            
            printMsg(con.WiimoteState.ExtensionType.ToString());
            var pX = con.WiimoteState.AccelCalibrationInfo.X0;
            var pY = con.WiimoteState.AccelCalibrationInfo.Y0;
            var pZ = con.WiimoteState.AccelCalibrationInfo.Z0;
            var pXG = con.WiimoteState.AccelCalibrationInfo.XG;
            var pYG = con.WiimoteState.AccelCalibrationInfo.YG;
            var pZG = con.WiimoteState.AccelCalibrationInfo.ZG;
            //var pIRX = c



            printMsg("ZTop Left " + pX);
            printMsg("ZTop Right " + pY);
            printMsg("ZBottom Left " + pZ);
            printMsg("ZTop Left " + pXG);
            printMsg("ZTop Right " + pYG);
            printMsg("ZBottom Left " + pZG);

            con.InitializeMotionPlus();
        }
        


        private void wiiDevice_WiimoteChanged(object sender, WiimoteChangedEventArgs e)
        {
            vypis++;
            WiimoteState ws = e.WiimoteState;
            //Thread.Sleep(500);
            
            
            if (vypis % 50 == 0)
            {
                //ws.ExtensionType = ExtensionType.MotionPlus;
                Debug.WriteLine(ws.AccelState.RawValues.ToString());
                Debug.WriteLine(ws.IRState.IRSensors[0].Position.X.ToString()+ ws.IRState.IRSensors[0].Position.Y.ToString());
                Debug.WriteLine(ws.IRState.IRSensors[1].Position.X.ToString() + ws.IRState.IRSensors[1].Position.Y.ToString());
                //ws.IRState.IRSensors[0].Found
                Debug.WriteLine("X: " + headX.ToString() + " Y: " + headY.ToString() + " Z: " + headDist.ToString());


                
                
                Debug.WriteLine("motplus: " + ws.MotionPlusState.RawValues.ToString());
                Debug.WriteLine("motplusx: " + ws.MotionPlusState.RawValues.X.ToString());
                Debug.WriteLine("motplusy: " + ws.MotionPlusState.RawValues.Y.ToString());
                Debug.WriteLine("motplusz: " + ws.MotionPlusState.RawValues.Z.ToString());
                Debug.WriteLine("ext: " + ws.ExtensionType.ToString());
                

                vypis = 0;
            }
            
            
            // Called every time there is a sensor update, values available using e.WiimoteState.
            // Use this for tracking and filtering rapid accelerometer and gyroscope sensor data.
            // The balance board values are basic, so can be accessed directly only when needed.
        }


        //-----------------------------------------------------------kalulacia X,Y,Z v preistore -----------------------

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

        //-----------------------------------------------este treba spravit pre remote2--------------
        public void ParseWiimoteData()
        {

            if (remote1.WiimoteState == null)
                return;

            Point2D firstPoint = new Point2D();
            Point2D secondPoint = new Point2D();
            int numvisible = 0;

            if (remote1.WiimoteState.IRState.IRSensors[0].Found)
            {
                wiimotePointsNormalized[0].x = 1.0f - remote1.WiimoteState.IRState.IRSensors[0].RawPosition.X / 768.0f;
                wiimotePointsNormalized[0].y = remote1.WiimoteState.IRState.IRSensors[0].RawPosition.Y / 768.0f;
                //wiiCursor1.isDown = true;
                firstPoint.x = remote1.WiimoteState.IRState.IRSensors[0].RawPosition.X;
                firstPoint.y = remote1.WiimoteState.IRState.IRSensors[0].RawPosition.Y;
                numvisible = 1;
            }

            if (remote1.WiimoteState.IRState.IRSensors[1].Found)
            {
                wiimotePointsNormalized[1].x = 1.0f - remote1.WiimoteState.IRState.IRSensors[1].RawPosition.X / 768.0f;
                wiimotePointsNormalized[1].y = remote1.WiimoteState.IRState.IRSensors[1].RawPosition.Y / 768.0f;
                if (numvisible == 0)
                {
                    firstPoint.x = remote1.WiimoteState.IRState.IRSensors[1].RawPosition.X;
                    firstPoint.y = remote1.WiimoteState.IRState.IRSensors[1].RawPosition.X;
                    numvisible = 1;
                }
                else
                {
                    secondPoint.x = remote1.WiimoteState.IRState.IRSensors[1].RawPosition.X;
                    secondPoint.y = remote1.WiimoteState.IRState.IRSensors[1].RawPosition.Y;
                    numvisible = 2;
                }
            }

            if (remote1.WiimoteState.IRState.IRSensors[2].Found)
            {
                wiimotePointsNormalized[2].x = 1.0f - remote1.WiimoteState.IRState.IRSensors[2].RawPosition.X / 768.0f;
                wiimotePointsNormalized[2].y = remote1.WiimoteState.IRState.IRSensors[2].RawPosition.Y/ 768.0f;
                if (numvisible == 0)
                {
                    firstPoint.x = remote1.WiimoteState.IRState.IRSensors[2].RawPosition.X;
                    firstPoint.y = remote1.WiimoteState.IRState.IRSensors[2].RawPosition.Y;
                    numvisible = 1;
                }
                else if (numvisible == 1)
                {
                    secondPoint.x = remote1.WiimoteState.IRState.IRSensors[2].RawPosition.X;
                    secondPoint.y = remote1.WiimoteState.IRState.IRSensors[2].RawPosition.Y;
                    numvisible = 2;
                }
            }

            if (remote1.WiimoteState.IRState.IRSensors[3].Found)
            {
                wiimotePointsNormalized[3].x = 1.0f - remote1.WiimoteState.IRState.IRSensors[3].RawPosition.X / 768.0f;
                wiimotePointsNormalized[3].y = remote1.WiimoteState.IRState.IRSensors[3].RawPosition.Y / 768.0f;
                //wiiCursor4.isDown = true;
                if (numvisible == 1)
                {
                    secondPoint.x = remote1.WiimoteState.IRState.IRSensors[3].RawPosition.X;
                    secondPoint.y = remote1.WiimoteState.IRState.IRSensors[3].RawPosition.Y;
                    numvisible = 2;
                }
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
        }

        //-----------------------------------koniec kalkulacie X,Y,Z v prestore-------------
        //headDist by malo byt os Z
        //headY by malo byt os Y
        //headX by malo byt os X


        static private void wiiDevice_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs e)
        {
            // This is not needed for balance boards.
        }
        
        Wiimote connectController(Wiimote wiiDeviceInc)
        {
            try
            {
                // Find all connected Wii devices.

                var deviceCollection = new WiimoteCollection();
                deviceCollection.FindAllWiimotes();

                //printMsg("Device count: "+deviceCollection.Count.ToString());

                Wiimote wiiDevice = deviceCollection[0];

                // Device type can only be found after connection, so prompt for multiple devices.

                // Setup update handlers.

                wiiDevice.WiimoteChanged += wiiDevice_WiimoteChanged;
                wiiDevice.WiimoteExtensionChanged += wiiDevice_WiimoteExtensionChanged;

                // Connect and send a request to verify it worked.

                wiiDevice.Connect();
                wiiDevice.SetReportType(InputReport.IRAccel, false); // FALSE = DEVICE ONLY SENDS UPDATES WHEN VALUES CHANGE!
                wiiDevice.SetLEDs(true, false, false, false);

                return wiiDevice;

            }
            catch (Exception ex)
            {
                //ex.StackTrace();
                return null;
            }

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

            connectController(remote1, controller1,1);
        }

       

    

       
    }

}
