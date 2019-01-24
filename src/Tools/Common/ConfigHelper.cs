using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public static class ConfigHelper<T>
    {
        private static Action<T> _callback;

        public static void Watch(string path, string pattern, Action<T> callback, bool relativePath = true)
        {
            _callback += callback;

            if (relativePath)
            {
                Pathfinder.ToRelativePath(ref path);
            }

            InitWatcher(path, pattern);
        }

        /// <summary>
        /// Start FileWatcher with given pattern in ModRoot
        /// </summary>
        public static void Watch(string pattern, Action<T> callback)
        {
            _callback += callback;

            InitWatcher(ModState.RootPath, pattern);
        }
        
        private static void InitWatcher(string path, string pattern)
        {
            var watcher = new FileSystemWatcher(path, pattern);
            watcher.Changed += OnChanged;
            watcher.EnableRaisingEvents = true;
        }

        private static T Load(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        public static bool TryLoad(string path, out T result, bool relativePath = true)
        {
            try
            {
                Logger.Log(ModState.RootPath);
                if (relativePath)
                {
                    Pathfinder.ToRelativePath(ref path);
                    Logger.Log(path);
                }

                result = Load(path);

                return true;
            }
            catch (Exception ex)
            {
                Logger.Log("Error while loading from path: " + path, ex);
                result = default(T);
                return false;
            }
        }

        private static void OnChanged(object sender, FileSystemEventArgs ev)
        {
            if (TryLoad(ev.FullPath, out var result, false))
            {
                _callback(result);
            }
        }
    }
}
