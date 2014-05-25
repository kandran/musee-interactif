using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace WPFPageSwitch
{
	public partial class MainMenu : UserControl, ISwitchable
	{

       

		public MainMenu()
		{
            
			InitializeComponent();

           
		}


        /// <summary>
        /// Clic sur le bouton option: on lance l'interface des options
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
		private void optionButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Switcher.Switch(Option.getInstance());
		}

        //private void ShowMessageBox(string title, string message, MessageBoxIcon icon)
        //{
            //MessageBoxChildWindow messageWindow =
            //    new MessageBoxChildWindow(title, message, MessageBoxButtons.Ok, icon);

            //messageWindow.Show();
        //}

        #region Event For Child Window
        private void loginWindowForm_SubmitClicked(object sender, EventArgs e)
        {
            //ShowMessageBox("Login Successful", "Welcome, " + loginForm.NameText, MessageBoxIcon.Information);

        }

        private void registerForm_SubmitClicked(object sender, EventArgs e)
        {
        }


        #endregion

        #region ISwitchable Members
        public void UtilizeState(object state)
        {
            throw new NotImplementedException();
        }

        
        #endregion

        private void image1_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {

        }
        /// <summary>
        /// Clic sur le bouton demarrer, on lance l'interface principale
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Demarrer_Click(object sender, RoutedEventArgs e)
        {
            Switcher.Switch(MainInterface.getInstance());
        }
		
	}
}