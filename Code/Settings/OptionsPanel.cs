using System.Linq;
using UnityEngine;
using ColossalFramework.UI;


namespace MoreCitizenUnits
{
    /// <summary>
    /// VSD options panel.
    /// </summary>
    public class MCUOptionsPanel : UIPanel
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float TitleMargin = Margin * 2f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;


        /// <summary>
        /// Performs initial setup for the panel; we don't use Start() as that's not sufficiently reliable (race conditions with size), and is not needed with the dynamic create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Get font reference.
            UIFont semiBold = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Semibold");

            // Size and placement.
            autoSize = false;
            autoLayout = false;
            float titleWidth = this.width - (TitleMargin * 2f);

            // Y position indicator.
            float currentY = Margin;

            // Language choice.
            UIDropDown languageDropDown = UIControls.AddPlainDropDown(this, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager.LocaleChanged();
                ModSettings.Save();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + GroupMargin;


            // Double units title and checkbox.
            currentY = AddTitle(this, "MCU_BOOST", semiBold, titleWidth, currentY);
            UICheckBox doubleCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_X2"));
            doubleCheck.isChecked = CitizenDeserialze.DoubleLimit;
            doubleCheck.eventCheckChanged += (control, value) => { CitizenDeserialze.DoubleLimit = value; };
            currentY += doubleCheck.height + Margin;

            // Add double units text.
            UILabel boost1 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_BOOST_TXT1"), titleWidth, 0.9f);
            currentY += boost1.height + TitleMargin;
            UILabel boost2 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_BOOST_TXT2"), titleWidth, 0.9f);
            currentY += boost2.height + GroupMargin;

            // Cleaning options title.
            currentY = AddTitle(this, "MCU_CLEAN", semiBold, titleWidth, currentY);
            currentY += boost2.height + TitleMargin;

            // Check and fix checkbox.
            UICheckBox fixCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_FIX"));
            fixCheck.isChecked = CitizenDeserialze.checkUnits;
            fixCheck.eventCheckChanged += (control, value) => { CitizenDeserialze.checkUnits = value; };
            currentY += fixCheck.height + TitleMargin;

            // Nuke checkbox.
            UICheckBox nukeCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_NUKE"));
            nukeCheck.isChecked = ModSettings.nukeAll;
            nukeCheck.eventCheckChanged += (control, value) => { ModSettings.nukeAll = value; };
            currentY += nukeCheck.height + Margin;

            // Add nuke options text.
            UILabel nuke1 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT1"), titleWidth, 0.9f);
            currentY += nuke1.height + TitleMargin;
            UILabel nuke2 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT2"), titleWidth, 0.9f);
            currentY += nuke2.height + TitleMargin;
            UILabel nuke3 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT3"), titleWidth, 0.9f);
            currentY += nuke3.height + TitleMargin;
            UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT4"), titleWidth, 0.9f);
        }

        /// <summary>
        /// Adds a spacer and new title to the given panel.
        /// </summary>
        /// <param name="titleKey">Title translation key</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="titleFont">Title font</param>
        /// <param name="width">Spacer width</param>
        /// <param name="yPos">Y-position indicator</param>
        /// <returns>Updated Y position indicator</returns>
        private float AddTitle(UIComponent parent, string titleKey, UIFont titleFont, float maxWidth, float yPos)
        {
            float currentY = yPos + Margin;
            UIControls.OptionsSpacer(parent, Margin, currentY, maxWidth);
            currentY += TitleMargin * 2f;
            UILabel label = UIControls.AddLabel(parent, Margin, currentY, Translations.Translate(titleKey), textScale: 1.2f);
            label.font = titleFont;
            return currentY + label.height + TitleMargin;
        }
    }
}