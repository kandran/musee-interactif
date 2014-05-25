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
    partial class GestionSensors
    {
        
        byte[] depth32;
        short[] pixelData32;

        //Options
        bool colorized = false;
        bool trackPlayer =true;

        DepthImageFrame frame;

        private int colorToDepthDivisor;
        private ColorImagePoint[] colorCoordinates;
        private DepthImagePixel[] depthPixels;
        private int[] playerPixelData;
        private int opaquePixelValue = -1;
        private WriteableBitmap colorBitmap;
        private WriteableBitmap playerOpacityMaskImage = null;
        



        /// <summary>
        /// Use depth for an img
        /// </summary>
        /// <param name="e">Caller object</param>
        public void useDepth(MainInterface e)
        {

            this.sensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            this.depthWidth = this.sensor.DepthStream.FrameWidth;
            this.depthHeight = this.sensor.DepthStream.FrameHeight;
            int colorWidth = this.sensor.ColorStream.FrameWidth;
            int colorHeight = this.sensor.ColorStream.FrameHeight;
            this.colorToDepthDivisor = colorWidth / this.depthWidth;

            sensor.SkeletonStream.Enable();

            this.colorCoordinates = new ColorImagePoint[this.sensor.DepthStream.FramePixelDataLength];
            this.depthPixels = new DepthImagePixel[this.sensor.DepthStream.FramePixelDataLength];
            this.playerPixelData = new int[this.sensor.DepthStream.FramePixelDataLength];
            this.colorBitmap = new WriteableBitmap(colorWidth, colorHeight, 96.0, 96.0, PixelFormats.Bgr32, null);
            this.caller.colorimageControl2.Source = this.colorBitmap;

           
            this.caller = e;
            sensor.DepthFrameReady += new EventHandler<DepthImageFrameReadyEventArgs>(sensor_DepthFrameReady);
        }

        /// <summary>
        /// Gets the color pixel data with distance.
        /// </summary>
        /// <param name="depthFrame">The depth frame.</param>
        /*private void GetColorPixelDataWithDistance(short[] depthFrame)
        {
            for (int depthIndex = 0, colorIndex = 0; depthIndex < depthFrame.Length && colorIndex < this.depth32.Length; depthIndex++, colorIndex += 4)
            {
                // Calculate the depth distance
                int distance = depthFrame[depthIndex] >> DepthImageFrame.PlayerIndexBitmaskWidth;
                // Colorize pixels for a range of distance
                if (distance <= 1000)
                {
                    depth32[colorIndex + 2] = 115; // red
                    depth32[colorIndex + 1] = 169;  // green
                    depth32[colorIndex + 0] = 9; // blue

                }
                else if (distance > 1000 && distance <= 2500)
                {
                    depth32[colorIndex + 2] = 255;
                    depth32[colorIndex + 1] = 61;
                    depth32[colorIndex + 0] = 0;
                }
                else if (distance > 2500)
                {
                    depth32[colorIndex + 2] = 169;
                    depth32[colorIndex + 1] = 9;
                    depth32[colorIndex + 0] = 115;
                }
            }
        }*/
        /// <summary>
        /// Tracks the player.
        /// </summary>
        /// <param name="depthFrame">The depth frame.</param>
       /* private void TrackPlayer(short[] depthFrame)
        {
            for (int depthIndex = 0, colorIndex = 0; depthIndex < depthFrame.Length && colorIndex < this.depth32.Length; depthIndex++, colorIndex += 4)
            {
                // Get the player
                int player = depthFrame[depthIndex] & DepthImageFrame.PlayerIndexBitmask;
                // Color the all pixels associated with a player
                if (player > 0 && pixelData!=null)
                {
                    depth32[colorIndex + 2] = pixelData[colorIndex*4 +2];
                        //169;
                    depth32[colorIndex + 1] = pixelData[colorIndex*4 +1];
                    //62;
                    depth32[colorIndex + 0] = pixelData[colorIndex*4];
                    //9;
                }
                
            }
        }*/

        /// <summary>
        /// Reversings the bit value with distance.
        /// </summary>
        /// <param name="depthImageFrame">The depth image frame.</param>
        /// <param name="pixelData">The pixel data.</param>
        /// <returns></returns>
       /* private short[] ReversingBitValueWithDistance(DepthImageFrame depthImageFrame, short[] pixelData32)
        {
            short[] reverseBitPixelData = new short[depthImageFrame.PixelDataLength];
            int depth;
            for (int index = 0; index < pixelData32.Length; index++)
            {
                // Caculate the distance
                depth = pixelData32[index] >> DepthImageFrame.PlayerIndexBitmaskWidth;


                int player = pixelData32[index] & DepthImageFrame.PlayerIndexBitmask;

                // Change the pixel value 
                if (depth < 1500 || depth > 3500)
                {
                    reverseBitPixelData[index] = (short)~pixelData32[index]; ;
                }
                else
                {
                    reverseBitPixelData[index] = pixelData32[index];
                }
            }

            return reverseBitPixelData;
        }*/

        /// <summary>
        /// Handles the DepthFrameReady event of the sensor control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="Microsoft.Kinect.DepthImageFrameReadyEventArgs"/> instance containing the event data.</param>
        void sensor_DepthFrameReady(object sender, DepthImageFrameReadyEventArgs e)
        {
            using (DepthImageFrame depthimageFrame = e.OpenDepthImageFrame())
            {
                if (depthimageFrame == null)
                {
                    return;
                }
                frame = depthimageFrame;
                pixelData32 = new short[depthimageFrame.PixelDataLength];

              

                depthimageFrame.CopyPixelDataTo(pixelData32);
          

                //short[] reversePixelData = new short[depthimageFrame.PixelDataLength];
                //reversePixelData = this.ReversingBitValueWithDistance(depthimageFrame, pixelData);



                //Modif mapping

                using (DepthImageFrame depthFrame = e.OpenDepthImageFrame())
                {
                    if (null != depthFrame)
                    {
                        // Copy the pixel data from the image to a temporary array
                        depthFrame.CopyDepthImagePixelDataTo(this.depthPixels);

                       
                    }
                }

                this.sensor.CoordinateMapper.MapDepthFrameToColorFrame(
                   DepthImageFormat.Resolution640x480Fps30,
                   this.depthPixels,
                   ColorImageFormat.RgbResolution1280x960Fps12,
                   this.colorCoordinates);

                Array.Clear(this.playerPixelData, 0, this.playerPixelData.Length);

                // loop over each row and column of the depth
                for (int y = 0; y < this.depthHeight; ++y)
                {
                    for (int x = 0; x < this.depthWidth; ++x)
                    {
                        // calculate index into depth array
                        int depthIndex = x + (y * this.depthWidth);

                        DepthImagePixel depthPixel = this.depthPixels[depthIndex];

                        int player = depthPixel.PlayerIndex;

                        // if we're tracking a player for the current pixel, sets it opacity to full
                        if (player > 0)
                        {
                            // retrieve the depth to color mapping for the current depth pixel
                            ColorImagePoint colorImagePoint = this.colorCoordinates[depthIndex];

                            // scale color coordinates to depth resolution
                            int colorInDepthX = colorImagePoint.X / this.colorToDepthDivisor;
                            int colorInDepthY = colorImagePoint.Y / this.colorToDepthDivisor;

                            // make sure the depth pixel maps to a valid point in color space
                            // check y > 0 and y < depthHeight to make sure we don't write outside of the array
                            // check x > 0 instead of >= 0 since to fill gaps we set opaque current pixel plus the one to the left
                            // because of how the sensor works it is more correct to do it this way than to set to the right
                            if (colorInDepthX > 0 && colorInDepthX < this.depthWidth && colorInDepthY >= 0 && colorInDepthY < this.depthHeight)
                            {
                                // calculate index into the player mask pixel array
                                int playerPixelIndex = colorInDepthX + (colorInDepthY * this.depthWidth);

                                // set opaque
                                this.playerPixelData[playerPixelIndex] = opaquePixelValue;

                                // compensate for depth/color not corresponding exactly by setting the pixel 
                                // to the left to opaque as well
                                this.playerPixelData[playerPixelIndex - 1] = opaquePixelValue;
                            }
                        }
                    }
                }


                // do our processing outside of the using block
                // so that we return resources to the kinect as soon as possible
                
                    // Write the pixel data into our bitmap
                    this.colorBitmap.WritePixels(
                        new Int32Rect(0, 0, this.colorBitmap.PixelWidth, this.colorBitmap.PixelHeight),
                        this.pixelData,
                        this.colorBitmap.PixelWidth * sizeof(int),
                        0);

                    this.caller.colorimageControl2.Source = colorBitmap;
                    if (this.playerOpacityMaskImage == null)
                    {
                        this.playerOpacityMaskImage = new WriteableBitmap(
                            this.depthWidth,
                            this.depthHeight,
                            96,
                            96,
                            PixelFormats.Bgra32,
                            null);
                        
                       this.caller.colorimageControl2.OpacityMask = new ImageBrush { ImageSource = this.playerOpacityMaskImage };
                    }

                    this.playerOpacityMaskImage.WritePixels(
                        new Int32Rect(0, 0, this.depthWidth, this.depthHeight),
                        this.playerPixelData,
                        this.depthWidth * ((this.playerOpacityMaskImage.Format.BitsPerPixel + 7) / 8),
                        0);
                
                //Fin modif mapping

                /*
               if (trackPlayer == true)
                {
                    depth32 = new byte[depthimageFrame.PixelDataLength * 4];
                    this.TrackPlayer(pixelData32);
                    this.caller.colorimageControl.Source = BitmapSource.Create(
                               depthimageFrame.Width, 
                               depthimageFrame.Height, 
                               96, 
                               96, 
                               PixelFormats.Bgr32,
                               null,
                               depth32, 
                               depthimageFrame.Width * 4
              );
                }
                else if (colorized == true)
                {
                    depth32 = new byte[depthimageFrame.PixelDataLength * 4];
                    this.GetColorPixelDataWithDistance(pixelData32);
                    this.caller.colorimageControl.Source = BitmapSource.Create(
                depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Bgr32, null, depth32, depthimageFrame.Width 
                );
                }
                else
                {
                    this.caller.colorimageControl.Source = BitmapSource.Create(
                       depthimageFrame.Width, depthimageFrame.Height, 96, 96, PixelFormats.Gray16, null, pixelData32, depthimageFrame.Width * depthimageFrame.BytesPerPixel
                       );
                }*/
               

            }
        }
    }
}
