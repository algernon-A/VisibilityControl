// <copyright file="LODDistanceOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;
    using static VisibilityControl.Patches.LodDistanceBuildings;
    using static VisibilityControl.Patches.LodDistanceCitizens;
    using static VisibilityControl.Patches.LodDistanceNets;
    using static VisibilityControl.Patches.LodDistanceProps;
    using static VisibilityControl.Patches.LodDistanceTrees;
    using static VisibilityControl.Patches.LodDistanceVehicles;

    /// <summary>
    /// Options panel for setting transparency LOD fix visibility options.
    /// </summary>
    internal sealed class LODDistanceOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 35f;
        private const float TitleMargin = 50f;
        private const float SliderMargin = 60f;

        // Panel components.
        private readonly UISlider _buildingMinDistanceSlider;
        private readonly UISlider _buildingMultSlider;
        private readonly UISlider _netMinDistanceSlider;
        private readonly UISlider _netMultSlider;
        private readonly UISlider _vehicleMinDistanceSlider;
        private readonly UISlider _vehicleMultSlider;
        private readonly UISlider _propMinDistanceSlider;
        private readonly UISlider _propMultSlider;
        private readonly UISlider _decalDistanceSlider;
        private readonly UISlider _treeDistanceSlider;
        private readonly UISlider _citizenLodDistanceSlider;
        private readonly UISlider _citizenMaxDistanceSlider;

        /// <summary>
        /// Initializes a new instance of the <see cref="LODDistanceOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal LODDistanceOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("OPTIONS_ULOD"), tabIndex, out UIButton _, autoLayout: false);

            // Add scrollable panel for hotkeys.
            UIScrollablePanel scrollPanel = panel.AddUIComponent<UIScrollablePanel>();
            scrollPanel.relativePosition = new Vector2(0, Margin);
            scrollPanel.autoSize = false;
            scrollPanel.autoLayout = false;
            scrollPanel.width = panel.width - GroupMargin;
            scrollPanel.height = panel.height - Margin - GroupMargin;
            scrollPanel.clipChildren = true;
            scrollPanel.builtinKeyNavigation = true;
            scrollPanel.scrollWheelDirection = UIOrientation.Vertical;
            UIScrollbars.AddScrollbar(panel, scrollPanel);

            // Rest to defaults.
            UIButton defaultsButton = UIButtons.AddButton(panel, LeftMargin, scrollPanel.relativePosition.y + scrollPanel.height + Margin, Translations.Translate("RESET_DEFAULT"), 300f);
            defaultsButton.eventClicked += (c, p) =>
            {
                _buildingMinDistanceSlider.value = DefaultBuildingMinDistance;
                _buildingMultSlider.value = DefaultBuildingMult;
                _netMinDistanceSlider.value = DefaultNetMinDistance;
                _netMultSlider.value = DefaultNetMult;
                _vehicleMinDistanceSlider.value = DefaultVehicleMinDistance;
                _vehicleMultSlider.value = DefaultVehicleMult;
                _propMinDistanceSlider.value = DefaultPropMinDistance;
                _propMultSlider.value = DefaultPropMult;
                _decalDistanceSlider.value = DefaultDecalDistance;
                _treeDistanceSlider.value = DefaultTreeDistance;
                _citizenLodDistanceSlider.value = DefaultCitizenLodDistance;
                _citizenMaxDistanceSlider.value = DefaultCitizenMaxDistance;
            };

            // Y position indicator.
            float currentY = Margin;

            // Header width.
            float headerWidth = OptionsPanelManager<OptionsPanel>.PanelWidth - (Margin * 2f);

            // Slider value formats.
            UISliders.SliderValueFormat distanceFormat = new UISliders.SliderValueFormat(valueMultiplier: 1f, roundToNearest: 1f, numberFormat: "N0", suffix: "m");
            UISliders.SliderValueFormat multiplierFormat = new UISliders.SliderValueFormat(valueMultiplier: 100f, roundToNearest: 1f, numberFormat: "N0", suffix: "%");

            // Building visibility options.
            UISpacers.AddTitle(scrollPanel, Margin, currentY, Translations.Translate("OPTIONS_BUILDINGS"));
            currentY += TitleMargin;

            _buildingMinDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("MIN_DISTANCE"), MinBuildingDistance, MaxBuildingDistance, 1f, BuildingMinDistance, distanceFormat);
            _buildingMinDistanceSlider.eventValueChanged += (c, value) => BuildingMinDistance = value;
            _buildingMinDistanceSlider.parent.tooltip = Translations.Translate("MIN_DISTANCE_TIP");
            currentY += SliderMargin;

            _buildingMultSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("DISTANCE_MULT"), MinBuildingMult, MaxBuildingMult, 0.5f, BuildingMultiplier, multiplierFormat);
            _buildingMultSlider.eventValueChanged += (c, value) => BuildingMultiplier = value;
            _buildingMultSlider.parent.tooltip = Translations.Translate("DISTANCE_MULT_TIP");
            currentY += SliderMargin;

            // Network visibility options.
            UISpacers.AddTitleSpacer(scrollPanel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_NETS"));
            currentY += TitleMargin;

            _netMinDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("MIN_DISTANCE"), MinNetDistance, MaxNetDistance, 1f, NetMinDistance, distanceFormat);
            _netMinDistanceSlider.eventValueChanged += (c, value) => NetMinDistance = value;
            _netMinDistanceSlider.parent.tooltip = Translations.Translate("MIN_DISTANCE_TIP");
            currentY += SliderMargin;

            _netMultSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("DISTANCE_MULT"), MinNetMult, MaxNetMult, 0.1f, NetMultiplier, multiplierFormat);
            _netMultSlider.eventValueChanged += (c, value) => NetMultiplier = value;
            _netMultSlider.parent.tooltip = Translations.Translate("DISTANCE_MULT_TIP");
            currentY += SliderMargin;

            // Vehicle visibility options.
            UISpacers.AddTitleSpacer(scrollPanel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_VEHICLES"));
            currentY += TitleMargin;

            _vehicleMinDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("MIN_DISTANCE"), MinVehicleDistance, MaxVehicleDistance, 1f, VehicleMinDistance, distanceFormat);
            _vehicleMinDistanceSlider.eventValueChanged += (c, value) => VehicleMinDistance = value;
            _vehicleMinDistanceSlider.parent.tooltip = Translations.Translate("MIN_DISTANCE_TIP");
            currentY += SliderMargin;

            _vehicleMultSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("DISTANCE_MULT"), MinVehicleMult, MaxVehicleMult, 0.1f, VehicleMultiplier, multiplierFormat);
            _vehicleMultSlider.eventValueChanged += (c, value) => VehicleMultiplier = value;
            _vehicleMultSlider.parent.tooltip = Translations.Translate("DISTANCE_MULT_TIP");
            currentY += SliderMargin;

            // Prop visibility options.
            UISpacers.AddTitleSpacer(scrollPanel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_PROPS"));
            currentY += TitleMargin;

            _propMinDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("MIN_DISTANCE"), MinPropDistance, MaxPropDistance, 1f, PropMinDistance, distanceFormat);
            _propMinDistanceSlider.eventValueChanged += (c, value) => PropMinDistance = value;
            _propMinDistanceSlider.parent.tooltip = Translations.Translate("MIN_DISTANCE_TIP");
            currentY += SliderMargin;

            _propMultSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("DISTANCE_MULT"), MinPropMult, MaxPropMult, 0.5f, PropMultiplier, multiplierFormat);
            _propMultSlider.eventValueChanged += (c, value) => PropMultiplier = value;
            _propMultSlider.parent.tooltip = Translations.Translate("DISTANCE_MULT_TIP");
            currentY += SliderMargin;

            _decalDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("DECAL_DISTANCE"), MinPropDistance, MaxPropDistance, 1f, DecalFadeDistance, distanceFormat);
            _decalDistanceSlider.eventValueChanged += (c, value) => DecalFadeDistance = value;
            _decalDistanceSlider.parent.tooltip = Translations.Translate("DECAL_DISTANCE_TIP");
            currentY += SliderMargin;

            // Tree visibility options.
            UISpacers.AddTitleSpacer(scrollPanel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_TREES"));
            currentY += TitleMargin;

            _treeDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("TREE_DISTANCE"), MinTreeDistance, MaxTreeDistance, 1f, TreeLodDistance, distanceFormat);
            _treeDistanceSlider.eventValueChanged += (c, value) => TreeLodDistance = value;
            _treeDistanceSlider.parent.tooltip = Translations.Translate("LOD_TRANSITION_ABS_TIP");
            currentY += SliderMargin;

            // Citizen visibility options.
            UISpacers.AddTitleSpacer(scrollPanel, Margin, currentY, headerWidth, Translations.Translate("OPTIONS_CITIZENS"));
            currentY += TitleMargin;

            _citizenLodDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("LOD_TRANSITION"), MinCitizenLodDistance, MaxCitizenLodDistance, 25f, CitizenLodDistance, distanceFormat);
            _citizenLodDistanceSlider.eventValueChanged += (c, value) => CitizenLodDistance = value;
            _citizenLodDistanceSlider.parent.tooltip = Translations.Translate("LOD_TRANSITION_ABS_TIP");
            currentY += SliderMargin;

            _citizenMaxDistanceSlider = UISliders.AddPlainSliderWithValue(scrollPanel, LeftMargin, currentY, Translations.Translate("MAX_DISTANCE"), MinCitizenMaxDistance, MaxCitizenMaxDistance, 1f, CitizenMaxDistance, distanceFormat);
            _citizenMaxDistanceSlider.eventValueChanged += (c, value) => CitizenMaxDistance = value;
            _citizenMaxDistanceSlider.parent.tooltip = Translations.Translate("MAX_DISTANCE_TIP");
            currentY += SliderMargin;
        }
    }
}