using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using NightLib;

namespace NightLib.PortDisplayDrawing
{
    public static class ConduitDisplayPortPatches
    {
        [HarmonyPatch(typeof(BuildingCellVisualizer))]
        [HarmonyPatch("DrawIcons")]
        public static class DrawPorts
        {
            // cache variables
            private static HashSet<string> buildings       = new HashSet<string>();
            
            public static bool Prefix(BuildingCellVisualizer __instance, HashedString mode)
            {

                if (buildings.Contains(__instance.GetBuilding().Def.PrefabID))
                {
                    UnityEngine.GameObject go = __instance.GetBuilding().gameObject;
                    PortDisplayController controller = go.GetComponent<PortDisplayController>();
                    if (controller != null)
                    {
                        return controller.Draw(__instance, mode, go);
                    }
                }
                return true;
            }

            internal static bool HasBuilding(string name)
            {
                return buildings.Contains(name);
            }

            // Add a building to the cache
            internal static void AddBuilding(string ID)
            {
                buildings.Add(ID);
            }
        }

        // Assign cells for ports while building to prevent other buildings from adding ports at the same cells
        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch("MarkArea")]
        public static class MarkArea
        {
            public static void Postfix(BuildingDef __instance, int cell, Orientation orientation, ObjectLayer layer, GameObject go)
            {
                foreach (PortDisplay2 portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
                {
                    ConduitType secondaryConduitType2 = portDisplay.type;
                    ObjectLayer objectLayerForConduitType4 = Grid.GetObjectLayerForConduitType(secondaryConduitType2);
                    CellOffset rotatedCellOffset8 = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
                    int cell11 = Grid.OffsetCell(cell, rotatedCellOffset8);
                    __instance.MarkOverlappingPorts(Grid.Objects[cell11, (int)objectLayerForConduitType4], go);
                    Grid.Objects[cell11, (int)objectLayerForConduitType4] = go;
                }
            }
        }

        // Check if ports are blocked prior to building
        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch("AreConduitPortsInValidPositions")]
        public static class AreConduitPortsInValidPositions
        {
            public static void Postfix(BuildingDef __instance, ref bool __result, GameObject source_go, int cell, Orientation orientation, ref string fail_reason)
            {
                if (__result)
                {
                    foreach (PortDisplay2 portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay2>())
                    {
                        CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(portDisplay.offset, orientation);
                        int utility_cell = Grid.OffsetCell(cell, rotatedCellOffset);
                        __result = (bool)NightLib.ReadPrivate.Call(__instance, "IsValidConduitConnection", source_go, portDisplay.type, utility_cell, ref fail_reason);
                        if (!__result)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}
