// <copyright file="Loading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using System.Collections.Generic;
    using AlgernonCommons.Patching;
    using ColossalFramework.UI;
    using ICities;
    using VisibilityControl.AdditiveShader;
    using static PrefabManager;

    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public sealed class Loading : PatcherLoadingBase<OptionsPanel, PatcherBase>
    {
        /// <summary>
        /// Gets or sets a value indicating whether vanilla mode should be enabled by default when loading a save.
        /// </summary>
        internal static bool LoadVanilla { get; set; } = false;

        /// <summary>
        /// Gets a list of permitted loading modes.
        /// </summary>
        protected override List<AppMode> PermittedModes => new List<AppMode> { AppMode.Game, AppMode.MapEditor, AppMode.AssetEditor };

        /// <summary>
        /// Called by the game when exiting a level.
        /// </summary>
        public override void OnLevelUnloading()
        {
            UIView.GetAView().StopCoroutine(ShaderManager.ShaderCoroutine());
            ShaderManager.ManagedAssets = null;

            base.OnLevelUnloading();
        }

        /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            base.LoadedActions(mode);

            // Apply vanilla settings if option is set.
            if (LoadVanilla)
            {
                CurrentMode = OverrideMode.Vanilla;
            }

            ScanPrefabs();
        }
    }
}