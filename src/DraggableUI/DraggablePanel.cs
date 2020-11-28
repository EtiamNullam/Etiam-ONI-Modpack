using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace DraggablePanelMod
{
    public class DraggablePanel : MonoBehaviour
    {
        public Vector2 Offset;

        private static readonly List<KeyCode> DragStartKeyCodes = new List<KeyCode>
        {
            KeyCode.LeftAlt,
            KeyCode.RightAlt,
        };

        // Use GetComponent<KScreen>() instead?
        public KScreen Screen;

        public Vector2 DefaultPosition = new Vector2();

        private bool _isDragging;

        public static void Attach(KScreen screen)
        {
            if (screen == null || screen.name == "SimpleInfoScreen")
            {
                return;
            }

            DraggablePanel panel = screen.FindOrAddUnityComponent<DraggablePanel>();

            if (panel == null)
            {
                return;
            }

            panel.Screen = screen;

            var rect = panel.GetComponentInParent<RectTransform>();

            if (rect == null)
            {
                return;
            }

            panel.DefaultPosition = rect.anchoredPosition;

            State.Common.Logger.LogDebug("Attached to KScreen", screen.displayName);
        }

        // TODO: call when position is set by game
        public static void SetPositionFromFile(KScreen screen)
        {
            DraggablePanel panel = screen.FindOrAddUnityComponent<DraggablePanel>();

            if (panel == null)
            {
                State.Common.Logger.LogDebug("Can't FindOrAddUnityComponent");

                return;
            }

            if (panel.LoadPosition(out Vector2 newPosition))
            {
                panel.SetPosition(newPosition);

                State.Common.Logger.Log($"Loaded position: {newPosition} for {screen.name}");
            }
        }

        public void Update()
        {
            if (this.Screen == null)
            {
                return;
            }

            Vector2 mousePos = Input.mousePosition;

            if (this._isDragging)
            {
                Vector2 newPosition = mousePos - this.Offset;

                if (Input.GetMouseButtonUp(0))
                {
                    this._isDragging = false;

                    this.SavePosition(newPosition);

                    State.Common.Logger.LogDebug("Saved new panel position: ", newPosition);
                }

                this.SetPosition(newPosition);
            }
            else if (this.ShouldStartDrag())
            {
                // TODO: cache RectTransform component
                this.Offset = mousePos - this.Screen.GetComponentInParent<RectTransform>().anchoredPosition;

                this._isDragging = true;
            }
            else if (this.ShouldResetPanel())
            {
                this.ResetPanel();
            }
        }

        private void ResetPanel()
        {
            this.SavePosition(this.DefaultPosition);
            this.SetPosition(this.DefaultPosition);
        }

        private bool ShouldStartDrag()
        {
            return this.Screen.GetMouseOver
                && Input.GetMouseButtonDown(0)
                && DragStartKeyCodes.Any(code => Input.GetKey(code));
        }

        private bool ShouldResetPanel()
        {
            return this.Screen.GetMouseOver
                && Input.GetMouseButtonUp(1)
                && DragStartKeyCodes.Any(code => Input.GetKey(code));
        }

        private bool LoadPosition(out Vector2 position)
        {
            return State.UIState.LoadWindowPosition(this.gameObject, out position);
        }

        private void SavePosition(Vector2 position)
        {
            State.UIState.SaveWindowPosition(this.gameObject, position);
        }

        // use offset?
        private void SetPosition(Vector3 newPosition)
        {
            var rect = this.Screen?.GetComponentInParent<RectTransform>();

            if (rect == null)
            {
                return;
            }

            rect.anchoredPosition = newPosition;
        }
    }
}