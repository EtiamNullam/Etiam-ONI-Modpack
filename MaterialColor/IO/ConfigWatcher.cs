using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MaterialColor.IO
{
    public class ConfigWatcher : IRender1000ms, IDisposable
    {
        private bool _configuratorStateChanged;
        private bool _elementColorInfosChanged;

        FileSystemWatcher colorInfosWatcher;
        FileSystemWatcher stateWatcher;

        public ConfigWatcher()
        {
            this.SubscribeToFileChangeNotifier();
        }

        void IRender1000ms.Render1000ms(float dt)
        {
            if (_elementColorInfosChanged || _configuratorStateChanged)
            {
                Painter.Refresh();
                _elementColorInfosChanged = _configuratorStateChanged = false;
            }
        }

        public void Dispose()
        {
            this.stateWatcher.Dispose();
            this.colorInfosWatcher.Dispose();
        }

        private void OnMaterialStateChanged(object sender, FileSystemEventArgs e)
        {
            if (!State.TryReloadConfiguratorState())
            {
                return;
            }

            _configuratorStateChanged = true;

            const string message = "Configurator state changed.";

            Debug.Log(message);
        }

        private void OnElementColorsInfosChanged(object sender, FileSystemEventArgs e)
        {
            bool reloadColorInfosResult = false;

            try
            {
                reloadColorInfosResult = State.TryReloadElementColorInfos();
            }
            catch (Exception ex)
            {
                Debug.Log("ReloadElementColorInfos failed.");
                Debug.Log(ex);
            }

            if (reloadColorInfosResult)
            {
                _elementColorInfosChanged = true;

                const string message = "Element color infos changed.";

                Debug.Log(message);
                Debug.LogError(message);
            }
            else
            {
                Debug.Log("Reload element color infos failed");
            }
        }

        private void SubscribeToFileChangeNotifier()
        {
            const string jsonFilter = "*.json";

            try
            {
                this.colorInfosWatcher = new FileSystemWatcher(Paths.ElementColorInfosDirectory, jsonFilter);
                this.stateWatcher = new FileSystemWatcher(Paths.MaterialConfigPath, Paths.MaterialColorStateFileName);

                this.colorInfosWatcher.Changed += this.OnElementColorsInfosChanged;
                this.stateWatcher.Changed += this.OnMaterialStateChanged;
            }
            catch (Exception e)
            {
                Debug.Log("SubscribeToFileChangeNotifier failed");
                Debug.Log(e);
            }
        }
    }
}
