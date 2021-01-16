using System.Collections.Generic;
using TUNING;
using System;

/*
 * Tools to add a new building
 * 
 * AddBuildingToPlanScreen
 * adds the building to the build menu
 * Arguments:
 *   category: the category to add the building to
 *   buildingId: the PrefabID of the building, usually just called Id or ID in the config class
 *   (optional) parentId: the buildingID will be added after parentId
 *   (optional) index: the index in the menu to add the building
 *   Note: max one optional argument is possible and missing or invalid optional arguments will place the building last in the list
 *   
 *  IntoTechTree
 *  Adds a tech requirement to the building
 *  Arguments:
 *   Tech:       The name of the tech the building requires
 *   BuildingID: the PrefabID of the building, usually just called Id or ID in the config class
 *  Note: not calling this class will make the building available from the start
 *  
 * AddStrings
 * Adds Name,  Description and Effect strings
 * Usage is self explainatory. Allows easy and clean access to correctly formatting strings
 */

namespace NightLib.AddBuilding
{
    internal static class AddBuilding
    {
        internal static void AddBuildingToPlanScreen(HashedString category, string buildingId, string parentId)
        {
            int index = GetCategoryIndex(category, buildingId);

            if (index == -1)
                return;

            int? indexBuilding = null;
            if (!parentId.IsNullOrWhiteSpace())
            {
                indexBuilding = (BUILDINGS.PLANORDER[index].data as IList<string>)?.IndexOf(parentId);
                if (indexBuilding != null)
                {
                    ++indexBuilding;
                }
            }

            if (indexBuilding == null)
            {
                Console.WriteLine("ERROR: building \"" + parentId + "\" not found in category " + category + ". Placing " + buildingId + " at the end of the list");
            }

            AddBuildingToPlanScreen(category, buildingId, indexBuilding);
        }

        internal static void AddBuildingToPlanScreen(HashedString category, string buildingId, int? index = null)
        {
            int CategoryIndex = GetCategoryIndex(category, buildingId);

            if (CategoryIndex == -1)
                return;
            if (index != null)
            {
                if (index >= 0 && index < (BUILDINGS.PLANORDER[CategoryIndex].data as IList<string>)?.Count)
                {
                    (BUILDINGS.PLANORDER[CategoryIndex].data as IList<string>)?.Insert(index.Value, buildingId);
                    return;
                }
            }
            
            (BUILDINGS.PLANORDER[CategoryIndex].data as IList<string>)?.Add(buildingId);
        }

        internal static void ReplaceBuildingInPlanScreen(HashedString category, string buildingId, string parentId)
        {
            int index = GetCategoryIndex(category, buildingId);

            if (index == -1)
                return;

            int? indexBuilding = null;
            indexBuilding = (BUILDINGS.PLANORDER[index].data as IList<string>)?.IndexOf(parentId);
            if (indexBuilding != null)
            {
                (BUILDINGS.PLANORDER[index].data as IList<string>)?.Remove(parentId);
                (BUILDINGS.PLANORDER[index].data as IList<string>)?.Insert(indexBuilding.Value, buildingId);
                return;
            }


            if (indexBuilding == null)
            {
                Console.WriteLine("ERROR: building \"" + parentId + "\" not found in category " + category + ". Placing " + buildingId + " at the end of the list");
            }

            AddBuildingToPlanScreen(category, buildingId, indexBuilding);
        }

        private static int GetCategoryIndex(HashedString category, string buildingId)
        {
            int index = BUILDINGS.PLANORDER.FindIndex(x => x.category == category);

            if (index == -1)
            {
                Console.WriteLine("ERROR: can't add building " + buildingId + " to non-existing category " + category);
            }

            return index;
        }


        // --------------------------------------

        internal static void IntoTechTree(string Tech, string BuildingID)
        {
#if DLC1
            int a = Db.Get().Techs.Count;
            if (Db.Get().Techs.Get(Tech) != null)
            {
                Db.Get().Techs.Get(Tech).unlockedItemIDs.Add(BuildingID);
            }
#else
            var TechGroup = new List<string>(Database.Techs.TECH_GROUPING[Tech]) { };
            TechGroup.Insert(1, BuildingID);
            Database.Techs.TECH_GROUPING[Tech] = TechGroup.ToArray();
#endif
            // TODO figure out how to control the order within a group
        }

        internal static void ReplaceInTechTree(string Tech, string BuildingID, string old)
        {
#if DLC1
            if (Db.Get().Techs.TryGet(Tech) != null)
            {
                int iIndex = Db.Get().Techs.Get(Tech).unlockedItemIDs.FindIndex( x => x == old);
                if (iIndex >= 0)
                {
                    Db.Get().Techs.Get(Tech).unlockedItemIDs[iIndex] = BuildingID;
                }
            }
#else
            var TechGroup = new List<string>(Database.Techs.TECH_GROUPING[Tech]) { };
            int index = TechGroup.FindIndex(x => x == old);
            if (index != -1)
            {
                TechGroup[index] = BuildingID;
                Database.Techs.TECH_GROUPING[Tech] = TechGroup.ToArray();
            }
            else
            {
                IntoTechTree(Tech, BuildingID);
            }
#endif
        }


        private static int GetTechCategoryIndex(HashedString category, string buildingId)
        {
            int index = BUILDINGS.PLANORDER.FindIndex(x => x.category == category);

            if (index == -1)
            {
                Console.WriteLine("ERROR: can't add building " + buildingId + " to non-existing category " + category);
            }

            return index;
        }

        internal static void AddStrings(string ID, string Name, string Description, string Effect)
        {
            // UI.FormatAsLink(Name, ID); would be the clean implementation of a link, but it has a nameclash with TURING

            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID.ToUpperInvariant()}.NAME", "<link=\"" + ID + "\">" + Name + "</link>");
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID.ToUpperInvariant()}.DESC", Description);
            Strings.Add($"STRINGS.BUILDINGS.PREFABS.{ID.ToUpperInvariant()}.EFFECT", Effect);
        }
    }
}
