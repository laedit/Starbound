using System;
using System.IO;

namespace StarboundModCreator.Core
{
    /// <summary>
    /// Represents a file included in the mod
    /// </summary>
    public interface IModFile
    {
        string RelativeFilePath { get; set; }

        void Write(StreamWriter writer);
    }
}
