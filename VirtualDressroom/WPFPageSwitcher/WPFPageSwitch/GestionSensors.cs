using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectStatusNotifier;

namespace WPFPageSwitch
{

    /// <summary>
    /// Manage sensors : Basics 
    /// </summary>
    partial class GestionSensors
    {
        /// <summary>
        /// Boolean for start sensor
        /// </summary>
        public const bool START = true;
        /// <summary>
        /// Boolean for stop sensor
        /// </summary>
        public const bool STOP = false;

        /// <summary>
        ///  Define the kinect sensor
        /// </summary>
        public KinectSensor sensor{ get;  set; }


        public int depthWidth;
        public int depthHeight;
        public int colorWidth;
        public int colorHeight;


        /// <summary>
        /// Constructor
        /// </summary>
        public GestionSensors()
        {

            //NOTHING TO DO
        }

       
       
        /// <summary>
        ///  Define the number of sensor available
        /// </summary>
        public int nbSensors {
             get { return KinectSensor.KinectSensors.Count; }
            private set {  }
         }


      

        /// <summary>
        /// Say if we have sensors up
        /// </summary>
        /// <returns>True if sensors was available false else</returns>
        public bool haveSensors()
        {
            return (this.nbSensors > 0) ? true : false;
        }




        /// <summary>
        /// Bind a sensor with the sensors manager. Verify if this sensors exist, else use
        /// the default value 0
        /// </summary>
        /// <param name="sensorNumber">Number of sensors</param>
        public int bindSensor(int sensorNumber =0)
        {
            if (sensorNumber >= this.nbSensors)
                sensorNumber = 0; 

            if(this.haveSensors())
            {
                this.sensor = KinectSensor.KinectSensors[sensorNumber];
                this.notifier.Sensors = KinectSensor.KinectSensors;
                return 1;
            }
            return -1;
            
            
        }

        /// <summary>
        /// Change the state of use for the sensors 
        /// </summary>
        /// <param name="state">New state</param>
        public void changeStateSensor(bool state)
        {
            if(state)
                this.sensor.Start();
            else
                this.sensor.Stop();
        }


        /// <summary>
        /// For using sensor in one step
        /// </summary>
        /// <param name="sensorNumber"></param>
        /// <returns></returns>
        public int useSensor(int sensorNumber=0)
        {
            int res=bindSensor(sensorNumber);
            if (res != -1)
                this.changeStateSensor(GestionSensors.START);
            return res;

            
        }

       
       
        
       
        /// <summary>
        /// Gets or sets the pixel data.
        /// </summary>
        /// <value>The pixel data.</value>
        private byte[] pixelData { get; set; }
        /// <summary>
        /// For using the dll which show the notification
        /// </summary>
        private StatusNotifier notifier = new StatusNotifier();
        /// <summary>
        /// For responds the data to the caller class
        /// </summary>
        private MainInterface caller;
     
       
    }
}
