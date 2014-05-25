using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Shapes;

namespace WPFPageSwitch
{

    /// <summary>
    /// Classe de gestion des bouton
    /// </summary>
    public static class ButtonExtension
    {
        /// <summary>
        /// Recupere les positions d'un bouton
        /// </summary>
        /// <param name="button">Bouton dont on veux les paramettre</param>
        /// <returns>Valeur des coins du bouton</returns>
        public static ButtonPosition GetPosition(this Button button)
        {
            Point buttonPosition = button.PointToScreen(new Point());
            return new ButtonPosition { Left = buttonPosition.X, 
                Right = buttonPosition.X + button.ActualWidth, 
                Top = buttonPosition.Y,
                Bottom = buttonPosition.Y + button.Height };
        }
    }
}
