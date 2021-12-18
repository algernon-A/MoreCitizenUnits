using System;
using System.Reflection.Emit;
using System.Collections.Generic;
using ColossalFramework;
using HarmonyLib;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Harmony patch to handle deserialization of game CitizenUnit data.
    /// </summary>
    [HarmonyPatch(typeof(CitizenManager.Data), nameof(CitizenManager.Data.Deserialize))]
    public static class CitizenDeserialze
    {
        // Constants.
        private const int OriginalUnitCount = 524288;
        private const int ExtraUnitCount = OriginalUnitCount;
        internal const int NewUnitCount = ExtraUnitCount + OriginalUnitCount;


        // Check for (and fix) invalid units on load.
        internal static bool checkUnits = true;

        // Status flag - are we loading an expanded CitizenUnit array?
        private static bool loadingExpanded = false;


        /// <summary>
        /// Harmony Transpilier for CitizenManager.Data.Deserialize to increase the size of the CitizenUnit array at deserialization.
        /// </summary>
        /// <param name="instructions">Original ILCode instructions</param>
        /// <returns></returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
                        yield return new CodeInstruction(OpCodes.Call, AccessTools.Property(typeof(CitizenDeserialze), nameof(CitizenDeserialze.DeserialiseSize)).GetGetMethod());

                        // Iterate forward, dropping all instructions until we reach our target (next stloc.s 6), then continue on as normal.
                        do
                        {
                            instructionsEnumerator.MoveNext();
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
        public static void Prefix()
        {
            Logging.Message("starting CitizenManager.Data.Deserialize Prefix");

            // Check to see if CitizenUnit array has been correctly resized.
            Array32<CitizenUnit> units = Singleton<CitizenManager>.instance.m_units;
            if (units.m_buffer.Length == NewUnitCount)
            {
                // Detect if we're loading an expanded or original CitizenUnit array.
                loadingExpanded = MetaData.LoadingExtended;

                // If we're expanding from vanilla saved data, ensure the CitizenUnit array is clear to start with.
                if (!loadingExpanded)
                {
                    Logging.Message("expanding from Vanilla save data");
                    Array.Clear(units.m_buffer, 0, units.m_buffer.Length);
                }

                // Apply SimulationStep transpiler.
                Patcher.TranspileSimulationStep();
            }
            else
            {
                // Buffer wasn't extended.
                Logging.Error("CitizenUnit buffer not extended");
            }

            Logging.Message("finished CitizenManager.Data.Deserialize Prefix");
        }


        /// <summary>
        /// Harmony Postfix patch for CitizenManager.Data.Deserialize to ensure proper unused item allocation and count after conversion from vanilla save data.
        /// Highest priority, to try and make sure array setup is done before any other mod tries to read the array.
        /// </summary>
        [HarmonyPriority(Priority.First)]
        public static void Postfix()
        {
            // Local references.
            Array32<CitizenUnit> unitArray = Singleton<CitizenManager>.instance.m_units;
            CitizenUnit[] unitBuffer = unitArray.m_buffer;

            Logging.Message("starting CitizenManager.Data.Deserialize Postfix");


            // Only need to do this if converting from vanilla saved data, or fixing invalid units.
            if (!loadingExpanded || checkUnits)
            {
                // First, check for and fix invalid units.
                Logging.Message("checking units");
                int invalidUnits = 0;

                // Iterate through each unit in buffer.
                for (uint i = 0; i < unitBuffer.Length; ++i)
                {
                    // Check for invalid units: ones flagged as 'Created', but with no building, vehicle, or citizen attached.
                    if ((unitBuffer[i].m_flags & CitizenUnit.Flags.Created) != CitizenUnit.Flags.None
                        && unitArray.m_buffer[i].m_building == 0
                        && unitArray.m_buffer[i].m_vehicle == 0
                        && unitArray.m_buffer[i].m_citizen0 == 0
                        && unitArray.m_buffer[i].m_citizen1 == 0
                        && unitArray.m_buffer[i].m_citizen2 == 0
                        && unitArray.m_buffer[i].m_citizen3 == 0
                        && unitArray.m_buffer[i].m_citizen4 == 0)
                    {
                        if (loadingExpanded)
                        {
                            // Don't bother logging on initial expansion.
                            Logging.Message("resetting invalid unit ", i);
                        }

                        // Increment invalid unit counter.
                        ++invalidUnits;

                        // Reset unit.
                        unitArray.m_buffer[i].m_flags = CitizenUnit.Flags.None;
                        unitArray.m_buffer[i].m_nextUnit = 0;
                    }
                }

                Logging.Message(invalidUnits, " invalid units cleared");

                // Clear unused elements array and list, and establish a debugging counter.
                Logging.Message("resetting unused instances");
                unitArray.ClearUnused();
                uint freedUnits = 0;

                // Iterate through each unit in buffer.
                for (uint i = 0; i < unitBuffer.Length; ++i)
                {
                    // Check if this unit is valid.
                    if ((unitBuffer[i].m_flags & CitizenUnit.Flags.Created) == CitizenUnit.Flags.None)
                    {
                        // Invalid unit - properly release it to ensure m_units array's internals are correctly set.
                        unitArray.ReleaseItem(i);

                        // Increment debugging message counter.
                        ++freedUnits;
                    }
                }

                Logging.Message("completed resetting unused instances; freed unit count was ", freedUnits);
            }

            Logging.Message("finished CitizenManager.Data.Deserialize Postfix");
        }


        /// <summary>
        /// Returns the correct size to deserialize a saved game array.
        /// </summary>
        public static int DeserialiseSize => loadingExpanded ? NewUnitCount : OriginalUnitCount;
    }
}