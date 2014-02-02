using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Bson;

namespace Research
{
    public class PakFile
    {
        public static void TestRead()
        {


            using (BinaryReader reader = new BinaryReader(File.OpenRead("FuriousKoalaSamples/jte_farming-gt.pak")))
            {
                using (BsonReader breader = new BsonReader(reader))
                {
                    var test = breader.ReadAsString();
                }
            }
        }
    }
}
