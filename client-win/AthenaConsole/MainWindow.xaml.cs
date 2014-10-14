using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AthenaConsole
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        // setup class members

        public String AthenaDir = @"C:\Athena\";

        public ArrayList modemList = new ArrayList();

        // all thread loops is this
        public static bool isRunning = true;

        // host address and port;
        // public static IPAddress hostIP = new IPAddress(new byte[] { 192, 168, 1, 71 }); // remote
        public static IPAddress hostIP = IPAddress.Any; // local
        public static int hostPort = 11420;

        // socket manager
        public SockMan mSockMan;


        // child windows
        public LoginWin mLoginWin;
        public AddUserForm mAddUserForm;
        public EditUserForm mEditUserForm;
        public Options mOptionsForm;
        public AddContactForm mAddContactForm;
        public EditContactForm mEditContactForm;
        public AddGroupForm mAddGroupForm;
        public EditScheduledJobForm mEditScheduledJobForm;


        //  bound to UI controls, must use dispatcher to update
        public ObservableCollection<string> mAllUsersList;
        public ObservableCollection<string> mOnlineUsersList;
        public ObservableCollection<string> mContactsList;

        public ObservableCollection<string> mSocketInList;
        public ObservableCollection<string> mSocketOutList;
        public ObservableCollection<string> mErrorLogList;

        public ObservableCollection<string> mModemLogList;

        public ObservableCollection<string> mOutgoingTextsList;
        public ObservableCollection<string> mFailedOutgoingList;

        public ObservableCollection<ComboBoxItem> mGroupsList;
        public ObservableCollection<ComboBoxItem> mEditGroupList;


        public ObservableCollection<ComboBoxItem> mModemsList;


        public ObservableCollection<ComboBoxItem> mUnscheduleJobsList;
        public ObservableCollection<ComboBoxItem> mDailyJobsList;
        public ObservableCollection<ComboBoxItem> mWeeklyJobsList;
        public ObservableCollection<ComboBoxItem> mMonthlyJobsList;



        // initialize variables for config info
        public string hostAddress = "";


        public int mTabCurrent = 0;
        public const int mTabAdmin = 1;
        public const int mTabJobs = 2;
        public const int mTabReports = 3;
        public const int mTabTexts = 4;
        public const int mTabModem = 5;
        public const int mTabLogs = 6;


        public const int sysMsgOff = 0;
        public const int sysMsgPhone = 1;
        public const int sysMsgText = 2;
        public int sysMsgMode = 0;






        // construct  main window
        public MainWindow()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            this.DataContext = this;


            mAllUsersList = new ObservableCollection<string>();
            mOnlineUsersList = new ObservableCollection<string>();
            mContactsList = new ObservableCollection<string>();

            mSocketInList = new ObservableCollection<string>();
            mSocketOutList = new ObservableCollection<string>();
            mErrorLogList = new ObservableCollection<string>();
            mModemLogList = new ObservableCollection<string>();

            mOutgoingTextsList = new ObservableCollection<string>();
            mFailedOutgoingList = new ObservableCollection<string>();

            mGroupsList = new ObservableCollection<ComboBoxItem>();
            mEditGroupList = new ObservableCollection<ComboBoxItem>();

            mModemsList = new ObservableCollection<ComboBoxItem>();

            mUnscheduleJobsList = new ObservableCollection<ComboBoxItem>();
            mDailyJobsList = new ObservableCollection<ComboBoxItem>();
            mWeeklyJobsList = new ObservableCollection<ComboBoxItem>();
            mMonthlyJobsList = new ObservableCollection<ComboBoxItem>();


            lbAllUsers.ItemsSource = mAllUsersList;
            lbOnlineUsers.ItemsSource = mOnlineUsersList;
            lbSockIn.ItemsSource = mSocketInList;
            lbSockOut.ItemsSource = mSocketOutList;
            lbErrLog.ItemsSource = mErrorLogList;
            lbModemLog.ItemsSource = mModemLogList;

            lbOutgoingTexts.ItemsSource = mOutgoingTextsList;
            lbFailedOutgoing.ItemsSource = mFailedOutgoingList;

            cbGroupList.ItemsSource = mGroupsList;
            mGroupsList.Add(new ComboBoxItem { Name = "load", Content = "Loading..." });
            cbEditGroupList.ItemsSource = mEditGroupList;
            mEditGroupList.Add(new ComboBoxItem { Name = "load", Content = "Loading..." });


            cbModemSelect.ItemsSource = mModemsList;
            mModemsList.Add(new ComboBoxItem { Name = "load", Content = "Loading..." });

            lbContacts.ItemsSource = mContactsList;

            cbUnscheduledJobs.ItemsSource = mUnscheduleJobsList;
            cbDailyJobs.ItemsSource = mDailyJobsList;
            cbWeekyJobs.ItemsSource = mWeeklyJobsList;
            cbMonthlyJobs.ItemsSource = mMonthlyJobsList;




            mainGridEnabled(false);

            //cbGroupList.IsEnabled = false;



            if (ReadConfigFile())
            {
                StartSockets();
            }

            
            

        }


        public bool ReadConfigFile()
        {
            bool X = false;

            // do config
            string[] cfg;
            try
            {
                cfg = File.ReadAllLines(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\athena_console_cfg.txt");
                foreach (String line in cfg)
                {
                    if (!line.StartsWith("//"))
                    {
                        string[] splitTxt = line.Split(new string[] { ":" }, StringSplitOptions.None);
                        if (splitTxt[0].Equals("ADDRESS"))
                        {
                            hostAddress = splitTxt[1];
                            if (!IPAddress.TryParse(hostAddress, out hostIP))
                            {
                                IPAddress[] hostInfo = Dns.GetHostAddresses(hostAddress);
                                hostIP = hostInfo[0];
                            }
                        }
                    }
                }

                if (hostAddress.Equals(""))
                {
                    ShowOptions("Information Missing!", hostAddress);
                }
                else
                {
                    X = true;
                }
            }
            catch (Exception e)
            {
                ShowOptions(e.Message, null);
            }
            return X;
        }



        public void StartSockets()
        {
            new Thread(new ThreadStart(
                delegate()
                {
                    mSockMan = new SockMan(this);
                }
                )).Start();
        }






        // UI Update methods

        //TODO add lock to updaters, should be one at a time

        public void ShowOptions(string s, string h)
        {
            if (s == null)
            {
                s = "";
            }
            new Thread(new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Loaded,
                            new Action(
                                delegate()
                                {
                                    Options optWin = new Options(this, h);
                                    optWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                    optWin.lErrorMsg.Content = s;
                                    optWin.Show();
                                }
                            ));
                    }
                )).Start();
        }


        public void ShowLogin()
        {
            new Thread(new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Loaded,
                            new Action(
                              delegate()
                              {
                                  // do login
                                  mLoginWin = new LoginWin(this);
                                  mLoginWin.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                                  mLoginWin.Activate();
                                  mLoginWin.Show();
                              }
                          ));
                    }
                )).Start();
        }






        public void OnlineUserUpdater(ArrayList list)
        {
            Thread thread = new Thread(
                new ThreadStart(
                  delegate()
                  {
                      Dispatcher.Invoke(
                        System.Windows.Threading.DispatcherPriority.Normal,
                        new Action(
                          delegate()
                          {
                              mOnlineUsersList.Clear();
                              foreach (string s in list)
                              {
                                  mOnlineUsersList.Add(s);

                              }
                          }
                      ));
                  }
            ));
            thread.Start();
        }


        public void AllUserUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mAllUsersList.Clear();
                                  foreach (string s in al)
                                  {
                                    mAllUsersList.Add(s);
                                  }
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void SocketInUpdater(string s)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mSocketInList.Add(s);
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void SocketOutUpdater(string s)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mSocketOutList.Add(s);
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void ErrorLogUpdater(string s)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mErrorLogList.Add(s);
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void ModemLogUpdater(string s)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mModemLogList.Add(s);
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void OutgoingTextsListUpdater(ArrayList list)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  foreach (string s in list)
                                  {
                                      mOutgoingTextsList.Add(s);
                                  }
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void FailedTextsListUpdater(ArrayList list)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  foreach (string s in list)
                                  {
                                      mFailedOutgoingList.Add(s);
                                  }
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void GroupsListUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  int indx = cbGroupList.SelectedIndex;
                                  mGroupsList.Clear();
                                  for (int i = 0; i < al.Count; i++)
                                  {
                                      mGroupsList.Add(new ComboBoxItem { Content = (string)al[i] });
                                  }
                                  if (indx < 0)
                                  {
                                      cbGroupList.SelectedIndex = 0;
                                  }
                                  else
                                  {
                                      if (indx < mGroupsList.Count)
                                      {
                                          cbGroupList.SelectedIndex = indx;
                                      }
                                      else
                                      {
                                          cbGroupList.SelectedIndex = 0;
                                      }
                                  }

                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void EditGroupListUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  int indx = cbEditGroupList.SelectedIndex;
                                  mEditGroupList.Clear();
                                  for (int i = 0; i < al.Count; i++)
                                  {
                                      mEditGroupList.Add(new ComboBoxItem { Content = (string)al[i] });
                                  }
                                  if (indx < 0)
                                  {
                                      cbEditGroupList.SelectedIndex = 0;
                                  }
                                  else
                                  {
                                      if (indx < mEditGroupList.Count)
                                      {
                                          cbEditGroupList.SelectedIndex = indx;
                                      }
                                      else
                                      {
                                          cbEditGroupList.SelectedIndex = 0;
                                      }
                                  }
                              }
                          ));
                      }
                ));
            thread.Start();
        }




        public void ModemsListUpdater(int c)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  if (c < 1)
                                  {
                                      mModemsList.Clear();
                                      mModemsList.Add(new ComboBoxItem { Content = "No Modems" });
                                      cbModemSelect.SelectedIndex = 0;
                                  }
                                  else
                                  {
                                      if (cbGroupList.SelectedIndex < 0)
                                      {
                                          mModemsList.Clear();
                                          for (int i = 0; i < c; i++)
                                          {
                                              mModemsList.Add(new ComboBoxItem { Content = "Modem " + (c - 1) });
                                              cbModemSelect.SelectedIndex = 0;
                                          }
                                      }
                                  }
                              }
                          ));
                      }
                ));
            thread.Start();
        }


        public void ModemLogListUpdater(string s)
        {
            Thread thread = new Thread(
                    new ThreadStart(
                      delegate()
                      {
                          Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mModemLogList.Add(s);
                              }
                          ));
                      }
                ));
            thread.Start();

        }


        public void MsgModeRadioUpdater(int n)
        {
            Thread thread = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  switch (n)
                                  {
                                      case 2:
                                          rEmailMode.IsChecked = true;
                                          break;
                                      case 1:
                                          rTextMode.IsChecked = true;
                                          break;
                                      default:
                                          rOffMode.IsChecked = true;
                                          break;
                                  }
                              }
                          ));
                    }
            ));
            thread.Start();
        }


        public void ContactsListUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mContactsList.Clear();
                                  foreach (string s in al)
                                  {
                                      mContactsList.Add(s);
                                  }
                              }
                          ));
                    }
            ));
            thread.Start();
        }


        public void UnscheduledJobsListUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mUnscheduleJobsList.Clear();
                                  foreach (string s in al)
                                  {
                                      mUnscheduleJobsList.Add(new ComboBoxItem { Content = s });
                                      cbUnscheduledJobs.SelectedIndex = 0;
                                  }
                              }
                          ));
                    }
            ));
            thread.Start();
        }


        public void DailyJobsListUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mDailyJobsList.Clear();
                                  foreach (string s in al)
                                  {
                                      mDailyJobsList.Add(new ComboBoxItem { Content = s });
                                      cbDailyJobs.SelectedIndex = 0;
                                  }
                              }
                          ));
                    }
            ));
            thread.Start();
        }


        public void WeeklyJobsListUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mWeeklyJobsList.Clear();
                                  foreach (string s in al)
                                  {
                                      mWeeklyJobsList.Add(new ComboBoxItem { Content = s });
                                      cbWeekyJobs.SelectedIndex = 0;
                                  }
                              }
                          ));
                    }
            ));
            thread.Start();
        }


        public void MonthlyJobsListUpdater(ArrayList al)
        {
            Thread thread = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  mMonthlyJobsList.Clear();
                                  foreach (string s in al)
                                  {
                                      mMonthlyJobsList.Add(new ComboBoxItem { Content = s });
                                      cbMonthlyJobs.SelectedIndex = 0;
                                  }
                              }
                          ));
                    }
            ));
            thread.Start();
        }


        public void DayTotalsUpdater(int og, int ic)
        {
            Thread thread = new Thread(
                new ThreadStart(
                    delegate()
                    {
                        Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Normal,
                            new Action(
                              delegate()
                              {
                                  lbOutgoingTotals.Content = "Out: " + og;
                                  lbIncomingTotals.Content = "In: " + ic;
                              }
                          ));
                    }
            ));
            thread.Start();
        }






        // Menu  handlers

        public void OnMenuOptionsClicked(object sender, RoutedEventArgs e)
        {
            mOptionsForm = new Options(this, hostAddress);
            mOptionsForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mOptionsForm.Show();
        }


        public void OnMenuExitClicked(object sender, RoutedEventArgs e)
        {
            ShutDown();
        }












        
        // button handlers

        public void OnAddUserClicked(object sender, RoutedEventArgs e)
        {
            mAddUserForm = new AddUserForm(this);
            mAddUserForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mAddUserForm.Show();

        }

        public void OnEditUserClicked(object sender, RoutedEventArgs e)
        {
            if (lbAllUsers.SelectedIndex >= 0)
            {
                mEditUserForm = new EditUserForm(this, lbAllUsers.SelectedValue.ToString());
                mEditUserForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mEditUserForm.Show();
            }

        }

        public void OnKickUserClicked(object sender, RoutedEventArgs e)
        {
            int num = lbOnlineUsers.SelectedIndex;
            if (num >= 0)
            {
                string conn = lbOnlineUsers.SelectedValue.ToString();

                MessageBoxResult result = MessageBox.Show("Are you sure you want to kick connection '" + conn + "' ?", "Athena", MessageBoxButton.YesNo);
                if(result.Equals(MessageBoxResult.Yes))
                {
                    mSockMan.mSockSend.KickConnection(num);
                    mSockMan.mSockSend.GetOnlineUsers();
                }
                //TODO UPGRADE
               // MessageBox.Show("This function will be available in the next upgrade!", "Athena", MessageBoxButton.OK);
            }
        }






        public void OnSingleModeClicked(object sender, RoutedEventArgs e)
        {
            gGroupTxt.Visibility = Visibility.Hidden;
            gSingleTxt.Visibility = Visibility.Visible;
        }

        public void OnGroupModeClicked(object sender, RoutedEventArgs e)
        {
            gSingleTxt.Visibility = Visibility.Hidden;
            gGroupTxt.Visibility = Visibility.Visible;
        }

        public void OnSendSingleTextClicked(object sender, RoutedEventArgs e)
        {
            gSingleTxt.IsEnabled = false;
            if (tbPhoneNumber.Text.Equals(""))
            {
                gSingleTxt.IsEnabled = true;
                MessageBox.Show("No Phone Number Entered!", "Athena", MessageBoxButton.OK);
            }
            else if (tbSingleMessage.Text.Equals(""))
            {
                gSingleTxt.IsEnabled = true;
                MessageBox.Show("No Message Entered!", "Athena", MessageBoxButton.OK);
            }
            else
            {
                MessageBoxResult singleResult = MessageBox.Show("Are you sure you want to send \r\n'" + tbSingleMessage.Text +"'\r\n to " + tbPhoneNumber.Text, "Athena", MessageBoxButton.YesNo);
                if (singleResult == MessageBoxResult.Yes)
                {
                    mSockMan.mSockSend.SendSingleText(tbPhoneNumber.Text, tbSingleMessage.Text);
                    gSingleTxt.IsEnabled = true;
                }
                else
                {
                    MessageBox.Show("Failed to send Message!", "Athena", MessageBoxButton.OK);
                }
                gSingleTxt.IsEnabled = true;
            }
        }

        public void OnSendGroupTextClicked(object sender, RoutedEventArgs e)
        {
            gGroupTxt.IsEnabled = false;
            ComboBoxItem cbItem = (ComboBoxItem)cbGroupList.SelectedItem;
            if (cbItem == null)
            {
                MessageBox.Show("No Group Selected!", "Athena", MessageBoxButton.OK);
                gGroupTxt.IsEnabled = true;
            }
            else if (cbItem.Name.Equals("load"))
            {
                MessageBox.Show("No Group Selected!", "Athena", MessageBoxButton.OK);
                gGroupTxt.IsEnabled = true;
            }
            else if (tbGroupMessage.Text.Equals(""))
            {
                MessageBox.Show("No Message Entered!", "Athena", MessageBoxButton.OK);
                gGroupTxt.IsEnabled = true;
            }
            else
            {
                MessageBoxResult groupResult = MessageBox.Show("Are you sure you want to send \r\n'" + tbGroupMessage.Text + "'\r\n to Group '" + cbGroupList.SelectionBoxItem.ToString() + "'", "Athena", MessageBoxButton.YesNo);
                if (groupResult.Equals(MessageBoxResult.Yes))
                {
                    mSockMan.mSockSend.SendGroupText(cbGroupList.SelectionBoxItem.ToString(), tbGroupMessage.Text);
                    gGroupTxt.IsEnabled = true;
                }
                gSingleTxt.IsEnabled = true;
                gGroupTxt.IsEnabled = true;
            }
        }





        public void OnGetDayTotals(object sender, SelectionChangedEventArgs e)
        {
            IList list = e.AddedItems;

            foreach (DateTime dt in list)
            {
                string sent = dt.Month + @"/" + dt.Day + @"/" + dt.Year;
                mSockMan.mSockSend.GetDayTotals(sent);
            }
        }


        public void OnGenerateReportClicked(object sender, RoutedEventArgs e)
        {
            gridReports.IsEnabled = false;
            int rType = cbReportType.SelectedIndex;
            if (rType >= 0)
            {
                if (rbEmail.IsChecked == true)
                {
                    // do email
                    MessageBoxResult emailResult = MessageBox.Show("Do you want to send a " + cbReportType.SelectionBoxItem.ToString() + " report via " + rbEmail.Content + "?", "Athena", MessageBoxButton.YesNo);
                    if(emailResult.Equals(MessageBoxResult.Yes))
                    {
                        mSockMan.mSockSend.GetReport(rType, 0);
                    }
                }
                else
                {
                    // do printer
                   MessageBox.Show("Printing will be supported in next upgrade", "Athena", MessageBoxButton.OK);                   

                }
            }
            else
            {
                MessageBox.Show("No Report Type selected", "Athena", MessageBoxButton.OK);
            }

            gridReports.IsEnabled = true;
        }


        public void OnAddContactClicked(object sender, RoutedEventArgs e)
        {
            mAddContactForm = new AddContactForm(this);
            mAddContactForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mAddContactForm.Show();
        }


        public void OnEditContactClicked(object sender, RoutedEventArgs e)
        {
            if (lbContacts.SelectedIndex >= 0)
            {
                mEditContactForm = new EditContactForm(this, lbContacts.SelectedValue.ToString());
                mEditContactForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mEditContactForm.Show();
            }
        }


        public void OnAddGroupClicked(object sender, RoutedEventArgs e)
        {
            mAddGroupForm = new AddGroupForm(this);
            mAddGroupForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            mAddGroupForm.Show();
        }


        public void OnDeleteGroupClicked(object sender, RoutedEventArgs e)
        {
            if (cbEditGroupList.SelectedIndex >= 0)
            {
                string name = cbEditGroupList.SelectionBoxItem.ToString();
                MessageBoxResult result = MessageBox.Show("Delete Group '" + name + "' ?", "Athena", MessageBoxButton.YesNo);
                if(result.Equals(MessageBoxResult.Yes))
                {
                    new Thread(new ThreadStart(
                        delegate()
                        {
                            mSockMan.mSockSend.DeleteGroup(name);
                        }
                      )).Start();
                }
                
            }
        }





        public void OnRunSingleDailyJob(object sender, RoutedEventArgs e)
        {
            if(cbDailyJobs.SelectedIndex >= 0)
            {
                string item = cbDailyJobs.SelectionBoxItem.ToString();
                MessageBoxResult result = MessageBox.Show("Are you sure you want to run '" + item + "' ?", "Athena", MessageBoxButton.YesNo);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    new Thread(new ThreadStart(
                        delegate()
                        {
                            mSockMan.mSockSend.RunSingleJob(item);
                        }
                      )).Start();
                }
            }
        }


        public void OnRunSingleWeeklyJob(object sender, RoutedEventArgs e)
        {
            if (cbWeekyJobs.SelectedIndex >= 0)
            {
                string item = cbWeekyJobs.SelectionBoxItem.ToString();
                MessageBoxResult result = MessageBox.Show("Are you sure you want to run '" + item + "' ?", "Athena", MessageBoxButton.YesNo);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    new Thread(new ThreadStart(
                        delegate()
                        {
                            mSockMan.mSockSend.RunSingleJob(item);
                        }
                      )).Start();
                }
            }
        }


        public void OnRunSingleMonthlyJob(object sender, RoutedEventArgs e)
        {
            if (cbMonthlyJobs.SelectedIndex >= 0)
            {
                string item = cbMonthlyJobs.SelectionBoxItem.ToString();
                MessageBoxResult result = MessageBox.Show("Are you sure you want to run '" + item + "' ?", "Athena", MessageBoxButton.YesNo);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    new Thread(new ThreadStart(
                        delegate()
                        {
                            mSockMan.mSockSend.RunSingleJob(item);
                        }
                      )).Start();
                }
            }
        }

        public void OnRunScheduledDailyJobs(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Run All Daily Jobs", "Athena", MessageBoxButton.YesNo);
            if (result.Equals(MessageBoxResult.Yes))
            {
                mSockMan.mSockSend.RunScheduledJobs(1);
            }
        }

        public void OnRunScheduledWeeklyJobs(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Run All Weekly Jobs", "Athena", MessageBoxButton.YesNo);
            if (result.Equals(MessageBoxResult.Yes))
            {
                mSockMan.mSockSend.RunScheduledJobs(2);
            }
        }

        public void OnRunScheduledMonthlyJobs(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Run All Monthly Jobs", "Athena", MessageBoxButton.YesNo);
            if (result.Equals(MessageBoxResult.Yes))
            {
                mSockMan.mSockSend.RunScheduledJobs(3);
            }
        }

        public void OnRunUnscheduledJob(object sender, RoutedEventArgs e)
        {
            if (cbUnscheduledJobs.SelectedIndex >= 0)
            {
                string item = cbUnscheduledJobs.SelectionBoxItem.ToString();
                MessageBoxResult result = MessageBox.Show("Are you sure you want to run '" + item + "'?", "Athena", MessageBoxButton.YesNo);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    new Thread(new ThreadStart(
                        delegate()
                        {
                            mSockMan.mSockSend.RunSingleJob(item);
                        }
                      )).Start();
                }
            }
        }

        public void OnRunAllUnscheduledJobs(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Run All Unscheduled Jobs", "Athena", MessageBoxButton.YesNo);
            if (result.Equals(MessageBoxResult.Yes))
            {
                mSockMan.mSockSend.RunScheduledJobs(0);
            }
        }





        public void OnAddUnscheduledJob(object sender, RoutedEventArgs e)
        {
            AddScheduledJobForm addSchJob = new AddScheduledJobForm(this, 0);
            addSchJob.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addSchJob.Show();
        }

        public void OnAddScheduledDailyJob(object sender, RoutedEventArgs e)
        {
            AddScheduledJobForm addSchJob = new AddScheduledJobForm(this, 1);
            addSchJob.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addSchJob.Show();
        }

        public void OnAddScheduledWeeklyJob(object sender, RoutedEventArgs e)
        {
            AddScheduledJobForm addSchJob = new AddScheduledJobForm(this, 2);
            addSchJob.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addSchJob.Show();
        }

        public void OnAddScheduledMonthlyJob(object sender, RoutedEventArgs e)
        {
            AddScheduledJobForm addSchJob = new AddScheduledJobForm(this, 3);
            addSchJob.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            addSchJob.Show();
        }



        public void OnEditUnscheduledJob(object sender, RoutedEventArgs e)
        {
            if (cbUnscheduledJobs.SelectedIndex >= 0)
            {
                ComboBoxItem item = (ComboBoxItem)cbUnscheduledJobs.SelectedItem;
                mEditScheduledJobForm = new EditScheduledJobForm(this, 0, item.Content.ToString());
                mEditScheduledJobForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mEditScheduledJobForm.Show();
            }
        }

        public void OnEditScheduledDailyJob(object sender, RoutedEventArgs e)
        {
            if (cbDailyJobs.SelectedIndex >= 0)
            {
                ComboBoxItem item = (ComboBoxItem)cbDailyJobs.SelectedItem;
                mEditScheduledJobForm = new EditScheduledJobForm(this, 1, item.Content.ToString());
                mEditScheduledJobForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mEditScheduledJobForm.Show();
            }
        }

        public void OnEditScheduledWeeklyJob(object sender, RoutedEventArgs e)
        {
            if (cbWeekyJobs.SelectedIndex >= 0)
            {
                ComboBoxItem item = (ComboBoxItem)cbWeekyJobs.SelectedItem;
                mEditScheduledJobForm = new EditScheduledJobForm(this, 2, item.Content.ToString());
                mEditScheduledJobForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mEditScheduledJobForm.Show();
            }
        }

        public void OnEditScheduledMonthlyJob(object sender, RoutedEventArgs e)
        {
            if (cbMonthlyJobs.SelectedIndex >= 0)
            {
                ComboBoxItem item = (ComboBoxItem)cbMonthlyJobs.SelectedItem;
                mEditScheduledJobForm = new EditScheduledJobForm(this, 3, item.Content.ToString());
                mEditScheduledJobForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                mEditScheduledJobForm.Show();
            }
        }



        public void OnDeleteUnscheduledJob(object sender, RoutedEventArgs e)
        {

        }
        
        public void OnDeleteScheduledDailyJob(object sender, RoutedEventArgs e)
        {

        }

        public void OnDeleteScheduledWeeklyJob(object sender, RoutedEventArgs e)
        {

        }

        public void OnDeleteScheduledMonthlyJob(object sender, RoutedEventArgs e)
        {

        }







        //RadioButton handlers
        public void OnOutgoingRadioClicked(object sender, RoutedEventArgs e)
        {
            mSockMan.mSockSend.GetTextLog(tbTextLogNumber.Text, "out");
        }


        public void OnIncomingRadioClicked(object sender, RoutedEventArgs e)
        {
            mSockMan.mSockSend.GetTextLog(tbTextLogNumber.Text, "in");
        }


        public void OnMsgModeClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                RadioButton btn = (RadioButton)sender;
                switch (btn.Content.ToString())
                {
                    case "Email":
                        mSockMan.mSockSend.SetSysMsgMode(2);
                        break;
                    case "Text":
                        mSockMan.mSockSend.SetSysMsgMode(1);
                        break;
                    default: //off
                        mSockMan.mSockSend.SetSysMsgMode(0);
                        break;
                }
            }
            catch (Exception x)
            {
                ErrorLogUpdater("MainWindow.OnMsgModeClicked: " + x.Message + "\r\n" + x.StackTrace);
            }
        }











        // tab loads
        private void TabItem_Admin_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            //admin tab
            if (!mTabCurrent.Equals(mTabAdmin))
            {
                mTabCurrent = mTabAdmin;
                lUserName.Content = "'" + mSockMan.mSyncUser.getUserName() + "'";
                new Thread(new ThreadStart(
                    delegate()
                    {
                        mSockMan.mSockSend.GetAllGroups();
                        mSockMan.mSockSend.GetAllUsers();
                        mSockMan.mSockSend.GetOnlineUsers();
                        mSockMan.mSockSend.GetSysMsgMode();
                        mSockMan.mSockSend.GetContacts();
                    }
                  )).Start();
            }
        }

        private void TabItem_Jobs_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (!mTabCurrent.Equals(mTabJobs))
            {
                mTabCurrent = mTabJobs;
                new Thread(new ThreadStart(
                    delegate()
                    {
                        mSockMan.mSockSend.GetScheduledJobs(0);
                        mSockMan.mSockSend.GetScheduledJobs(1);
                        mSockMan.mSockSend.GetScheduledJobs(2);
                        mSockMan.mSockSend.GetScheduledJobs(3);
                    }
                  )).Start();
            }
        }

        private void TabItem_Reports_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (!mTabCurrent.Equals(mTabReports))
            {
                mTabCurrent = mTabReports;
                cbReportType.SelectedIndex = 0;

                

                new Thread(new ThreadStart(
                    delegate()
                    {
                        DateTime t = DateTime.Now;
                        string sent = t.Month + @"/" + t.Day + @"/" + t.Year;
                        mSockMan.mSockSend.GetDayTotals(sent);
                    }
                  )).Start();
            }
        }

        private void TabItem_Texts_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (!mTabCurrent.Equals(mTabTexts))
            {
                mTabCurrent = mTabTexts;
                // data tab
                new Thread(new ThreadStart(
                    delegate()
                    {
                        mSockMan.mSockSend.GetTextLog("", "out");
                        mSockMan.mSockSend.GetFailedLog();
                    }
                  )).Start();
            }
        }

        private void TabItem_Modem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (!mTabCurrent.Equals(mTabModem))
            {
                mTabCurrent = mTabModem;
                // log tab
                new Thread(new ThreadStart(
                    delegate()
                    {
                        mSockMan.mSockSend.GetModems();
                        //get modem file(s)
                    }
                  )).Start();
            }
        }

        private void TabItem_Info_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (!mTabCurrent.Equals(mTabLogs))
            {
                mTabCurrent = mTabLogs;
                // log tab
                new Thread(new ThreadStart(
                    delegate()
                    {
                        //
                    }
                  )).Start();
            }
        }





        private void tbPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            
            TextBox tb = (TextBox)sender;
            string txt = tb.Text;
            long result;
            if(long.TryParse(txt, out result))
            {
                if (result < 10000000000)
                {
                    tb.Foreground = new SolidColorBrush(Colors.Black);
                    bSingleSend.IsEnabled = true;
                }
                else
                {
                    tb.Foreground = new SolidColorBrush(Colors.Red);
                    bSingleSend.IsEnabled = false;
                }
            }
            else
            {
                tb.Foreground = new SolidColorBrush(Colors.Red);
                bSingleSend.IsEnabled = false;
            }



        }




        private void tbTextLogNumber_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key.Equals(Key.Enter))
            {
                mSockMan.mSockSend.GetTextLog(tbTextLogNumber.Text, "out");
            }
        }






        // general methods




        public static void DoLogging(string s)
        {
            using (FileStream file = File.Open(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\athena_console_log.txt", FileMode.Append))
            {
                using (StreamWriter writer = new StreamWriter(file))
                {
                    writer.Write(s + "\r\n");
                    writer.Flush();
                    writer.Close();
                }
                file.Close();
            }
        }

        /*
        public void readConfigFile()
        {
            try
            {

                string cs;
                using (StreamReader cfg = new StreamReader(AthenaDir + @"conf\athenasms.conf"))
                {
                    cs = cfg.ReadToEnd();
                    cfg.Close();
                }
                string[] configInfo = cs.Split(new string[] { "\r\n" }, StringSplitOptions.None);

                for (int i = 0; i < configInfo.Length; i++)
                {
                    if (configInfo[i].Length > 0 && !configInfo[i].StartsWith("//"))
                    {
                        string[] tmp = configInfo[i].Split(new string[] { ":" }, StringSplitOptions.None);
                        if (tmp[0].Trim().Equals("MODEM") && tmp.Length > 1)
                        {
                            modemList.Add(tmp[1].Trim());
                        }
                    }
                }

            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Athena", MessageBoxButton.OK);
            }

        }
        */

        public void mainGridEnabled(bool b)
        {
            gridMain.IsEnabled = b;
        }


        public void ShutDown()
        {
            isRunning = false;
            if (mSockMan != null)
            {
                if (mSockMan.mClient.Client != null)
                {
                    if (mSockMan.mClient.Client.Connected)
                    {
                        mSockMan.mSockSend.CloseConnection();
                        mSockMan.mClient.Close();
                    }
                }
            }
            Environment.Exit(0);
        }



        // overrides

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
            base.OnClosing(e);
            ShutDown();
        }




    }
}
