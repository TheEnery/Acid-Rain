
using UnityEngine;

namespace AcidRain
{
    public class Settings : MonoBehaviour
    {
        public static bool InvertCamera { get; private set; } = true;
        public static float MouseSensitivity { get; private set; } = 3.5f;
        public static KeyCode SwitchFpvToTpvModeKey { get; private set; } = KeyCode.Tab;
        public static KeyCode SwitchTpvToSpectatorModeKey { get; private set; } = KeyCode.LeftControl;
    }
}