// <copyright file="PropTransparencyRecord.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace VisibilityControl
{
    using System.Xml.Serialization;
    using static VisibilityControl.Patches.TransparentLODFix;

    /// <summary>
    /// Class to handle prop transparency visibility settings.
    /// </summary>
    public class PropTransparencyRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropTransparencyRecord"/> class.
        /// </summary>
        public PropTransparencyRecord()
        {
        }

        /// <summary>
        /// Gets or sets the fallback prop render distance.
        /// This is used when no other value can be calculated.
        /// </summary>
        [XmlElement("PropFallbackRenderDistance")]
        public float XMLPropFallbackRenderDistance { get => PropFallbackRenderDistance; set => PropFallbackRenderDistance = value; }

        /// <summary>
        /// Gets or sets the prop minimum visibility distance.
        /// Props will always be visible within this distance.
        /// </summary>
        [XmlElement("PropMinimumDistance")]
        public float XMLPropMinimumDistance { get => PropMinimumDistance; set => PropMinimumDistance = value; }

        /// <summary>
        /// Gets or sets the prop distance multiplier.
        /// This determines how far away the prop is visible.
        /// </summary>
        [XmlElement("PropDistanceMultiplier")]
        public float XMLPropDistanceMultiplier { get => PropDistanceMultiplier; set => PropDistanceMultiplier = value; }

        /// <summary>
        /// Gets or sets the prop LOD transition multiplier.
        /// This determines how far away the model will transition from full mesh to LOD.
        /// </summary>
        [XmlElement("PropLODTransitionMultiplier")]
        public float XMLPropLODTransitionMultiplier { get => PropLODTransitionMultiplier; set => PropLODTransitionMultiplier = value; }
    }
}
