// <copyright file="ModUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits
{
    using System;
    using System.Reflection;
    using AlgernonCommons;
    using ColossalFramework;

    /// <summary>
    /// Class that manages interactions with other mods, including compatibility and functionality checks.
    /// </summary>
    internal static class ModUtils
    {
        // Specific reference to TM:PE assembly.
        private static Assembly tmpe;

        /// <summary>
        /// Gets the TM:PE assembly reference.
        /// </summary>
        internal static Assembly TMPE
        {
            get
            {
                if (tmpe == null)
                {
                    tmpe = AssemblyUtils.GetEnabledAssembly("TrafficManager");
                }

                return tmpe;
            }
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
                Logging.Message("couldn't reflect ", targetName);
            }
        }
    }
}
