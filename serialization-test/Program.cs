using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using revit_to_vr_common;
using System.Text.Json;

namespace serialization_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TestData a = new TestData();
            a.anotherValue = "Lalala";
            string serialized = JsonSerializer.Serialize(a, Configuration.jsonSerializerOptions);
            Console.WriteLine(serialized);
            Console.ReadKey(true);
      
        }
    }
}
