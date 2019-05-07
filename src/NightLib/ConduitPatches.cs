using Harmony;
using DisplayPortHelpers;

namespace NightLib
{
    public static class ConduitDisplayPortPatches
    {
        [HarmonyPatch(typeof(BuildingCellVisualizer))]
        [HarmonyPatch("DisableIcons")]
        public static class HidePorts
        {
            public static void Postfix(BuildingCellVisualizer __instance)
            {
                if (__instance.GetBuilding().name.IsBuildingPartOfThisMod())
                {
                    UnityEngine.GameObject go = __instance.GetBuilding().gameObject;
                    foreach (NightLib.PortDisplay portDisplay in go.GetComponents<NightLib.PortDisplay>())
                    {
                        portDisplay.DisableIcons();
                    }
                }
            }
        }
        

        [HarmonyPatch(typeof(BuildingCellVisualizer))]
        [HarmonyPatch("Tick")]
        public static class ApplyPorts
        {
            public static void Postfix(BuildingCellVisualizer __instance, HashedString mode)
            {
                ConduitType type =
                    (mode == OverlayModes.LiquidConduits.ID) ? ConduitType.Liquid :
                    (mode == OverlayModes.GasConduits.ID) ? ConduitType.Gas :
                    (mode == OverlayModes.SolidConveyor.ID) ? ConduitType.Solid :
                    ConduitType.None;

                if (type != ConduitType.None && __instance.GetBuilding().name.IsBuildingPartOfThisMod())
                {
                    UnityEngine.GameObject go = __instance.GetBuilding().gameObject;
                    foreach (NightLib.PortDisplay portDisplay in go.GetComponents<NightLib.PortDisplay>())
                    {
                        if (type == portDisplay.type)
                        {
                            portDisplay.Draw(go, __instance);
                        }
                    }
                }
            }

        }

    }
}
