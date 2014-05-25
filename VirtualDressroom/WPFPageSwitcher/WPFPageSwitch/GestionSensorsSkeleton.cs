using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using KinectStatusNotifier;


using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Collections.ObjectModel;


namespace WPFPageSwitch
{
    /// <summary>
    /// Classe de gestion des capteurs
    /// </summary>
    partial class GestionSensors
    {
        Skeleton skeleton;
        ObservableCollection<SkeletonInfo> skeletonCollection = new ObservableCollection<SkeletonInfo>();
        private DrawingGroup drawingGroup;

        private bool showSkel = false;

        /// <summary>
        /// Permet d'utiliser le squelette et afficher les vetements
        /// </summary>
        /// <param name="e">Instance vers l'appellant</param>
        public void useSkeleton(MainInterface e)
        {

            this.sensor.SkeletonStream.Enable();
            this.sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
            this.skeletonCollection.Clear();

            this.sensor.DepthStream.Range = DepthRange.Default;
            this.caller = e;
            sensor.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
        }


        /// <summary>
        /// Fonction appelle lorsque le squelette peut etre vraiment utilisé
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //efface les vetements precedent
            this.caller.myCanvas.Children.Clear();
            Brush brush = new SolidColorBrush(Colors.Red);
            Skeleton[] skeletons = null;
            SkeletonFrame frame;
            // utilisation du squelette
            using (frame = e.OpenSkeletonFrame())
            {
                if (frame != null)
                {
                    skeletons = new Skeleton[frame.SkeletonArrayLength];
                    frame.CopySkeletonDataTo(skeletons);
                    
                }
            }
            
            //cas d'erreur
            if (skeletons == null) return;

            //on traque le premier squelette
            skeleton = (from trackSkeleton in skeletons
                        where trackSkeleton.TrackingState == SkeletonTrackingState.Tracked
                        select trackSkeleton).FirstOrDefault();

            //cas d'erreur
            if (skeleton == null)
                return;

            if (skeletonCollection.Count <= 1000)
            {
                skeletonCollection.Add(new SkeletonInfo { FrameID = frame.FrameNumber, Skeleton = skeleton });
            }

            int trackedSkeleton = skeleton.Joints.Where(item => item.TrackingState == JointTrackingState.Tracked).Count();

            //using (DrawingContext dc = this.drawingGroup.Open())
            //{
            // Draw a transparent background to set the render size
            // dc.DrawRectangle(Brushes.Black, null, new Rect(0.0, 0.0, 640, 480));

            //si on a des squelette
            if (skeletons.Length != 0)
            {
                //pour chaque squelette
                foreach (Skeleton skel in skeletons)
                {

                    //si on traque le squelette
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        if (this.showSkel) //si on veux afficher le squelette
                            this.DrawSkeleton(skel);
                    }

                    //si la tete est trouve
                    if (skel.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked)
                    {
                        //on affiche le vetement et les chapeau
                        this.caller.Myjupe.Opacity = 1;
                        this.caller.chapeau.Opacity = 1;
                        //this.caller.MapJointsWithUIElement(skel);

                        //affiche les vetements au bon endroit
                        Habits.MapJointsWithUIElement(skel, this, this.caller);
                    }


                    // place le curseur sur la main droite
                    var rightHand = skeleton.Joints[JointType.HandRight];
                    //on met a jour la position du curseur
                    caller.handCursor.SetPosition(this.sensor, rightHand);
                    //on met le click sur les elements
                    this.caller.habit1.ValidatePoisition(caller.handCursor);
                    this.caller.habit2.ValidatePoisition(caller.handCursor);
                    this.caller.Armoire.ValidatePoisition(caller.handCursor);
                    this.caller.PM.ValidatePoisition(caller.handCursor);
                    this.caller.Photo.ValidatePoisition(caller.handCursor);
                    this.caller.HMvR.ValidatePoisition(caller.handCursor);
                    this.caller.HMvL.ValidatePoisition(caller.handCursor);
                    this.caller.CMvR.ValidatePoisition(caller.handCursor);
                    this.caller.CMvL.ValidatePoisition(caller.handCursor);
                    this.caller.chapeau1.ValidatePoisition(caller.handCursor);
                    this.caller.chapeau2.ValidatePoisition(caller.handCursor);

                    /*else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                    {
                        dc.DrawEllipse(
                        this.centerPointBrush,
                        null,
                        this.SkeletonPointToScreen(skel.Position),
                        BodyCenterThickness,
                        BodyCenterThickness);
                    }*/
                    // break;
                }
            }

            // As we are checking for Right Hand, check if Right Hand Tracking State.  Call the mapping method only if the joint is tracked.

