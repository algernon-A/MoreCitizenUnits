using ColossalFramework;
using HarmonyLib;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Harmony Postfix patch for OnLevelLoaded.  This enables us to perform setup tasks after all loading has been completed.
    /// </summary>
    [HarmonyPatch(typeof(LoadingWrapper))]
    [HarmonyPatch("OnLevelLoaded")]
    public static class OnLevelLoadedPatch
    {
        /// <summary>
        /// Harmony postfix to perform actions require after the level has loaded.
        /// </summary>
        public static void Postfix()
        {
            // Set simulation metatdata flag.
            MetaData.SetMetaData();

            Logging.Message("current CitizenUnit array size is ", Singleton<CitizenManager>.instance.m_units.m_buffer.Length.ToString("N0"), " with m_size ", Singleton<CitizenManager>.instance.m_units.m_size.ToString("N0"));
            Logging.KeyMessage("loading complete");
        }
    }
}