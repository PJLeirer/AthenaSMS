using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AthenaCore
{
    public interface IModem
    {

        bool IsInstalledProperly();
        bool IsModemReady();
        bool IsOffhook();
        int GetMyNumber();
        string GetMyPortName();
        void ShutDown();
        void sendSMS(String num, String msg);
    }
}
