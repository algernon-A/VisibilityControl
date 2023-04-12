// <copyright file="ShaderData.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard), Quistar (Simon Ueng), aubergine10, Ronyx69. All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl.AdditiveShader
{
    using System.Globalization;
    using AlgernonCommons;

    /// <summary>
    /// <para>Class to extract and store additive shader info from a mesh name.</para>
    /// <para>
    /// It also derives whether the shader stays on across the midnight boundary,
    /// and whether it is always on.
    /// </para>
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Defined struct fields consistent with DotNet Runtime coding style")]
    public struct ShaderData
    {
        /// <summary>
        /// Shader profile flags.
        /// </summary>
        public readonly Profiles m_profile;

        /// <summary>
        /// <para>A value which controls fading of the additive shader.</para>
        /// <para>
        /// The additive shader decreases opacity as it gets closer to other objects.
        /// Higher value means less fading, because reasons.
        /// </para>
        /// </summary>
        public float m_fade;

        /// <summary>
        /// <para>A value indicating the light intensity multiplier to apply to the additive shader.</para>
        /// <para>Values above 1 may start to bloom.</para>
        /// </summary>
        public float m_intensity;

        // Hard-coded values derived from SimulationManager's `SUNRISE_HOUR` and `SUNSET_HOUR` members.
        // This is done because mods such as Real Time can alter those vanilla values. We need the
        // original values as that's what most asset authors base their twilight shader on/off times on.
        private const float SUNRISE = 5f;
        private const float SUNSET = 20f;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderData"/> struct.
        /// Shader parameters are extracted from the provided mesh name.
        /// </summary>
        /// <param name="meshName">Raw mesh name.</param>
        public ShaderData(string meshName)
        {
            // Assign default values.
            m_profile = Profiles.None;
            OnTime = 0f;
            OffTime = 0f;
            m_fade = 0f;
            m_intensity = 0f;

            // Shader info starts at character 15, after "AdditiveShader ".
            string attributes = meshName.Substring(15);

            if (attributes[0] >= '0' && attributes[0] <= '9')
            {
                // Legacy Ronyx69 profile format.
                m_profile = Profiles.OldRonyxProfile;

                // Found the space delimiter; parse the details.
                OnTime = ParseFloat(attributes, 0, out int charIndex);
                OffTime = ParseFloat(attributes, charIndex, out charIndex);
                m_fade = ParseFloat(attributes, charIndex, out charIndex);
                m_intensity = ParseFloat(attributes, charIndex, out _);
            }
            else
            {
                // Newer format (keword-based).
                if (attributes.StartsWith("AlwaysOn"))
                {
                    m_profile = Profiles.AlwaysOn;
                    OnTime = 0f;
                    OffTime = 24f;
                }
                else if (attributes.StartsWith("Modded"))
                {
                    m_profile = Profiles.Modded;
                    OnTime = -1f;
                    OffTime = -1f;
                }
                else if (attributes.StartsWith("DayTime"))
                {
                    m_profile = Profiles.DayTime;
                    OnTime = SUNRISE;
                    OffTime = SUNSET;
                }
                else if (attributes.StartsWith("NightTime"))
                {
                    m_profile = Profiles.NightTime;
                    OnTime = SUNSET;
                    OffTime = SUNRISE;
                }
                else if (attributes.StartsWith("Container"))
                {
                    m_profile = Profiles.Container;
                    OnTime = 0f;
                    OffTime = 24f;
                }
                else
                {
                    Logging.Error("invalid additive shader mesh attributes ", attributes);
                    return;
                }

                // Parse fade and intensity values.
                ParseFadeIntensity(attributes);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShaderData"/> struct using the given profile.
        /// </summary>
        /// <param name="profile">Profile to assign.</param>
        public ShaderData(Profiles profile)
        {
            m_profile = profile;
            OnTime = 0f;
            OffTime = 0f;
            m_fade = 0f;
            m_intensity = 0f;
        }

        /// <summary>
        /// Included profiles.
        /// </summary>
        public enum Profiles : ushort
        {
            /// <summary>
            /// No profile set.
            /// </summary>
            None = 0,

            /// <summary>
            /// Remotely controlled.
            /// </summary>
            RemotelyControlled = 0x0001,

            /// <summary>
            /// Always on.
            /// </summary>
            OnAlways = 0X0002,

            /// <summary>
            /// Static.
            /// </summary>
            Static = 0x0004,

            /// <summary>
            /// Overlaps midight.
            /// </summary>
            OverlapsMidnight = 0x0008,

            /// <summary>
            /// Toggled by twilight.
            /// </summary>
            ToogledByTwilight = 0x0010,

            /// <summary>
            /// Daytime only.
            /// </summary>
            DayOnly = 0x0020,

            /// <summary>
            /// Nighttime only.
            /// </summary>
            NightOnly = 0x0040,

            /// <summary>
            /// Always on.
            /// </summary>
            AlwaysOn = 0x0100 | OnAlways | Static,

            /// <summary>
            /// Modded.
            /// </summary>
            Modded = 0x0200 | RemotelyControlled | Static,

            /// <summary>
            /// Daytime.
            /// </summary>
            DayTime = 0x0400 | ToogledByTwilight | DayOnly,

            /// <summary>
            /// Nighttime.
            /// </summary>
            NightTime = 0x0800 | OverlapsMidnight | ToogledByTwilight | NightOnly,

            /// <summary>
            /// Container.
            /// </summary>
            Container = 0x1000 | OnAlways,

            /// <summary>
            /// Legacy Ronyx69 profile.
            /// </summary>
            OldRonyxProfile = 0x2000,

            /// <summary>
            /// Profile type mask.
            /// </summary>
            TypeMask = 0xFF00,
        }

        /// <summary>
        /// <para>Gets a value defining the game time at which shader is shown.</para>
        /// <para>Note: Will be negative if <c>AlwaysOff</c> keyword was used.</para>
        /// </summary>
        public float OnTime { get; }

        /// <summary>
        /// Gets a value defining the game time at which shader is hidden.
        /// </summary>
        public float OffTime { get; }

        /// <summary>
        /// Gets a value indicating whether OnTime > OffTime (ie. the shader is visible across the midnight boundary).
        /// </summary>
        public bool OverlapsMidnight => (m_profile & Profiles.OverlapsMidnight) == Profiles.OverlapsMidnight;

        /// <summary>
        /// <para>Gets a value indicating whether the shader is toggled at dusk/dawn.</para>
        /// <para>One of <see cref="IsDayTimeOnly"/> or <see cref="IsNightTimeOnly"/> will be <c>true</c>.</para>
        /// </summary>
        public bool IsToggledByTwilight => (m_profile & Profiles.ToogledByTwilight) == Profiles.ToogledByTwilight;

        /// <summary>
        /// <para>Gets a value indicating whether the shader is on all day _and_ off all night.</para>
        /// <para>Note: This is determined by the <c>DayTime</c> keyword, not on/off times.</para>
        /// </summary>
        public bool IsDayTimeOnly => (m_profile & Profiles.DayOnly) == Profiles.DayOnly;

        /// <summary>
        /// <para>Gets a value indicating whether the shader is on all night _and_ off all day.</para>
        /// <para>Note: This is determined by either the <c>NightTime</c> keyword, or on/off times which occur during twilight.</para>
        /// </summary>
        public bool IsNightTimeOnly => (m_profile & Profiles.NightOnly) == Profiles.NightOnly;

        /// <summary>
        /// Gets a value indicating whether the shader is permanently visible.
        /// </summary>
        public bool IsAlwaysOn => (m_profile & Profiles.OnAlways) == Profiles.OnAlways;

        /// <summary>
        /// Gets a value indicating whether the shader is remotely controlled by other mods.
        /// </summary>
        public bool IsRemotelyControlled => (m_profile & Profiles.RemotelyControlled) == Profiles.RemotelyControlled;

        /// <summary>
        /// <para>Gets a value indicating whether the shader is static (always on, or always off).</para>
        /// <para>Note: If <c>true</c>, and <see cref="IsAlwaysOn"/> is <c>false</c>, it means "always off".</para>
        /// </summary>
        public bool IsStatic => (m_profile & Profiles.Static) == Profiles.Static;

        /// <summary>
        /// Parses fade and intensity data from the provided attribute string.
        /// </summary>
        /// <param name="attributes">Attributes string..</param>
        private void ParseFadeIntensity(string attributes)
        {
            // Skip characters until first space.
            int charIndex = attributes.IndexOf(' ');

            // Next item is fade.
            m_fade = ParseFloat(attributes, ++charIndex, out charIndex);

            // Next (final) item is intensity.
            m_intensity = ParseFloat(attributes, charIndex, out _);
        }

        /// <summary>
        /// Parses a single floating point value from the provided attribute string.
        /// </summary>
        /// <param name="attributes">Attributes string.</param>
        /// <param name="startIndex">Starting character index to parse from.</param>
        /// <param name="endIndex">Character index at end of parse, commencing after trailing space.</param>
        /// <returns>Parsed floating-point value.</returns>
        private float ParseFloat(string attributes, int startIndex, out int endIndex)
        {
            // Starting at the given starting character index, scan forward until a space is detected.
            int charIndex = startIndex;
            while (attributes[charIndex++] != ' ')
            {
                // Keep scanning until a space is encountered, or if we reach the end of the string.
                if (charIndex >= attributes.Length)
                {
                    // Increment character index to emulate finding a space immediately beyond the end of the string.
                    ++charIndex;
                    break;
                }
            }

            // Found the space delimiter; now, record the ending index and parse the float itself.
            endIndex = charIndex;
            return float.Parse(attributes.Substring(startIndex, charIndex - startIndex - 1), CultureInfo.InvariantCulture);
        }
    }
}
