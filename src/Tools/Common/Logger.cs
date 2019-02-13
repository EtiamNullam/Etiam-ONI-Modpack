using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Common
{
    public class Logger
    {
        // TODO: move logDebugMessages to config
        public Logger(string modName, bool logDebugMessages)
        {
            this._modName = modName;
            this._debug = logDebugMessages;
        }

        public static Logger Default = new Logger("Common", true);

        private readonly string _modName;
        private readonly bool _debug;

        public static readonly HashSet<string> Messages = new HashSet<string>();

        public void LogOnce(string message, object data)
        {
            if (!Messages.Contains(message))
            {
                this.Log(message + Environment.NewLine + data);
                Messages.Add(message);
            }
            else
            {
                this.LogDebug(message, data);
            }
        }

        public void LogOnce(object data)
        {
            var message = data.ToString();
            if (!Messages.Contains(message))
            {
                this.Log(data);
                Messages.Add(message);
            }
            else
            {
                this.LogDebug(data);
            }
        }

        public void LogDebug(string message, object data)
        {
            if (this._debug)
            {
                this.Log(message + Environment.NewLine + data);
            }
        }

        public void LogDebug(object data)
        {
            if (this._debug)
            {
                this.Log(data);
            }
        }

        public void Log(string message, object data)
        {
            Debug.Log(this._modName + ": " + message + Environment.NewLine + data);
        }

        public void Log(object data)
        {
            Debug.Log(this._modName + ": " + data);
        }
    }
}
