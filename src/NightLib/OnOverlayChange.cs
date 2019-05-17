using Harmony;
using System.Collections.Generic;

namespace NightLib.OnOverlayChange
{
    internal interface IOverlayChangeEvent
    {
        void OnOverlayChange(HashedString mode);
    }


    [HarmonyPatch(typeof(Game))]
    [HarmonyPatch("LateUpdateComponents")]
    internal static class OverlayChangeController
    {
        private static HashSet<IOverlayChangeEvent> list = new HashSet<IOverlayChangeEvent>();
        private static HashedString lastMode;

        internal static void Add(IOverlayChangeEvent obj)
        {
            list.Add(obj);
        }

        internal static void Remove(IOverlayChangeEvent obj)
        {
            list.Remove(obj);
        }

        public static void Postfix()
        {
            if (OverlayScreen.Instance != null)
            {
                HashedString mode = OverlayScreen.Instance.GetMode();
                if (mode != lastMode)
                {
                    lastMode = mode;
                    foreach (IOverlayChangeEvent obj in list)
                    {
                        obj.OnOverlayChange(mode);
                    }
                }
            }
        }

    }
}
