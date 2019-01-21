using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Common
{
    // TODO: rename from - it doesnt have to do anything with config yet
    public static class ConfigWatcher
    {
        // TODO: add remove watcher too? is it neede though?
        public static void SetWatcher(string path, string pattern, FileSystemEventHandler handler)
        {
            var watcher = new FileSystemWatcher(path, pattern);
            watcher.Changed += handler;
            watcher.EnableRaisingEvents = true;
        }
    }
}
