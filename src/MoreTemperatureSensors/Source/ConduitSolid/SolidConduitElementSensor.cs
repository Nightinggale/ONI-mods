using KSerialization;
using System;

namespace MoreTemperatureSensors
{

    [SerializationConfig(MemberSerialization.OptIn)]
    public class SolidConduitElementSensor : SolidConduitSensor
    {
        [MyCmpGet]
        private Filterable filterable;

        private SimHashes desiredElement = SimHashes.Void;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.filterable.onFilterChanged += new Action<Tag>(this.OnFilterChanged);
            this.OnFilterChanged(this.filterable.SelectedTag);

            // Update currentValue to avoid all displays from showing default value on load.
            // No functional change. It's purely a display issue.
            this.ConduitUpdate(-10);
        }

        private void OnFilterChanged(Tag tag)
        {
            this.desiredElement = SimHashes.Void;
            if (!tag.IsValid)
            {
                return;
            }
            Element element = ElementLoader.GetElement(tag);
            if (element == null)
            {
                return;
            }
            this.desiredElement = element.id;
        }

        protected override void ConduitUpdate(float dt)
        {
            SimHashes currentElement = SimHashes.Vacuum;

            int cell = Grid.PosToCell(this);
            SolidConduitFlow.ConduitContents contents = Game.Instance.solidConduitFlow.GetContents(cell);
            if (contents.pickupableHandle.IsValid())
            {
                Pickupable pickupable = Game.Instance.solidConduitFlow.GetPickupable(contents.pickupableHandle);
                PrimaryElement primaryElement = pickupable.GetComponent<PrimaryElement>();

                if (primaryElement != null)
                {
                    currentElement = primaryElement.ElementID;
                }
            }

            // spawn code should never toggle as it crashes on load
            if (dt < 0)
            {
                return;
            }

            if (base.IsSwitchedOn)
            {
                if (currentElement != this.desiredElement)
                {
                    this.Toggle();
                }
            }
            else if (currentElement == this.desiredElement)
            {
                this.Toggle();
            }
        }
    }
}
