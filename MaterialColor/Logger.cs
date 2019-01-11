using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MaterialColor
{
    public static class Logger
    {
        private const string ModName = "MaterialColor";

        // TODO: try printing all messages on crash/exit
        private static List<object> Messages = new List<object>();

        public static void LogOnce(object message)
        {
            // TODO: check probably wont work with "object" type
            if (!Messages.Contains(message))
            {
                Log(message);
                Messages.Add(message);
            }
            else LogDebug(message);
        }

        public static void LogDebug(object message)
        {
            if (State.Config.Debug)
            {
                Log(message);
            }
        }

        public static void Log(object message)
        {
            Debug.Log(ModName + ": " + message);
        }
    }
}
