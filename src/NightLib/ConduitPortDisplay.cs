using UnityEngine;

namespace NightLib
{
    internal class PortDisplay0 : PortDisplay { }
    internal class PortDisplay1 : PortDisplay { }
    internal class PortDisplay2 : PortDisplay { }
    internal class PortDisplay3 : PortDisplay { }
    internal class PortDisplay4 : PortDisplay { }
    internal class PortDisplay5 : PortDisplay { }
    internal class PortDisplay6 : PortDisplay { }
    internal class PortDisplay7 : PortDisplay { }
    internal class PortDisplay8 : PortDisplay { }

    // can't be stored in components. It somehow gets reset before it's used
    // Serialization doesn't seem to help at all
    internal class DisplayConduitPortInfo
    {
        readonly internal ConduitType type;
        readonly internal CellOffset offset;
        readonly internal bool input;
        readonly internal Color color;

        internal DisplayConduitPortInfo(ConduitType type, CellOffset offset, bool input = false, Color? color = null)
        {
            this.type = type;
            this.offset = offset;
            this.input = input;

            this.color = color ?? (input ? new Color(0.4f, 0.4f, 0.4f) : new Color(0.1f, 0.5f, 0.2f));
        }
    }


    internal abstract class PortDisplay : KMonoBehaviour
    {
        private GameObject portObject;
        private int utilityCell = -1;

        [SerializeField]
        internal ConduitType type;

        [SerializeField]
        internal CellOffset offset;

        [SerializeField]
        internal bool input;

        [SerializeField]
        internal Color32 color;

        internal void AssignPort(DisplayConduitPortInfo port)
        {
            this.type = port.type;
            this.offset = port.offset;
            this.input = port.input;
            this.color = port.color;
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
