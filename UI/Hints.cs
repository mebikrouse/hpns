using static CitizenFX.Core.Native.API;

namespace HPNS.UI
{
    public static class Hints
    {
        public static void ShowNextHint(string content)
        {
            SendNuiMessage("{\"type\":\"show_hint\",\"content\":\"" + content + "\"}");
        }

        public static void HideCurrentHint()
        {
            SendNuiMessage("{\"type\":\"hide_hint\"}");
        }
    }
}