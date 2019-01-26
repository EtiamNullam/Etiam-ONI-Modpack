using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    // TODO: won't work if watching multiple paths for the same generic type, in one mod
    public static class ConfigHelper<T>
    {
        private static Action<T> _callback;

        public static void Watch(string path, string pattern, Action<T> callback, bool relativePath = true)
        {
            _callback += callback;

            if (relativePath)
            {
                path = Pathfinder.ToRelativeToConfigPath(path);
            }

            InitWatcher(path, pattern);
        }

        /// <summary>
        /// Start FileWatcher with given pattern in config root (ConfigPath).
        /// </summary>
        public static void Watch(string pattern, Action<T> callback)
        {
            _callback += callback;

            InitWatcher(ModState.ConfigPath, pattern);
        }
        
        private static void InitWatcher(string path, string pattern)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var watcher = new FileSystemWatcher(path, pattern);

                    watcher.Changed += OnChanged;
                    watcher.Created += OnChanged;
                    watcher.Renamed += OnChanged;
                    watcher.Deleted += OnChanged;

                    watcher.EnableRaisingEvents = true;

                    Logger.LogDebug($"Set FileWatcher on {path}{Path.DirectorySeparatorChar}{pattern}");
                }
                else
                {
                    Logger.Log("Config parent directory doesn't exist:" + path);
                }
            }
            catch (Exception e)
            {
                Logger.Log("Failed to initialize watcher at " + path, e);
            }
        }

        private static T Load(string path)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }

        public static bool TryLoad(string path, out T result, bool relativePath = true)
        {
            try
            {
                if (relativePath)
                {
                    path = Pathfinder.ToRelativeToConfigPath(path);
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
