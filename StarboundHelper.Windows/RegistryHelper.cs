using Microsoft.Win32;
using System;

namespace StarboundHelper.Windows
{
    public static class RegistryHelper
    {
        /// <summary>
        /// Get starbound installation folder path
        /// </summary>
        /// <returns>Starbound installation folder path</returns>
        public static string GetStarboundInstallationPath()
        {
            RegistryKey registryKey;
            if (Environment.Is64BitOperatingSystem)
            {
                registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 211820");
            }
            else
            {
                registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 211820");
            }
            if (registryKey == null)
            {
                return null;
            }
            return (string)registryKey.GetValue("InstallLocation");
        }
    }
}
