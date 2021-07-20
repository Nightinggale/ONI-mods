using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nightinggale.PipedOutput
{
    public class Helpers
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void PrintDebug(string text)
        {
            Console.Write(System.DateTime.UtcNow.ToString("[HH:mm:ss.fff]"));
            Console.Write(" [1] [DEBUG] [PipedOutput] ");
            Console.WriteLine(text);
        }
    }
}
