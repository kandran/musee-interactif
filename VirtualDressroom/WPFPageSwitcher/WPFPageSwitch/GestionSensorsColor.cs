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
    /// Manage Sensors : specialized for color stream
    /// </summary>
    partial class GestionSensors
    {
        /// <summary>
        /// Enable color stream and attach him a target
        /// </summary>
        /// <param name="m">Caller class</param>
        public void useColorStream(MainInterface m)
        {
            this.caller = m;
            this.sensor.ColorStream.Disable();
            if (!this.sensor.ColorStream.IsEnabled)
            {
                this.sensor.ColorStream.Enable(ColorImageFormat.RgbResolution1280x960Fps12);
              

                this.sensor.ColorFrameReady += new EventHandler<ColorImageFrameReadyEventArgs>(sensor_ColorFrameReady);
            }
        }
        

        /// <summary>
        /// Color frame main class
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void sensor_ColorFrameReady(object sender, ColorImageFrameReadyEventArgs e)
        {
            using (ColorImageFrame imageFrame = e.OpenColorImageFrame())
            {
                // Check if the incoming frame is not null
                if (imageFrame != null)
                {
                    // Get the pixel data in byte array
                    this.pixelData = new byte[imageFrame.PixelDataLength];


                    // Copy the pixel data
                    imageFrame.CopyPixelDataTo(this.pixelData);
                  
                    // assign the bitmap image source into image control
                    BitmapSource bitmapS = BitmapSource.Create(
                     imageFrame.Width,
                     imageFrame.Height,
                     96,
                     96,
                     PixelFormats.Bgr32,
                     null,
                     pixelData,
                     imageFrame.Width *4 );

                   // this.caller.setImg(bitmapS); //we send data to the caller class

                }
            }
        }
    }
}
