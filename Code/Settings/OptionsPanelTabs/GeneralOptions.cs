// <copyright file="GeneralOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using AlgernonCommons;
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Options panel for setting basic mod options.
    /// </summary>
    internal sealed class GeneralOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;
        private const float TitleMargin = 50f;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal GeneralOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("OPTIONS_GENERAL"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // Language choice.
            UIDropDown languageDropDown = UIDropDowns.AddPlainDropDown(panel, LeftMargin, currentY, Translations.Translate("LANGUAGE_CHOICE"), Translations.LanguageList, Translations.Index);
            languageDropDown.eventSelectedIndexChanged += (control, index) =>
            {
                Translations.Index = index;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
            };
            languageDropDown.parent.relativePosition = new Vector2(LeftMargin, currentY);
            currentY += languageDropDown.parent.height + Margin;

            // Logging checkbox.
            currentY += 20f;
            UICheckBox loggingCheck = UICheckBoxes.AddPlainCheckBox(panel, LeftMargin, currentY, Translations.Translate("DETAIL_LOGGING"));
            loggingCheck.isChecked = Logging.DetailLogging;
            loggingCheck.eventCheckChanged += (c, isChecked) => { Logging.DetailLogging = isChecked; };
            currentY += loggingCheck.height + GroupMargin;

            // Vanilla on mode.
            float headerWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (Margin * 2f);
            UISpacers.AddOptionsSpacer(panel, Margin, currentY, headerWidth);
            currentY += TitleMargin;

            UICheckBox loadVanillaCheck = UICheckBoxes.AddPlainCheckBox(panel, LeftMargin, currentY, Translations.Translate("LOAD_VANILLA"));
            loadVanillaCheck.isChecked = Loading.LoadVanilla;
            loadVanillaCheck.eventCheckChanged += (c, isChecked) => { Loading.LoadVanilla = isChecked; };
            currentY += loadVanillaCheck.height + GroupMargin;

            // Hotkey options.
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("HOTKEYS"));
            currentY += TitleMargin;

            // Screenshot mode keymapping.
            OptionsKeymapping screenshotKeyMapping = OptionsKeymapping.AddKeymapping(panel, LeftMargin, currentY, Translations.Translate("KEY_SCREENSHOT"), UIThreading.ScreenshotModeKey);
            currentY += screenshotKeyMapping.Panel.height + Margin;

            // LOD mode keymapping.
            OptionsKeymapping lodKeyMapping = OptionsKeymapping.AddKeymapping(panel, LeftMargin, currentY, Translations.Translate("KEY_LOD"), UIThreading.LodModeKey);
            currentY += lodKeyMapping.Panel.height + Margin;

            // Vanilla mode keymapping.
            OptionsKeymapping vanillaKeyMapping = OptionsKeymapping.AddKeymapping(panel, LeftMargin, currentY, Translations.Translate("KEY_VANILLA"), UIThreading.VanillaModeKey);
            currentY += vanillaKeyMapping.Panel.height + Margin;
        }
    }
}