/// <summary>
/// Extention methods
/// 
/// This file contains extension methods to various classes in ONI.
/// 
/// </summary>


using UnityEngine;
using KSerialization;
using System;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
         | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}

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

        // Get the index of the layer with the connectors (ports) for the conduit type in question 
        internal static bool IsConnected(this ConduitType conduitType, int cell)
        {
            return Grid.Objects[cell, conduitType.GetConduitObjectLayer()] != null;
        }

        // Get a cell of a building. Takes rotation into account
        internal static int GetCellWithOffset(this Building building, CellOffset offset)
        {
            Vector3 position = building.transform.GetPosition();
            int bottomLeftCell = Grid.PosToCell(position);

            CellOffset rotatedOffset = building.GetRotatedOffset(offset);
            return Grid.OffsetCell(bottomLeftCell, rotatedOffset);
        }
        
        internal static bool IsPreview(this GameObject go)
        {
            string name = go.PrefabID().Name;
            return name.Substring(name.Length - 7) == "Preview";
        }

        
        internal static void Subscribe(this KMonoBehaviour behavior, GameHashes hash, Action<object> handler)
        {
            behavior.Subscribe((int)hash, handler);
        }
        
        internal static void Trigger(this KMonoBehaviour behavior, int hash, object data = null)
        {
            behavior.Trigger((int)hash, data);
        }

        // wrapper to avoid typecasting hash
        internal static int Subscribe<ComponentType>(this KMonoBehaviour behaviour, GameHashes hash, EventSystem.IntraObjectHandler<ComponentType> handler) where ComponentType : Component
        {
            return behaviour.Subscribe((int)hash, handler);
        }

        internal static int Subscribe<ComponentType>(this KMonoBehaviour behaviour, GameHashes hash, Action<object> handler) where ComponentType : Component
        {
            return behaviour.Subscribe((int)hash, handler);
        }

        internal static int Subscribe<ComponentType>(this KMonoBehaviour behaviour, GameObject target, GameHashes hash, Action<object> handler) where ComponentType : Component
        {
            return behaviour.Subscribe(target, (int)hash, handler);
        }
    }
}
