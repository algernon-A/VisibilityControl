// <copyright file="UIThreading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using AlgernonCommons.Keybinding;
    using ICities;
    using UnityEngine;
    using static PrefabManager;

    /// <summary>
    /// Threading to capture hotkeys.
    /// </summary>
    public sealed class UIThreading : ThreadingExtensionBase
    {
        // Hotkeys.
        private static Keybinding s_screenshotModeKey = new Keybinding(KeyCode.Period, true, false, false);
        private static Keybinding s_lodModeKey = new Keybinding(KeyCode.Period, true, false, true);
        private static Keybinding s_vanillaModeKey = new Keybinding(KeyCode.Period, true, true, false);

        // Flags.
        private bool _screenshotKeyProcessed = false;
        private bool _lodKeyProcessed = false;
        private bool _vanillaKeyProcessed = false;

        /// <summary>
        /// Gets or sets the screenshot mode hotkey.
        /// </summary>
        internal static Keybinding ScreenshotModeKey { get => s_screenshotModeKey; set => s_screenshotModeKey = value; }

        /// <summary>
        /// Gets or sets the LOD mode hotkey.
        /// </summary>
        internal static Keybinding LodModeKey { get => s_lodModeKey; set => s_lodModeKey = value; }

        /// <summary>
        /// Gets or sets the vanilla mode hotkey.
        /// </summary>
        internal static Keybinding VanillaModeKey { get => s_vanillaModeKey; set => s_vanillaModeKey = value; }

        /// <summary>
        /// Look for keypress to activate tool.
        /// </summary>
        /// <param name="realTimeDelta">Real-time delta since last update.</param>
        /// <param name="simulationTimeDelta">Simulation time delta since last update.</param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Check for screenshot mode hotkey.
            if (s_screenshotModeKey.IsPressed())
            {
                // Only process if we're not already doing so.
                if (!_screenshotKeyProcessed)
                {
                    // Set processed flag.
                    _screenshotKeyProcessed = true;

                    // Toggle screenshot mode.
                    if (CurrentMode == OverrideMode.Screenshot)
                    {
                        CurrentMode = OverrideMode.None;
                    }
                    else
                    {
                        CurrentMode = OverrideMode.Screenshot;
                    }
                }
            }
            else
            {
                // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                _screenshotKeyProcessed = false;
            }

            // Check for LOD mode hotkey.
            if (s_lodModeKey.IsPressed())
            {
                // Only process if we're not already doing so.
                if (!_lodKeyProcessed)
                {
                    // Set processed flag.
                    _lodKeyProcessed = true;

                    // Toggle LOD mode.
                    if (CurrentMode == OverrideMode.LOD)
                    {
                        CurrentMode = OverrideMode.None;
                    }
                    else
                    {
                        CurrentMode = OverrideMode.LOD;
                    }
                }
            }
            else
            {
                // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                _lodKeyProcessed = false;
            }

            // Check for vanilla mode hotkey.
            if (s_vanillaModeKey.IsPressed())
            {
                // Only process if we're not already doing so.
                if (!_vanillaKeyProcessed)
                {
                    // Set processed flag.
                    _vanillaKeyProcessed = true;

                    // Toggle LOD mode.
                    if (CurrentMode == OverrideMode.Vanilla)
                    {
                        CurrentMode = OverrideMode.None;
                    }
                    else
                    {
                        CurrentMode = OverrideMode.Vanilla;
                    }
                }
            }
            else
            {
                // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                _vanillaKeyProcessed = false;
            }
        }
    }
}