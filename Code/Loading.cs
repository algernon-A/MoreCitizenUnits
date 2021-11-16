using ICities;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
    {
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
    }
}