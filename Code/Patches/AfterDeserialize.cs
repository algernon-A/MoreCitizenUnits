// <copyright file="AfterDeserialize.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits.Code.Patches
{
    using System.Collections.Generic;
    using AlgernonCommons;
    using ColossalFramework;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch for post-deserialization cleanups.
    /// </summary>
    internal static class AfterDeserialize
    {
        [HarmonyPatch(typeof(CitizenManager.Data), nameof(CitizenManager.Data.AfterDeserialize))]
        internal static class CheckUnits
        {
            /// <summary>
            /// Harmony Postfix patch for CitizenManager.Data.AfterDeserialze to peform post-deserialization cleanups.
            /// Highest priority, to try and make sure array setup is done before any other mod tries to read the array.
            /// </summary>
            [HarmonyPriority(Priority.First)]
            internal static void Postfix()
            {
                // Local references.
                Array32<CitizenUnit> unitArray = Singleton<CitizenManager>.instance.m_units;
                CitizenUnit[] unitBuffer = unitArray.m_buffer;

                Logging.Message("starting CitizenManager.Data.AfterDeserialize Postfix with unitBuffer size ", Singleton<CitizenManager>.instance.m_units.m_buffer.Length);

                // Check for and fix invalid units, if set, or if expanding from Vanilla (needed to properly reset unused unit count and list of newly-expanded vanilla array).
                if ((CitizenDeserialze.s_checkUnits && !LoadLevelComplete.NukeAll) || !CitizenDeserialze.s_loadingExpanded)
                {
                    Logging.Message("checking units");

                    // Clear all unused items.
                    unitArray.ClearUnused();

                    // List of invalid units.
                    List<uint> invalidUnits = new List<uint>();

                    // Hashset of m_nextUnit references.
                    Dictionary<uint, uint> nextUnits = new Dictionary<uint, uint>();

                    // Iterate through each unit in buffer.
                    for (uint i = 0; i < unitBuffer.Length; ++i)
                    {
                        // Any units with no flags get added to our unused buffer immediately.
                        CitizenUnit.Flags unitFlags = unitBuffer[i].m_flags;
                        if (unitFlags == CitizenUnit.Flags.None)
                        {
                            unitArray.ReleaseItem(i);
                        }
                        else
                        {
                            // Flags aren't empty - check for invalid units: ones flagged as 'Created', but with no building, vehicle, or citizen attached.
                            uint nextUnit = unitBuffer[i].m_nextUnit;
                            if ((unitBuffer[i].m_flags & CitizenUnit.Flags.Created) == CitizenUnit.Flags.None
                                && unitBuffer[i].m_building == 0
                                && unitBuffer[i].m_vehicle == 0
                                && unitBuffer[i].m_citizen0 == 0
                                && unitBuffer[i].m_citizen1 == 0
                                && unitBuffer[i].m_citizen2 == 0
                                && unitBuffer[i].m_citizen3 == 0
                                && unitBuffer[i].m_citizen4 == 0)
                            {
                                Logging.Message("found empty unit ", i, " with invalid flags ", unitBuffer[i].m_flags);
                                invalidUnits.Add(i);
                            }
                            else if (nextUnit != 0)
                            {
                                // Check for nextUnit reference and add to list of references, if it isn't already there.
                                if (nextUnits.ContainsKey(nextUnit))
                                {
                                    Logging.Error("duplicate m_nextUnit reference to unit ", nextUnit);
                                }
                                else
                                {
                                    nextUnits.Add(nextUnit, i);
                                }
                            }
                        }
                    }

                    Logging.Message(invalidUnits.Count, " invalid units detected");

                    // Now, iterate through list of invalid units and clear all those without a m_nextUnit reference pointing TO them.
                    uint clearedCount = 0;
                    foreach (uint invalidUnit in invalidUnits)
                    {
                        if (nextUnits.ContainsKey(invalidUnit))
                        {
                            Logging.Message("leaving invalid unit ", invalidUnit, " referred to by m_nextUnit");
                        }
                        else
                        {
                            // Clear flags and release unit.
                            unitBuffer[invalidUnit].m_flags = CitizenUnit.Flags.None;
                            unitArray.ReleaseItem(invalidUnit);
                            ++clearedCount;
                        }
                    }

                    Logging.Message("completed releasing ", clearedCount, " CitizenUnits with invalid flags");
                }

                Logging.Message("finished CitizenManager.Data.AfterDeserialize Postfix");
            }
        }
    }
}
