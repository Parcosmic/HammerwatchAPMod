using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TiltedEngine.Drawing;
using HammerwatchAP.Archipelago;

namespace HammerwatchAP.Util
{
    public class Logging
    {
        public static void Log(string message)
        {
            ResourceContext.Log(message);
        }
        public static void Debug(string message)
        {
            if (!ArchipelagoManager.DEBUG_MODE)
                return;
            ResourceContext.Log("--DEBUG: "+message);
        }
        public static void DebugObject(object obj)
        {
            if (!ArchipelagoManager.DEBUG_MODE)
                return;
            try
            {
                Debug(obj.ToString());
            }
            catch (NullReferenceException)
            {
                Debug("null");
            }
        }
        public static void GameLog(string message)
        {
            ArchipelagoMessageManager.SendHWMessage(message);
        }

        public static void LogConnectionInfo(string info)
        {
            string domain = info.Split(':')[0];
            string[] noncustomDomains = new string[]{
                    "archipelago",
                    "localhost",
                    "127.0.0.1",
                    "127.0.1.1",
                };
            if (!noncustomDomains.Contains(domain))
            {
                domain = "custom domain";
            }
            Log($"Creating AP session at {domain}");
        }
    }
}
