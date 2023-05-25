// <copyright file="ModLimitTranspiler.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits
{
    using System;
    using System.Reflection;
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmony transpilers to replace hardcoded CitizenUnit limits in mods.
    /// </summary>
    internal static class ModLimitTranspiler
    {
        /// <summary>
        /// Attempts to transpile hardcoded CitizenUnit limits in the given method from the given type.
        /// </summary>
        /// <param name="harmony">Harmony instance.</param>
        /// <param name="type">Type to patch.</param>
        /// <param name="methodName">Method to patch.</param>
        private static void PatchModMethod(Harmony harmony, Type type, string methodName)
        {
            // Check that reflection succeeded before proceeding,
            if (type == null)
            {
                // If this was the ParkVehiclePatch Prefix, not finding it is fine - that's only in TM:PE 11.6+.
                if (methodName.Equals("Prefix"))
                {
                    Logging.Message("TM:PE ParkVehiclePatch not found (this is fine)");
                }
                else
                {
                    Logging.Error("null type when attempting to patch ", methodName ?? "null");
                }

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
            Logging.Message("transpiling ", type, ":", methodName);
            harmony.Patch(method, transpiler: new HarmonyMethod(typeof(GameLimitTranspiler), nameof(GameLimitTranspiler.Transpiler)));
        }
    }
}