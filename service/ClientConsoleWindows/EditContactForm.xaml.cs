using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for EditContactForm.xaml
    /// </summary>
    public partial class EditContactForm : Window
    {

        MainWindow mMainWin;
        string contactName;


        public ObservableCollection<ComboBoxItem> mEditContactGroupList;




        public EditContactForm(MainWindow w, string c)
        {
            mMainWin = w;
            contactName = c;
            InitializeComponent();

            lContactName.Content = contactName;
            mEditContactGroupList = new ObservableCollection<ComboBoxItem>();
            cbEditContactGroup.ItemsSource = mEditContactGroupList;
            new Thread(new ThreadStart(
                delegate()
                {
                    mMainWin.mSockMan.mSockSend.GetAllGroups();
                    mMainWin.mSockMan.mSockSend.GetContactInfo(c);
                }
              )).Start();

        }



        public void OnUpdateNumberClicked(object sender, RoutedEventArgs e)
        {
            gridEditContact.IsEnabled = false;
            if (tbEditContactNumber.Text.Equals(""))
            {
                MessageBox.Show("No Number Entered!", "Athena", MessageBoxButton.OK);
            }
            else
            {
                mMainWin.mSockMan.mSockSend.EditAthenaContact(contactName, "Phone_Number", tbEditContactNumber.Text);
            }
            gridEditContact.IsEnabled = true;
        }


        public void OnUpdateGroupClicked(object sender, RoutedEventArgs e)
        {
            gridEditContact.IsEnabled = false;
            if (cbEditContactGroup.SelectedIndex < 0)
            {
                MessageBox.Show("No Group Selected!", "Athena", MessageBoxButton.OK);
            }
            else
            {
                mMainWin.mSockMan.mSockSend.EditAthenaContact(contactName, "Text_Group", cbEditContactGroup.SelectionBoxItem.ToString());
            }
            gridEditContact.IsEnabled = true;
        }


        public void OnDeleteContactClicked(object sender, RoutedEventArgs e)
        {
            gridEditContact.IsEnabled = false;

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete '" + lContactName.Content.ToString().Trim() + "'", "Athena", MessageBoxButton.YesNo);
            if (result.Equals(MessageBoxResult.Yes))
            {
                mMainWin.mSockMan.mSockSend.DeleteAthenaContact(lContactName.Content.ToString().Trim());
                Hide();
            }
            gridEditContact.IsEnabled = true;
        }



        public void ContactGroupsUpdater(ArrayList list)
        {
            new Thread(new ThreadStart(
                delegate()
                {
                    Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Loaded,
                            new Action(
                                delegate()
                                {
                                    mEditContactGroupList.Clear();
                                    foreach (string s in list)
                                    {
                                        mEditContactGroupList.Add(new ComboBoxItem { Content = s });
                                    }
                                }
                            ));
                }
              )).Start();
        }



        public void FieldsUpdater(string Cname, string Cphone, string Cgroup)
        {

            new Thread(new ThreadStart(
                delegate()
                {
                    Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Loaded,
                            new Action(
                                delegate()
                                {
                                    lContactName.Content = Cname;
                                    tbEditContactNumber.Text = Cphone;

                                    for (int i = 0; i < mEditContactGroupList.Count; i++)
                                    {
                                        ComboBoxItem item = mEditContactGroupList[i];
                                        if (item.Content.ToString().Trim().Equals(Cgroup.Trim()))
                                        {
                                            cbEditContactGroup.SelectedIndex = i;
                                        }
                                    }
                                }
                            ));
                }
              )).Start();
        }







    }
}
