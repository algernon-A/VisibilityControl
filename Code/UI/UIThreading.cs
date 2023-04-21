// <copyright file="UIThreading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using AlgernonCommons.Keybinding;
    using ICities;
    using UnityEngine;

    /// <summary>
    /// Threading to capture hotkeys.
    /// </summary>
    public sealed class UIThreading : ThreadingExtensionBase
    {
        // Hotkeys.
        private static Keybinding s_lodModeKey = new Keybinding(KeyCode.Period, false, false, true);

        // Flags.
        private bool _lodyKeyProcessed = false;

        /// <summary>
        /// Gets or sets the LOD mode hotkey.
        /// </summary>
        internal static Keybinding LodModeKey { get => s_lodModeKey; set => s_lodModeKey = value; }

        /// <summary>
        /// Look for keypress to activate tool.
        /// </summary>
        /// <param name="realTimeDelta">Real-time delta since last update.</param>
        /// <param name="simulationTimeDelta">Simulation time delta since last update.</param>
        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {
            // Check for anarchy hotkey.
            if (s_lodModeKey.IsPressed())
            {
                // Only process if we're not already doing so.
                if (!_lodyKeyProcessed)
                {
                    // Set processed flag.
                    _lodyKeyProcessed = true;

                    // Toggle LOD mode.
                    PrefabManager.LodMode = !PrefabManager.LodMode;
                }
            }
            else
            {
                // Relevant keys aren't pressed anymore; this keystroke is over, so reset and continue.
                _lodyKeyProcessed = false;
            }
        }
    }
}