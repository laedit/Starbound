using System;
using System.Collections.Generic;

namespace StarboundModCreator.Core
{
    public class ModPart
    {
        public string Name { get; set; }

        public Dictionary<string, IModFile> Files { get; set; }

        // TODO write / read
    }
}
