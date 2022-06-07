using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using ICities;
using ColossalFramework;
using ColossalFramework.Plugins;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Class that manages interactions with other mods, including compatibility and functionality checks.
    /// </summary>
    internal static class ModUtils
    {
        // Specific reference to TM:PE assembly.
        private static Assembly tmpe;


        /// <summary>
        /// The TM:PE assembly reference.
        /// </summary>
        internal static Assembly TMPE
        {
            get
            {
                if (tmpe == null)
                {
                    tmpe = GetEnabledAssembly("TrafficManager");
                }

                return tmpe;
            }
        }


        /// <summary>
        /// Returns the filepath of the current mod assembly.
        /// </summary>
        /// <returns>Mod assembly filepath</returns>
        internal static string GetAssemblyPath()
        {
            // Get list of currently active plugins.
            IEnumerable<PluginManager.PluginInfo> plugins = PluginManager.instance.GetPluginsInfo();

            // Iterate through list.
            foreach (PluginManager.PluginInfo plugin in plugins)
            {
                try
                {
                    // Get all (if any) mod instances from this plugin.
                    IUserMod[] mods = plugin.GetInstances<IUserMod>();

                    // Check to see if the primary instance is this mod.
                    if (mods.FirstOrDefault() is MCUMod)
                    {
                        // Found it! Return path.
                        return plugin.modPath;
                    }
                }
                catch
                {
                    // Don't care.
                }
            }

            // If we got here, then we didn't find the assembly.
            Logging.Error("assembly path not found");
            throw new FileNotFoundException(MCUMod.ModName + ": assembly path not found!");
        }


        /// <summary>
        /// Checks to see if another mod is installed and enabled, based on a provided assembly name, and if so, returns the assembly reference.
        /// Case-sensitive!  PloppableRICO is not the same as ploppablerico!
        /// </summary>
        /// <param name="assemblyName">Name of the mod assembly</param>
        /// <returns>Assembly reference if target is found and enabled, null otherwise</returns>
        internal static Assembly GetEnabledAssembly(string assemblyName)
        {
            // Iterate through the full list of plugins.
            foreach (PluginManager.PluginInfo plugin in PluginManager.instance.GetPluginsInfo())
            {
                // Only looking at enabled plugins.
                if (plugin.isEnabled)
                {
                    foreach (Assembly assembly in plugin.GetAssemblies())
                    {
                        if (assembly.GetName().Name.Equals(assemblyName))
                        {
                            Logging.Message("found enabled mod assembly ", assemblyName, ", version ", assembly.GetName().Version);
                            return assembly;
                        }
                    }
                }
            }

            // If we've made it here, then we haven't found a matching assembly.
            Logging.Message("didn't find enabled assembly ", assemblyName);
            return null;
        }


        /// <summary>
        /// Uses reflection to forcibly update TM:PE CitizenUnitExtensions._citizenUnitBuffer field to correct value (current game CitizenUnit buffer).
        /// </summary>
        internal static void UpdateTMPEUnitsRef()
        {
            if (TMPE != null)
            {
                Logging.Message("reflecting TM:PE CitizenUnit buffer");
                string targetName = "TrafficManager.Util.Extensions.CitizenUnitExtensions";
                Type citizenUnitExtensions = tmpe.GetType(targetName);

                if (citizenUnitExtensions != null)
                {
                    targetName = "_citizenUnitBuffer";
                    FieldInfo citizenUnitBuffer = citizenUnitExtensions.GetField(targetName, BindingFlags.Static | BindingFlags.NonPublic);
                    if (citizenUnitBuffer != null)
                    {
                        Logging.Message("forcibly updating TM:PE CitizenUnit buffer reference");
                        citizenUnitBuffer.SetValue(null, Singleton<CitizenManager>.instance.m_units.m_buffer);
                        return;
                    }

                }

                // If we got here, reflection failed somewhere.
                Logging.Error("couldn't reflect ", targetName);
            }
        }
    }
}
