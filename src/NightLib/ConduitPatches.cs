﻿using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;
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
                if (DrawPorts.HasBuilding(__instance.GetBuilding().name))
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
            // cache variables
            private static bool useGas    = false;
            private static bool useLiquid = false;
            private static bool useSolid  = false;

            private static HashSet<string> buildings       = new HashSet<string>();
            private static HashSet<string> buildingsGas    = new HashSet<string>();
            private static HashSet<string> buildingsLiquid = new HashSet<string>();
            private static HashSet<string> buildingsSolid  = new HashSet<string>();

            // Draw port icons on the screen
            // Uses the cache to skip buildings the cache tells will not have ports
            // Uses cache to skip overlays entirely if no ports exist for the overlay in question
            public static void Postfix(BuildingCellVisualizer __instance, HashedString mode)
            {
                if (useGas && mode == OverlayModes.GasConduits.ID)
                {
                    if (buildingsGas.Contains(__instance.GetBuilding().name))
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
                    if (buildingsLiquid.Contains(__instance.GetBuilding().name))
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
                    if (buildingsSolid.Contains(__instance.GetBuilding().name))
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

            private static void AddBuilding(string ID, ref HashSet<String> list)
            {
                list.Add(ID + "Complete");
                list.Add(ID + "UnderConstruction");
            }

            internal static bool HasBuilding(string name)
            {
                return buildings.Contains(name);
            }

            // Add a building to the cache
            internal static void AddBuilding(string ID, ConduitType type)
            {
                AddBuilding(ID, ref buildings);
                switch (type)
                {
                    case ConduitType.Gas:
                        AddBuilding(ID, ref buildingsGas);
                        useGas = true;
                        break;
                    case ConduitType.Liquid:
                        AddBuilding(ID, ref buildingsLiquid);
                        useLiquid = true;
                        break;
                    case ConduitType.Solid:
                        AddBuilding(ID, ref buildingsSolid);
                        useSolid = true;
                        break;
                }
            }
        }

        // Assign cells for ports while building to prevent other buildings from adding ports at the same cells
        [HarmonyPatch(typeof(BuildingDef))]
        [HarmonyPatch("MarkArea")]
        public static class MarkArea
        {
            public static void Postfix(BuildingDef __instance, int cell, Orientation orientation, ObjectLayer layer, GameObject go)
            {
                foreach (PortDisplay portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay>())
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
                    foreach (PortDisplay portDisplay in __instance.BuildingComplete.GetComponents<PortDisplay>())
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
