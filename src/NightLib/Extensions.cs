/// <summary>
/// Extention methods
/// 
/// This file contains extension methods to various classes in ONI.
/// 
/// </summary>


using UnityEngine;

namespace NightLib
{
    internal static class ExtensionMethods
    {
        // Get the index of the layer with the pipes for the conduit type in question 
        internal static int GetConduitObjectLayer(this ConduitType conduitType)
        {
            switch (conduitType)
            {
                case ConduitType.Gas:    return (int)ObjectLayer.GasConduit;
                case ConduitType.Liquid: return (int)ObjectLayer.LiquidConduit;
                case ConduitType.Solid:  return (int)ObjectLayer.SolidConduit;
            }
            return 0;
        }

        // Get the index of the layer with the connectors (ports) for the conduit type in question 
        internal static int GetPortObjectLayer(this ConduitType conduitType)
        {
            switch (conduitType)
            {
                case ConduitType.Gas:    return (int)ObjectLayer.GasConduitConnection;
                case ConduitType.Liquid: return (int)ObjectLayer.LiquidConduitConnection;
                case ConduitType.Solid:  return (int)ObjectLayer.SolidConduitConnection;
            }
            return 0;
        }

        // Get a cell of a building. Takes rotation into account
        internal static int GetCellWithOffset(this Building building, CellOffset offset)
        {
            Vector3 position = building.transform.GetPosition();
            int bottomLeftCell = Grid.PosToCell(position);

            CellOffset rotatedOffset = building.GetRotatedOffset(offset);
            return Grid.OffsetCell(bottomLeftCell, rotatedOffset);
        }
        
    }
}
