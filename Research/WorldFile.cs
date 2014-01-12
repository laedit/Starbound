using System.IO;
using System.Text;

namespace Research
{
    class WorldFile
    {
        public static void Test()
        {
            // .world
            // On laisse de côté pour le moment
            using (BinaryReader reader = new BinaryReader(File.OpenRead("AngryKoalaSamples/gamma_39833742_18631889_-23963189_10_9.world")))
            {
                //StringBuilder sb = new StringBuilder();
                //string line = null;
                var test = reader.ReadUInt32();
            }
        }
    }
}
