using System.Text;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using CitiesHarmony.API;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Class to manage the mod's Harmony patches.
    /// </summary>
    public static class Patcher
    {
        // Unique harmony identifier.
        private const string harmonyID = "com.github.algernon-A.csl.mcu";

        // Flag.
        internal static bool Patched => _patched;
        private static bool _patched = false;


        /// <summary>
        /// Apply all Harmony patches.
        /// </summary>
        public static void PatchAll()
        {
            // Don't do anything if already patched.
            if (!_patched)
            {
                // Ensure Harmony is ready before patching.
                if (HarmonyHelper.IsHarmonyInstalled)
                {
                    Logging.KeyMessage("deploying Harmony patches");

                    // Apply all annotated patches and update flag.
                    Harmony harmonyInstance = new Harmony(harmonyID);
                    harmonyInstance.PatchAll();
                    _patched = true;
                }
                else
                {
                    Logging.Error("Harmony not ready");
                }
            }
        }


        /// <summary>
        /// Remove all Harmony patches.
        /// </summary>
        public static void UnpatchAll()
        {
            // Only unapply if patches appplied.
            if (_patched)
            {
                Logging.KeyMessage("reverting Harmony patches");

                // Unapply patches, but only with our HarmonyID.
                Harmony harmonyInstance = new Harmony(harmonyID);
                harmonyInstance.UnpatchAll(harmonyID);
                _patched = false;
            }
        }


        /// <summary>
        ///  Apply Harmony patches to mods.
        /// </summary>
        public static void PatchMods() => ModLimitTranspiler.PatchMods(new Harmony(harmonyID));


        /// <summary>
        /// Applies patches to CitizenManager.SimulationStepImpl to change the simulation frame size for CitizenUnits.
        /// </summary>
        public static void TranspileSimulationStep()
        {
            Logging.KeyMessage("deploying CitizenManager.SimulationStepImpl transpiler");
            Harmony harmonyInstance = new Harmony(harmonyID);
            MethodBase targetMethod = typeof(CitizenManager).GetMethod("SimulationStepImpl", BindingFlags.Instance | BindingFlags.NonPublic);
            MethodInfo patchMethod = typeof(SimulationStepImplPatch).GetMethod(nameof(SimulationStepImplPatch.Transpiler));

            harmonyInstance.Patch(targetMethod, transpiler: new HarmonyMethod(patchMethod));
        }


        /// <summary>
        /// Lists all methods patched by Harmony.
        /// </summary>
        public static void ListMethods()
        {
            Harmony harmonyInstance = new Harmony(harmonyID);
            StringBuilder logMessage = new StringBuilder("Listing patches");

            // Get all patched methods via Harmony instance and iterate through.
            IEnumerable<MethodBase> patchedMethods = harmonyInstance.GetPatchedMethods();
            foreach (MethodBase patchedMethod in patchedMethods)
            {
                // Add the method info as header.
                logMessage.Append(patchedMethod.DeclaringType);
                logMessage.Append(".");
                logMessage.AppendLine(patchedMethod.Name);

                // Get Harmony patch info for this method and log details.
                Patches patches = Harmony.GetPatchInfo(patchedMethod);

                // Print out patch owners.
                foreach (string owner in patches.Owners)
                {
                    logMessage.Append("    ");
                    logMessage.AppendLine(owner);
                }

                // Print out patch indexes and types.
                foreach (var prefix in patches.Prefixes)
                {
                    logMessage.Append("        Prefix ");
                    logMessage.Append(prefix.index);
                    logMessage.Append(": ");
                    logMessage.AppendLine(prefix.owner);
                }
                foreach (var postfix in patches.Prefixes)
                {
                    logMessage.Append("        Prefix ");
                    logMessage.Append(postfix.index);
                    logMessage.Append(": ");
                    logMessage.AppendLine(postfix.owner);
                }
                foreach (var transpiler in patches.Prefixes)
                {
                    logMessage.Append("        Transpiler ");
                    logMessage.Append(transpiler.index);
                    logMessage.Append(": ");
                    logMessage.AppendLine(transpiler.owner);
                }
                foreach (var finalizer in patches.Finalizers)
                {
                    logMessage.Append("        Finalizer ");
                    logMessage.Append(finalizer.index);
                    logMessage.Append(": ");
                    logMessage.AppendLine(finalizer.owner);
                }
            }

            // Write message to log.
            Logging.Message(logMessage);
        }
    }
}