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
    /// Interaction logic for AddScheduledJobForm.xaml
    /// </summary>
    public partial class AddScheduledJobForm : Window
    {


        MainWindow mMainWin;

        public AddScheduledJobForm(MainWindow w, int initSchedule = 0)
        {
            mMainWin = w;
            InitializeComponent();
            cbSchedules.SelectedIndex = initSchedule;
        }



        public void OnAddJobClicked(object sender, RoutedEventArgs e)
        {
            gridAddScheduledJob.IsEnabled = false;
            if (cbSchedules.SelectedIndex < 0)
            {
                MessageBox.Show("No Schedule Selected!", "Athena", MessageBoxButton.OK);
            }
            else if(tbJobName.Text.Equals(""))
            {
                MessageBox.Show("No Name Entered!", "Athena", MessageBoxButton.OK);
            }
            else if(tbJobLoc.Text.Equals(""))
            {
                MessageBox.Show("No Location Entered!", "Athena", MessageBoxButton.OK);
            }
            else if(tbJobFile.Text.Equals(""))
            {
                MessageBox.Show("No File Entered!", "Athena", MessageBoxButton.OK);
            }
            else
            {
                
                mMainWin.mSockMan.mSockSend.AddScheduledJob(cbSchedules.SelectedIndex, tbJobName.Text, tbJobLoc.Text, tbJobFile.Text);
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
                //MessageBox.Show("Job '" + tbJobName.Text + "' Added!", "Athena", MessageBoxButton.OK);
                   
            }
            gridAddScheduledJob.IsEnabled = true;
        }



    }
}
