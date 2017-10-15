using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AthenaConsole
{
    /// <summary>
    /// Interaction logic for LoginWin.xaml
    /// </summary>
    public partial class LoginWin : Window
    {

        MainWindow mMainWin;

        public LoginWin(MainWindow w)
        {
            mMainWin = w;
            InitializeComponent();
        }


        


        // UI handlers
        public void loginButton(object sender, RoutedEventArgs e)
        {
            lLoginErrorMessage.Content = "Logging in...";
            //gridLogin.IsEnabled = false;
            bLoginBtn.IsEnabled = false;
            new Thread(new ThreadStart(
                delegate()
                {
                    Dispatcher.Invoke(
                      System.Windows.Threading.DispatcherPriority.Loaded,
                      new Action(
                        delegate()
                        {
                            if (tbUserName.Text.Equals("") || tbUserName.Text == null)
                            {
                                lLoginErrorMessage.Content = "No Username Entered!";
                                gridLogin.IsEnabled = true;
                                bLoginBtn.IsEnabled = true;
                            }
                            else if (pbUserPass.Password.Equals("") || pbUserPass.Password == null)
                            {
                                lLoginErrorMessage.Content = "No Password Entered!";
                                gridLogin.IsEnabled = true;
                                bLoginBtn.IsEnabled = true;
                            }
                            else
                            {
                                if (mMainWin.mSockMan.mSockSend.Login(tbUserName.Text, pbUserPass.Password))
                                {
                                    //
                                    if (mMainWin.mSockMan.mSyncUser.isLoggedIn())
                                    {
                                        mMainWin.mainGridEnabled(true);
                                        Hide();

                                        mMainWin.mSockMan.loginEvent.Set();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Error Logging In", "Athena", MessageBoxButton.OK);
                                    }
                                }
                                else
                                {
                                    lLoginErrorMessage.Content = "Username or Password is Incorrect!";
                                    gridLogin.IsEnabled = true;
                                    bLoginBtn.IsEnabled = true;
                                }
                            }
                        }
                    ));
                }
            )).Start();
            
            
        }



        // overrides

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            base.OnClosing(e);
            mMainWin.ShutDown();
        }


    }
}
