using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using Microsoft.Kinect;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.BackgroundRemoval;
using System.Threading;
using System.Media;



namespace WPFPageSwitch
{


    public partial class MainInterface : UserControl, ISwitchable
    {
        /// <summary>
        ///Pour set ou get le statut
        /// </summary>
        public ActionStatus Status { get; set; }

        /// <summary>
        /// Kinect Sensor
        /// </summary>
        KinectSensor sensor = null;

        /// <summary>
        /// Squelette
        /// </summary>
        private Skeleton[] skeletons = new Skeleton[6];

        /// <summary>
        /// Vêtements
        /// </summary>
        public ImageSource VetementSelectionne { get; set; }
        public ImageSource ChapeauSelectionne { get; set; }
 

        /// <summary>
        /// Button Range
        /// </summary>
       // ButtonPosition buttonPoint;


        /// <summary>
        /// Gestionnaire du capteur
        /// </summary>
        private GestionSensors sensorManager = new GestionSensors();

        private Option Op;

        private int i = 0;
        private int j = 0;

        private static MainInterface __instance = null;

        public static MainInterface getInstance()
        {
            if (__instance == null)
                __instance = new MainInterface();
            return __instance;
        }
       
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainInterface()
        {
            Op = Option.getInstance();
            Op.getConfig("Fonds", false);
            Op.getConfig("Chapeaux", false);
            Op.getConfig("Vêtements-2D",false);
            Op.getConfig("Vêtements-3D",false);
            
            VetementSelectionne = new BitmapImage();

            InitializeComponent();
            Loaded += new RoutedEventHandler(this.MainWindow_Loaded);
            Unloaded += new RoutedEventHandler(MainWindow_Unloaded);

            DisplayClothes(i);
            DisplayChapeaux(j);
            //__instance = this;

        }

        /// <summary>
        /// Handles the Unloaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        void MainWindow_Unloaded(object sender, RoutedEventArgs e)
        {
            this.sensorManager.changeStateSensor(GestionSensors.STOP);
        }

        /// <summary>
        /// Handles the Loaded event of the MainWindow control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.RoutedEventArgs"/> instance containing the event data.</param>
        protected void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

            // Check if there kinect connected
            if (sensorManager.haveSensors())
            {
                this.sensorManager.useSensor();
                this.sensorManager.useColorStream(this);
               this.sensorManager.useDepth(this);
                this.sensorManager.useSkeleton(this);
                
            }
            else
            {
                MessageBox.Show("No Device Connected");
            }
        }




        /// <summary>
        /// Met une image dans le controleur d'image
        /// </summary>
        /// <param name="bitmapS">notre image</param>
        public void setImg(BitmapSource bitmapS)
        {
            this.colorimageControl2.Source = bitmapS;
        }

        /// <summary>
        /// Clic sur le bouton de selection du premier chapeau
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selection_Chapeau1(object sender, RoutedEventArgs e)
        {
            Image imgchapeau1bis = this.imgchapeau1.Children[1] as Image;
            this.ChapeauSelectionne = imgchapeau1bis.Source;

            SelectC.Opacity = 0;
            SelectC.Visibility = System.Windows.Visibility.Collapsed;

            chapeau1.Opacity = 0;
            chapeau1.Visibility = System.Windows.Visibility.Collapsed;

            chapeau2.Opacity = 0;
            chapeau2.Visibility = System.Windows.Visibility.Collapsed;


            imgchapeau1.Opacity = 0;
            imgchapeau1.Visibility = System.Windows.Visibility.Collapsed;

            imgchapeau2.Opacity = 0;
            imgchapeau2.Visibility = System.Windows.Visibility.Collapsed;

            PM.Visibility = System.Windows.Visibility.Visible;
            Photo.Visibility = System.Windows.Visibility.Visible;
            Armoire.Visibility = System.Windows.Visibility.Visible;

            CMvR.Visibility = System.Windows.Visibility.Collapsed;
            CMvL.Visibility = System.Windows.Visibility.Collapsed;

            LeftArrow.Opacity = 0;
            RightArrow.Opacity = 0;

            myCanvas2.Opacity = 1;
            colorimageControl2.Opacity = 1;
        }

