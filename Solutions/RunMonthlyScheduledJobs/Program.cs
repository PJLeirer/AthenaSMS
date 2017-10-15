using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RunMonthlyScheduledJobs
{
    class Program
    {
        // host address and port;
        public static IPAddress hostIP = new IPAddress(new byte[] { 127, 0, 0, 1 });
        public static int hostPort = 11420;

        public static string ClientUserName = "client";
        public static string ClientUserPass = "client";


        public static SyncUser mSyncUser;

        public static SockMan mSockMan;

        public static bool isRunning = true;



        static void Main(string[] args)
        {

            mSyncUser = new SyncUser();

            mSockMan = new SockMan();


        }
    }
}
