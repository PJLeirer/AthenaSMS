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
    /// Interaction logic for EditUserForm.xaml
    /// </summary>
    public partial class EditUserForm : Window
    {
        string mUserName;

        MainWindow mMainWin;

        public EditUserForm(MainWindow w, string n)
        {
            mMainWin = w;
            mUserName = n;
            InitializeComponent();


            //change to get users info first, then update labels

            lEditingUser.Content = "User:  " + mUserName;

        }



        //button handlers

        public void changePasswordClicked(object sender, RoutedEventArgs e)
        {
            gridEditUser.IsEnabled = false;
            if (pbNewPass.Password.Equals("") || pbNewPass.Password == null)
            {

                lEditUserError.Content = "No New Password";
                gridEditUser.IsEnabled = true;
            }
            else if (pbConPass.Password.Equals("") || pbConPass.Password == null)
            {

                lEditUserError.Content = "No Confirm Password";
                gridEditUser.IsEnabled = true;
            }
            else if (!pbNewPass.Password.Equals(pbConPass.Password))
            {

                lEditUserError.Content = "Passwords Do Not Match";
                gridEditUser.IsEnabled = true;
            }
            else
            {
                mMainWin.mSockMan.mSockSend.ChangeUserPass(mUserName, pbNewPass.Password);
                if (mMainWin.mSockMan.changedPass)
                {
                    pbNewPass.Password = "";
                    pbConPass.Password = "";
                    lEditUserError.Content = "Changed Users Password";
                    gridEditUser.IsEnabled = true;
                }
                else
                {
                    lEditUserError.Content = "Failed to Change Password";
                    gridEditUser.IsEnabled = true;
                }

            }
        }

        public void changeLevelClicked(object sender, RoutedEventArgs e)
        {
            gridEditUser.IsEnabled = false;
            if (cbNewLevel.SelectedIndex < 0)
            {
                gridEditUser.IsEnabled = true;
            }
            else
            {
                mMainWin.mSockMan.mSockSend.ChangeUserLevel(mUserName, cbNewLevel.SelectedIndex + 1);
                if (mMainWin.mSockMan.changedLevel)
                {
                    lEditUserError.Content = "Changed User Level";
                    gridEditUser.IsEnabled = true;
                }
                else
                {
                    lEditUserError.Content = "Failed to Change Level";
                    gridEditUser.IsEnabled = true;
                }
            }
        }

        public void deleteUserClicked(object sender, RoutedEventArgs e)
        {
            gridEditUser.IsEnabled = false;
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete user '" + mUserName + "'?", "Athena", MessageBoxButton.YesNo);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    mMainWin.mSockMan.mSockSend.DeleteAthenaUser(mUserName);
                    if (mMainWin.mSockMan.deletedUser)
                    {
                        Hide();
                        mMainWin.mSockMan.mSockSend.GetAllUsers();
                    }
                    else
                    {
                        lEditUserError.Content = "Failed to delete user";
                    }
                    break;
            }
            gridEditUser.IsEnabled = true;
        }



    }
}
