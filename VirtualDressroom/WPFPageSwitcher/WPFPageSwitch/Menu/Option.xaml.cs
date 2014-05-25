using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
/// <sumary>
/// Classe de gestions des options de notre application
/// </summary>
namespace WPFPageSwitch
{
    public partial class Option : System.Windows.Controls.UserControl, ISwitchable
    {


        public  List<string> images { get ; set; }
        public  List<string> obj2D { get ; set; }
        public  List<string> obj3D { get ; set; }
        public  List<string> chapeaux { get; set; }

        List<String> text = new List<string>();
        //singleton
        protected static Option __instance=null;

        /// <summary>
        /// Recupere une instance de la classe d'option (singleton)
        /// </summary>
        /// <returns>instance</returns>
        public static Option getInstance()
        {
            if( __instance==null)
                __instance=new Option();
            return __instance;
        }
        /// <summary>
        /// Constructeur de notre classe
        /// </summary>
        protected Option()
        {
            images = new List<string>();
            obj2D = new List<string>();
            obj3D = new List<string>();
            chapeaux = new List<string>();

            // Required to initialize variables
            InitializeComponent();


        }



        /// <summary>
        /// Bouton d'ajout par la boite de dialogue des fichiers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AjVet_Click(object sender, System.EventArgs e)
        {
            BoiteDial();
            comboBox1_Initialized(sender, e);
        }




