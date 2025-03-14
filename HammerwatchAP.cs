using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HarmonyLib;
using System.Reflection;
using HammerwatchAP.Hooks;
using HammerwatchAP.Util;

namespace HammerwatchAP
{
    public static class HammerwatchAP
    {
        public static Assembly hammerwatchAssembly = typeof(ARPGGame.GameBase).Assembly;

        public static void Initialize()
        {
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string librariesPath = Path.Combine(baseDirectory, "libraries");

            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                string assemblyName = new AssemblyName(args.Name).Name + ".dll";
                string assemblyPath = Path.Combine(librariesPath, assemblyName);
                if (File.Exists(assemblyPath))
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                return null; // Let the default resolver handle it
            };
            Logging.Log("Loaded mod assemblies");

            HooksMain.ApplyHooks();
            Logging.Log("Applied mod hooks");
        }
    }
}
