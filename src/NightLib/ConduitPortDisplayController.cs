using UnityEngine;
using System.Collections.Generic;
using TUNING;
using System;

namespace NightLib
{
    [SkipSaveFileSerialization]
    internal class PortDisplayController : KMonoBehaviour
    {
        [SerializeField]
        private HashedString lastMode = OverlayModes.None.ID;

        [SerializeField]
        private List<PortDisplay> gasNewOverlay = new List<PortDisplay>();

        [SerializeField]
        private List<PortDisplay> gasUpdate = new List<PortDisplay>();

        [SerializeField]
        private List<PortDisplay> liquidNewOverlay = new List<PortDisplay>();

        [SerializeField]
        private List<PortDisplay> liquidUpdate = new List<PortDisplay>();

        [SerializeField]
        private List<PortDisplay> solidNewOverlay = new List<PortDisplay>();

        [SerializeField]
        private List<PortDisplay> solidUpdate = new List<PortDisplay>();

        [SerializeField]
        private bool hasPower = false;

        [SerializeField]
        private bool hasGas = false;

        [SerializeField]
        private bool hasLiquid = false;

        [SerializeField]
        private bool hasSolid = false;

        [SerializeField]
        private bool hasDisease = false;

        public void AssignPort(GameObject go, DisplayConduitPortInfo port)
        {
            PortDisplay portDisplay = go.AddComponent<PortDisplay>();
            portDisplay.AssignPort(port);

            // The design strategy here is to cache ports for when they should be drawn.
            // When a frame is drawn, rather than checking what should be drawn, pick the list matching the overlay in question.
            // 2 lists exist for each frame: when changing to the overlay and a screen update on the same overlay.
            // If a port doesn't change graphics, it will stay until told to go away. There is no need to refresh to the same graphics and position.
            // If the port can change color and/or position (moving preview), drawing on each frame can't be avoided.

            switch (port.type)
            {
                case ConduitType.Gas:
                    {
                        this.gasNewOverlay.Add(portDisplay);
                        if (port.colorConnected != port.colorDisconnected || go.IsPreview())
                        {
                            this.gasUpdate.Add(portDisplay);
                        }
                    }
                    break;
                case ConduitType.Liquid:
                    {
                        this.liquidNewOverlay.Add(portDisplay);
                        if (port.colorConnected != port.colorDisconnected || go.IsPreview())
                        {
                            this.liquidUpdate.Add(portDisplay);
                        }
                    }
                    break;
                case ConduitType.Solid:
                    {
                        this.solidNewOverlay.Add(portDisplay);
                        if (port.colorConnected != port.colorDisconnected || go.IsPreview())
                        {
                            this.solidUpdate.Add(portDisplay);
                        }
                    }
                    break;
            }
        }

        public void Init(GameObject go)
        {
            string ID = go.GetComponent<KPrefabID>().PrefabTag.Name;

            NightLib.PortDisplayDrawing.ConduitDisplayPortPatches.DrawPorts.AddBuilding(ID);

            // cache which overlays the vanilla code can use. Used to skip vanilla code when nothing is drawn anyway.
            BuildingDef def = go.GetComponent<BuildingDef>();
            if (def != null)
            {
                this.hasPower   = BuildingCellVisualizer.CheckRequiresPowerInput (def) || BuildingCellVisualizer.CheckRequiresPowerOutput (def);
                this.hasGas     = BuildingCellVisualizer.CheckRequiresGasInput   (def) || BuildingCellVisualizer.CheckRequiresGasOutput   (def);
                this.hasLiquid  = BuildingCellVisualizer.CheckRequiresLiquidInput(def) || BuildingCellVisualizer.CheckRequiresLiquidOutput(def);
                this.hasSolid   = BuildingCellVisualizer.CheckRequiresSolidInput (def) || BuildingCellVisualizer.CheckRequiresSolidOutput (def);
                this.hasDisease = def.DiseaseCellVisName != null;
            }
        }

        public bool Draw(BuildingCellVisualizer __instance, HashedString mode, GameObject go)
        {
            bool isNewMode = mode != this.lastMode;

            if (isNewMode)
            {
                this.ClearPorts();
                this.lastMode = mode;
            }

            foreach (PortDisplay port in this.GetPorts(mode, isNewMode))
            {
                port.Draw(go, __instance, isNewMode);
            }

            // return true if the vanilla port drawing code has anything to do
            return isNewMode
                || (this.hasPower   && mode == OverlayModes.Power.ID)
                || (this.hasGas     && mode == OverlayModes.GasConduits.ID)
                || (this.hasLiquid  && mode == OverlayModes.LiquidConduits.ID)
                || (this.hasSolid   && mode == OverlayModes.SolidConveyor.ID)
                || (this.hasDisease && mode == OverlayModes.Disease.ID)
            ;
        }

        private void ClearPorts()
        {
            foreach (PortDisplay port in this.GetPorts(this.lastMode, true))
            {
                port.DisableIcons();
            }
        }

        private List<PortDisplay> GetPorts(HashedString mode, bool newOverlay)
        {
            if (newOverlay)
            {
                if (mode == OverlayModes.GasConduits   .ID) return this.gasNewOverlay;
                if (mode == OverlayModes.LiquidConduits.ID) return this.liquidNewOverlay;
                if (mode == OverlayModes.SolidConveyor .ID) return this.solidNewOverlay;
            }
            else
            {
                if (mode == OverlayModes.GasConduits   .ID) return this.gasUpdate;
                if (mode == OverlayModes.LiquidConduits.ID) return this.liquidUpdate;
                if (mode == OverlayModes.SolidConveyor .ID) return this.solidUpdate;
            }
            return new List<PortDisplay>();
        }
    }
}
