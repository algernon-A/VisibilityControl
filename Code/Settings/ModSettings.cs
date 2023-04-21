// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons.Keybinding;
    using AlgernonCommons.XML;

    /// <summary>
    /// Global mod settings.
    /// </summary>
    [XmlRoot("VisibilityControl")]
    public class ModSettings : SettingsXMLBase
    {
        /// <summary>
        /// Settings file name.
        /// </summary>
        [XmlIgnore]
        private static readonly string SettingsFileName = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "VisibilityControl.xml");

        /// <summary>
        /// Gets or sets building visibility setttings for transparency LOD fixing.
        /// </summary>
        [XmlElement("BuildingTransparency")]
        public BuildingTransparencyRecord BuildingTransparency { get; set; } = new BuildingTransparencyRecord();

        /// <summary>
        /// Gets or sets building visibility setttings for transparency LOD fixing.
        /// </summary>
        [XmlElement("PropTransparency")]
        public PropTransparencyRecord PropTransparency { get; set; } = new PropTransparencyRecord();

        /// <summary>
        /// Gets or sets settings for LOD distance adjustment.
        /// </summary>
        [XmlElement("LodDistance")]
        public LodDistanceRecord LodDistance { get; set; } = new LodDistanceRecord();

        /// <summary>
        /// Gets or sets the LOD mode hotkey.
        /// </summary>
        [XmlElement("LodModeKey")]
        public Keybinding LodModeKey { get => UIThreading.LodModeKey; set => UIThreading.LodModeKey = value; }

        /// <summary>
        /// Loads settings from file.
        /// </summary>
        internal static void Load() => XMLFileUtils.Load<ModSettings>(SettingsFileName);

        /// <summary>
        /// Saves settings to file.
        /// </summary>
        internal static void Save() => XMLFileUtils.Save<ModSettings>(SettingsFileName);
    }
}