using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    public class ConfigWatcher<T> : IDisposable
    {
        public ConfigWatcher(string path, string pattern, Action<T> callback)
        {
            this._callback += callback;
            this.InitWatcher(path, pattern);
        }

        private Action<T> _callback;
        private FileSystemWatcher _watcher;
        
        private void InitWatcher(string path, string pattern)
        {
            try
            {
                if (Directory.Exists(path))
                {
                    var watcher = new FileSystemWatcher(path, pattern);

                    watcher.Changed += this.OnChanged;
                    watcher.Created += this.OnChanged;
                    watcher.Renamed += this.OnChanged;
                    watcher.Deleted += this.OnChanged;

                    watcher.EnableRaisingEvents = true;

                    Logger.Default.LogDebug($"Set FileWatcher on {path}{Path.DirectorySeparatorChar}{pattern}");
                }
                else
                {
                    Logger.Default.Log("Config parent directory doesn't exist:" + path);
                }
            }
            catch (Exception e)
            {
                Logger.Default.Log("Failed to initialize watcher at " + path, e);
            }
        }

        private void OnChanged(object sender, FileSystemEventArgs ev)
        {
            try
            {
                this._callback(ConfigHelper.Load<T>(ev.FullPath));
            }
            catch (Exception ex)
            {
                Logger.Default.Log("Error while loading from path: " + ev.FullPath, ex);
            }
        }

        public void Dispose()
        {
            this._callback = null;
            this._watcher?.Dispose();
            this._watcher = null;
        }
    }
}
