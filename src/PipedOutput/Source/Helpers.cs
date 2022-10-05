using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Nightinggale.PipedOutput
{
    public static class Helpers
    {
        [System.Diagnostics.Conditional("DEBUG")]
        public static void PrintDebug(string text)
        {
            Console.Write(System.DateTime.UtcNow.ToString("[HH:mm:ss.fff]"));
            Console.Write(" [1] [DEBUG] [PipedOutput] ");
            Console.WriteLine(text);
        }

        public static void ManualDeliveryForce(this GameObject go)
        {
            foreach (var delivery in go.GetComponents<ManualDeliveryKG>())
                delivery.operationalRequirement = Operational.State.None;
        }
    }
}
