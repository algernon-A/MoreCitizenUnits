using ICities;
using ColossalFramework;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
        // Internal flags.
        internal static bool isModEnabled = false;

        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            Logging.KeyMessage("version ", MCUMod.Version, " loading");

            // Apply Harmony patches to mods.
            Patcher.PatchMods();

            base.OnCreated(loading);
        }


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Set simulation metatdata flag.
            MetaData.SetMetaData();

            Logging.Message("current CitizenUnit array size is ", Singleton<CitizenManager>.instance.m_units.m_buffer.Length.ToString("N0"), " with m_size ", Singleton<CitizenManager>.instance.m_units.m_size.ToString("N0"));

            Logging.Message("loading complete");
        }
    }
}