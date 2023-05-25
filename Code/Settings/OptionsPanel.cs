// <copyright file="OptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits
{
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;

    /// <summary>
    /// The mod's options panel.
    /// </summary>
    public class OptionsPanel : OptionsPanelBase
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float TitleMargin = Margin * 3f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 55f;

        /// <summary>
        /// Performs on-demand panel setup.
        /// </summary>
        protected override void Setup()
        {
            // Size and placement.
            autoLayout = false;
            float titleWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (TitleMargin * 2f);

            // Y position indicator.
            float currentY = Margin;

            // Language choice.
            UIDropDown languageDropDown = UIDropDowns.AddPlainDropDown(this, LeftMargin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (c, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            currentY += languageDropDown.parent.height + Margin;

            // Logging checkbox.
            UICheckBox loggingCheck = UICheckBoxes.AddPlainCheckBox(this, TitleMargin, currentY, Translations.Translate("DETAIL_LOGGING"));
            loggingCheck.isChecked = Logging.DetailLogging;
            loggingCheck.eventCheckChanged += (c, isChecked) => { Logging.DetailLogging = isChecked; };
            currentY += GroupMargin;

            // Double units title and checkbox.
            UISpacers.AddTitleSpacer(this, Margin, currentY, titleWidth, Translations.Translate("MCU_BOOST"));
            currentY += GroupMargin;
            UICheckBox doubleCheck = UICheckBoxes.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_X2"));
            doubleCheck.isChecked = CitizenDeserialize.DoubleLimit;
            doubleCheck.eventCheckChanged += (c, value) => { CitizenDeserialize.DoubleLimit = value; };
            currentY += doubleCheck.height + Margin;

            // Add double units text.
            UILabel boost1 = UILabels.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_BOOST_TXT1"), titleWidth, 0.9f);
            currentY += boost1.height + TitleMargin;
            UILabel boost2 = UILabels.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_BOOST_TXT2"), titleWidth, 0.9f);
            currentY += boost2.height + GroupMargin;

            // Cleaning options title.
            UISpacers.AddTitleSpacer(this, Margin, currentY, titleWidth, Translations.Translate("MCU_CLEAN"));
            currentY += GroupMargin;

            // Check and fix checkbox.
            UICheckBox fixCheck = UICheckBoxes.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_FIX"));
            fixCheck.isChecked = CitizenDeserialize.CheckUnits;
            fixCheck.eventCheckChanged += (c, value) => { CitizenDeserialize.CheckUnits = value; };
            currentY += fixCheck.height + TitleMargin;

            // Nuke checkbox.
            UICheckBox nukeCheck = UICheckBoxes.AddPlainCheckBox(this, Margin, currentY, Translations.Translate("MCU_NUKE"));
            nukeCheck.isChecked = LoadLevelComplete.NukeAll;
            nukeCheck.eventCheckChanged += (c, value) => { LoadLevelComplete.NukeAll = value; };
            currentY += nukeCheck.height + Margin;

            // Add nuke options text.
            UILabel nuke1 = UILabels.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT1"), titleWidth, 0.9f);
            currentY += nuke1.height + TitleMargin;
            UILabel nuke2 = UILabels.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT2"), titleWidth, 0.9f);
            currentY += nuke2.height + TitleMargin;
            UILabel nuke3 = UILabels.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT3"), titleWidth, 0.9f);
            currentY += nuke3.height + TitleMargin;
            UILabels.AddLabel(this, TitleMargin, currentY, Translations.Translate("MCU_NUKE_TXT4"), titleWidth, 0.9f);
        }
    }
}