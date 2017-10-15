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
    /// Interaction logic for EditScheduledJobForm.xaml
    /// </summary>
    public partial class EditScheduledJobForm : Window
    {

        private MainWindow mMainWin;

        public EditScheduledJobForm(MainWindow w, int sch, string name)
        {
            mMainWin = w;
            InitializeComponent();

            if (sch >= 0 && sch < 4)
            {
                cbJobSchedule.SelectedIndex = sch;
                new Thread(new ThreadStart(
                    delegate()
                    {
                        mMainWin.mSockMan.mSockSend.GetJobInfo(name);
                    }
                  )).Start();
                
            }

            

        }


        public void FieldUpdater(string name, string loc, string file, int sch)
        {
            new Thread(new ThreadStart(
                delegate()
                {
                    Dispatcher.Invoke(
                            System.Windows.Threading.DispatcherPriority.Loaded,
                            new Action(
                                delegate()
                                {
                                    tbJobName.Text = name;
                                    tbJobLocation.Text = loc;
                                    tbJobFile.Text = file;
                                    cbJobSchedule.SelectedIndex = sch;
                                    mMainWin.ErrorLogUpdater(name + " " + loc + " " + file + " " + sch);
                                }
                            ));
                }
              )).Start();
        }


        private void Update_Job(object sender, RoutedEventArgs e)
        {
            if (cbJobSchedule.SelectedIndex < 0)
            {
                lbErrorMsg.Content = "No Schedule Selected!";
            }
            else if (tbJobName.Text.Equals(""))
            {
                lbErrorMsg.Content = "Blank Fields Not Allowed!";
            }
            else if (tbJobLocation.Text.Equals(""))
            {
                lbErrorMsg.Content = "Blank Fields Not Allowed!";
            }
            else if (tbJobFile.Text.Equals(""))
            {
                lbErrorMsg.Content = "Blank Fields Not Allowed!";
            }
            else
            {
                mMainWin.mSockMan.mSockSend.EditScheduledJob(cbJobSchedule.SelectedIndex, tbJobName.Text, tbJobLocation.Text, tbJobFile.Text);
            }
        }

        private void Delete_Job(object sender, RoutedEventArgs e)
        {
            if (tbJobName.Equals(""))
            {
                lbErrorMsg.Content = "What Job?";
            }
            else if (cbJobSchedule.SelectedIndex < 0)
            {
                lbErrorMsg.Content = "No Schedule Selected!";
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("Delete Job '" + tbJobName.Text + "'?", "Athena", MessageBoxButton.YesNo);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    mMainWin.mSockMan.mSockSend.DeleteScheduledJob(tbJobName.Text, cbJobSchedule.SelectedIndex);
                    Hide();
                }
            }
            
        }

        private void Close_Window(object sender, RoutedEventArgs e)
        {
            new Thread(new ThreadStart(
                    delegate()
                    {
                        mMainWin.mSockMan.mSockSend.GetScheduledJobs(0);
                        mMainWin.mSockMan.mSockSend.GetScheduledJobs(1);
                        mMainWin.mSockMan.mSockSend.GetScheduledJobs(2);
                        mMainWin.mSockMan.mSockSend.GetScheduledJobs(3);
                    }
                  )).Start();
            Hide();
        }



        


    }
}
