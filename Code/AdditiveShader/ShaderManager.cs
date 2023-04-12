// <copyright file="ShaderManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard), Quistar (Simon Ueng), aubergine10, Ronyx69. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.AdditiveShader
{
    using System.Collections;
    using ColossalFramework;
    using UnityEngine;

    /// <summary>
    /// Class to manage additive shader assets.
    /// </summary>
    public static class ShaderManager
    {
        // Coroutine sleep duration.
        private const float SleepDuration = 2.8f;

        // String key used in mesh names to indicate that this mesh uses the additive shader.
        private const string ShaderKey = "AdditiveShader";

        private static ManagedPrefab[] _managedAssets;

        /// <summary>
        /// Gets or sets the array of managed prefabs.
        /// </summary>
        internal static ManagedPrefab[] ManagedAssets { get => _managedAssets; set => _managedAssets = value; }

        /// <summary>
        /// Check if a mesh name contains the additive shader key.
        /// </summary>
        /// <param name="meshName">Mesh name to check.</param>
        /// <returns><c>true</c> if the mesh name has the additive shader key, <c>false</c> otherwise.</returns>
        public static bool HasShaderKey(string meshName) => !string.IsNullOrEmpty(meshName) && meshName.StartsWith(ShaderKey);

        /// <summary>
        /// Checks if a building contains props using the additive shader; if so, increases the maximum prop render distance for that building.
        /// Needed as prop LODs don't support the addtive shader, so the solution is to extend the prop visibility distance instead.
        /// </summary>
        /// <param name="building">Building prefab to check.</param>
        /// <returns><c>true</c> if the building contains props using the addtive shader, <c>false</c> otherwise.</returns>
        public static bool ContainsShaderProps(BuildingInfo building)
        {
            bool result = false;

            // Iterate through all props in building.
            BuildingInfo.Prop[] props = building.m_props;
            for (int i = 0; i < props.Length; ++i)
            {
                // Check eaxh prop for the additive shader key.
                PropInfo prop = props[i].m_finalProp;
                if (prop && prop.m_mesh && HasShaderKey(prop.m_mesh?.name))
                {
                    building.m_maxPropDistance = ManagedPrefab.ExpandedPropVisibilityDistance;
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Additive shader coroutine.
        /// </summary>
        /// <returns>Coroutine <see cref="IEnumerator"/>.</returns>
        internal static IEnumerator ShaderCoroutine()
        {
            // Local reference.
            SimulationManager simulationManager = Singleton<SimulationManager>.instance;

            // Wait period.
            WaitForSeconds sleepDuration = new WaitForSeconds(SleepDuration);
            while (true)
            {
                // Sleep coroutine.
                yield return sleepDuration;

                // Get time.
                float time = simulationManager.m_currentDayTimeHour;
                bool isNightTime = simulationManager.m_isNightTime;

                // Iterate through each managed asset.
                for (int i = 0; i < _managedAssets.Length; ++i)
                {
                    // Perform time-based actions.
                    ShaderData profile = _managedAssets[i].Profile;
                    switch (profile.m_profile & ShaderData.Profiles.TypeMask)
                    {
                        // Legacy Ronyx69 profile (time-based):
                        case ShaderData.Profiles.OldRonyxProfile:
                            float onTime = profile.OnTime;
                            float offTime = profile.OffTime;
                            if (onTime < offTime)
                            {
                                if (time >= onTime && time < offTime)
                                {
                                    _managedAssets[i].SetVisibility(true);
                                }
                                else if (time < onTime || time >= offTime)
                                {
                                    _managedAssets[i].SetVisibility(false);
                                }
                            }
                            else
                            {
                                if (time >= offTime && time < onTime)
                                {
                                    _managedAssets[i].SetVisibility(false);
                                }
                                else if (time < offTime || time >= onTime)
                                {
                                    _managedAssets[i].SetVisibility(true);
                                }
                            }

                            break;

                        // Container profile (always on).
                        case ShaderData.Profiles.Container:
                            _managedAssets[i].SetVisibility(true);
                            break;

                        // Standard profile.
                        default:
                            if (!_managedAssets[i].IsContainer && !_managedAssets[i].Profile.IsStatic)
                            {
                                if (_managedAssets[i].Profile.IsToggledByTwilight)
                                {
                                    // Visible at night.
                                    _managedAssets[i].SetVisibliltyByTwilight(isNightTime);
                                }
                                else
                                {
                                    // Time-based.
                                    _managedAssets[i].SetVisiblityByTime(time);
                                }
                            }

                            break;
                    }

                    yield return null;
                }
            }
        }
    }
}
