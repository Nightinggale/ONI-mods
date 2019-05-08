using Harmony;
using DisplayPortHelpers;
using NightLib;

namespace NightLib.PortDisplayDrawing
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
                    foreach (PortDisplay portDisplay in go.GetComponents<PortDisplay>())
                    {
                        portDisplay.DisableIcons();
                    }
                }
            }
        }
        

        [HarmonyPatch(typeof(BuildingCellVisualizer))]
        [HarmonyPatch("Tick")]
        public static class DrawPorts
        {
            internal static bool useGas = false;
            internal static bool useLiquid = false;
            internal static bool useSolid = false;

            public static void Postfix(BuildingCellVisualizer __instance, HashedString mode)
            {
                if (useGas && mode == OverlayModes.GasConduits.ID)
                {
                    if (__instance.GetBuilding().name.IsBuildingPartOfThisMod())
                    {
                        UnityEngine.GameObject go = __instance.GetBuilding().gameObject;
                        foreach (PortDisplay portDisplay in go.GetComponents<PortDisplayGas>())
                        {
                            portDisplay.Draw(go, __instance);
                        }
                    }
                    return;
                }
                if (useLiquid && mode == OverlayModes.LiquidConduits.ID)
                {
                    if (__instance.GetBuilding().name.IsBuildingPartOfThisMod())
                    {
                        UnityEngine.GameObject go = __instance.GetBuilding().gameObject;
                        foreach (PortDisplay portDisplay in go.GetComponents<PortDisplayLiquid>())
                        {
                            portDisplay.Draw(go, __instance);
                        }
                    }
                    return;
                }
                if (useSolid && mode == OverlayModes.SolidConveyor.ID)
                {
                    if (__instance.GetBuilding().name.IsBuildingPartOfThisMod())
                    {
                        UnityEngine.GameObject go = __instance.GetBuilding().gameObject;
                        foreach (PortDisplay portDisplay in go.GetComponents<PortDisplaySolid>())
                        {
                            portDisplay.Draw(go, __instance);
                        }
                    }
                    return;
                }
            }

        }

    }
}
