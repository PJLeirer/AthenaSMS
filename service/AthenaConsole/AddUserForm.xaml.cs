using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AddUserForm.xaml
    /// </summary>
    public partial class AddUserForm : Window
    {


        MainWindow mMainWin;



        public AddUserForm(MainWindow w)
        {
            mMainWin = w;
            InitializeComponent();


        }



        public void createButton(object sender, RoutedEventArgs e)
        {
            gridAddUser.IsEnabled = false;
            string name = tbAddUserName.Text;
            string pass = pbAddUserPass.Password;
            int lvl = cbAddUserLevel.SelectedIndex+1;
            
            if(name.Equals("") || name==null)
            {
                lAddUserError.Content = "No Username";
                gridAddUser.IsEnabled = true;
            }
            else if(pass.Equals("") || pass==null)
            {
                lAddUserError.Content = "No Password";
                gridAddUser.IsEnabled = true;
            }
            else if(lvl<1 || lvl>9)
            {
                lAddUserError.Content = "Level must be 1-9";
                gridAddUser.IsEnabled = true;
            }
            else
            {
                mMainWin.mSockMan.mSockSend.AddAthenaUser(name, pass, lvl);
                if (mMainWin.mSockMan.addedUser)
                {
                    Hide();
                    MessageBox.Show("Successfully Added User '"+name+"'", "Athena", MessageBoxButton.OK);
                    mMainWin.mSockMan.mSockSend.GetAllUsers();
                }
                else
                {
                    lAddUserError.Content = "Failed to add user!";
                    gridAddUser.IsEnabled = true;
                }
            }
        }


    }
}
