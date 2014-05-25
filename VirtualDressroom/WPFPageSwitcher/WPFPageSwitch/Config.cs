using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WPFPageSwitch
{
    /// <summary>
    /// Classe permettant certaines configuration
    /// </summary>
    class Config //to do
    {
        /// <summary>
        /// Renvoie le ratio entre la largeur de l'ecran et la largeur du flux
        /// </summary>
        /// <param name="resolutionXFlux">largeur du flux</param>
        /// <returns>ratio</returns>
        public static float proportionScaleX(int resolutionXFlux)
        {
            return Screen.PrimaryScreen.Bounds.Width / resolutionXFlux;
        }
        /// <summary>
        /// Renvoie le ratio entre la hauteur de l'ecran et la hauteur du flux
        /// </summary>
        /// <param name="resolutionXFlux">hauteur du flux</param>
        /// <returns>ratio</returns>
        public static float proportionScaleY(int resolutionXFlux)
        {
            return Screen.PrimaryScreen.Bounds.Height / resolutionXFlux;
        }
        /// <summary>
        /// Adaptation de la valeur en x            
        /// </summary>
        /// <param name="x">valeur a adapter</param>
        /// <param name="resolutionXFlux">valeur en x de la resolution</param>
        /// <returns></returns>
        public static int scaleX(int x, int resolutionXFlux)
        {
            //return (int) (x * proportionScaleX(resolutionXFlux));
            return x * 3;
        }
        /// <summary>
        /// Adaptation de la valeur en y
        /// </summary>
        /// <param name="y">valeur a adapter</param>
        /// <param name="resolutionYFlux">valeur de y de la resolution</param>
        /// <returns></returns>
        public static int scaleY(int y, int resolutionYFlux)
        {
            // return (int) (y * proportionScaleY(resolutionYFlux));
            return (int)(y * 2.25);
        }
    }
}