            // prevent drawing outside of our render area
            //  this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, 640, 480));
            //}

        }
        /// <summary>
        /// Draws the bone.
        /// </summary>
        /// <param name="trackedJoint1">The tracked joint1.</param>
        /// <param name="trackedJoint2">The tracked joint2.</param>
        /// 
        void drawBone(Joint trackedJoint1, Joint trackedJoint2)
        {
            Line bone = new Line();
            bone.Stroke = Brushes.Red;
            bone.StrokeThickness = 3;
            Point joint1 = this.ScalePosition(trackedJoint1.Position);
            bone.X1 = joint1.X;
            bone.Y1 = joint1.Y;

            Point mappedPoint1 = this.ScalePosition(trackedJoint1.Position);
            Rectangle r = new Rectangle(); r.Height = 10; r.Width = 10;
            r.Fill = Brushes.Red;
            Canvas.SetLeft(r, mappedPoint1.X - 2);
            Canvas.SetTop(r, mappedPoint1.Y - 2);
            this.caller.myCanvas.Children.Add(r);


            Point joint2 = this.ScalePosition(trackedJoint2.Position);
            bone.X2 = joint2.X;
            bone.Y2 = joint2.Y;

            Point mappedPoint2 = this.ScalePosition(trackedJoint2.Position);


            if (LeafJoint(trackedJoint2))
            {
                Rectangle r1 = new Rectangle(); r1.Height = 10; r1.Width = 10;
                r1.Fill = Brushes.Red;
                Canvas.SetLeft(r1, mappedPoint2.X - 2);
                Canvas.SetTop(r1, mappedPoint2.Y - 2);
                this.caller.myCanvas.Children.Add(r1);
                Point mappedPoint = this.ScalePosition(trackedJoint2.Position);

            }

            this.caller.myCanvas.Children.Add(bone);
        }

        /// <summary>
        /// Scales the position.
        /// </summary>
        /// <param name="skeletonPoint">The skeleton point.</param>
        /// <returns></returns>
        public Point ScalePosition(SkeletonPoint skeletonPoint)
        {

            DepthImagePoint depthPoint = this.sensor.CoordinateMapper.MapSkeletonPointToDepthPoint(skeletonPoint, DepthImageFormat.Resolution640x480Fps30);
            return new Point(Config.scaleX(depthPoint.X, 640), Config.scaleY(depthPoint.Y, 480));
        }
        public float Scale(float value, int max)
        {

            return Math.Min(Math.Max((max >> 1) + (value * (max >> 1)), 0), max);
        }



        /// <summary>
        /// Leafs the joint.
        /// </summary>
        /// <param name="j2">The j2.</param>
        /// <returns></returns>
        private bool LeafJoint(Joint j2)
        {
            if (j2.JointType == JointType.HandRight || j2.JointType == JointType.HandLeft || j2.JointType == JointType.FootLeft || j2.JointType == JointType.FootRight)
            {
                return true;
            }
            return false;
        }


        /// <summary>
        /// Draws the skeleton.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        private void DrawSkeleton(Skeleton skeleton)
        {

            drawBone(skeleton.Joints[JointType.Head], skeleton.Joints[JointType.ShoulderCenter]);
            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.Spine]);

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderLeft]);
            drawBone(skeleton.Joints[JointType.ShoulderLeft], skeleton.Joints[JointType.ElbowLeft]);
            drawBone(skeleton.Joints[JointType.ElbowLeft], skeleton.Joints[JointType.WristLeft]);
            drawBone(skeleton.Joints[JointType.WristLeft], skeleton.Joints[JointType.HandLeft]);

            drawBone(skeleton.Joints[JointType.ShoulderCenter], skeleton.Joints[JointType.ShoulderRight]);
            drawBone(skeleton.Joints[JointType.ShoulderRight], skeleton.Joints[JointType.ElbowRight]);
            drawBone(skeleton.Joints[JointType.ElbowRight], skeleton.Joints[JointType.WristRight]);
            drawBone(skeleton.Joints[JointType.WristRight], skeleton.Joints[JointType.HandRight]);

            drawBone(skeleton.Joints[JointType.Spine], skeleton.Joints[JointType.HipCenter]);
            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipLeft]);
            drawBone(skeleton.Joints[JointType.HipLeft], skeleton.Joints[JointType.KneeLeft]);
            drawBone(skeleton.Joints[JointType.KneeLeft], skeleton.Joints[JointType.AnkleLeft]);
            drawBone(skeleton.Joints[JointType.AnkleLeft], skeleton.Joints[JointType.FootLeft]);

            drawBone(skeleton.Joints[JointType.HipCenter], skeleton.Joints[JointType.HipRight]);
            drawBone(skeleton.Joints[JointType.HipRight], skeleton.Joints[JointType.KneeRight]);
            drawBone(skeleton.Joints[JointType.KneeRight], skeleton.Joints[JointType.AnkleRight]);
            drawBone(skeleton.Joints[JointType.AnkleRight], skeleton.Joints[JointType.FootRight]);
        }





    }
}

