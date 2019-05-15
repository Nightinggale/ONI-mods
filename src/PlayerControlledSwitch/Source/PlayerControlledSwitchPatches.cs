using Harmony;
using NightLib;

namespace PlayerControlledSwitch
{
    public static class PlayerControlledSwitch
    {
        [HarmonyPatch(typeof(Switch))]
        [HarmonyPatch("OnMinionToggle")]
        public static class SwitchTogglePatch
        {
            public static bool Prefix(Switch __instance)
            {
                ReadPrivate.Call(__instance, "Toggle");
                return false;
            }
        }
    }
}