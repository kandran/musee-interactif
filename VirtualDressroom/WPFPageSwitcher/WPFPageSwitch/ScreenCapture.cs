using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
namespace WPFPageSwitch
{
    /// <summary>
    /// Classe permettant de faire des captures d'écran.
    /// 
    /// </summary>
    class ScreenCapture
    {
        /// <summary>
        /// Fonction de capture du bureau windows dans l'état actuel.
        /// </summary>
        /// <returns>Capture d'ecran au format Bitmap</returns>
        public static Bitmap CapturerBureauWindows()
        {
            Rectangle tailleTotale; // Taille totale du bureau de Windows
            Bitmap image;           // Capture de l'écran

            // Récupérer la taille totale du bureau de Windows
            tailleTotale = System.Windows.Forms.Screen.AllScreens[0].Bounds;
            for (int i = 1; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
            {
                tailleTotale = Rectangle.Union(tailleTotale, System.Windows.Forms.Screen.AllScreens[i].Bounds);
            }

            // Créer une image de la taille du bureau de Windows
            image = new Bitmap(tailleTotale.Width, tailleTotale.Height);

            // Créer un Graphics à partir de l'image et faire la capture dans celui-ci
            using (Graphics g = Graphics.FromImage(image))
            {
                g.CopyFromScreen(tailleTotale.Location, Point.Empty, tailleTotale.Size);
            }

            return image;
        }

        /// <summary>
        /// Fonction qui effectue une capture d'écran et l'enregistre sur le disque dur.
        /// </summary>
        /// <example>Voila un exemple d'utilisation de la méthode : <c>ScreenCapture.EnregistreCaptureBureau();</c></example>

        public static void EnregistreCaptureBureau()
        {
            //capture
            Bitmap capture;
            capture = ScreenCapture.CapturerBureauWindows();
            //creation de l'url d'enregistrement
            string test = "C:\\VirtualDressroom\\WPFPageSwitcher\\WPFPageSwitch\\Screenshots\\";
            test += DateTime.Now.ToString("ddMMyyyyhhmmss");
            test += ".jpg";
            //enregistrement
            capture.Save(@test);
        }
    }
}
