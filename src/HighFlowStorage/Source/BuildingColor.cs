using UnityEngine;

namespace HighFlowStorage
{
    [SkipSaveFileSerialization]
    internal class BuildingColor : KMonoBehaviour
    {
        [SerializeField]
        public Color32 color = new Color32(0,0,0,0);

        protected override void OnSpawn()
        {
            base.OnSpawn();

            if (color.a != 0)
            {
                KBatchedAnimController component = base.GetComponent<KBatchedAnimController>();
                if (component != null)
                {
                    component.TintColour = this.color;
                }
            }
        }
    }
}