        /// <summary>
        /// Clic sur le bouton de selection du second chapeau
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selection_Chapeau2(object sender, RoutedEventArgs e)
        {
            Image imgchapeau2bis = this.imgchapeau2.Children[1] as Image;
            this.ChapeauSelectionne = imgchapeau2bis.Source;

            SelectC.Opacity = 0;
            SelectC.Visibility = System.Windows.Visibility.Collapsed;

            chapeau1.Opacity = 0;
            chapeau1.Visibility = System.Windows.Visibility.Collapsed;

            chapeau2.Opacity = 0;
            chapeau2.Visibility = System.Windows.Visibility.Collapsed;


            imgchapeau1.Opacity = 0;
            imgchapeau1.Visibility = System.Windows.Visibility.Collapsed;

            imgchapeau2.Opacity = 0;
            imgchapeau2.Visibility = System.Windows.Visibility.Collapsed;

            PM.Visibility = System.Windows.Visibility.Visible;
            Photo.Visibility = System.Windows.Visibility.Visible;
            Armoire.Visibility = System.Windows.Visibility.Visible;

            CMvR.Visibility = System.Windows.Visibility.Collapsed;
            CMvL.Visibility = System.Windows.Visibility.Collapsed;

            LeftArrow.Opacity = 0;
            RightArrow.Opacity =0;

            myCanvas2.Opacity = 1;
            colorimageControl2.Opacity = 1;
        }

        /// <summary>
        /// Sélection du premier vetement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selection_Vetement1(object sender, RoutedEventArgs e)
        {
            Image imghabit1bis = this.imghabit1.Children[1] as Image;
            this.VetementSelectionne = imghabit1bis.Source;

            SelectV.Opacity = 0;
            SelectV.Visibility = System.Windows.Visibility.Collapsed;

            habit1.Opacity = 0;
            habit1.Visibility = System.Windows.Visibility.Collapsed;

            habit2.Opacity = 0;
            habit2.Visibility = System.Windows.Visibility.Collapsed;

           
            imghabit1.Opacity = 0;
            imghabit1.Visibility = System.Windows.Visibility.Collapsed;

            imghabit2.Opacity = 0;
            imghabit2.Visibility = System.Windows.Visibility.Collapsed;


            PM.Visibility = System.Windows.Visibility.Visible;
            Photo.Visibility = System.Windows.Visibility.Visible;
            Armoire.Visibility = System.Windows.Visibility.Visible;

            HMvR.Visibility = System.Windows.Visibility.Collapsed;
            HMvL.Visibility = System.Windows.Visibility.Collapsed;

            LeftArrow.Opacity = 0;
            RightArrow.Opacity =0;

            myCanvas2.Opacity = 1;
            colorimageControl2.Opacity = 1;

          
        }

        /// <summary>
        /// Sélection du second vetement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Selection_Vetement2(object sender, RoutedEventArgs e)
        {
            Image imghabit2bis = this.imghabit2.Children[1] as Image;
            this.VetementSelectionne = imghabit2bis.Source ;

            SelectV.Opacity = 0;
            SelectV.Visibility = System.Windows.Visibility.Collapsed;

            habit1.Opacity = 0;
            habit1.Visibility = System.Windows.Visibility.Collapsed;

            imghabit1.Opacity = 0;
            imghabit1.Visibility = System.Windows.Visibility.Collapsed;

            imghabit2.Opacity = 0;
            imghabit2.Visibility = System.Windows.Visibility.Collapsed;

            habit2.Opacity = 0;
            habit2.Visibility = System.Windows.Visibility.Collapsed;

            PM.Visibility = System.Windows.Visibility.Visible;
            Photo.Visibility = System.Windows.Visibility.Visible;
            Armoire.Visibility = System.Windows.Visibility.Visible;

            HMvR.Visibility = System.Windows.Visibility.Collapsed;
            HMvL.Visibility = System.Windows.Visibility.Collapsed;

            LeftArrow.Opacity = 0;
            RightArrow.Opacity = 0;
            
            myCanvas2.Opacity = 1;
            colorimageControl2.Opacity = 1;
        }


        //Envoie vers la selection vetements
        /// <summary>
        /// Envoie vers l'interface de selection des vetements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Armoire_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            
            SelectV.Opacity = 1;
            SelectV.Visibility=System.Windows.Visibility.Visible;

            LeftArrow.Opacity = 1;
            RightArrow.Opacity = 1;

            HMvR.Visibility = System.Windows.Visibility.Visible;
            HMvL.Visibility = System.Windows.Visibility.Visible;

            habit1.Opacity = 1;
            imghabit1.Opacity = 1;
            habit1.Visibility = System.Windows.Visibility.Visible;
            imghabit1.Visibility = System.Windows.Visibility.Visible;

            habit2.Opacity = 1;
            imghabit2.Opacity = 1;
            habit2.Visibility = System.Windows.Visibility.Visible;
            imghabit2.Visibility = System.Windows.Visibility.Visible;

