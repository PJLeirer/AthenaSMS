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
    /// Interaction logic for AddContactForm.xaml
    /// </summary>
    public partial class AddContactForm : Window
    {




        MainWindow mMainWin;



        public AddContactForm(MainWindow w)
        {
            mMainWin = w;
            InitializeComponent();
            cbNewContactGroup.ItemsSource = mMainWin.mGroupsList;
        }


        public void createButton(object sender, RoutedEventArgs e)
        {

            

            string name = tbAddContactName.Text;
            string phone = tbAddContactPhone.Text;
            int index = cbNewContactGroup.SelectedIndex;

            if (name.Equals("") || name == null)
            {
                lAddContactError.Content = "No Name Entered";
            }
            else if (phone.Equals("") || name == null)
            {
                lAddContactError.Content = "No Phone Number Entered";
            }
            else if (index < 0)
            {
                lAddContactError.Content = "No Group Selected";
            }
            else
            {
                string grpName = cbNewContactGroup.SelectionBoxItem.ToString().Trim();
                mMainWin.mSockMan.mSockSend.AddAthenaContact(name, phone, grpName);
                Hide();
            }

            gridAddContact.IsEnabled = true;

        }




    }
}
