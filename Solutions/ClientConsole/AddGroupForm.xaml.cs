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
    /// Interaction logic for AddGroupForm.xaml
    /// </summary>
    public partial class AddGroupForm : Window
    {

        MainWindow mMainWin;

        public AddGroupForm(MainWindow w)
        {
            mMainWin = w;
            InitializeComponent();
        }


        public void OnAddGroupClicked(object sender, RoutedEventArgs e)
        {
            string addGroupName = tbAddGroupName.Text;
            if (!addGroupName.Equals(""))
            {
                MessageBoxResult result = MessageBox.Show("Add Group '" + addGroupName + "' ?", "Athena", MessageBoxButton.YesNo);
                if (result.Equals(MessageBoxResult.Yes))
                {
                    new Thread(new ThreadStart(
                        delegate()
                        {
                            mMainWin.mSockMan.mSockSend.AddGroup(addGroupName);
                        }
                      )).Start();
                    
                    Hide();
                }
            }
        }


    }
}