            PM.Visibility = System.Windows.Visibility.Collapsed;
            Photo.Visibility = System.Windows.Visibility.Collapsed;
            Armoire.Visibility = System.Windows.Visibility.Collapsed;

            myCanvas2.Opacity = 0;
            colorimageControl2.Opacity = 0;
            
            DisplayClothes(i);
            
        }

        /// <summary>
        /// Envoie vers l'interface de selection des chapeau
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PM_Click(object sender, RoutedEventArgs e)
        {
            SelectC.Opacity = 1;
            SelectC.Visibility = System.Windows.Visibility.Visible;

            LeftArrow.Opacity = 1;
            RightArrow.Opacity = 1;

            CMvR.Visibility = System.Windows.Visibility.Visible;
            CMvL.Visibility = System.Windows.Visibility.Visible;

            chapeau1.Opacity = 1;
            chapeau1.Visibility = System.Windows.Visibility.Visible;

            chapeau2.Opacity = 1;
            chapeau2.Visibility = System.Windows.Visibility.Visible;


            imgchapeau1.Opacity = 1;
            imgchapeau1.Visibility = System.Windows.Visibility.Visible;

            imgchapeau2.Opacity = 1;
            imgchapeau2.Visibility = System.Windows.Visibility.Visible;

            Armoire.Visibility = System.Windows.Visibility.Collapsed;
            Photo.Visibility = System.Windows.Visibility.Collapsed;
            PM.Visibility = System.Windows.Visibility.Collapsed;

            myCanvas2.Opacity = 0;
            colorimageControl2.Opacity = 0;

            DisplayChapeaux(j);
        }


        /// <summary>
        /// Effectue une capteur d'ecran
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save(object sender, RoutedEventArgs e)
        {
            ScreenCapture.EnregistreCaptureBureau();
            Environment.Exit(0);
        }
        /// <summary>
        /// Lorsque le curseur est sur l'armoire 
        /// Met en surbrillance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Armoire_ActionEnter(object sender, ActionEventArgs e)
        {
            
            ArmoireSelected.Opacity = 1;
        }
        /// <summary>
        /// Lorsque le curseur sort de l'armoire
        /// Leve en surbrillance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Armoire_ActionExit(object sender, ActionEventArgs e)
        {
            ArmoireSelected.Opacity = 0;
        }

        /// <summary>
        /// Lorsque le curseur est sur le porte manteau 
        /// Met en surbrillance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PM_ActionEntry(object sender, ActionEventArgs e)
        {
            ChapeauxSelected.Opacity = 1;
        }
        /// <summary>
        /// Lorsque le curseur sort du porte manteau
        /// Leve en surbrillance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PM_ActionExit(object sender, ActionEventArgs e)
        {
            ChapeauxSelected.Opacity = 0;
        }
        /// <summary>
        /// Lorsque le curseur est sur l'appareil photo
        /// Leve en surbrillance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Photo_ActionEnter(object sender, ActionEventArgs e)
        {
            ScrollSelected.Opacity = 1;
        }
        /// <summary>
        /// Lorsque le curseur sort de l'appareil photo
        /// Leve en surbrillance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Photo_ActionExit(object sender, ActionEventArgs e)
        {
            ScrollSelected.Opacity = 0;
        }


        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }


        #endregion
        /// <summary>
        /// Notre timer pour revenir sur l'interface de base apres une photo
        /// </summary>
        private System.Windows.Threading.DispatcherTimer messageTimer;
        /// <summary>
        /// Fonction lancant la photo (changement d'interfac etc)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Photo_Click(object sender, RoutedEventArgs e)
        {
            Photo.Visibility = System.Windows.Visibility.Collapsed;
            Armoire.Visibility = System.Windows.Visibility.Collapsed;
            PM.Visibility = System.Windows.Visibility.Collapsed;
            Screen.Visibility = System.Windows.Visibility.Visible;


            DisplayScreen();
            SoundPlayer simpleSound = new SoundPlayer("C:\\VirtualDressroom\\WPFPageSwitcher\\WPFPageSwitch\\Resources\\Bip.wav");
            simpleSound.Play();

                var Handle = EasyTimer.SetTimeout(() =>
                {


                    ScreenCapture.EnregistreCaptureBureau();



                }, 5000);



            messageTimer = new System.Windows.Threading.DispatcherTimer();
            messageTimer.Tick += new EventHandler(photo_out);
            messageTimer.Interval = new TimeSpan(0, 0, 0, 0, 5100);
            messageTimer.Start();
            
          
        }
        /// <summary>
        /// Remet l'interface de base apres avoir pris une photo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void photo_out(object sender, EventArgs e) 
        {
            Thread.Sleep(3000);
            Photo.Visibility = System.Windows.Visibility.Visible;
            Armoire.Visibility = System.Windows.Visibility.Visible;
            PM.Visibility = System.Windows.Visibility.Visible;
            Screen.Visibility = System.Windows.Visibility.Collapsed;
            messageTimer.Stop();
        }
        /// <summary>
        /// Affiche un des fond aleatoire
        /// </summary>
        private void DisplayScreen()
        {

           

            if (Op.images != null )
            {
                   Random rndNumbers = new Random();
                   int rndNumber = 0;
                   rndNumber = rndNumbers.Next(Op.images.Count);

                   BitmapImage bi3 = new BitmapImage();
                   bi3.BeginInit();

                   bi3.UriSource = new Uri(Op.images[rndNumber]);

                   bi3.EndInit();

                   Image screen = this.Screen.Children[0] as Image;
                   screen.Source = bi3;
            } 
        }
        /// <summary>
        /// Affiche l'interface de selection des vetements
        /// </summary>
        /// <param name="i"></param>
        private void DisplayClothes(int i)
        {
           

            if (i < 0)
            {
                i = Op.obj2D.Count-1;
            }

            //if (i >= Op.obj2D.Count)
            else
            {
                i = i % Op.obj2D.Count;
            }

            int temp = (i + 1) % Op.obj2D.Count;

            if (Op.obj2D != null)
            {
                

                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();
               


                bi3.UriSource = new Uri(Op.obj2D[i]);

                bi3.EndInit();

                Image imghabit1 = this.imghabit1.Children[0] as Image;
                imghabit1.Source = bi3;

                Image imghabit1bis = this.imghabit1.Children[1] as Image;
                imghabit1bis.Source = bi3;

                if (temp >= Op.obj2D.Count)
                {
                    temp = temp % Op.obj2D.Count;
                }

               

                BitmapImage bi4 = new BitmapImage();
                bi4.BeginInit();

                bi4.UriSource = new Uri(Op.obj2D[temp]);

                bi4.EndInit();

                
                Image imghabit2 = this.imghabit2.Children[0] as Image;
                imghabit2.Source = bi4;

                Image imghabit2bis = this.imghabit2.Children[1] as Image;
                imghabit2bis.Source = bi4;
            }
        }

        /// <summary>
        /// Affiche l'interface de selection des chepeau
        /// </summary>
        /// <param name="j"></param>
        private void DisplayChapeaux(int j)
        {


            if (j < 0)
            {
                j = Op.chapeaux.Count - 1;
            }

            //if (j >= Op.chapeaux.Count)
            else
            {
                j = j % Op.chapeaux.Count;
            }

            int temp = j + 1;

            if (Op.chapeaux != null)
            {


                BitmapImage bi3 = new BitmapImage();
                bi3.BeginInit();



                bi3.UriSource = new Uri(Op.chapeaux[j]);

                bi3.EndInit();

                Image imgchapeau1 = this.imgchapeau1.Children[0] as Image;
                imgchapeau1.Source = bi3;

                Image imgchapeau1bis = this.imgchapeau1.Children[1] as Image;
                imgchapeau1bis.Source = bi3;

                if (temp >= Op.chapeaux.Count)
                {
                    temp = temp % Op.chapeaux.Count;
                }



                BitmapImage bi4 = new BitmapImage();
                bi4.BeginInit();

                bi4.UriSource = new Uri(Op.chapeaux[temp]);

                bi4.EndInit();


                Image imgchapeau2 = this.imgchapeau2.Children[0] as Image;
                imgchapeau2.Source = bi4;

                Image imgchapeau2bis = this.imgchapeau2.Children[1] as Image;
                imgchapeau2bis.Source = bi4;
            }
        }
        /// <summary>
        /// Rotation des choix de vetement (droite)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveRight(object sender, RoutedEventArgs e)
        {
           // i++;
            Op.rotate_right(Op.obj2D);
            DisplayClothes(i);

        }
        /// <summary>
        /// Rotation des choix de vetement (gauche)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveLeft(object sender, RoutedEventArgs e)
        {
           // i--;
            Op.rotate_left(Op.obj2D);     
                
           DisplayClothes(i);
        }
        /// <summary>
        /// Rotation des choix de chapeau (gauche)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CMoveLeft(object sender, RoutedEventArgs e)
        {
            Op.rotate_left(Op.chapeaux);
            DisplayChapeaux(j);
        }
        /// <summary>
        /// Rotation des choix de chapeau (droite)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CMoveRight(object sender, RoutedEventArgs e)
        {
            Op.rotate_right(Op.chapeaux);
            DisplayChapeaux(j);
        }

       


       


    }


}
