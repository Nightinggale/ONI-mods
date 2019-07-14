

namespace Nightinggale.CoalGenerator
{
    public static class AddStrings
    {
        public static void AddString(string str)
        {
            Strings.Add($"STRINGS.BUILDINGS.NIGHTINGGALE.COALGENERATOR.TITLE." + str, str + " delivery controls");
            Strings.Add($"STRINGS.BUILDINGS.NIGHTINGGALE.COALGENERATOR.BATTERYTHRESHOLD." + str, "Duplicants will deliver " + str + " when the battery charge falls below the selected threshold.\nNote that this slider is ignored if the coal generator is connected to an automation wire.");
        }
    }
}
