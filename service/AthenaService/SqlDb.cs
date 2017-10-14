using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaService
{
    class SqlDb
    {

        // custumizable for client
        private String dBase = "athenasms";
        private String dbUser = "sa";  //TODO change to new sql user i created
        private String dbPass = "7grinch5"; // and pass

        private SqlConnection mConnection;

        public String amReady = "Not Connected!";

        String[] seperator1 = new String[] { "\r\n" };
        String[] seperator2 = new String[] { "," };
        String[] seperator3 = new String[] { " " };


        //SqlDataReader mReader;
        //SqlCommand mCommand;

        String host;


        ArrayList groupList;

        public SqlDb()
        {
            if (Program.mSqlHost != null)
            {
                host = Program.mSqlHost;
            }
            else
            {
                host = @"ATHENASMS\ATHENASQL";
            }

            mConnection = new SqlConnection("integrated security=SSPI;data source=" + host + ";Persist Security Info=false;UID=" + dbUser + ";PWD=" + dbPass + ";Initial Catalog=" + dBase + ";");

            if (Connect())
            {
                amReady = "Connected!";
            }
            if (!Check())
            {
                amReady += " Database Modified!";
            }
        }


        public bool isReady()
        {
            try
            {
                if (mConnection.State == ConnectionState.Open)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Program.doLogFile("SqlDb.isReady(): " + e.Message + "\r\n" + e.StackTrace, 0);
                return false;
            }
        }

        public bool Connect()
        {
            try
            {
                mConnection.Open();
                return true;
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.dbConnect: " + e.Message + "\r\n" + e.StackTrace, 0);
                return false;
            }
        }

        public bool Disconnect()
        {
            if (isReady())
            {
                try
                {
                    mConnection.Close();
                    return true;
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.dbDisconnect: " + e.Message + "\r\n" + e.StackTrace, 0);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool Check()
        {
            //bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {

                try
                {

                    bool isUsers = false;
                    bool isOutgoing = false;
                    bool isIncoming = false;
                    bool isContacts = false;
                    bool isErrors = false;
                    bool isFailed = false;

                    using (SqlCommand mCommand = new SqlCommand("select name from " + dBase + "..sysobjects where xtype = 'U';", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            while (mReader.Read())
                            {
                                String temp = mReader["name"].ToString().Trim();
                                if (temp.Equals("Users"))
                                {
                                    isUsers = true;
                                }
                                else if (temp.Equals("OutgoingTexts"))
                                {
                                    isOutgoing = true;
                                }
                                else if (temp.Equals("IncomingTexts"))
                                {
                                    isIncoming = true;
                                }
                                else if (temp.Equals("Contacts"))
                                {
                                    isContacts = true;
                                }
                                else if (temp.Equals("Errors"))
                                {
                                    isErrors = true;
                                }
                                else if (temp.Equals("FailedOutgoing"))
                                {
                                    isFailed = true;
                                }
                            }
                            mReader.Close();
                            mCommand.Dispose();
                        }
                    }
                    if (!isUsers)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.Users ("
                                + "Name varchar(50) NOT NULL, "
                                + "Pass varchar(50) NOT NULL, "
                                + "Level int NOT NULL"
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                            mCommand.Dispose();
                        }
                        
                        //x1 = true;

                        using (SqlCommand mCommand = new SqlCommand("insert into Users ( Name, Pass, Level ) values ( 'admin', HASHBYTES('MD5', 'admin'), 9 );", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                            mCommand.Dispose();
                        }

                        //Program.doAlert("an admin user with the pw of 'admin' has been created!\r\n you should change this asap!", "info");

                    }
                    if (!isOutgoing)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.OutgoingTexts ("
                                + "Reference int NOT NULL DEFAULT(0), "
                                + "Modem int NOT NULL DEFAULT(0), "
                                + "Number varchar(50) NOT NULL, "
                                + "Message varchar(50) NOT NULL, "
                                + "Sent varchar(50) NOT NULL, "
                                + "Report_Timestamp varchar(50), "
                                + "Report_Recipient varchar(50), "
                                + "Report_Discharge varchar(50), "
                                + "Report_Status int NOT NULL DEFAULT(0)"
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                            mCommand.Dispose();
                        }
                    }
                    if (!isIncoming)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.IncomingTexts ("
                                + "Modem int NOT NULL DEFAULT(0), "
                                + "Number varchar(50) NOT NULL, "
                                + "Message varchar(50) NOT NULL, "
                                + "Received varchar(50) NOT NULL, "
                                + "Processed int NOT NULL"
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                            mCommand.Dispose();
                        }
                    }
                    if (!isContacts)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.Contacts ("
                                + "Name varchar(50) NOT NULL, "
                                + "Phone_Number varchar(50) NOT NULL, "
                                + "Text_Group varchar(50) NOT NULL "
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                            mCommand.Dispose();
                        }
                    }
                    if (!isErrors)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.Errors ("
                                + "ID bigint PRIMARY KEY IDENTITY, "
                                + "ErrorMsg varchar(255) NOT NULL, "
                                + "Occurred varchar(50) NOT NULL "
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                            mCommand.Dispose();
                        }
                    }
                    if (!isFailed) // FailedOutgoing (Number, TxtMsg, Sent)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.FailedOutgoing ("
                                + "Number int NOT NULL, "
                                + "TxtMsg varchar(255) NOT NULL, "
                                + "Sent varchar(50) NOT NULL "
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                            mCommand.Dispose();
                        }
                    }

                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.dbCheck: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            else
            {
                Program.doEventLog("SqlDb.dbCheck: No Database Found!", 0);
            }

            return true;
        }

        public String[] getUserInfo(String U, String P)
        {
            string[] userInfo = new String[3];
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {

                    string query = "select * from Users where Name='" + U.Trim().ToLower() + "' and Pass=HASHBYTES('MD5', '" + P.Trim().ToLower() + "');";
                    using (SqlCommand mCommand = new SqlCommand(query, mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            if (mReader.Read())
                            {

                                userInfo[0] = "1";
                                userInfo[1] = (string)mReader["Name"];
                                userInfo[2] = "" + (int)mReader["Level"];

                            }
                            mReader.Close();
                            mReader.Dispose();
                        }
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.getUserInfo: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return userInfo;
        }

        public ArrayList getUserList()
        {
            ArrayList UL = new ArrayList();
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {

                    using (SqlCommand mCommand = new SqlCommand("select * from Users;", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            while (mReader.Read())
                            {
                                UL.Add(mReader["Name"]);
                            }
                            mReader.Close();
                            mReader.Dispose();
                        }
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.getUserList: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return UL;
        }

        public bool changeUserPass(String U, String P)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {

                    using (SqlCommand mCommand = new SqlCommand("update Users set Pass=HASHBYTES('MD5', '" + P.ToLower() + "') where Name='" + U.ToLower() + "' ;", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.getUserPass: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool changeUserLevel(String U, int L)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {

                    using (SqlCommand mCommand = new SqlCommand("update Users set Level = " + L + " where Name='" + U.ToLower() + "' ;", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.changeUserLevel: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool addAthenaUser(String U, String P, int L)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (SqlCommand mCommand = new SqlCommand("insert Users (Name, Pass, Level) values (" + U.ToLower() + ", HASHBYTES('MD5', '" + P.ToLower() + "'), " + L + ");", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.addAthenaUser: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool failedOutgoing(String num, String msg)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (SqlCommand mCommand = new SqlCommand("insert into FailedOutgoing (Number, TxtMsg, Sent) values ('" + num + "', '" + msg + "', getdate());", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.failedOutgoing: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool addErrorLogEntry(String Message)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {

                    using (SqlCommand mCommand = new SqlCommand("insert into Errors( ErrorMsg, Occurred ) values ( '" + Message + "', '" + getTimeStamp() + "');", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.addErrorLogEntry: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool addOutgoingEntry(int Modem, String Reference, String Number, String Message)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {
                    String q = "insert into OutgoingTexts (Modem, Reference, Number, Message, Sent) "
                                + "values (" + Modem + ", " + Reference + ", '" + Number + "', '" + Message + "', '" + getTimeStamp() + "');";
                    using (SqlCommand mCommand = new SqlCommand(q, mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.addOutgoingEntry: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool updateOutgoingEntryReport(int Modem, String Receipt)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {

                    String[] Report = Receipt.Split(seperator3, StringSplitOptions.None);
                    String[] subRep = Report[1].Split(seperator2, StringSplitOptions.None);
                    String Reference = subRep[2];
                    String Recipient = subRep[3];
                    String Timestamp = subRep[5] + " " + subRep[6];
                    String Discharge = subRep[7] + " " + subRep[8];
                    String Status = subRep[9];
                    String q = "update OutgoingTexts set "
                                + "Modem = '" + Modem + "', "
                                + "Report_Recipient = '" + Recipient + "', "
                                + "Report_Timestamp = '" + Timestamp + "', "
                                + "Report_Discharge = '" + Discharge + "', "
                                + "Report_Status = " + Status + " "
                                + "where Reference = " + Reference + " and Modem = " + Modem + ";";
                    using (SqlCommand mCommand = new SqlCommand(q, mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.updateOutgoingEntryReport: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool moveBadEntries()
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {
                    ArrayList badList = new ArrayList();
                    using (SqlCommand mCommand = new SqlCommand("select * from OutgoingTexts where Report_Status <> 32768;", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            while (mReader.Read())
                            {
                                //TODO
                                 ///    *****************

                                ArrayList tmp = new ArrayList();
                                tmp.Add(mReader[""]);
                                tmp.Add(mReader[""]);
                                tmp.Add(mReader[""]);
                                tmp.Add(mReader[""]);
                                tmp.Add(mReader[""]);
                               
                                badList.Add(tmp);
                            }

                            X = true;
                            mReader.Close();
                            mReader.Dispose();
                        }
                        mCommand.Dispose();
                    }

                    foreach (ArrayList tmpList in badList)
                    {
                        using (SqlCommand mCommand = new SqlCommand("insert into FailedOutgoing() values ();", mConnection))  //TODO
                        {
                            mCommand.ExecuteNonQuery();
                        }
                    }

                    using (SqlCommand mCommand = new SqlCommand("delete from OutgoingTexts where Report_Status <> 32768;", mConnection))
                    {
                        mCommand.ExecuteNonQuery();
                    }

                }
                    
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.moveBadEntries: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public bool addIncomingEntry(String N, String M)
        {
            bool X = false;
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {
                    using (SqlCommand mCommand = new SqlCommand("insert into IncomingTexts (Number, Message, Received, Processed) values ('" + N + "', '" + M + "', '" + getTimeStamp() + "', 0);", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                        mCommand.Dispose();
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.addIncomingEntry: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }

        public String getTimeStamp()
        {
            string myTime = DateTime.Now.ToString("MM/dd/yyy HH:mm:ss");
            return myTime;
        }

        public String getTodayStamp()
        {
            string myTime = DateTime.Now.ToString("yyyy/MM/dd");
            return myTime;
        }

        public ArrayList getOutgoingLog(String filter)
        {
            ArrayList result = new ArrayList();
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                if (filter == null)
                {
                    filter = "";
                }
                if (filter.Length > 0)
                {
                    filter = " where Number like '%" + filter + "%'";
                }
                try
                {
                    if (mConnection.State == ConnectionState.Open)
                    {
                        using (SqlCommand mCommand = new SqlCommand("select Number, Message, Sent, Report_Status, Modem from OutgoingTexts " + filter + ";", mConnection))
                        {
                            using (SqlDataReader mReader = mCommand.ExecuteReader())
                            {
                                while (mReader.Read())
                                {
                                    ArrayList row = new ArrayList();
                                    row.Add(mReader["Number"]);
                                    row.Add(mReader["Message"]);
                                    row.Add(mReader["Sent"]);
                                    row.Add(mReader["Report_Status"]);
                                    row.Add(mReader["Modem"]);

                                    result.Add(row);
                                }

                                mReader.Close();
                                mReader.Dispose();
                            }
                            mCommand.Dispose();
                        }
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.getOutgoingLog: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return result;
        }


        public ArrayList getIncomingLog(String filter)
        {

            ArrayList result = new ArrayList();
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {

                if (filter == null)
                {
                    filter = "";
                }
                if (filter.Length > 0)
                {
                    filter = " where Number like '%" + filter + "%'";
                }
                try
                {

                    using (SqlCommand mCommand = new SqlCommand("select Number, Message, Sent, Report_Status, Modem from OutgoingTexts " + filter + ";", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {

                            while (mReader.Read())
                            {
                                ArrayList row = new ArrayList();
                                row.Add(mReader["Number"]);
                                row.Add(mReader["Message"]);
                                row.Add(mReader["Receved"]);
                                if (Int32.Parse((string)mReader["Processed"]) > 0)
                                {
                                    row.Add("Yes");
                                }
                                else
                                {
                                    row.Add("No");
                                }

                                result.Add(row);
                            }

                            mReader.Close();
                            mReader.Dispose();
                        }
                        mCommand.Dispose();
                    }

                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.getIncomingLog: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return result;
        }

        public int[] getTodaysCount()
        {
            String today = getTodayStamp();
            int[] todaysTexts = new int[2];
            todaysTexts[0] = 0;
            todaysTexts[1] = 0;
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {

                    int inCount = 0;
                    using (SqlCommand mCommand = new SqlCommand("select * from IncomingTexts where Received like '" + today + "%';", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            while (mReader.Read())
                            {
                                inCount++;
                            }
                            mReader.Close();
                            mReader.Dispose();
                        }
                        mCommand.Dispose();
                    }

                    int outCount = 0;
                    using (SqlCommand mCommand = new SqlCommand("select * from OutgoingTexts where Sent like '" + today + "%';", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            while (mReader.Read())
                            {
                                outCount++;
                            }
                            mReader.Close();
                            mReader.Dispose();
                        }
                        mCommand.Dispose();
                    }
                    todaysTexts[0] = inCount;
                    todaysTexts[1] = outCount;

                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.getTodaysCount: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return todaysTexts;
        }

        public String[] getGroupList()
        {
            String[] s = null;

            try
            {
                groupList = new ArrayList();
                using (SqlCommand mCommand = new SqlCommand("select * from Text_Groups;", mConnection))
                {
                    using (SqlDataReader mReader = mCommand.ExecuteReader())
                    {
                        while (mReader.Read())
                        {
                            groupList.Add(mReader.GetString(0));
                        }

                        mReader.Close();
                        mReader.Dispose();
                    }
                    mCommand.Dispose();
                }
                s = new String[groupList.Count];
                for (int i = 0; i < s.Length; i++)
                {
                    s[i] = (String)groupList[i];
                }

            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.getTodaysCount: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return s;
        }


        public ArrayList getContactGroup(String grp)
        {
            ArrayList result = null;
            String[] row;
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("select * from Contacts where Text_Group = '" + grp + "';", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {

                            for (int i = 0; mReader.Read(); i++)
                            {
                                row = new String[2];
                                row[0] = (String)mReader["Name"];
                                row[1] = (String)mReader["Phone_Number"];
                                result.Add(row);
                            }
                            mReader.Close();
                            mReader.Dispose();
                        }
                        mCommand.Dispose();
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.getContactGroup: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return result;
        }


    }
}
