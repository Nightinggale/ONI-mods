using UnityEngine;

namespace BlackHoleGarbageDisposal
{
    [SkipSaveFileSerialization]
    public class BlackHoleGarbageDisposalComponent : KMonoBehaviour, ISim4000ms
    {
        [MyCmpGet]
        internal Storage storage;

        [MyCmpGet]
        internal KBatchedAnimController controller;

        protected override void OnSpawn()
        {
            base.OnSpawn();
            this.UpdateColor();
        }

        public void Sim4000ms(float dt)
        {
            this.UpdateColor();
            foreach (GameObject current in storage.items)
            {
                current.DeleteObject();
            }
        }

        private void UpdateColor()
        {
            controller.TintColour = new Color32(10, 10, 10, 255);
        }
    }
}
