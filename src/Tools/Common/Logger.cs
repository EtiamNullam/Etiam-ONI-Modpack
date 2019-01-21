using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common
{
    public static class Logger
    {
        // TODO: try printing all messages on crash/exit
        public static readonly HashSet<string> Messages = new HashSet<string>();

        public static void LogOnce(string message, object data)
        {
            if (!Messages.Contains(message))
            {
                Log(message + Environment.NewLine + data);
                Messages.Add(message);
            }
            else
            {
                LogDebug(message, data);
            }
        }

        public static void LogOnce(object data)
        {
            var message = data.ToString();
            if (!Messages.Contains(message))
            {
                Log(data);
                Messages.Add(message);
            }
            else
            {
                LogDebug(data);
            }
        }

        public static void LogDebug(string message, object data)
        {
            if (ModState.Debug)
            {
                Log(message + Environment.NewLine + data);
            }
        }

        public static void LogDebug(object data)
        {
            if (ModState.Debug)
            {
                Log(data);
            }
        }

        public static void Log(string message, object data)
        {
            Debug.Log(ModState.Name + ": " + message + Environment.NewLine + data);
        }

        public static void Log(object data)
        {
            Debug.Log(ModState.Name + ": " + data);
        }
    }
}
