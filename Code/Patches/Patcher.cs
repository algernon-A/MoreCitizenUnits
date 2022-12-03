// <copyright file="Patcher.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits
{
    using System.Reflection;
    using AlgernonCommons;
    using AlgernonCommons.Patching;
    using HarmonyLib;

    /// <summary>
    /// Class to manage the mod's Harmony patches.
    /// </summary>
    public sealed class Patcher : PatcherBase
    {
        /// <summary>
        /// Applies patches to CitizenManager.SimulationStepImpl to change the simulation frame size for CitizenUnits.
        /// </summary>
        public void TranspileSimulationStep()
        {
            Logging.KeyMessage("deploying CitizenManager.SimulationStepImpl transpiler");
            Harmony harmonyInstance = new Harmony(HarmonyID);
            MethodBase targetMethod = AccessTools.Method(typeof(CitizenManager), "SimulationStepImpl");
            MethodInfo patchMethod = AccessTools.Method(typeof(SimulationStepImplPatch), nameof(SimulationStepImplPatch.Transpiler));

            harmonyInstance.Patch(targetMethod, transpiler: new HarmonyMethod(patchMethod));
        }
    }
}