﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AthenaCore;
using System.Collections;

namespace AthenaCLI
{
    class Program
    {
        private static bool isRunning = true;
        static void Main(string[] args)
        {

            Console.WriteLine("Starting Athena Core");

            Core mCore = new Core();

            Console.WriteLine("\r\n\r\nWelcome to Athena (enter 'help' for menu)");

            // menu?

            while (isRunning)
            {
                Console.Write("\r\nAthena >");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "help":
                        PrintMenu();
                        break;
                    case "modems":
                        Console.WriteLine("\r\nModems:");
                        int modemCount = 0;
                        foreach (IModem modem in mCore.mModemManager.myModems)
                        {
                            Console.WriteLine("modem: " + modem.GetMyNumber() + " -  port: " + modem.GetMyPortName());
                            modemCount++;
                        }
                        if (modemCount < 1)
                        {
                            Console.WriteLine("No Modems");
                        }
                        break;
                    case "users":
                        Console.WriteLine("\r\nUsers:");
                        int regCount = 0;
                        int userCount = 0;

                        Console.WriteLine("Registered Users...");
                        List<Dictionary<string, object>> list = mCore.mSqlDb.getUserList();
                        foreach (Dictionary<string, object> user in list)
                        {
                            Console.WriteLine("- " + user["Name"] + ", " + user["Level"]);
                            regCount++;

                        }
                        if (regCount < 1)
                        {
                            Console.WriteLine("No Users Registered");
                        }

                        Console.WriteLine("\r\nConnected Users...");
                        foreach (SockReciever receiver in mCore.mSocketManager.mRecievers)
                        {
                            Console.WriteLine(receiver.mUserName);
                            userCount++;
                        }
                        if (userCount < 1)
                        {
                            Console.WriteLine("No Users Connected");
                        }

                        break;
                    case "status":
                        Console.WriteLine("Everything is Peachy!");
                        break;
                    case "exit":
                    case "quit":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("unknown command '" + input + "'");
                        break;
                }
            }

            Console.WriteLine("GoodBye");
        }

        private static void PrintMenu()
        {
            Console.WriteLine("\r\n*Athena Console Menu*\r\nstatus - show service status\r\nusers - show socket connections.\r\nmodems - show all modems.\r\nquit/exit - shutdown athena\r\n");
        }

    }
}
