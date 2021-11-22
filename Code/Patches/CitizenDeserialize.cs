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
        /// Highest priority, to try and make sure array resizing is done before any other mod tries to read the array.
        /// </summary>
        [HarmonyPriority(Priority.First)]
        public static void Prefix()
        {
            // Detect if we're loading an expanded or original CitizenUnit array.
            loadingExpanded = MetaData.LoadingExtended;

            // If we're expanding from vanilla saved data, ensure the CitizenUnit array is clear to start with.
            if (!loadingExpanded)
            {
                CitizenUnit[] units = Singleton<CitizenManager>.instance.m_units.m_buffer;
                Array.Clear(units, 0, units.Length);
            }
        }


        /// <summary>
        /// Harmony Postfix patch for CitizenManager.Data.Deserialize to ensure proper unused item allocation and count after conversion from vanilla save data.
        /// Highest priority, to try and make sure array resizing is done before any other mod tries to read the array.
        /// </summary>
        [HarmonyPriority(Priority.First)]
        public static void Postfix()
        {
            // Only need to do this if converting from vanilla saved data.
            if (!loadingExpanded)
            {
                Logging.Message("resetting unused instances");

                // Local reference.
                Array32<CitizenUnit> unitArray = Singleton<CitizenManager>.instance.m_units;

                // Clear unused elements array and list, and establish a debugging counter.
                unitArray.ClearUnused();
                uint freedUnits = 0;

                // Iterate through each unit in buffer.
                for (uint i = 0; i < unitArray.m_buffer.Length; ++i)
                {
                    // Check if this unit is valid.
                    if ((unitArray.m_buffer[i].m_flags & CitizenUnit.Flags.Created) == CitizenUnit.Flags.None)
                    {
                        // Invalid unit - properly release it to ensure m_units array's internals are correctly set.
                        unitArray.ReleaseItem(i);

                        // Increment debugging message counter.
                        ++freedUnits;
                    }
                }

                Logging.Message("completed resetting unused instances; freed unit count was ", freedUnits);
            }
        }


        /// <summary>
        /// Returns the correct size to deserialize a saved game array.
        /// </summary>
        public static int DeserialiseSize => loadingExpanded ? NewUnitCount : OriginalUnitCount;
    }
}