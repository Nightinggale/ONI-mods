using UnityEngine;

namespace NightLib
{
    internal class PortDisplayGas    : PortDisplay { public void AssignPort(string ID, PortDisplayGasBase    port) { base.AssignPort(ID, port); } }
    internal class PortDisplayLiquid : PortDisplay { public void AssignPort(string ID, PortDisplayLiquidBase port) { base.AssignPort(ID, port); } }
    internal class PortDisplaySolid  : PortDisplay { public void AssignPort(string ID, PortDisplaySolidBase  port) { base.AssignPort(ID, port); } }

    internal abstract class PortDisplay : KMonoBehaviour
    {
        private GameObject portObject;
        internal int utilityCell = -1;

        [SerializeField]
        internal ConduitType type;

        [SerializeField]
        internal CellOffset offset;

        [SerializeField]
        internal bool input;

        [SerializeField]
        internal Color32 color;

        protected void AssignPort(string ID, DisplayConduitPortInfo port)
        {
            this.type = port.type;
            this.offset = port.offset;
            this.input = port.input;
            this.color = port.color;

            // Add the building/overlay combo to the drawing code cache
            // For performance reasons only building/overlay combos in the cache will attempt to draw modded ports
            // Call added here as this makes the cache self configuring without extra code for each building
            NightLib.PortDisplayDrawing.ConduitDisplayPortPatches.DrawPorts.AddBuilding(ID, type);
        }

        internal void Draw(GameObject obj, BuildingCellVisualizer visualizer)
        {
            
            if (utilityCell == -1)
            {
                utilityCell = visualizer.GetBuilding().GetCellWithOffset(this.offset);
            }
            visualizer.DrawUtilityIcon(utilityCell, GetSprite(visualizer), ref portObject, color, Color.red);
        }

        private Sprite GetSprite(BuildingCellVisualizer visualizer)
        {
            if (input)
            {
                if (this.type == ConduitType.Gas)
                {
                    return visualizer.GetResources().gasInputIcon;
                }
                else if (this.type == ConduitType.Liquid || this.type == ConduitType.Solid)
                {
                    return visualizer.GetResources().liquidInputIcon;
                }
            }
            else
            {
                if (this.type == ConduitType.Gas)
                {
                    return visualizer.GetResources().gasOutputIcon;
                }
                else if (this.type == ConduitType.Liquid || this.type == ConduitType.Solid)
                {
                    return visualizer.GetResources().liquidOutputIcon;
                }
            }

            return null;
        }

        internal void DisableIcons()
        {
            if (this.portObject != null)
            {
                if (this.portObject != null && this.portObject.activeInHierarchy)
                {
                    this.portObject.SetActive(false);
                }
            }
        }

        protected override void OnCleanUp()
        {
            base.OnCleanUp();
            if (this.portObject != null)
            {
                UnityEngine.Object.Destroy(this.portObject);
            }
        }
    }
}
