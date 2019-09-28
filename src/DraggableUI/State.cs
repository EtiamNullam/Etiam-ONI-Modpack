using Common;
using DraggablePanelMod.Data;

namespace DraggablePanelMod
{
    public static class State
    {
        public static Core Common = new Core("DraggableUI", "1870540864", null, false);

        public static string StateFileName = "State.json";

        private static DraggableUIState _uiState;

        public static DraggableUIState UIState => _uiState ?? (_uiState = new DraggableUIState());
    }
}