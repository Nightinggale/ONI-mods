using UnityEngine;
using NightLib.SavegameUnsafe;

// don't modify or use. It's added purely for savegame compatibility reasons.

// it's never actually called and never used. However it ended up in old savegames by mistake
// Just load the data if needed and ignore it.

namespace NightLib
{
    internal class PortConduitDispenser : PortConduitDispenserBase { }

    internal class PortDisplay : KMonoBehaviour
    {
        private GameObject portObject;

        // The cache for last location/color.
        // The default values doesn't matter and will be overwritten on first call.
        // However there is a theoredical risk that no default value can cause a crash, hence setting them to something.
        [SerializeField]
        private int lastUtilityCell = -1;

        [SerializeField]
        private Color lastColor = Color.black;

        [SerializeField]
        internal ConduitType type;

        [SerializeField]
        internal CellOffset offset;

        [SerializeField]
        internal CellOffset offsetFlipped;

        [SerializeField]
        internal bool input;

        [SerializeField]
        internal Color32 colorConnected;

        [SerializeField]
        internal Color32 colorDisconnected;

        [SerializeField]
        internal Sprite sprite;
    }
}
