/*
 * SqlDb
 * 
 * All SQL commands are handled in this file
 * 
 */



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
        private String dBase = "athenaservice";
        private String dbUser = "sa";  //TODO change to new sql user i created
        private String dbPass = ""; // and pass

        private SqlConnection mConnection;

        public String amReady = "Not Connected!";

        String[] seperator1 = new String[] { "\r\n" };
        String[] seperator2 = new String[] { "," };
        String[] seperator3 = new String[] { " " };


        //SqlDataReader mReader;
        //SqlCommand mCommand;

        String host;


        //stats report codes
        //
        //Network Problems
        const int CDS_ADDRESS_VACANT = 0;// Address vacant
        const int CDS_ADDRESS_TRANSLATION_FAILURE = 1;// Address translation failure
        const int CDS_NETWORK_RESOURCE_SHORTAGE = 2;// Network resource shortage
        const int CDS_NETWORK_FAILURE = 3;// Network failure
        const int CDS_INVALID_TELESERVICE_ID = 4;// Invalid teleservice ID
        const int CDS_OTHER_NETWORK_PROBLEM = 5;// Other network problem

        //Terminal Problems
        const int CDS_NO_PAGE_RESPONSE = 32;// No page response
        const int CDS_DESTINATION_BUSY = 33;// Destination busy
        const int CDS_NO_ACKNOWLEDGE = 34;// No acknowledgment from transport layer
        const int CDS_DESTINATION_RESOURCE_SHORTAGE = 35;// Destination resource shortage
        const int CDS_DELIVERY_POSTPONED = 36;// SMS delivery postponed
        const int CDS_DESTINATION_OUT_OF_SERVICE = 37;// Destination out of service
        const int CDS_DESTINATION_NO_LONGER_AT_ADDRESS = 38;// Destination no longer at this address
        const int CDS_OTHER_TERMINAL_PROBLEM = 39;// Other terminal problem

        //Radio Interface Problems
        const int CDS_RADIO_INTERFACE_RESOURCE_SHORTAGE = 64;// Radio interface resource shortage
        const int CDS_RADIO_INTERFACE_INCOMPATIBLE = 65;// Radio interface incompatible
        const int CDS_OTHER_RADIO_INTERFACE_PROBLEM = 66;// Other radio interface problem

        //General problems (IS-41D)
        const int CDS_UNEXPECTED_PARAMETER_SIZE = 96;// Unexpected parameter size
        const int CDS_ORIGINATION_DENIED = 97;// SMS Origination denied
        const int CDS_TERMINATION_DENIED = 98;// SMS Termination denied
        const int CDS_SUPPLEMENTARY_SERVICE_NOT_SUPPORTED = 99;// Supplementary service not supported
        const int CDS_SMS_NOT_SUPPORTED = 100;// SMS not supported
        const int CDS_RESERVED1 = 101;// Reserved
        const int CDS_MISSING_EXPECTED_PARAMETERS = 102;// Missing expected parameters
        const int CDS_MISSING_MANDATORY_PARAMETERS = 103;// Missing mandatory parameters
        const int CDS_UNRECOGNIZED_PARAMETER_VALUE1 = 104;// Unrecognized parameter value
        const int CDS_UNRECOGNIZED_PARAMETER_VALUE2 = 105;// Unexpected parameter value
        const int CDS_USER_DATA_SIZE_ERROR = 106;// User data size error
        const int CDS_NO_ACKNOWLEDGE_UNKOWN_ERROR = 107;// -255 No acknowledgement / Unknown error

        //General Codes
        const int CDS_SMS_OK = 32768;// SMS OK. Message successfully delivered to base station
        const int CDS_OUT_OF_RESOURCES = 32770;// Out of resources
        const int CDS_MESSAGE_TOO_LARGE_FOR_ACCESS_CHANNEL = 32771;// Message too large for access channel
        const int CDS_MESSAGE_TOO_LARGE_FOR_DEDICATED_CHANNEL = 32772;// Message too large for dedicated channel
        const int CDS_NETWORK_NOT_READY = 32773;// Network not ready
        const int CDS_PHONE_NOT_READY = 32774;// Phone not ready
        const int CDS_NOT_ALLOWED_IN_AMPS = 32775;// Not allowed in AMPS
        const int CDS_CANNOT_SEND_BROADCAST = 32776;// Cannot send broadcast

        



        public SqlDb()
        {
            if (Program.mSqlHost != null)
            {
                host = Program.mSqlHost;
            }
            else
            {
                host = @"CANDY-PC\ATHENASQL";
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

            Program.modeSysMsg = GetSysMsgMode();

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
                Program.doEventLog("SqlDb.isReady(): " + e.Message + "\r\n" + e.StackTrace, 0);
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
                    bool isGroups = false;
                    bool isSettings = false;
                    bool isJobs = false;

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
                                else if (temp.Equals("Text_Groups"))
                                {
                                    isGroups = true;
                                }
                                else if(temp.Equals("Athena_Settings"))
                                {
                                    isSettings = true;
                                }
                                else if (temp.Equals("Jobs"))
                                {
                                    isJobs = true;
                                }
                            }
                            mReader.Close();
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
                                + "Reference bigint NOT NULL DEFAULT(0), "
                                + "Modem int NOT NULL DEFAULT(0), "
                                + "Number varchar(50) NOT NULL, "
                                + "Message varchar(300) NOT NULL, "
                                + "Sent varchar(50) NOT NULL, "
                                + "Report_Timestamp varchar(50), "
                                + "Report_Recipient varchar(50), "
                                + "Report_Discharge varchar(50), "
                                + "Report_Status int NOT NULL DEFAULT(0), "
                                + "Log_Time datetime NOT NULL"
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                        }
                    }
                    if (!isIncoming)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.IncomingTexts ("
                                + "Modem int NOT NULL DEFAULT(0), "
                                + "Number varchar(50) NOT NULL, "
                                + "Message varchar(300) NOT NULL, "
                                + "Received varchar(50) NOT NULL, "
                                + "Processed int NOT NULL DEFAULT(0)"
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                        }
                    }
                    if (!isContacts)
                    {
                        using (SqlCommand mCommand = new SqlCommand("create table " + dBase + ".dbo.Contacts ("
                                + "Name varchar(50) NOT NULL, "
                                + "Phone_Number varchar(50) NOT NULL, "
                                + "Text_Group nchar(30) NOT NULL "
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
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
                        }
                    }
                    if (!isFailed) // FailedOutgoing (Number, TxtMsg, Sent)
                    {
                        using (SqlCommand mCommand = new SqlCommand(
                            "create table " + dBase + ".dbo.FailedOutgoing ("
                                + "Number varchar(50) NOT NULL, "
                                + "TxtMsg varchar(300) NOT NULL, "
                                + "ErrCode int NOT NULL DEFAULT(0),"
                                + "ErrMsg varchar(180) NOT NULL,"
                                + "Sent varchar(50) NOT NULL"
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                        }
                    }
                    if (!isGroups)
                    {
                        using (SqlCommand mCommand = new SqlCommand(
                            "create table " + dBase + ".dbo.Text_Groups ("
                                + "Name nchar(30) NOT NULL, "
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                        }
                    }
                    if (!isSettings)
                    {
                        using (SqlCommand mCommand = new SqlCommand(
                            "create table " + dBase + ".dbo.Athena_Settings ("
                                + "SysMsgMode int NOT NULL DEFAULT(0), "
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                        }
                        using (SqlCommand mCommand = new SqlCommand(
                            "insert into " + dBase + ".dbo.Athena_Settings (SysMsgMode) values (0);", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                        }
                    }
                    if (!isJobs)
                    {
                        using (SqlCommand mCommand = new SqlCommand(
                            "create table " + dBase + ".dbo.Jobs ("
                                + "Job_Name varchar(50) NOT NULL, "
                                + "Job_Location varchar(50) NOT NULL, "
                                + "Job_File varchar(50) NOT NULL, "
                                + "Job_Schedule int NOT NULL DEFAULT(0)"
                                + ");", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
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
                        }
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
                        }
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
                    using (SqlCommand mCommand = new SqlCommand("insert into Users (Name, Pass, Level) values ('" + U.ToLower() + "', HASHBYTES('MD5', '" + P.ToLower() + "'), " + L + ");", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.addAthenaUser: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }





        public bool deleteAthenaUser(String U)
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
                    using (SqlCommand mCommand = new SqlCommand("delete from Users where Name = '" + U.ToLower() + "';", mConnection))
                    {
                        X = (mCommand.ExecuteNonQuery() > 0);
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.deleteAthenaUser: " + e.Message + "\r\n" + e.StackTrace, 0);
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
                    String q = "insert into OutgoingTexts (Modem, Reference, Number, Message, Sent, Log_Time) "
                                + "values (" + Modem + ", " + Reference + ", '" + Number + "', '" + Message + "', "+getTimeStamp()+", NOW());";
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
                    }
                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.updateOutgoingEntryReport: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return X;
        }





        public bool MoveBadEntries()
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
                    using (SqlCommand mCommand = new SqlCommand("select * from OutgoingTexts where Report_Status <> " + CDS_SMS_OK + ";", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            while (mReader.Read())
                            {
                                //TODO
                                 ///    *****************

                                ArrayList tmp = new ArrayList();
                                tmp.Add(mReader["Number"]);
                                tmp.Add(mReader["Message"]);

                                int status = (int)mReader["Report_Status"];
                                string msg = GetErrMsg(status);
                                tmp.Add(status);
                                tmp.Add(msg);

                                tmp.Add(mReader["Sent"]);
                               
                                badList.Add(tmp);
                            }

                            X = true;
                            mReader.Close();
                        }
                    }

                    foreach (ArrayList tmpList in badList)
                    {
                        using (SqlCommand mCommand = new SqlCommand("insert into FailedOutgoing(Number, TxtMsg, ErrCode, ErrMsg, Sent)"
                            + "values ('" + tmpList[0] + "', '" + tmpList[1] + "', " + tmpList[2] + ", '" + tmpList[3] + "', '" + tmpList[4] + "');", mConnection))
                        {
                            mCommand.ExecuteNonQuery();
                        }
                    }

                    using (SqlCommand mCommand = new SqlCommand("delete from OutgoingTexts where Report_Status <> " + CDS_SMS_OK + ";", mConnection))
                    {
                        mCommand.ExecuteNonQuery();
                    }

                }
                    
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.MoveBadEntries: " + e.Message + "\r\n" + e.StackTrace, 0);
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




        public bool createReport(int type, int sendTo)
        {
            bool X = false;
            try
            {
                //
                string reportMsg = "";
                string reportType = "";

                ArrayList list = new ArrayList();
                string query = "";

                switch (type)
                {
                    case 2:
                        //yearly
                        reportType = "Yearly";
                        query = "select * from OutgoingTexts where DATEPART(year, Log_Time) = " + DateTime.Now.Year + ";";
                        break;
                    case 1:
                        //monthly
                        reportType = "Monthly";
                        query = "select * from OutgoingTexts where DATEPART(year, Log_Time) = " + DateTime.Now.Year + " and DATEPART(month, Log_Time) = " + DateTime.Now.Month + ";";
                        break;
                    default:
                        //weekly
                        reportType = "Weekly";
                        query = "select * from OutgoingTexts where Log_Time >= DATEADD(dd,DATEDIFF(dd,0,GETDATE()),0)-7 and Log_Time  <= DATEADD(dd,DATEDIFF(dd,0,GETDATE()),0);";
                        break;
                }
                using (SqlCommand command = new SqlCommand(query, mConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.NextResult())
                        {
                            ArrayList row = new ArrayList();
                            row.Add(reader[0]);
                            row.Add(reader[1]);
                            row.Add(reader[2]);
                            row.Add(reader[3]);
                            row.Add(reader[4]);
                            row.Add(reader[5]);
                            row.Add(reader[6]);
                            row.Add(reader[7]);
                            row.Add(reader[8]);
                            row.Add(reader[9]);
                            list.Add(row);
                        }
                        reportMsg = reportType + " Report!\r\n\r\n\tAthena has sent " + list.Count + " Text Messages.";
                    }
                }



                switch (sendTo)
                {
                    case 1:
                        Program.sendPrintJob(reportType + " Text Report", reportMsg);
                        break;
                    default:
                        // email
                        Program.doNotify(reportType + " Text Report", reportMsg);
                        break;
                }

                X = true;

            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.createReport " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return X;
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
                            }
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

                    using (SqlCommand mCommand = new SqlCommand("select Number, Message, Received, Processed from IncomingTexts " + filter + ";", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {

                            while (mReader.Read())
                            {
                                ArrayList row = new ArrayList();
                                int proc = 0;
                                row.Add(mReader["Number"]);
                                row.Add(mReader["Message"]);
                                row.Add(mReader["Received"]);
                                Int32.TryParse((string)mReader["Processed"], out proc);
                                if (proc > 0)
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
                        }
                    }

                }
                catch (Exception e)
                {
                    Program.doEventLog("SqlDb.getIncomingLog: " + e.Message + "\r\n" + e.StackTrace, 0);
                }
            }
            return result;
        }



        public ArrayList getFailedLog()
        {

            ArrayList result = new ArrayList();
            if (mConnection.State != ConnectionState.Open)
            {
                Connect();
            }

            if (mConnection.State == ConnectionState.Open)
            {
                try
                {

                    using (SqlCommand mCommand = new SqlCommand("select Number, TxtMsg, ErrCode, ErrMsg, Sent from FailedOutgoing;", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {

                            while (mReader.Read())
                            {
                                ArrayList row = new ArrayList();
                                row.Add(mReader["Number"]);
                                row.Add(mReader["TxtMsg"]);
                                row.Add(mReader["ErrCode"]);
                                row.Add(mReader["ErrMsg"]);
                                row.Add(mReader["Sent"]);

                                result.Add(row);
                            }

                            mReader.Close();
                        }
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
                        }
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
                        }
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







        public ArrayList getGroupList()
        {
            ArrayList tmp = new ArrayList();
            try
            {
                using (SqlCommand mCommand = new SqlCommand("select * from Text_Groups;", mConnection))
                {
                    using (SqlDataReader mReader = mCommand.ExecuteReader())
                    {
                        while (mReader.Read())
                        {
                            tmp.Add(mReader.GetString(0));
                        }
                        mReader.Close();
                    }
                }

            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.getTodaysCount: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return tmp;
        }





        public bool addAthenaContact(string Name, string Phone, string Group)
        {
            bool X = false;

            try
            {
                using (SqlCommand mCommand = new SqlCommand("insert into Contacts (Name, Phone_Number, Text_Group) values ('" + Name + "', '"+Phone+"', '"+Group+"');", mConnection))
                {
                    if (mCommand.ExecuteNonQuery() > 0)
                    {
                        X = true;
                    }
                }

            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.addContact: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return X;
        }



        public bool editAthenaContact(string Name, string col, string val)
        {
            bool X = false;

            try
            {
                using (SqlCommand mCommand = new SqlCommand("update Contacts set " + col + " = '" + val + "' where Name = '" + Name + "';", mConnection))
                {
                    if (mCommand.ExecuteNonQuery() > 0)
                    {
                        X = true;
                    }
                }

            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.editContact: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return X;
        }



        public bool deleteAthenaContact(string Name)
        {
            bool X = false;

            try
            {
                using (SqlCommand mCommand = new SqlCommand("delete from Contacts where Name = '" + Name + "';", mConnection))
                {
                    if (mCommand.ExecuteNonQuery() > 0)
                    {
                        X = true;
                    }
                }

            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.deleteContact: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return X;
        }



        public ArrayList getContactsList()
        {
            ArrayList list = new ArrayList();

            try
            {
                using (SqlCommand mCommand = new SqlCommand("select * from Contacts;", mConnection))
                {
                    using (SqlDataReader mReader = mCommand.ExecuteReader())
                    {
                        if (mReader.HasRows)
                        {
                            while (mReader.Read())
                            {
                                list.Add(mReader.GetString(0));
                            }
                        }
                        mReader.Close();
                    }
                }

            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.getContactsList: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return list;
        }



        public ArrayList getContactInfo(String n)
        {
            ArrayList list = new ArrayList();

            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("select * from Contacts where Name = '" + n + "';", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            if (mReader.HasRows)
                            {
                                mReader.Read();
                                list.Add((String)mReader["Name"]);
                                list.Add((String)mReader["Phone_Number"]);
                                list.Add((String)mReader["Text_Group"]);
                            }
                            mReader.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.getContactInfo: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return list;
        }



        public ArrayList getContactGroup(String grp)
        {
            ArrayList result = new ArrayList();
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
                            if (mReader.HasRows)
                            {
                                for (int i = 0; mReader.Read(); i++)
                                {
                                    row = new String[2];
                                    row[0] = (String)mReader["Name"];
                                    row[1] = (String)mReader["Phone_Number"];
                                    result.Add(row);
                                }
                            }
                            mReader.Close();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.getContactGroup: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return result;
        }



        public int GetSysMsgMode()
        {
            int mode = 0;

            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("select SysMsgMode from Athena_Settings;", mConnection))
                    {
                        using (SqlDataReader mReader = mCommand.ExecuteReader())
                        {
                            if (mReader.HasRows)
                            {
                                mReader.Read();
                                mode = (int)mReader[0];
                                mReader.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.GetSysMsgMode: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return mode;
        }



        public bool SetSysMsgMode(int mode)
        {
            bool X = false;
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("update Athena_Settings set SysMsgMode = " + mode + ";", mConnection))
                    {
                        if (mCommand.ExecuteNonQuery() > 0)
                        {
                            X = true;
                            Program.modeSysMsg = mode;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.GetSysMsgMode: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return X;
        }




        /*
         *  0 = off
         *  1 = daily
         *  2 = weekly
         *  3 = monthly
         */
        public ArrayList GetScheduledJobs(int s)
        {
            ArrayList list = new ArrayList();
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("select * from Jobs where Job_Schedule = " + s + ";", mConnection))
                    {
                        using (SqlDataReader reader = mCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    string[] job = new string[3];
                                    job[0] = reader.GetString(0);
                                    job[1] = reader.GetString(1);
                                    job[2] = reader.GetString(2);
                                    list.Add(job);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.GetSysMsgMode: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return list;
        }



        public bool AddGroup(string name)
        {
            bool X= false;

            if (!name.Equals(""))
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("insert into Text_Groups (Name) values ('" + name + "');", mConnection))
                    {
                        if (mCommand.ExecuteNonQuery() > 0)
                        {
                            X = true;
                        }
                    }
                }
            }

            return X;
        }



        public bool DeleteGroup(string name)
        {
            bool X = false;

            if (!name.Equals(""))
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("delete from Text_Groups where Name = '" + name + "';", mConnection))
                    {
                        Console.WriteLine("delete from Text_Groups where Name = '" + name + "';");
                        if (mCommand.ExecuteNonQuery() > 0)
                        {
                            X = true;
                        }
                        Console.WriteLine("effected rows: " + X.ToString());
                    }
                }
            }

            return X;
        }




        public ArrayList GetJobInfo(string name)
        {
            ArrayList list = new ArrayList();
            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("select * from Jobs where Job_Name= '" + name + "';", mConnection))
                    {
                        using (SqlDataReader reader = mCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                if (reader.Read())
                                {
                                    list.Add(reader.GetString(0));
                                    list.Add(reader.GetString(1));
                                    list.Add(reader.GetString(2));
                                    list.Add(reader.GetInt32(3)+"");
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.GetSysMsgMode: " + e.Message + "\r\n" + e.StackTrace, 0);
            }
            return list;
        }




        public bool AddScheduledJob(int schedule, string name, string location, string file)
        {
            bool X = false;

            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("insert into Jobs (Job_Name, Job_Location, Job_File, Job_Schedule) values ('" + name + "', '"+ location + "', '" + file + "', " + schedule + ");", mConnection))
                    {
                        if (mCommand.ExecuteNonQuery() > 0)
                        {
                            X = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.AddScheduledJob: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return X;
        }



        public bool EditScheduledJob(int schedule, string name, string location, string file)
        {
            bool X = false;

            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("update Jobs set Job_Name = '" + name + "', Job_Location = '" + location + "', Job_File = '" + file + "', Job_Schedule = " + schedule + " where Job_Name = '" + name + "';", mConnection))
                    {
                        if (mCommand.ExecuteNonQuery() > 0)
                        {
                            X = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.EditScheduledJob: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return X;
        }



        public bool DeleteScheduledJob(string name, int type)
        {
            bool X = false;

            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand("delete from Jobs where Job_Name = '" + name + "' and Job_Schedule = " + type + ";", mConnection))
                    {
                        if (mCommand.ExecuteNonQuery() > 0)
                        {
                            X = true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.AddScheduledJob: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return X;
        }






        public int[] getDayTotals(string day)
        {
            int[] totals = new int[]{0, 0};

            try
            {
                if (mConnection.State != ConnectionState.Open)
                {
                    Connect();
                }

                if (mConnection.State == ConnectionState.Open)
                {
                    using (SqlCommand mCommand = new SqlCommand(@"select Reference from OutgoingTexts where Sent like '%" + day + @"%';", mConnection))
                    {
                        using (SqlDataReader reader = mCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                int count = 1;
                                while (reader.NextResult())
                                {
                                    count++;
                                }
                                totals[0] = count;
                            }
                        }
                    }
                    using (SqlCommand mCommand = new SqlCommand(@"select Number from IncomingTexts where Received like '%" + day + @"%';", mConnection))
                    {
                        using (SqlDataReader reader = mCommand.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                int count = 1;
                                while (reader.NextResult())
                                {
                                    count++;
                                }
                                totals[1] = count;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Program.doEventLog("SqlDb.GetDayTotals: " + e.Message + "\r\n" + e.StackTrace, 0);
            }

            return totals;
        }










        public string GetErrMsg(int c)
        {
            string r = "";
            switch (c)
            {
                // Network
                case CDS_ADDRESS_VACANT:
                    r = "Network Problem: ADDRESS VACANT";
                    break;

                case CDS_ADDRESS_TRANSLATION_FAILURE:
                    r = "Network Problem: ADDRESS TRANSLATION FAILURE";
                    break;

                case CDS_NETWORK_RESOURCE_SHORTAGE:
                    r = "Network Problem: NETWORK RESOURCE SHORTAGE";
                    break;

                case CDS_NETWORK_FAILURE:
                    r = "Network Problem: NETWORK FAILURE";
                    break;

                case CDS_INVALID_TELESERVICE_ID:
                    r = "Network Problem: INVALID TELESERVICE ID";
                    break;

                case CDS_OTHER_NETWORK_PROBLEM:
                    r = "Network Problem: OTHER NETWORK PROBLEM";
                    break;


                // Terminal
                case CDS_NO_PAGE_RESPONSE:
                    r = "Terminal Problem: NO PAGE RESPONSE";
                    break;

                case CDS_DESTINATION_BUSY:
                    r = "Terminal Problem: DESTINATION BUSY";
                    break;

                case CDS_NO_ACKNOWLEDGE:
                    r = "Terminal Problem: NO ACKNOWLEDGE";
                    break;

                case CDS_DESTINATION_RESOURCE_SHORTAGE:
                    r = "Terminal Problem: DESTINATION RESOURCE SHORTAGE";
                    break;

                case CDS_DELIVERY_POSTPONED:
                    r = "Terminal Problem: DELIVERY POSTPONED";
                    break;

                case CDS_DESTINATION_OUT_OF_SERVICE:
                    r = "Terminal Problem: DESTINATION OUT OF SERVICE";
                    break;

                case CDS_DESTINATION_NO_LONGER_AT_ADDRESS:
                    r = "Terminal Problem: DESTINATION NO LONGER AT ADDRESS";
                    break;

                case CDS_OTHER_TERMINAL_PROBLEM:
                    r = "Terminal Problem: OTHER TERMINAL PROBLEM";
                    break;



                //Radio Interface
                case CDS_RADIO_INTERFACE_RESOURCE_SHORTAGE:
                    r = "Radio InterfAce Problem: RADIO INTERFACE RESOURCE SHORTAGE";
                    break;

                case CDS_RADIO_INTERFACE_INCOMPATIBLE:
                    r = "RadioInterface Problem: RADIO INTERFACE INCOMPATIBLE";
                    break;

                case CDS_OTHER_RADIO_INTERFACE_PROBLEM:
                    r = "Radio Interface Problem: OTHER RADIO INTERFACE PROBLEM";
                    break;



                // general
                case CDS_UNEXPECTED_PARAMETER_SIZE:
                    r = "General Problem: UNEXPECTED PARAMETER SIZE";
                    break;

                case CDS_ORIGINATION_DENIED:
                    r = "General Problem: SMS Origination Denied";
                    break;

                case CDS_TERMINATION_DENIED:
                    r = "General Problem: SMS Termination Denied";
                    break;

                case CDS_SUPPLEMENTARY_SERVICE_NOT_SUPPORTED:
                    r = "General Problem: SUPPLEMENTARY SERVICE NOT SUPPORTED";
                    break;

                case CDS_SMS_NOT_SUPPORTED:
                    r = "General Problem: SMS NOT SUPPORTED";
                    break;

                case CDS_RESERVED1:
                    r = "General Problem: RESERVED1";
                    break;

                case CDS_MISSING_EXPECTED_PARAMETERS:
                    r = "General Problem: MISSING EXPECTED PARAMETERS";
                    break;

                case CDS_MISSING_MANDATORY_PARAMETERS:
                    r = "General Problem: MISSING MANDATORY PARAMETERS";
                    break;

                case CDS_UNRECOGNIZED_PARAMETER_VALUE1:
                    r = "General Problem: UNRECOGNIZED PARAMETER VALUE1";
                    break;

                case CDS_UNRECOGNIZED_PARAMETER_VALUE2:
                    r = "General Problem: UNRECOGNIZED PARAMETER VALUE2";
                    break;

                case CDS_USER_DATA_SIZE_ERROR:
                    r = "General Problem: USER DATA SIZE ERROR";
                    break;


                //General Codes
                case CDS_OUT_OF_RESOURCES:
                    r = "General Code: OUT OF RESOURCES";
                    break;

                case CDS_MESSAGE_TOO_LARGE_FOR_ACCESS_CHANNEL:
                    r = "General Code: MESSAGE TOO LARGE FOR ACCESS CHANNEL";
                    break;

                case CDS_MESSAGE_TOO_LARGE_FOR_DEDICATED_CHANNEL:
                    r = "General Code: MESSAGE TOO LARGE FOR DEDICATED CHANNEL";
                    break;

                case CDS_NETWORK_NOT_READY:
                    r = "General Code: NETWORK NOT READY";
                    break;

                case CDS_PHONE_NOT_READY:
                    r = "General Code: PHONE NOT READY";
                    break;

                case CDS_NOT_ALLOWED_IN_AMPS:
                    r = "General Code: NOT ALLOWED IN AMPS";
                    break;

                case CDS_CANNOT_SEND_BROADCAST:
                    r = "General Code: CANNOT SEND BROADCAST";
                    break;


                // unknown error 107-255
                default:
                    r = "General Problem: Unknown Error";
                    break;

            }
            return r;
        }

    }
}
