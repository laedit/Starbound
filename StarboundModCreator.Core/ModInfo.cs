using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace StarboundModCreator.Core
{
    /// <summary>
    /// ModInfo file
    /// </summary>
    public class ModInfo : IModFile
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Path { get; set; }
        public List<string> Dependencies { get; set; }
        public ModInfoMetaData MetaData { get; set; }

        public string RelativeFilePath
        {
            get { return string.Format("{0}.modinfo", this.Name); }
            set { throw new InvalidOperationException(); }
        }

        public void Write(StreamWriter writer)
        {
            writer.Write(JsonConvert.SerializeObject(this));
        }
    }
}
