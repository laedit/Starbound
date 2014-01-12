using Microsoft.Win32;
using System;

namespace Research
{
    class Registry
    {
        // Get starbound installation path
        public static void Test()
        {
            RegistryKey registryKey;
            if (Environment.Is64BitOperatingSystem)
            {
                registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Wow6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 211820");
            }
            else
            {
                registryKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Uninstall\\Steam App 211820");
            }
            if (registryKey == null)
            {
                Console.WriteLine("Starbound is not installed");
            }
            Console.WriteLine((string)registryKey.GetValue("InstallLocation"));
        }
    }
}
