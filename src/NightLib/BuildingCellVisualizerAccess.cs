using System.Linq;
using UnityEngine;
using System.Reflection;


namespace NightLib
{
    internal static class BuildingCellVisualizerAccess
    {
        internal static Building GetBuilding(this BuildingCellVisualizer __instance)
        {
            return ReadPrivate.Get(typeof(BuildingCellVisualizer), __instance, "building") as Building;
        }

        internal static void DrawUtilityIcon(this BuildingCellVisualizer __instance, int cell, Sprite icon_img, ref GameObject visualizerObj, Color tint, Color connectorColor, float scaleMultiplier = 1.5f, bool hideBG = false)
        {
            var mi = typeof(BuildingCellVisualizer).GetMethods(BindingFlags.InvokeMethod | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            .Where(m => m.Name == "DrawUtilityIcon" && m.GetParameters().Length == 7).First();
            if (mi != null)
            {
                var varlist = new object[] { cell, icon_img, visualizerObj, tint, connectorColor, scaleMultiplier, hideBG };
                mi.Invoke(__instance, varlist);
                visualizerObj = varlist[2] as GameObject;
            }
        }
    }
}
