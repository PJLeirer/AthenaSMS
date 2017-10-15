using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for Options.xaml
    /// </summary>
    public partial class Options : Window
    {

        MainWindow mMainWin;

        public Options(MainWindow w, String addr)
        {
            InitializeComponent();

            mMainWin = w;
            if (addr != null)
            {
                tbAddress.Text = addr;
            }

            

        }


        public void UpdateClicked(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            gridOptions.IsEnabled = false;

            if (tbAddress.Text.Equals("") || tbAddress.Text == null)
            {
                lErrorMsg.Content = "No Address Set";
                gridOptions.IsEnabled = true;
            }
            else
            {
                using (FileStream file = File.Open(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\athena_console_cfg.txt", FileMode.OpenOrCreate))
                {
                    using (StreamWriter writer = new StreamWriter(file))
                    {
                        string fd =
                                "// Athena Console Config file.\r\n" +
                                "// Server Address\r\n" +
                                "ADDRESS:" + tbAddress.Text + "\r\n" +
                                "\r\n\r\n\r\n"
                            ;
                        writer.Write(fd);
                        writer.Close();
                    }
                    file.Close();
                }

                Hide();
                MessageBox.Show("Successfully updated Setting\r\nPlease Restart!", "Athena", MessageBoxButton.OK);
                if (mMainWin.mSockMan != null)
                {
                    mMainWin.ShutDown();
                }
            }


        }



        

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (mMainWin.mSockMan.mSyncUser != null)
            {
                if (mMainWin.mSockMan.mSyncUser.isLoggedIn())
                {
                    Hide();
                    base.OnClosing(e);
                }
                else
                {
                    e.Cancel = true;
                    base.OnClosing(e);
                    mMainWin.ShutDown();
                }
            }
            else
            {
                e.Cancel = true;
                base.OnClosing(e);
                mMainWin.ShutDown();
            }
        }






    }
}
