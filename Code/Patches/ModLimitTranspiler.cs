using System;
using System.Reflection;
using HarmonyLib;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Harmony transpilers to replace hardcoded CitizenUnit limits in mods.
    /// </summary>
    internal static class ModLimitTranspiler
    {
        /// <summary>
        /// Determines list of target methods to patch - in this case, identified mods and methods with hardcoded CitizenUnit limits.
        /// </summary>
        /// <returns>List of target methods to patch</returns>
        internal static void PatchMods(Harmony harmony)
        {
            if (harmony == null)
            {
                Logging.Error("null harmony instance passed to PatchMods");
                return;
            }

            // TM:PE.
            Assembly tmpe = ModUtils.GetEnabledAssembly("TrafficManager");
            if (tmpe != null)
            {
                Logging.Message("reflecting TM:PE");
                PatchModMethod(harmony, tmpe.GetType("TrafficManager.Manager.Impl.ExtVehicleManager"), "GetDriverInstanceId");
                PatchModMethod(harmony, tmpe.GetType("TrafficManager.Manager.Impl.VehicleBehaviorManager"), "ParkPassengerCar");
                PatchModMethod(harmony, tmpe.GetType("TrafficManager.Patch._VehicleAI._PassengerCarAI.ParkVehiclePatch"), "Prefix");
            }

            // Real Time.
            Assembly realTime = ModUtils.GetEnabledAssembly("RealTime");
            if (realTime != null)
            {
                Logging.Message("reflecting Real Time");
                PatchModMethod(harmony, realTime.GetType("RealTime.GameConnection.BuildingManagerConnection"), "BuildingCanBeVisited");
            }
        }


        /// <summary>
        /// Attempts to transpile hardcoded CitizenUnit limits in the given method from the given type. 
        /// </summary>
        /// <param name="harmony">Harmony instance</param>
        /// <param name="type">Type to reflect</param>
        /// <param name="methodName">Method name to reflect</param>
        private static void PatchModMethod(Harmony harmony, Type type, string methodName)
        {
            if (type == null)
            {
                Logging.Error("null param when attempting to patch ", methodName ?? "null");
                return;
            }

            MethodInfo method = AccessTools.Method(type, methodName);

            // Report error and return false if reflection failed.
            if (method == null)
            {
                Logging.Error("unable to reflect ", methodName);
                return; 
            }

            // If we got here, all good; apply transpiler.
            Logging.Message("transpiling ", type , ":", methodName);
            harmony.Patch(method, transpiler: new HarmonyMethod(typeof(GameLimitTranspiler), nameof(GameLimitTranspiler.Transpiler)));
        }
    }
}