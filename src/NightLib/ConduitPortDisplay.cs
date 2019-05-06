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

    internal class DisplayConduitPortInfo
    {
        readonly internal ConduitType conduitType;

        readonly internal CellOffset offset;

        readonly internal bool input;

        readonly internal Color color;

        internal DisplayConduitPortInfo(ConduitType type, CellOffset offset, bool input = false, Color? color = null)
        {
            this.conduitType = type;
            this.offset = offset;
            this.input = input;

            this.color = color ?? (input ? Color.white : Color.green);
        }
    }


    internal abstract class PortDisplay : KMonoBehaviour
    {
        private GameObject portObject;
        private int utilityCell = -1;

        [SerializeField]
        internal ConduitPortInfo portInfo;

        internal bool input = false;

        internal ConduitType GetSecondaryConduitType()
        {
            return this.portInfo.conduitType;
        }

        internal CellOffset GetSecondaryConduitOffset()
        {
            return this.portInfo.offset;
        }

        internal Color32 portColor = new Color(0.4f, 0.4f, 0.4f);

        internal void Draw(GameObject obj, BuildingCellVisualizer visualizer)
        {
            
            if (utilityCell == -1)
            {
                utilityCell = visualizer.GetBuilding().GetCellWithOffset(GetSecondaryConduitOffset());
            }
            visualizer.DrawUtilityIcon(utilityCell, GetSprite(visualizer), ref portObject, portColor, Color.white);
        }

        private Sprite GetSprite(BuildingCellVisualizer visualizer)
        {
            ConduitType type = GetSecondaryConduitType();
            if (input)
            {
                if (type == ConduitType.Gas)
                {
                    return visualizer.GetResources().gasInputIcon;
                }
                else if (type == ConduitType.Liquid || type == ConduitType.Solid)
                {
                    return visualizer.GetResources().liquidInputIcon;
                }
            }
            else
            {
                if (type == ConduitType.Gas)
                {
                    return visualizer.GetResources().gasOutputIcon;
                }
                else if (type == ConduitType.Liquid || type == ConduitType.Solid)
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
