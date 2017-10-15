using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * This class is a base template for an SMS service API
 */

namespace AthenaCore
{
    class ApiModem : IModem
    {

        private bool installedProperly = false;
        private bool modemReady = false;
        private bool offHook = false;
        private int myNumber = 0;
        private string portName = null;
        protected Core mCore;

        public ApiModem(Core core, String port, int num)
        {
            // TODO

            try
            {
                mCore = core;
                portName = port;
                myNumber = num;

                installedProperly = true;
                modemReady = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\r\n" + e.StackTrace);
            }


        }
        public bool IsInstalledProperly()
        {
            return installedProperly.Equals(true);
        }
        public bool IsModemReady()
        {
            return modemReady.Equals(true);
        }
        public bool IsOffhook()
        {
            return offHook.Equals(true);
        }
        public int GetMyNumber()
        {
            return (myNumber + 0);
        }
        public string GetMyPortName()
        {
            return new string(portName.ToCharArray());
        }
        public void ShutDown()
        {

        }
        public void sendSMS(String num, String msg)
        {

        }
    }
}
