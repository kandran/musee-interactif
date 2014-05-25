using System.Windows.Controls;

namespace WPFPageSwitch
{
    /// <summary>
    /// Classe statique permettant le changement d'ihm
    /// </summary>
  	public static class Switcher
  	{
    	public static PageSwitcher pageSwitcher;
        /// <summary>
        /// Fonction de changement de page
        /// </summary>
        /// <param name="newPage">Instance vers la nouvelle page</param>
    	public static void Switch(UserControl newPage)
    	{
      		pageSwitcher.Navigate(newPage);
    	}

        /// <summary>
        /// Fonction de changement de page
        /// </summary>
        /// <param name="newPage">Instance vers la nouvelle page</param>
        /// <param name="state"></param>
    	public static void Switch(UserControl newPage, object state)
    	{
      		pageSwitcher.Navigate(newPage, state);
    	}
  	}
}
