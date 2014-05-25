using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Kinect;

namespace WPFPageSwitch
{
    class Habits
    {
       /*  Ancien systeme de selection des vetements (en dur)
        * 
        * public static System.Windows.Media.ImageSource getVetementImg(int id)
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            switch (id)
            {
                case 1:
                    bi3.UriSource = new Uri("/JagoanFisika;component/Resources/Manteau.png", UriKind.RelativeOrAbsolute);
                    break;
                case 2:
                    bi3.UriSource = new Uri("/JagoanFisika;component/Resources/Robe.png", UriKind.RelativeOrAbsolute);
                    break;
                default:
                    bi3.UriSource = new Uri("/JagoanFisika;component/Resources/Robe.png", UriKind.RelativeOrAbsolute);
                    break;
                    
            }
            
            bi3.EndInit();
            return bi3;
        }*/



        /// <summary>
        /// Lie les vetements avec le squelette
        /// </summary>
        /// <param name="skeleton">Skelette</param>
        /// <param name="fenetre">Instance vers la classe d'affichage de notre application</param>
        /// <param name="sensor">Entree vers les capteurs de la kinect</param>
        public static void MapJointsWithUIElement(Skeleton skeleton, GestionSensors sensor, MainInterface fenetre)
        {
            // Get the Points in 2D Space to map in UI. for that call the Scale postion method.
            Point mappedPoint = sensor.ScalePosition(skeleton.Joints[JointType.ShoulderCenter].Position);
            Point mappedPoint2 = sensor.ScalePosition(skeleton.Joints[JointType.Head].Position);

            //largeur des epaules
            double largeur = Math.Sqrt(Math.Pow(Math.Abs(sensor.ScalePosition(skeleton.Joints[JointType.ShoulderRight].Position).X - sensor.ScalePosition(skeleton.Joints[JointType.ShoulderLeft].Position).X), 2) + Math.Pow(Math.Abs(sensor.ScalePosition(skeleton.Joints[JointType.ShoulderRight].Position).Y - sensor.ScalePosition(skeleton.Joints[JointType.ShoulderLeft].Position).Y), 2));

            //Vetements
            Image vetement = fenetre.Vetements.Children[0] as Image;
            Image vetementHead = fenetre.Chapeaux.Children[0] as Image;
           
            //adaptation de la largeur
            if (largeur > 0)
                vetement.Width = largeur * 1.25;
            //  Vetements.Width= largeur;

            //hauteur
            double hauteur = (sensor.ScalePosition(skeleton.Joints[JointType.AnkleLeft].Position).Y - sensor.ScalePosition(skeleton.Joints[JointType.ShoulderCenter].Position).Y)/1.25;

            //adaptation de la hauteur
            if (hauteur > 0)
                vetement.Height = hauteur * 1.25;

            //largeur tete = 1/12 * taille corps
            /*
            double largeurChapeau = sensor.ScalePosition(skeleton.Joints[JointType.Head].Position).Y - sensor.ScalePosition(skeleton.Joints[JointType.FootLeft].Position).Y *  (1 / 6);
            if (largeurChapeau > 0)
                vetementHead.Width = largeurChapeau;*/
            //Fixe les vetements
            vetement.Stretch = Stretch.Fill;
            vetement.Source = fenetre.VetementSelectionne;

            //Fixe les chapeaux
            vetementHead.Stretch = Stretch.Fill;
            vetementHead.Source = fenetre.ChapeauSelectionne;



            //Fixes les elements visuels sur notre canvas
            Canvas.SetLeft(fenetre.Vetements, mappedPoint.X - (fenetre.Vetements.ActualWidth * 0.4));
            Canvas.SetTop(fenetre.Vetements, mappedPoint.Y);

            Canvas.SetLeft(fenetre.Chapeaux, mappedPoint.X - largeur/5);
            Canvas.SetTop(fenetre.Chapeaux, mappedPoint.Y - hauteur/2.3 );


        }
    }
}
