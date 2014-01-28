using System.IO;

namespace StarboundModCreator.Core
{
    public static class IOHelper
    {
        /// <summary>
        /// Creates all directories and subdirectories in the specified path.
        /// Delete the directory if already existing.
        /// </summary>
        /// <param name="path"></param>
        public static void DirectoryCreateNew(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// Delete a file if the specified file exists
        /// </summary>
        /// <param name="path"></param>
        public static void FileDeleteIfExists(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
