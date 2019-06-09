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
        private static HashSet<IOverlayChangeEvent> brokenList = new HashSet<IOverlayChangeEvent>();
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
                    bool foundError = false;

                    lastMode = mode;
                    foreach (IOverlayChangeEvent obj in list)
                    {
                        try
                        {
                            obj.OnOverlayChange(mode);
                        }
                        catch
                        {
                            // loading a game while another game is running might leave garbage.
                            // the catch condition should only trigger the first time an overlay is changed after load.
                            brokenList.Add(obj);
                            foundError = true;
                        }
                    }
                    if (foundError)
                    {
                        // Remove all broken buildings.
                        // Since they were in the last game, they will never recover.
                        // Removing them will prevent a slowdown from keeping all the garbage.
                        foreach (IOverlayChangeEvent obj in brokenList)
                        {
                            list.Remove(obj);
                        }
                        brokenList.Clear();
                    }
                }
            }
        }

    }
}
