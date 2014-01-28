using Newtonsoft.Json;
using System;
using System.IO;

namespace StarboundModCreator.Core
{
    /// <summary>
    /// Application's configuration
    /// </summary>
    public class AppConfig
    {
        private const string FileName = "app.config";

        /// <summary>
        /// Starbound installation folder path
        /// </summary>
        public string StarboundInstallationFolderPath { get; set; }

        /// <summary>
        /// Saves the configuration of the application in the app.config file
        /// </summary>
        public void Save()
        {
            using (StreamWriter writer = new StreamWriter(FileName))
            {
                writer.Write(JsonConvert.SerializeObject(this));
            }
        }

        /// <summary>
        /// Reads the app.config fil if exists; otherwise call the defaultCreation Func
        /// </summary>
        /// <param name="defaultCreation"></param>
        /// <returns></returns>
        public static AppConfig Read(Func<AppConfig> defaultCreation)
        {
            if(File.Exists(FileName))
            {
                using (StreamReader reader = new StreamReader(FileName))
                {
                    return JsonConvert.DeserializeObject<AppConfig>(reader.ReadToEnd());
                }
            }
            else
            {
                return defaultCreation();
            }
        }
    }
}
