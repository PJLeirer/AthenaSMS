using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunDailyScheduledJobs
{
    public class SyncUser
    {
        private int userID = 0;
        private String userName = null;
        private int userLevel = 0;
        private bool loggedIn = false;
        public bool isLoggedIn()
        {
            if (loggedIn && userID > 0 && userLevel > 0 && userName != null)
            {
                return true;
            }
            else
            {
                if (!loggedIn)
                {
                    Console.WriteLine("loggedIn is false");
                }
                if (userID < 1)
                {
                    Console.WriteLine("userID less than 1");
                }
                if (userLevel < 1)
                {
                    Console.WriteLine("userLevel less than 1");
                }
                if (userName == null)
                {
                    Console.WriteLine("userName is null");
                }
                return false;
            }
        }
        public int getUserID()
        {
            return userID;
        }
        public String getUserName()
        {
            return userName;
        }
        public int getUserLevel()
        {
            return userLevel;
        }

        //used by sockrecv
        public void assignUserInfo(int ID, string Name, int Level)
        {
            Console.WriteLine("assigning local user info");
            this.userID = ID;
            this.userName = Name;
            this.userLevel = Level;
            if (ID > 0)
            {
                loggedIn = true;
            }
        }
    }
}
