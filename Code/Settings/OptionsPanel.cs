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


        // Y position indicator.
        float currentY = Margin;


        /// <summary>
        /// Performs initial setup for the panel; we don't use Start() as that's not sufficiently reliable (race conditions with size), and is not needed with the dynamic create/destroy process.
        /// </summary>
        internal void Setup(float width, float height)
        {
            // Size and placement.
            autoSize = false;
            autoLayout = false;
            this.width = width - 22f;
            this.height = height - 22f;
            float maxWidth = this.width - (TitleMargin * 2f);

            // Language choice.
            UIDropDown languageDropDown = UIControls.AddPlainDropDown(this, Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                ModSettings.Save();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + GroupMargin;

            // Check and fix checkbox.
            UICheckBox fixCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_FIX"));
            fixCheck.isChecked = CitizenDeserialze.checkUnits;
            fixCheck.eventCheckChanged += (control, value) => { CitizenDeserialze.checkUnits = value; };
            currentY += fixCheck.height + Margin;

            // Nuke options checkbox.
            AddTitle(this, "MCU_CLEAN", Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Semibold"), maxWidth);
            UICheckBox nukeCheck = UIControls.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_NUKE"));
            nukeCheck.isChecked = ModSettings.nukeAll;
            nukeCheck.eventCheckChanged += (control, value) => { ModSettings.nukeAll = value; };
            currentY += nukeCheck.height + Margin;

            // Add nuke options text.
            UILabel nuke1 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT1"), maxWidth, 0.9f);
            currentY += nuke1.height + TitleMargin;
            UILabel nuke2 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT2"), maxWidth, 0.9f);
            currentY += nuke2.height + TitleMargin;
            UILabel nuke3 = UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT3"), maxWidth, 0.9f);
            currentY += nuke3.height + TitleMargin;
            UIControls.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT4"), maxWidth, 0.9f);
        }


        /// <summary>
        /// Adds a spacer and new title to the given panel.
        /// </summary>
        /// <param name="titleKey">Title translation key</param>
        /// <param name="titleFont">Title font</param>
        private void AddTitle(UIComponent parent, string titleKey, UIFont titleFont, float maxWidth)
        {
            currentY += Margin;
            UIControls.OptionsSpacer(parent, Margin, currentY, maxWidth);
            currentY += TitleMargin * 2f;
            UILabel label = UIControls.AddLabel(parent, Margin, currentY, Translations.Translate(titleKey), textScale: 1.2f);
            label.font = titleFont;
            currentY += label.height + TitleMargin;
        }
    }
}