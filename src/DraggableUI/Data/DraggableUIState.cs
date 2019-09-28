using Newtonsoft.Json;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace DraggablePanelMod.Data
{

    public class DraggableUIState
    {
        private Dictionary<string, SerializeableVector2> _windowPositions;

        private Dictionary<string, SerializeableVector2> WindowPositions =>
        this._windowPositions ?? (this._windowPositions = this.LoadFile());

        public bool LoadWindowPosition(GameObject window, out Vector2 position)
        {
            if (window == null)
            {
                position= Vector2.zero;
                return false;
            }

            string key = this.ExtractKey(window);

            bool result = this.WindowPositions.TryGetValue(key, out SerializeableVector2 sVector2);

            position = sVector2.ToVector2();

            return result;
        }

        public void SaveWindowPosition(GameObject window, Vector2 position)
        {
            string key = this.ExtractKey(window);

            this.WindowPositions[key] = this.VectorToTuple(position);

            this.UpdateFile();
        }

        private string ExtractKey(GameObject window) => window.name;

        private Dictionary<string, SerializeableVector2> LoadFile()
        {
            try
            {
                return State.Common.LoadConfig<Dictionary<string, SerializeableVector2>>(State.StateFileName);
            }
            catch (Exception e)
            {
                State.Common.Logger.LogOnce("Draggable UI state load failed.", e);

                return new Dictionary<string, SerializeableVector2>();
            }
        }

        private void UpdateFile()
        {
            try
            {
                State.Common.SaveConfig(State.StateFileName, this.WindowPositions);
            }
            catch (Exception e)
            {
                State.Common.Logger.LogOnce("Draggable UI state save failed.", e);
            }
        }

        private SerializeableVector2 VectorToTuple(Vector2 vector) => new SerializeableVector2(vector.x, vector.y);
    }
}