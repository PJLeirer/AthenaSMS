using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace AthenaService
{
    class SockReciever
    {

        public SockSender mSender;

        public bool doRun = true;

        public bool loggedIn = false;
        public int mUserId = 0;
        public String mUserName = "nobody";
        public int mUserLevel = 0;

        string sGroup;
        String sGxt;
        String sSxt;

        bool athena_obj;

        ArrayList mRecievers;
        public TcpClient mClient;

        NetworkStream mStream;

        public bool deadClient = false;


        public SockReciever(ArrayList r,TcpClient c)
        {
            mRecievers = r;
            mClient = c;
            mStream = mClient.GetStream();

            mSender = new SockSender(mClient);

            new Thread(looper).Start();

        }

        private void looper()
        {
            while (doRun)
            {
                try
                {

                    int allBytes;

                    StringBuilder builder = new StringBuilder();


                    while (mStream.DataAvailable)
                    {
                        char byt = (char)mStream.ReadByte();
                        if (byt == 0X00)
                        {
                            break;
                        }
                        builder.Append(byt);

                        /*
                        byte[] buff = new byte[1024];


                        //this may be useless
                        allBytes = mStream.Read(buff, 0, buff.Length);
                        if (allBytes < 1)
                        {
                            builder.Clear();
                            doRun = false;
                            //mStream.Close();
                            //mStream.Dispose();
                            Console.WriteLine(mUserName + " disconnected");
                            mClient.Client.Shutdown(SocketShutdown.Both);
                            mClient.Client.Disconnect(false);
                            deadClient = true;
                            mClient.Close();
                            break;
                        }

                        for (int i = 0; i < buff.Length; i++)
                        {
                            if (buff[i] != 0x00)
                            {
                                builder.Append((char)buff[i]);
                            }
                        }
                         */
                    }

                    Thread.Sleep(20);


                    String xdata = builder.ToString();
                    if (xdata.Length > 0)
                    {


                        Console.WriteLine("procesing string.\r\n" + xdata);

                        if (xdata.Length < 20)
                        {
                            // parse xdata // maybe just polling
                        }
                        else
                        {

                            using (XmlReader mXmlReader = XmlReader.Create(new StringReader(xdata.Trim())))
                            {
                                athena_obj = false;
                                while (mXmlReader.Read())
                                {



                                    if (mXmlReader.IsStartElement())
                                    {
                                        if (mXmlReader.Name.Trim().Equals("AthenaObj"))
                                        {
                                            Console.WriteLine("Got AthenaObj\r\n");
                                            athena_obj = true;
                                        }


                                        if (athena_obj)
                                        {

                                            switch (mXmlReader.Name.Trim())
                                            {





                                                case "txUserLogin":
                                                    Console.WriteLine("Got UserLogin\r\n");

                                                    string userName = "";
                                                    string userPass = "";

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("user"))
                                                        {
                                                            mXmlReader.Read();
                                                            userName = mXmlReader.Value.Trim();
                                                            Console.WriteLine("username: " + userName);
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("pass"))
                                                        {
                                                            mXmlReader.Read();
                                                            userPass = mXmlReader.Value.Trim().ToLower();
                                                            Console.WriteLine("userpass: " + "*******");
                                                            mXmlReader.Read();// end tag
                                                        }
                                                    }

                                                    if (userName.Length > 0 && userPass.Length > 0)
                                                    {
                                                        string[] info = Program.mSqlDb.getUserInfo(userName, userPass);
                                                        if (info != null)
                                                        {
                                                            if (info[0] != null)
                                                            {
                                                                Int32.TryParse(info[0], out mUserId);
                                                            }
                                                            if (info[1] != null)
                                                            {
                                                                mUserName = info[1];
                                                            }
                                                            if (info[2] != null)
                                                            {
                                                                Int32.TryParse(info[2], out mUserLevel);
                                                            }
                                                            Console.WriteLine("got user info");
                                                        }
                                                    }
                                                    
                                                    mSender.sendLoginInfo(mUserId, mUserName, mUserLevel);

                                                    break;








                                                case "txAddAthenaUser":
                                                    bool didadduser = false;
                                                    Console.WriteLine("Got AddAthenaUser\r\n");

                                                    string addName = "";
                                                    string addPass = "";
                                                    int addLvl = 0;

                                                    Console.WriteLine("processing add user command");

                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            Console.WriteLine("got name");
                                                            mXmlReader.Read();
                                                            addName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("pass"))
                                                        {
                                                            Console.WriteLine("got pass");
                                                            mXmlReader.Read();
                                                            addPass = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("level"))
                                                        {
                                                            Console.WriteLine("got level");
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out addLvl);
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    Console.WriteLine("Checking values: " + addName + ", " + addPass + ", " + addLvl);

                                                    if (!addName.Equals("") && !addPass.Equals("") && addLvl > 0)
                                                    {
                                                        if (Program.mSqlDb.addAthenaUser(addName, addPass, addLvl))
                                                        {
                                                            Console.WriteLine("add user was a success");
                                                            didadduser = true;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("sql add user failed");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Name, Pass or Level Failed");
                                                    }


                                                    mSender.SendAddUserResult(didadduser);


                                                    break;








                                                case "txDeleteAthenaUser":


                                                    Console.WriteLine("Got DeleteAthenaUser\r\n");

                                                    bool diddeleteuser = false;

                                                    string deleteName = "";

                                                    Console.WriteLine("processing delete user command");

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            Console.WriteLine("got name");
                                                            mXmlReader.Read();
                                                            deleteName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    if (!deleteName.Equals(""))
                                                    {
                                                        if (Program.mSqlDb.deleteAthenaUser(deleteName))
                                                        {
                                                            Console.WriteLine("delete user was a success");
                                                            diddeleteuser = true;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("sql delete user failed");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Name Failed");
                                                    }

                                                    mSender.SendDeleteUserResult(diddeleteuser);


                                                    break;









                                                case "txChangeUserPass":
                                                    Console.WriteLine("Got ChangeUserPass\r\n");

                                                    bool didchangepass = false;


                                                    string changePName = "";
                                                    string changePPass = "";

                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            Console.WriteLine("got name");
                                                            mXmlReader.Read();
                                                            changePName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("pass"))
                                                        {
                                                            Console.WriteLine("got pass");
                                                            mXmlReader.Read();
                                                            changePPass = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    Console.WriteLine("Checking values: " + changePName + ", " + changePPass);

                                                    if (!changePName.Equals("") && !changePPass.Equals(""))
                                                    {
                                                        if (Program.mSqlDb.changeUserPass(changePName, changePPass))
                                                        {
                                                            Console.WriteLine("change user pass was a success");
                                                            didchangepass = true;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("sql change user pass failed");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Name or Pass Failed");
                                                    }

                                                    mSender.SendChangeUserPassResult(didchangepass);


                                                    break;








                                                case "txChangeUserLevel":
                                                    Console.WriteLine("Got ChangeUserLevel\r\n");

                                                    bool didchangelevel = false;

                                                    string changeLName = "";
                                                    int changeLLevel = 0;

                                                    Console.WriteLine("processing change pass command");

                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            Console.WriteLine("got name");
                                                            mXmlReader.Read();
                                                            changeLName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("level"))
                                                        {
                                                            Console.WriteLine("got level");
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out changeLLevel);
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    Console.WriteLine("Checking values: " + changeLName + ", " + changeLLevel);

                                                    if (!changeLName.Equals("") && changeLLevel > 0)
                                                    {
                                                        if (Program.mSqlDb.changeUserLevel(changeLName, changeLLevel))
                                                        {
                                                            Console.WriteLine("change user level was a success");
                                                            didchangelevel = true;
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("sql change level pass failed");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Name or Pass Failed");
                                                    }


                                                    mSender.SendChangeUserLevelResult(didchangelevel);


                                                    break;








                                                case "txGetUserList":
                                                    Console.WriteLine("Got GetUserList\r\n");

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("users"))
                                                        {
                                                            mXmlReader.Read();
                                                            if (mXmlReader.Value.Trim().Equals("yes"))
                                                            {
                                                                mSender.sendUserList();
                                                            }
                                                        }
                                                        mXmlReader.Read(); //end tag

                                                    }

                                                    break;





                                                case "txGetOnlineList":
                                                    Console.WriteLine("Got GetOnlineList\r\n");

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("users"))
                                                        {
                                                            mXmlReader.Read();
                                                            if (mXmlReader.Value.Trim().Equals("yes"))
                                                            {
                                                                mSender.sendOnlineList(Program.mSocketManager.WhosOnline());
                                                            }
                                                        }
                                                        mXmlReader.Read(); //end tag

                                                    }

                                                    break;





                                                

                                                case "txGetSysMsgMode":
                                                    Console.WriteLine("Got GetSysMsgMode\r\n");
                                                    
                                                    int msgMode = 0;
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("mode"))
                                                        {
                                                            mXmlReader.Read();
                                                            if (mXmlReader.Value.Trim().Equals("yes"))
                                                            {
                                                                msgMode = Program.mSqlDb.GetSysMsgMode();
                                                            }
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    mSender.SendGetMsgModeResult(msgMode);

                                                    break;








                                                case "txSetSysMsgMode":
                                                    Console.WriteLine("Got SetSysMsgMode\r\n");

                                                    int smm = 0;
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("mode"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out smm);
                                                            mXmlReader.Read(); //end tag
                                                        }
                                                    }

                                                    mSender.SendSetMsgModeResult(Program.mSqlDb.SetSysMsgMode(smm));

                                                    break;








                                                case "txAddGroup":

                                                    string addGrpName = "";
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("group"))
                                                        {
                                                            mXmlReader.Read();
                                                            addGrpName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    mSender.SendAddGroupResult(Program.mSqlDb.AddGroup(addGrpName));

                                                    break;



                                                case "txDeleteGroup":

                                                    Console.WriteLine("Got Delete Group");

                                                    string deleteGrpName = "";
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("group"))
                                                        {
                                                            mXmlReader.Read();
                                                            deleteGrpName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    Console.WriteLine("Name: " + deleteGrpName);

                                                    mSender.SendDeleteGroupResult(Program.mSqlDb.DeleteGroup(deleteGrpName));

                                                    break;







                                                case "txGetGroupList":
                                                    Console.WriteLine("Got GetGroupList\r\n");

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("groups"))
                                                        {
                                                            mXmlReader.Read();
                                                            if (mXmlReader.Value.Trim().Equals("yes"))
                                                            {
                                                                mSender.sendGroupList();
                                                            }
                                                        }
                                                        mXmlReader.Read(); //end tag

                                                    }

                                                    break;






                                                case "txGrpTxt":
                                                    Console.WriteLine("Got GrpText\r\n");

                                                    bool didgrouptext = false;

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("group"))
                                                        {
                                                            mXmlReader.Read();
                                                            sGroup = mXmlReader.Value.Trim();
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("gtxt"))
                                                        {
                                                            mXmlReader.Read();
                                                            sGxt = mXmlReader.Value.Trim();
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    if (sGxt != null)
                                                    {

                                                        if (Program.mGroups.textGroup(sGroup, sGxt))
                                                        {
                                                            didgrouptext = true;
                                                        }

                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("text is not long enough");
                                                    }

                                                    mSender.SendGrpTxtResult(didgrouptext);

                                                    break;








                                                case "txSngTxt":
                                                    Console.WriteLine("Got SngTxt\r\n");

                                                    bool didsingletxt = false;

                                                    string phone_number = "";
                                                    string single_text = "";

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("number"))
                                                        {
                                                            mXmlReader.Read();
                                                            phone_number = mXmlReader.Value.Trim();
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("stxt"))
                                                        {
                                                            mXmlReader.Read();
                                                            single_text = mXmlReader.Value.Trim();
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    if (!phone_number.Equals("") && !single_text.Equals(""))
                                                    {
                                                        ArrayList single_txt_list = new ArrayList();
                                                        String[] single_txt_msg = { phone_number, single_text };
                                                        single_txt_list.Add(single_txt_msg);
                                                        String[] what = { "sysmsg", phone_number + " Single Text" };

                                                        if (Program.mModemManager.addToAndProcessOutgoingMessages(single_txt_list, what))
                                                        {
                                                            didsingletxt = true;
                                                        }
                                                    }


                                                    mSender.SendSngTxtResult(didsingletxt);



                                                    break;







                                                case "txAddAthenaContact":
                                                    Console.WriteLine("Got AddAthenaContact\r\n");

                                                    string addCName = "";
                                                    string addPhone = "";
                                                    string addCGroup = "";

                                                    Console.WriteLine("processing add contact command");

                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            Console.WriteLine("got name");
                                                            mXmlReader.Read();
                                                            addCName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("phone"))
                                                        {
                                                            Console.WriteLine("got phone");
                                                            mXmlReader.Read();
                                                            addPhone = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("group"))
                                                        {
                                                            Console.WriteLine("got group");
                                                            mXmlReader.Read();
                                                            addCGroup = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    Console.WriteLine("Checking values: " + addCName + ", " + addPhone + ", " + addCGroup);

                                                    if (!addCName.Equals("") && !addPhone.Equals("") && !addCGroup.Equals(""))
                                                    {
                                                        if (Program.mSqlDb.addAthenaContact(addCName, addPhone, addCGroup))
                                                        {
                                                            Console.WriteLine("add contact was a success");
                                                            mSender.sendAddContactResult(true);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("sql add contact failed");
                                                            mSender.sendAddContactResult(false);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Name, Phone or Group Failed");
                                                        mSender.sendAddContactResult(false);
                                                    }

                                                    break;





                                                case "txEditAthenaContact":
                                                    Console.WriteLine("Got EditAthenaContact\r\n");

                                                    string addEName = "";
                                                    string addECol = "";
                                                    string addEVal = "";

                                                    Console.WriteLine("processing edit contact command");

                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            Console.WriteLine("got name");
                                                            mXmlReader.Read();
                                                            addEName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("col"))
                                                        {
                                                            Console.WriteLine("got col");
                                                            mXmlReader.Read();
                                                            addECol = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        else if (mXmlReader.Name.Equals("val"))
                                                        {
                                                            Console.WriteLine("got val");
                                                            mXmlReader.Read();
                                                            addEVal = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    Console.WriteLine("Checking values: " + addEName + ", " + addECol + ", " + addEVal);

                                                    if (!addEName.Equals("") && !addECol.Equals("") && !addEVal.Equals(""))
                                                    {
                                                        if (Program.mSqlDb.editAthenaContact(addEName, addECol, addEVal))
                                                        {
                                                            Console.WriteLine("edit contact was a success");
                                                            mSender.sendEditContactResult(true);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("sql edit contact failed");
                                                            mSender.sendEditContactResult(false);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Name, Col or Val Failed");
                                                        mSender.sendEditContactResult(false);
                                                    }

                                                    break;





                                                case "txDeleteAthenaContact":
                                                    Console.WriteLine("Got DeleteAthenaContact\r\n");

                                                    string deleteCName = "";

                                                    Console.WriteLine("processing delete contact command");

                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            Console.WriteLine("got name");
                                                            mXmlReader.Read();
                                                            deleteCName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    Console.WriteLine("Checking values: " + deleteCName);

                                                    if (!deleteCName.Equals(""))
                                                    {
                                                        if (Program.mSqlDb.deleteAthenaContact(deleteCName))
                                                        {
                                                            Console.WriteLine("delete contact was a success");
                                                            mSender.sendDeleteContactResult(true);
                                                        }
                                                        else
                                                        {
                                                            Console.WriteLine("sql delete contact failed");
                                                            mSender.sendDeleteContactResult(false);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Console.WriteLine("Contact Name Failed");
                                                        mSender.sendDeleteContactResult(false);
                                                    }

                                                    break;









                                                case "txGetContactsList":
                                                    Console.WriteLine("Got GetContactsList\r\n");

                                                    ArrayList conList = new ArrayList();
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("contacts"))
                                                        {
                                                            mXmlReader.Read();
                                                            if (mXmlReader.Value.Trim().Equals("yes"))
                                                            {
                                                                conList = Program.mSqlDb.getContactsList();
                                                            }
                                                        }
                                                        mXmlReader.Read(); //end tag

                                                    }

                                                    mSender.sendContactsList(conList);

                                                    break;





                                                case "txGetContactInfo":
                                                    Console.WriteLine("Got GetContactInfo\r\n");

                                                    ArrayList conInfo = new ArrayList();
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            mXmlReader.Read();
                                                            conInfo = Program.mSqlDb.getContactInfo(mXmlReader.Value.Trim());
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read(); //end tag

                                                    }

                                                    mSender.sendContactInfo(conInfo);

                                                    break;



                                                



                                                case "txGetLog":
                                                    Console.WriteLine("Got GetLog\r\n");

                                                    string filter = "";
                                                    string direction = "";

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("filter"))
                                                        {
                                                            mXmlReader.Read();
                                                            filter = mXmlReader.Value.Trim();
                                                            Console.WriteLine("filter: " + filter);
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("direction"))
                                                        {
                                                            mXmlReader.Read();
                                                            direction = mXmlReader.Value.Trim();
                                                            Console.WriteLine("direction: " + direction);
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    ArrayList log;
                                                    int todaysIncoming = 0;
                                                    int todaysOutgoing = 0;

                                                    if (direction.Equals("in"))
                                                    {
                                                        log = Program.mSqlDb.getIncomingLog(filter);
                                                    }
                                                    else
                                                    {
                                                        log = Program.mSqlDb.getOutgoingLog(filter);
                                                    }
                                                    int[] count = Program.mSqlDb.getTodaysCount();
                                                    todaysIncoming = count[0];
                                                    todaysOutgoing = count[1];
                                                    mSender.sendTxtLog(log, todaysIncoming, todaysOutgoing);

                                                    break;





                                                case "txGetFailed":
                                                    Console.WriteLine("Got GetFailed\r\n");

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("failed"))
                                                        {
                                                            mXmlReader.Read();
                                                            if (mXmlReader.Value.Trim().Equals("yes"))
                                                            {
                                                                mSender.sendFailedLog(Program.mSqlDb.getFailedLog());
                                                            }
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    break;





                                                


                                                case "txGetModems":
                                                    Console.WriteLine("Got GetModems\r\n");

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Trim().Equals("modems"))
                                                        {
                                                            mXmlReader.Read();
                                                            if (mXmlReader.Value.Trim().Equals("yes"))
                                                            {
                                                                mSender.SendModemCount(Program.mModemManager.modemCount);
                                                            }
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    break;




                                                    //remove
                                                case "txGetModemLog":
                                                    Console.WriteLine("Go GetModemLog\r\n");

                                                    int mLog = -1;
                                                    string outputModemLog = "";

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Trim().Equals("modem"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out mLog);
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    if (mLog >= 0)
                                                    {
                                                        try
                                                        {
                                                            using (FileStream fs = File.OpenRead(Program.AthenaDir + @"logs\modem" + mLog + "_" + DateTime.Now.ToString("MM-dd-yyyy")))
                                                            {
                                                                using (StreamReader reader = new StreamReader(fs))
                                                                {
                                                                    outputModemLog = reader.ReadToEnd();
                                                                    outputModemLog = outputModemLog.Replace("<", "*^");
                                                                    outputModemLog = outputModemLog.Replace(">", "^*");
                                                                }
                                                            }
                                                        }
                                                        catch (Exception e)
                                                        {
                                                            Program.doEventLog("SockReciever.looper: " + e.Message + "\r\n" + e.StackTrace, 2);
                                                        }
                                                    }

                                                    mSender.SendModemLog(outputModemLog);

                                                    break;





                                                case "txGetDayTotals" :

                                                    string day = "";

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        mXmlReader.Read();
                                                        day = mXmlReader.Value.Trim();
                                                        mXmlReader.Read();
                                                    }

                                                    mSender.SendDayTotals(Program.mSqlDb.getDayTotals(day));

                                                    break;





                                                case "txGetReport":
                                                    Console.WriteLine("Got GetReport\r\n");

                                                    int typ = -1;
                                                    int snd = -1;

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Trim().Equals("type"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out typ);
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Trim().Equals("send"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out snd);
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    if (typ < 0 || snd < 0)
                                                    {
                                                        mSender.SendReportResult(false);
                                                    }
                                                    else
                                                    {
                                                        mSender.SendReportResult(Program.mSqlDb.createReport(typ, snd));
                                                    }

                                                    break;







                                                // jobs

                                                case "txGetJobInfo":

                                                    ArrayList jobInfo = new ArrayList();
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            mXmlReader.Read();
                                                            jobInfo = Program.mSqlDb.GetJobInfo(mXmlReader.Value.Trim());
                                                            mXmlReader.Read();
                                                        }
                                                    }

                                                    mSender.sendJobInfo(jobInfo);


                                                    break;


                                                case "txRunSingleJob":
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            mXmlReader.Read();
                                                            mSender.sendSingleJobResult(Program.runSingleJob(mXmlReader.Value.Trim()));
                                                            mXmlReader.Read();
                                                        }
                                                        else
                                                        {
                                                            mSender.sendSingleJobResult(false);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        mSender.sendSingleJobResult(false);
                                                    }

                                                    break;


                                                case "txGetScheduledJobs":

                                                    ArrayList scheduleJobs = new ArrayList();
                                                    int schJob = -1;
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("schedule"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out schJob);
                                                            if (schJob >= 0)
                                                            {
                                                                scheduleJobs =  Program.mSqlDb.GetScheduledJobs(schJob);
                                                            }
                                                        }
                                                    }

                                                    mSender.sendScheduledJobs(scheduleJobs, schJob);

                                                    break;


                                                case "txRunScheduledJobs":
                                                    Console.WriteLine("Got RunScheduledJobs\r\n");

                                                    bool didRunSchJobs = false;
                                                    int schToRun = 0;
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("schedule"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out schToRun);
                                                            if (schToRun>0)
                                                            {
                                                                didRunSchJobs =  Program.runScheduledJobs(schToRun);
                                                            }
                                                        }
                                                        mXmlReader.Read(); //end tag
                                                    }

                                                    mSender.sendScheduledJobsResult(didRunSchJobs);
                                                    //mSender.sendData("schjob", ranJob);

                                                    break;


                                                case "txAddScheduledJob":

                                                    int AddJobSch = -1;
                                                    string AddJobName = "";
                                                    string AddJobLocation = "";
                                                    string AddJobFile = "";
                                                    mXmlReader.Read();
                                                    while(mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("schedule"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out AddJobSch);
                                                            mXmlReader.Read();
                                                        }
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            mXmlReader.Read();
                                                            AddJobName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        if (mXmlReader.Name.Equals("location"))
                                                        {
                                                            mXmlReader.Read();
                                                            AddJobLocation = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        if (mXmlReader.Name.Equals("file"))
                                                        {
                                                            mXmlReader.Read();
                                                            AddJobFile = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    bool didAddSchJob = false;
                                                    if (AddJobSch >= 0 && !AddJobName.Equals("") && !AddJobLocation.Equals("") && !AddJobFile.Equals(""))
                                                    {
                                                        didAddSchJob = Program.mSqlDb.AddScheduledJob(AddJobSch, AddJobName, AddJobLocation, AddJobFile);
                                                    }

                                                    mSender.SendAddScheduledJobResult(didAddSchJob);

                                                    break;


                                                case "txEditScheduledJob":

                                                    int EditJobSch = -1;
                                                    string EditJobName = "";
                                                    string EditJobLocation = "";
                                                    string EditJobFile = "";
                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("schedule"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out EditJobSch);
                                                            mXmlReader.Read();
                                                        }
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            mXmlReader.Read();
                                                            EditJobName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        if (mXmlReader.Name.Equals("location"))
                                                        {
                                                            mXmlReader.Read();
                                                            EditJobLocation = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        if (mXmlReader.Name.Equals("file"))
                                                        {
                                                            mXmlReader.Read();
                                                            EditJobFile = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    bool didEditSchJob = false;
                                                    if (EditJobSch >= 0 && !EditJobName.Equals("") && !EditJobLocation.Equals("") && !EditJobFile.Equals(""))
                                                    {
                                                        didEditSchJob = Program.mSqlDb.EditScheduledJob(EditJobSch, EditJobName, EditJobLocation, EditJobFile);
                                                    }

                                                    mSender.SendEditScheduledJobResult(didEditSchJob);

                                                    break;


                                                case "txDeleteScheduledJob":

                                                    bool didDeleteSchJob = false;
                                                    string delSchJobName = "";
                                                    int delSchJobSch = -1;
                                                    mXmlReader.Read();
                                                    while (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("name"))
                                                        {
                                                            mXmlReader.Read();
                                                            delSchJobName = mXmlReader.Value.Trim();
                                                            mXmlReader.Read();
                                                        }
                                                        if (mXmlReader.Name.Equals("schedule"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out delSchJobSch);
                                                            mXmlReader.Read();
                                                        }
                                                        mXmlReader.Read();
                                                    }

                                                    if (!delSchJobName.Equals("") && delSchJobSch >= 0)
                                                    {
                                                        Console.WriteLine("deleting job '" + delSchJobName + "'");
                                                        didDeleteSchJob = Program.mSqlDb.DeleteScheduledJob(delSchJobName, delSchJobSch);
                                                    }


                                                    mSender.SendDeleteScheduledJobResult(didDeleteSchJob);



                                                    break;







                                                case "txCloseConn":
                                                    Console.WriteLine("Got CloseCon\r\n");

                                                    mClient.GetStream().Close();
                                                    deadClient = true;
                                                    doRun = false;

                                                    break;





                                                case "txKickConn":
                                                    Console.WriteLine("Got KickConn\r\n");

                                                    int kickCon = -1;
                                                    bool didKickCon = false;
                                                    mXmlReader.Read();
                                                    if (mXmlReader.IsStartElement())
                                                    {
                                                        if (mXmlReader.Name.Equals("conn"))
                                                        {
                                                            mXmlReader.Read();
                                                            Int32.TryParse(mXmlReader.Value.Trim(), out kickCon);
                                                            if (kickCon >= 0)
                                                            {
                                                                if (this != (SockReciever)mRecievers[kickCon])
                                                                {
                                                                    didKickCon = Program.mSocketManager.KickConnection(kickCon);
                                                                }

                                                            }
                                                            mXmlReader.Read(); //end tag
                                                        }

                                                    }

                                                    mSender.SendKickConnResult(didKickCon);

                                                    break;




                                                

                                            }


                                        }

                                    }
                                }
                            }

                        }



                    }




                }
                catch (Exception e)
                {
                    Console.WriteLine("SockReciever.looper " + e.Message + "\r\n" + e.StackTrace);
                    Program.doEventLog("SockReciever.looper " + e.Message + "\r\n" + e.StackTrace, 0);
                    mClient.Client.Close();
                    deadClient = true;
                    doRun = false;
                }
            }
        }

    }
}
