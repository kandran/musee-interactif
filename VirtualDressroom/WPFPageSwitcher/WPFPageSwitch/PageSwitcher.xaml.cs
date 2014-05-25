using System;
using System.Windows;
using System.Windows.Controls;

namespace WPFPageSwitch
{
    /// <summary>
    /// Interaction logic for Window.xaml
    /// </summary>
    public partial class PageSwitcher : Window
    {
        /// <summary>
        /// Initialisation du switcher et lancement du menu principal
        /// </summary>
        public PageSwitcher()
        {
            InitializeComponent();
            Switcher.pageSwitcher = this;
            Switcher.Switch(new MainMenu());
            
        }

      
        /// <summary>
        /// Fonction effectuant le chagement de page
        /// </summary>
        /// <param name="nextPage">instance vers la nouvelle page</param>
        public void Navigate(UserControl nextPage)
        {
            this.Content = nextPage;
        }
        /// <summary>
        /// Fonction effectuant le chagement de page
        /// </summary>
        /// <param name="nextPage">instance vers la nouvelle page</param>
        /// <param name="state"></param>
        public void Navigate(UserControl nextPage, object state)
        {
            this.Content = nextPage;
            ISwitchable s = nextPage as ISwitchable;

            if (s != null)
                s.UtilizeState(state);
            else
                throw new ArgumentException("NextPage is not ISwitchable! "
                  + nextPage.Name.ToString());
        }
    }
}
