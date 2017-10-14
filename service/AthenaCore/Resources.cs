using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaCore
{
    public static class Resources
    {

        // read from config file
        public static String appTitle = null;
        public static String companyName = null;
        public static String emailHost = null;
        public static String emailSender = null;
        public static String emailRecipient = null;
        public static String mailHeader = null;

        //public static AthenaService.SqlDb mSqlDb;
        public static String mSqlHost;
        public static string mSqlUser = "sa";
        public static string mSqlPass = "";

        public static Groups mGroups;

        //public static AthenaService.SocketManager mSocketManager;

        //public static AthenaService.ModemManager mModemManager;

        public static String AthenaDir = @"C:\Athena\";
        public static bool notifyOnStartup = false;


        public static String FileDir = AthenaDir + @"jobs\";
        public static String fileJobType = "daily_notifications-";
        public static String[] todaysHoldNotices;
        public static ArrayList todaysHoldTexts;
        //public static int todaysHoldCount;


        

        public static bool isRunning = true;

        public static string[] seperator1 = new string[] { "\r\n" };
        public static string[] seperator2 = new string[] { ":" };








        



    }
}
