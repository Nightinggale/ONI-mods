using HarmonyLib;
using UnityEngine;
using NightLib;
using System.Collections.Generic;
using System;

namespace Nightinggale.PipedOutput
{
    [HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
    public class DebugTest
    {
        public static void Postfix(BuildingDef def)
        {
            if (def == null)
                return;

            try
            {
                string text = "";

                foreach (var con in def.BuildingComplete?.GetComponents<ElementConverter>() ?? Array.Empty<ElementConverter>())
                {
                    text += " converter: ";
                    foreach (var elem in con.outputElements)
                        text += elem.elementHash.ToString() + " ";
                }

                foreach (var gen in def.BuildingComplete?.GetComponents<EnergyGenerator>() ?? Array.Empty<EnergyGenerator>())
                {
                    text += " generator: ";
                    foreach (var elem in gen.formula.outputs)
                        text += elem.element.ToString() + " ";
                }

                if (text != "")
                    Debug.Log($"Building {def.PrefabID} has{text}");
            }
            catch (Exception)
            {
            }
        }
    }

    // Note: I figured out this is unnecessary. The same can be archived with patching DoPostConfigureComplete, then modifing BuildingPreview/BuildingUnderConstruction.

    //[HarmonyPatch(typeof(IBuildingConfig), nameof(IBuildingConfig.DoPostConfigurePreview))]
    public class Base_DoPostConfigurePreview
    {
        public const string Preview = "Preview";

        public static Dictionary<Tag, System.Action<GameObject>> list = new Dictionary<Tag, System.Action<GameObject>>() {
            { ElectrolyzerConfig.ID + Preview, OxygenBuildingGenerationPatches.AddElectrolyzer },
        };

        public static void Postfix(GameObject go)
        {
            list.TryGetValue(go.PrefabID(), out var action);
            action?.Invoke(go);
        }
    }

    //[HarmonyPatch(typeof(IBuildingConfig), nameof(IBuildingConfig.DoPostConfigureUnderConstruction))]
    public class Base_DoPostConfigureUnderConstruction
    {
        public const string UnderConstruction = "UnderConstruction";

        public static Dictionary<Tag, System.Action<GameObject>> list = new Dictionary<Tag, System.Action<GameObject>>() {
            { ElectrolyzerConfig.ID + UnderConstruction, OxygenBuildingGenerationPatches.AddElectrolyzer },
        };

        public static void Postfix(GameObject go)
        {
            list.TryGetValue(go.PrefabID(), out var action);
            action?.Invoke(go);
        }
    }
}
