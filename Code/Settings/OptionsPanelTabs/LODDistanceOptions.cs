// <copyright file="LODDistanceOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using static VisibilityControl.Patches.LodDistance;

    /// <summary>
    /// Options panel for setting transparency LOD fix visibility options.
    /// </summary>
    internal sealed class LODDistanceOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;
        private const float TitleMargin = 55f;

        // Panel components.
        private readonly UISlider _treeDistanceSlider;
        private readonly UISlider _buildingMinDistanceSlider;
        private readonly UISlider _buildingMultSlider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LODDistanceOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal LODDistanceOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("OPTIONS_ULOD"), tabIndex, out UIButton _, autoLayout: false);

            // Y position indicator.
            float currentY = GroupMargin;

            // Header width.
            float headerWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (Margin * 2f);

            // Tree visibility options.
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_TREE"));
            currentY += TitleMargin;

            _treeDistanceSlider = UISliders.AddPlainSliderWithIntegerValue(panel, LeftMargin, currentY, Translations.Translate("TREE_DISTANCE"), MinLodDistance, MaxLodDistance, 100f, TreeLodDistance);
            _treeDistanceSlider.eventValueChanged += (c, value) => TreeLodDistance = value;
            currentY += _treeDistanceSlider.parent.height + Margin;

            // Building visibility options.
            UISpacers.AddTitleSpacer(panel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_BUILDING"));
            currentY += TitleMargin;

            _buildingMinDistanceSlider = UISliders.AddPlainSliderWithIntegerValue(panel, LeftMargin, currentY, Translations.Translate("MIN_DISTANCE"), MinBuildingDistance, MaxLodDistance, 100f, BuildingMinDistance);
            _buildingMinDistanceSlider.eventValueChanged += (c, value) => BuildingMinDistance = value;
            _buildingMinDistanceSlider.parent.tooltip = Translations.Translate("MIN_DISTANCE_TIP");
            currentY += _buildingMinDistanceSlider.parent.height + Margin;

            _buildingMultSlider = UISliders.AddPlainSliderWithValue(panel, LeftMargin, currentY, Translations.Translate("DISTANCE_MULT"), MinBuildingMult, MaxBuildingMult, 0.5f, BuildingMultiplier);
            _buildingMultSlider.eventValueChanged += (c, value) => BuildingMultiplier = value;
            _buildingMultSlider.parent.tooltip = Translations.Translate("DISTANCE_MULT_TIP");
            currentY += _buildingMultSlider.parent.height + Margin;

            UIButton defaultsButton = UIButtons.AddButton(panel, LeftMargin, currentY, Translations.Translate("RESET_DEFAULT"), 300f);
            defaultsButton.eventClicked += (c, p) =>
            {
                _treeDistanceSlider.value = DefaultTreeDistance;
            };
        }
    }
}