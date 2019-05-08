using System;

// IsBuildingPartOfThisMod is an important extension method used by the conduit display.
// 
// Return true if building has at least one PortDisplay component.
// Fill in all such buildings added by your mod.
//
// Autodetection has been ruled out due to performance issues and risk of failure to autodetect correctly.

namespace DisplayPortHelpers
{
    internal static class DisplayPortHelpers
    {
        internal static bool IsBuildingPartOfThisMod(this String building)
        {
            return building.Equals(HighFlowStorage.HighFlowLiquidReservoirConfig.ID + "Complete") || building.Equals(HighFlowStorage.HighFlowLiquidReservoirConfig.ID + "UnderConstruction");
        }
    }
}
