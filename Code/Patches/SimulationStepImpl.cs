using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Harmony transpiler to increase simulation CitizenUnit frame size to meet new limit.
    /// </summary>
    public static class SimulationStepImplPatch
    {
        /// <summary>
        /// Harmony transpiler to increase simulation CitizenUnit frame size to meet new limit.
        /// Finds ldc.i4 128 (which is unique in this method's game code to the CitizenUnit framing) and replaces the operand with our updated maximum.
        /// </summary>
        /// <param name="instructions">Original ILCode</param>
        /// <returns>Patched ILCode</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            // Instruction parsing.
            IEnumerator<CodeInstruction> instructionsEnumerator = instructions.GetEnumerator();
            CodeInstruction instruction;

            Logging.Message("starting CitizenManager.SimulationStepImpl transpiler");

            // Status flag.
            bool foundTarget = false;

            // New frame size (4096 frames).
            int newFrame = (int)CitizenDeserialze.NewUnitCount / 4096;

            // Iterate through all instructions in original method.
            while (instructionsEnumerator.MoveNext())
            {
                // Get next instruction and add it to output.
                instruction = instructionsEnumerator.Current;

                // Is this ldc.i4 128 (CitizenManger.ReleaseUnits)?
                if (instruction.opcode == OpCodes.Ldc_I4 && instruction.operand is int thisInt && thisInt == 128)
                {
                    // Yes - change operand to our new unit count max.
                    Logging.Message("Found ldc.i4 with operand ", instruction.operand.GetType().ToString(), " ", instruction.operand.ToString(), "; replacing with ", newFrame.ToString());
                    instruction.operand = newFrame;

                    // Set flag.
                    foundTarget = true;
                }

                // Output instruction.
                yield return instruction;
            }

            // If we got here without finding our target, something went wrong.
            if (!foundTarget)
            {
                Logging.Error("no ldc.i4 128 found");
            }
        }
    }
}