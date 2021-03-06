using ColossalFramework;
using HarmonyLib;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Harmony Prefix patch for LoadingManager.LoadLevelComplete.  This enables us to perform setup tasks after all loading has been completed, but before OnLevelLoaded is called.
    /// </summary>
    [HarmonyPatch(typeof(LoadingManager))]
    [HarmonyPatch("LoadLevelComplete")]
    public static class LoadLevelComplete
    {
        /// <summary>
        /// Harmony postfix to perform actions require after the level has loaded.
        /// </summary>
        public static void Prefix()
        {
            // Get buffer size.
            Array32<CitizenUnit> units = Singleton<CitizenManager>.instance.m_units;
            int bufferSize = units.m_buffer.Length;
            Logging.Message("current CitizenUnit array size is ", bufferSize.ToString("N0"), " with m_size ", units.m_size.ToString("N0"));

            // If we're doing a clean reset, do so.
            if (ModSettings.nukeAll)
            {
                // Don't preserve existing units by default.
                UnitUtils.ResetUnits(false);

                // Update TMPE CitizenUnit reference.
                ModUtils.UpdateTMPEUnitsRef();

                // Clear setting after use - supposed to be once-off.
                ModSettings.nukeAll = false;
                ModSettings.Save();
            }

            // Check for successful implementation.
            if (bufferSize == CitizenDeserialze.NewUnitCount)
            {
                // Buffer successfully enlarged - set simulation metatdata flag.
                MetaData.SetMetaData();
                Logging.KeyMessage("loading complete");

                // List Harmony patches.
                Patcher.ListMethods();
            }
            else
            {
                // Buffer size not changed - log error and undo Harmony patches.
                Logging.Error("CitizenUnit array size not increased; aborting operation and reverting Harmony patches");
                Patcher.UnpatchAll();
            }

            // Set up options panel event handler (need to redo this now that options panel has been reset after loading into game).
            OptionsPanelManager.OptionsEventHook();
        }
    }
}