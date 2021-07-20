using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightinggale.PipedOutput
{
    [HarmonyPatch(typeof(Assets), nameof(Assets.AddBuildingDef))]
    public class MasterPatch
    {
        public static void Prefix(BuildingDef def)
        {
            if (def.PrefabID == GeneratorConfig.ID)
                PowerBuildingGenerationPatches.CoalBurnerComplete(def);
            else if (def.PrefabID == WoodGasGeneratorConfig.ID)
                PowerBuildingGenerationPatches.WoodBurnerComplete(def);
            else if (def.PrefabID == PetroleumGeneratorConfig.ID)
                PowerBuildingGenerationPatches.OilBurnerComplete(def);
            else if (def.PrefabID == MethaneGeneratorConfig.ID)
                PowerBuildingGenerationPatches.GasBurnerComplete(def);

            else if (def.PrefabID == OilRefineryConfig.ID)
                RefinementBuildingGenerationPatches.OilRefineryComplete(def);
            else if (def.PrefabID == FertilizerMakerConfig.ID)
                RefinementBuildingGenerationPatches.FertilizerMakerComplete(def);
            else if (def.PrefabID == EthanolDistilleryConfig.ID)
                RefinementBuildingGenerationPatches.EthanolDistilleryComplete(def);
            else if (def.PrefabID == PolymerizerConfig.ID)
                RefinementBuildingGenerationPatches.PolymerComplete(def);

            else if (def.PrefabID == AlgaeHabitatConfig.ID)
                OxygenBuildingGenerationPatches.AlgaeHabitatComplete(def);
            else if (def.PrefabID == ElectrolyzerConfig.ID)
                OxygenBuildingGenerationPatches.ElectrolyzerComplete(def);
            else if (def.PrefabID == MineralDeoxidizerConfig.ID)
                OxygenBuildingGenerationPatches.MineralDeoxidizerComplete(def);
            else if (def.PrefabID == RustDeoxidizerConfig.ID)
                OxygenBuildingGenerationPatches.RustComplete(def);

            else if (def.PrefabID == GourmetCookingStationConfig.ID)
                CookingBuildingGenerationPatches.GourmetCookingComplete(def);

            else if (def.PrefabID == OilWellCapConfig.ID)
                UtilityBuildingGenerationPatches.OilWellComplete(def);
        }
    }
}
