using ICities;
using CitiesHarmony.API;


namespace MoreCitizenUnits
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class MCUMod : IUserMod
    {
        public static string ModName => "More CitizenUnits";
        public static string Version => "0.5";

        public string Name => ModName + " " + Version;
        public string Description => Translations.Translate("MCU_DESC");


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());

            // Load the settings file.
            ModSettings.Load();
        }


        /// <summary>
        /// Called by the game when the mod is disabled.
        /// </summary>
        public void OnDisabled()
        {
            // Unapply Harmony patches via Cities Harmony.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.UnpatchAll();
            }
        }


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // Setup options panel reference.
            helper.AddCheckbox(Translations.Translate("MCU_FIX"), CitizenDeserialze.checkUnits, (isChecked) => { CitizenDeserialze.checkUnits = isChecked; ModSettings.Save(); });
        }
    }
}