        /// <summary>
        /// Boite de dialogue
        /// </summary>
        private void BoiteDial()
        {
            Stream myStream = null;
            int canInsert = 0;
            int i = 0;
            OpenFileDialog openFileDialog1 = new OpenFileDialog();


            openFileDialog1.InitialDirectory = "c:\\";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((myStream = openFileDialog1.OpenFile()) != null)
                    {
                        using (myStream)
                        {

                            foreach (string str in File.ReadAllLines(@"C:\VirtualDressroom\WPFPageSwitcher\WPFPageSwitch\config.txt"))
                            {
                                text.Add(str);
                            }

                            for (i = 0; i < text.Count; i++)
                            {
                                if (canInsert == 1)
                                {
                                    text.Insert(i, openFileDialog1.FileName);


                                    canInsert = 0;

                                }

                                if (String.Compare(text[i], comboBox2.Text) == 0)
                                {
                                    canInsert++;

                                }

                            }

                            File.WriteAllText(@"C:\VirtualDressroom\WPFPageSwitcher\WPFPageSwitch\config.txt", "");

                            foreach (string str in text)
                            {
                                File.AppendAllText(@"C:\VirtualDressroom\WPFPageSwitcher\WPFPageSwitch\config.txt", str + Environment.NewLine);
                            }

                            text.Clear();


                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }


        //region Iswhitchable
        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Switcher.Switch(new MainMenu());
        }
        #endregion





        //initialisations des champs

        /// <summary>
        /// Initialisation des champs (categorie)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_Initialized(object sender, EventArgs e)
        {

            comboBox2.Items.Clear();

            comboBox2.Items.Add("Chapeaux");
            comboBox2.Items.Add("Fonds");
            comboBox2.Items.Add("Vêtements-2D");
            comboBox2.Items.Add("Vêtements-3D");

        }
        /// <summary>
        /// Initialisation des champs (item)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_Initialized(object sender, EventArgs e)
        {
            int i;
            comboBox1.Items.Clear();
            getConfig(comboBox2.Text, true);

            if (comboBox2.Text == "Chapeaux")
            {
                for (i = 0; i < chapeaux.Count; i++)
                {
                    comboBox1.Items.Add(chapeaux[i]);
                }
            }

            if (comboBox2.Text == "Fonds")
            {
                for (i = 0; i < images.Count; i++)
                {
                    comboBox1.Items.Add(images[i]);
                }
            }

            if (comboBox2.Text == "Vêtements-2D")
            {
                for (i = 0; i < obj2D.Count; i++)
                {
                    comboBox1.Items.Add(obj2D[i]);
                }
            }

            if (comboBox2.Text == "Vêtements-3D")
            {
                for (i = 0; i < obj3D.Count; i++)
                {
                    comboBox1.Items.Add(obj3D[i]);
                }
            }

        }




        /// <summary>
        /// Recuperation des donnes depuis le fichier de configurations
        /// </summary>
        /// <param name="param"></param>
        /// <param name="clear"></param>
        public void getConfig(string param, bool clear)
        {

            bool sel = false;
            string temp;

            if (clear)
            {
                images.Clear();
                obj2D.Clear();
                obj3D.Clear();
                chapeaux.Clear();
            }



            using (var sr = new StreamReader(@"C:\VirtualDressroom\WPFPageSwitcher\WPFPageSwitch\config.txt"))
            {
                sr.BaseStream.Position = 0;

                while (!sr.EndOfStream)
                {
                    temp = sr.ReadLine();

                    if (param == "Chapeaux")
                    {
                        if (temp == "end")
                        {
                            sel = false;
                        }

                        if (sel)
                        {
                            chapeaux.Add(temp);
                        }

                        if (temp == "Chapeaux")
                        {
                            sel = true;
                        }


                    }

                    if (param == "Fonds")
                    {
                        if (temp == "end")
                        {
                            sel = false;
                        }

                        if (sel)
                        {
                            images.Add(temp);
                        }

                        if (temp == "Fonds")
                        {
                            sel = true;
                        }



                    }

                    if (param == "Vêtements-2D")
                    {
                        if (temp == "end")
                        {
                            sel = false;
                        }

                        if (sel)
                        {
                            obj2D.Add(temp);
                        }

                        if (temp == "Vêtements-2D")
                        {
                            sel = true;
                        }



                    }

                    if (param == "Vêtements-3D")
                    {
                        if (temp == "end")
                        {
                            sel = false;
                        }

                        if (sel)
                        {
                            obj3D.Add(temp);
                        }

                        if (temp == "Vêtements-3D")
                        {
                            sel = true;
                        }



                    }


                }
            }

        }

        /// <summary>
        /// Supression de l'item selectionné
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            int i;

            foreach (string str in File.ReadAllLines(@"C:\VirtualDressroom\WPFPageSwitcher\WPFPageSwitch\config.txt"))
            {
                text.Add(str);
            }

            for (i = 0; i < text.Count; i++)
            {
                if (comboBox1.Text == text[i] )
                {
                    text.RemoveAt(i);
                }

            }

            StreamWriter sw = new StreamWriter(@"C:\VirtualDressroom\WPFPageSwitcher\WPFPageSwitch\config.txt", false);

            for (i = 0; i < text.Count; i++)
            {
                sw.WriteLine(text[i]);
            }

            sw.Close();
            text.Clear();

            comboBox1_Initialized(sender, e);

        }


        /// <summary>
        /// Re-initialise la liste
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox2_DropDownClosed(object sender, EventArgs e)
        {
            comboBox1_Initialized(sender, e);

        }
        /// <summary>
        /// recuperation des données sur le vetemnt choisi
        /// </summary>
        /// <returns>source du vetement</returns>
        private System.Windows.Media.ImageSource setVetementImg()
        {
            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();

            if (comboBox1.Text != null && comboBox1.Text !="")
            {

                bi3.UriSource = new Uri(comboBox1.Text);

                bi3.EndInit();
                return bi3;
            }

            return null;
        }


        /// <summary>
        /// Affiche le vetement ou le fond selectionne
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {
            Image vetement = this.stackPanel1.Children[0] as Image;
            vetement.Source = setVetementImg();
        }
        /// <summary>
        /// rotation d'une liste a gauche
        /// </summary>
        /// <param name="l"></param>
        public void rotate_left(List<string> l)
        {

            string first = l[0];
              l.RemoveAt(0);
            l.Add(first);
        }
        /// <summary>
        /// rotation d'une liste a droite
        /// </summary>
        /// <param name="l"></param>
        public void rotate_right(List<string> l)
       {
           string last = l[l.Count - 1];
           l.RemoveAt(l.Count - 1);
           l.Insert(0, last);
       }
    }
}
