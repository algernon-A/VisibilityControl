// <copyright file="TransparencyOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using static VisibilityControl.Patches.TransparentLODFix;

    /// <summary>
    /// Options panel for setting transparency LOD fix visibility options.
    /// </summary>
    internal sealed class TransparencyOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;
        private const float TitleMargin = 55f;

        // Panel components.
        private readonly UISlider _buildingFallbackDistanceSlider;
        private readonly UISlider _buildingMinimumDistanceSlider;
        private readonly UISlider _buildingLodTransitionSlider;
        private readonly UISlider _propFallbackDistanceSlider;
        private readonly UISlider _propMinimumDistanceSlider;
        private readonly UISlider _propDistanceMultiplierSlider;
        private readonly UISlider _propLodTransitionSlider;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransparencyOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal TransparencyOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("OPTIONS_TRANSPARENCY"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // Header width.
            float headerWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (Margin * 2f);

            // Slider value formats.
            UISliders.SliderValueFormat distanceFormat = new UISliders.SliderValueFormat(valueMultiplier: 1f, roundToNearest: 1f, numberFormat: "N0", suffix: "m");
            UISliders.SliderValueFormat percentageFormat = new UISliders.SliderValueFormat(valueMultiplier: 100f, roundToNearest: 1f, numberFormat: "N0", suffix: "%");
            UISliders.SliderValueFormat directPercentageFormat = new UISliders.SliderValueFormat(valueMultiplier: 1f, roundToNearest: 1f, numberFormat: "N0", suffix: "%");

            // Building visibility options.
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_BUILDINGS"));
            currentY += TitleMargin;

            _buildingFallbackDistanceSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("FALLBACK_DISTANCE"), MinFallbackDistance, MaxFallbackDistance, 100f, BuildingFallbackRenderDistance, distanceFormat);
            _buildingFallbackDistanceSlider.eventValueChanged += (c, value) => BuildingFallbackRenderDistance = value;
            _buildingFallbackDistanceSlider.parent.tooltip = Translations.Translate("FALLBACK_DISTANCE_TIP");
            currentY += _buildingFallbackDistanceSlider.parent.height + Margin;

            _buildingMinimumDistanceSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("MIN_DISTANCE"), MinMinimumDistance, MaxMinimumDistance, 1f, BuildingMinimumDistance, distanceFormat);
            _buildingMinimumDistanceSlider.eventValueChanged += (c, value) => BuildingMinimumDistance = value;
            _buildingMinimumDistanceSlider.parent.tooltip = Translations.Translate("MIN_DISTANCE_TIP");
            currentY += _buildingMinimumDistanceSlider.parent.height + Margin;

            _buildingLodTransitionSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("LOD_TRANSITION"), MinLODTransitionMultiplier, MaxLODTransitionMultiplier, 0.05f, BuildingLODTransitionMultiplier, percentageFormat);
            _buildingLodTransitionSlider.eventValueChanged += (c, value) => BuildingLODTransitionMultiplier = value;
            _buildingLodTransitionSlider.parent.tooltip = Translations.Translate("LOD_TRANSITION_TIP");
            currentY += _buildingLodTransitionSlider.parent.height + Margin;

            // Prop visibility options.
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_PROPS"));
            currentY += TitleMargin;

            _propFallbackDistanceSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("FALLBACK_DISTANCE"), MinFallbackDistance, MaxFallbackDistance, 1000f, PropFallbackRenderDistance, distanceFormat);
            _propFallbackDistanceSlider.eventValueChanged += (c, value) => PropFallbackRenderDistance = value;
            _propFallbackDistanceSlider.parent.tooltip = Translations.Translate("FALLBACK_DISTANCE_TIP");
            currentY += _propFallbackDistanceSlider.parent.height + Margin;

            _propMinimumDistanceSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("MIN_DISTANCE"), MinMinimumDistance, MaxMinimumDistance, 1f, PropMinimumDistance, distanceFormat);
            _propMinimumDistanceSlider.eventValueChanged += (c, value) => PropMinimumDistance = value;
            _propMinimumDistanceSlider.parent.tooltip = Translations.Translate("MIN_DISTANCE_TIP");
            currentY += _propMinimumDistanceSlider.parent.height + Margin;

            _propDistanceMultiplierSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("DISTANCE_MULT"), MinDistanceMultiplier, MaxDistanceMultiplier, 1f, PropDistanceMultiplier, directPercentageFormat);
            _propDistanceMultiplierSlider.eventValueChanged += (c, value) => PropDistanceMultiplier = value;
            _propDistanceMultiplierSlider.parent.tooltip = Translations.Translate("DISTANCE_MULT_TIP");
            currentY += _propDistanceMultiplierSlider.parent.height + Margin;

            _propLodTransitionSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("LOD_TRANSITION"), MinLODTransitionMultiplier, MaxLODTransitionMultiplier, 0.05f, PropLODTransitionMultiplier, percentageFormat);
            _propLodTransitionSlider.eventValueChanged += (c, value) => PropLODTransitionMultiplier = value;
            _propLodTransitionSlider.parent.tooltip = Translations.Translate("LOD_TRANSITION_TIP");
            currentY += _propLodTransitionSlider.parent.height + Margin;

            UIButton defaultsButton = UIButtons.AddButton(panel, LeftMargin, currentY, Translations.Translate("RESET_DEFAULT"), 300f);
            defaultsButton.eventClicked += (c, p) =>
            {
                _buildingFallbackDistanceSlider.value = DefaultFallbackDistance;
                _buildingMinimumDistanceSlider.value = DefaultMinimumDistance;
                _buildingLodTransitionSlider.value = DefaultLODTransitionMultiplier;
                _propFallbackDistanceSlider.value = DefaultFallbackDistance;
                _propMinimumDistanceSlider.value = DefaultMinimumDistance;
                _propDistanceMultiplierSlider.value = DefaultDistanceMultiplier;
                _propLodTransitionSlider.value = DefaultLODTransitionMultiplier;
            };
        }
    }
}