using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;

namespace WPFPageSwitch
{
    /// <summary>
    /// Interaction logic for HandCursor.xaml
    /// </summary>
    public partial class HandCursor : UserControl, ISwitchable
    {

        public HandCursor()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Modifie l'emplacement du curseur sur le flux video
        /// </summary>
        /// <param name="kinect"> capteur</param>
        /// <param name="joint">squelette</param>
        public void SetPosition(KinectSensor kinect, Joint joint)
        {
            ColorImagePoint colorImagePoint = kinect.CoordinateMapper.MapSkeletonPointToColorPoint(joint.Position, ColorImageFormat.RgbResolution640x480Fps30);
            Canvas.SetLeft(this, colorImagePoint.X*3);
            Canvas.SetTop(this, colorImagePoint.Y*2.25);
        }
        /// <summary>
        /// Recupere le point au centre du curseur utile pour les clics
        /// </summary>
        /// <returns>coordonne du point</returns>
        public  CursorPoint GetCursorPoint()
        {
            Point elementTopLeft = this.PointToScreen(new Point());
            double centerX = elementTopLeft.X + (this.ActualWidth / 2);
            double centerY = elementTopLeft.Y + (this.ActualHeight / 2);
            return new CursorPoint { X = centerX, Y = centerY };
        }

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        #endregion

    }
}
