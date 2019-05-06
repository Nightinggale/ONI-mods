using System;
using System.Reflection;
using System.Linq;


namespace NightLib
{
    internal static class ReadPrivate
    {
        // copied from https://stackoverflow.com/questions/3303126/how-to-get-the-value-of-private-field-in-c/3303182

        /// <summary>
        /// Uses reflection to get the field value from an object.
        /// </summary>
        ///
        /// <param name="type">The instance type.</param>
        /// <param name="instance">The instance object.</param>
        /// <param name="fieldName">The field's name which is to be fetched.</param>
        ///
        /// <returns>The field value from the object.</returns>
        internal static object Get(Type type, object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            FieldInfo field = type.GetField(fieldName, bindFlags);
            return field.GetValue(instance);
        }

        /// usage:
        /// Make an extension function to act as a get function for the data in question.
        /// Example
        ///         public static Building GetBuilding(this BuildingCellVisualizer __instance)
        ///         {
        ///             return NightLib.ReadPrivate.GetInstanceField(typeof(BuildingCellVisualizer), __instance, "building") as Building;
        ///         }
        /// This will allow an instance of BuildingCellVisualizer to use .GetBuilding() to get the contents of the private member variable building
        /// 



        // copied from https://stackoverflow.com/questions/135443/how-do-i-use-reflection-to-invoke-a-private-method

        

        internal static object Call(object o, string methodName, params object[] args)
        {
            var mi = o.GetType().GetMethod(methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (mi != null)
            {
                return mi.Invoke(o, args);
            }
            return null;
        }
    }
}
