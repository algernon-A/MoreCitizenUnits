﻿// <copyright file="CitizenDeserialize.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using AlgernonCommons;
    using AlgernonCommons.Patching;
    using ColossalFramework;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch to handle deserialization of game CitizenUnit data.
    /// </summary>
    [HarmonyPatch(typeof(CitizenManager.Data), nameof(CitizenManager.Data.Deserialize))]
    public static class CitizenDeserialize
    {
        /// <summary>
        /// Expanded unit count.
        /// </summary>
        internal const uint NewUnitCount = ExtraUnitCount + OriginalUnitCount;

        // Private constants.
        private const int OriginalUnitCount = 524288;
        private const int ExtraUnitCount = OriginalUnitCount;

        // Automatically double limits on virgin savegames.
        private static bool s_doubleLimit = true;

        /// <summary>
        /// Gets the correct size to deserialize a saved game array.
        /// </summary>
        public static uint DeserialiseSize => LoadingExpanded ? NewUnitCount : OriginalUnitCount;

        /// <summary>
        /// Gets or sets a value indicating whether invalid units should be checked for and fixed on load.
        /// </summary>
        internal static bool CheckUnits { get; set; } = false;

        /// <summary>
        /// Gets  a value indicating whether an expanded CitizenUnit array is being loaded.
        /// </summary>
        internal static bool LoadingExpanded { get; private set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether CitizenUnit limit doubling is enabled.
        /// </summary>
        internal static bool DoubleLimit
        {
            get => s_doubleLimit;

            set => s_doubleLimit = value;
        }

        /// <summary>
        /// Harmony Transpilier for CitizenManager.Data.Deserialize to increase the size of the CitizenUnit array at deserialization.
        /// </summary>
        /// <param name="instructions">Original ILCode.</param>
        /// <returns>Modified ILCode.</returns>
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Local variable ILCode indexes (original method).
            const int num2VarIndex = 6;

            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;

            // Status flag.
            bool inserted = false;

            Logging.Message("starting CitizenManager.Data.Deserialize transpiler");

            // Iterate through all instructions in original method.
            while (instructionsEnumerator.MoveNext())
            {
                // Get current instruction.
                instruction = instructionsEnumerator.Current;

                // If we haven't already inserted, look for our start indicator (first ldloc 2 in code).
                if (!inserted)
                {
                    // Is this ldloc.2?
                    if (instruction.opcode == OpCodes.Ldloc_2)
                    {
                        Logging.Message("dropping from Ldloc_2");

                        // Yes - set flag.
                        inserted = true;

                        // Insert new instruction, calling DeserializeSize to determine correct buffer size to deserialize.
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(CitizenDeserialize), nameof(DeserialiseSize)).GetGetMethod());

                        // Iterate forward, dropping all instructions until we reach our target (next stloc.s 6), then continue on as normal.
                        do
                        {
                            // This should never happen, but just in case....
                            if (!instructionsEnumerator.MoveNext())
                            {
                                Logging.Error("Couldn't find Stloc_S");
                                yield break;
                            }

                            instruction = instructionsEnumerator.Current;
                        }
                        while (!(instruction.opcode == OpCodes.Stloc_S && instruction.operand is LocalBuilder localBuilder && localBuilder.LocalIndex == num2VarIndex));
                        Logging.Message("resuming from Stloc_S");
                    }
                }

                // Add current instruction to output.
                yield return instruction;
            }
        }

        /// <summary>
        /// Harmony Prefix patch for CitizenManager.Data.Deserialize to determine if this mod was active when the game was saved.
        /// Highest priority, to try and make sure array setup is done before any other mod tries to read the array.
        /// </summary>
        [HarmonyPriority(Priority.First)]
        private static void Prefix()
        {
            Logging.Message("starting CitizenManager.Data.Deserialize Prefix");

            // Detect if we're loading an expanded or original CitizenUnit array.
            LoadingExpanded = MetaData.LoadingExtended;

            // If we're loading expanded data, automatically set double limit desearalization.
            bool usingDouble = s_doubleLimit | LoadingExpanded;

            // Check to see if CitizenUnit array has been correctly resized.
            Array32<CitizenUnit> units = Singleton<CitizenManager>.instance.m_units;
            if (units.m_buffer.Length == NewUnitCount)
            {
                // Are we using double limits (deliberately or because we're loading expanded data)?
                if (usingDouble)
                {
                    // If we're expanding from vanilla saved data, ensure the CitizenUnit array is clear to start with (just in case).
                    if (!LoadingExpanded)
                    {
                        Logging.KeyMessage("expanding from Vanilla save data");
                        Array.Clear(units.m_buffer, 0, units.m_buffer.Length);
                    }

                    // Apply SimulationStep transpiler.
                    PatcherManager<Patcher>.Instance.TranspileSimulationStep();
                }
                else
                {
                    // Not using double limits; resize back to vanilla (array will be completely refilled from save data, so no need to clear).
                    Logging.KeyMessage("resetting to vanilla buffer size");
                    Singleton<CitizenManager>.instance.m_units = new Array32<CitizenUnit>(CitizenManager.MAX_UNIT_COUNT);
                }
            }
            else
            {
                // Buffer wasn't extended - is this intentional?
                if (!usingDouble)
                {
                    Logging.Error("CitizenUnit buffer not extended");
                }
                else
                {
                    Logging.Message("using vanilla buffer size");
                }
            }

            // Update TM:PE CitizenUnit array reference.
            ModUtils.UpdateTMPEUnitsRef();

            Logging.Message("finished CitizenManager.Data.Deserialize Prefix");
        }

        /// <summary>
        /// Harmony Postfix patch for CitizenManager.Data.Deserialize to ensure proper unused item allocation and count after conversion from vanilla save data.
        /// Highest priority, to try and make sure array setup is done before any other mod tries to read the array.
        /// </summary>
        [HarmonyPriority(Priority.First)]
        private static void Postfix()
        {
            // Local references.
            Array32<CitizenUnit> unitArray = Singleton<CitizenManager>.instance.m_units;
            CitizenUnit[] unitBuffer = unitArray.m_buffer;

            Logging.Message("starting CitizenManager.Data.Deserialize Postfix with unitBuffer size ", unitBuffer.Length);

            // If expanding from vanilla saved data, ensure all new units are properly cleared.
            if (!LoadingExpanded)
            {
                // Iterate through each unit in buffer.
                for (uint i = OriginalUnitCount; i < unitBuffer.Length; ++i)
                {
                    // Reset all values.
                    unitBuffer[i].m_flags = CitizenUnit.Flags.None;
                    unitBuffer[i].m_building = 0;
                    unitBuffer[i].m_vehicle = 0;
                    unitBuffer[i].m_citizen0 = 0;
                    unitBuffer[i].m_citizen1 = 0;
                    unitBuffer[i].m_citizen2 = 0;
                    unitBuffer[i].m_citizen3 = 0;
                    unitBuffer[i].m_citizen4 = 0;
                    unitBuffer[i].m_nextUnit = 0;
                }
            }

            Logging.Message("finished CitizenManager.Data.Deserialize Postfix");
        }
    }
}