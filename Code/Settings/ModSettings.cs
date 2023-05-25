// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace MoreCitizenUnits
{
    using System.IO;
    using System.Xml.Serialization;
    using AlgernonCommons.XML;

    /// <summary>
    /// Global mod settings.
    /// </summary>
    [XmlRoot("MoreCitizenUnits")]
    public class ModSettings : SettingsXMLBase
    {
        // Settings file name.
        [XmlIgnore]
        private static readonly string SettingsFileName = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "MoreCitizenUnits.xml");

        /// <summary>
        /// Gets or sets a value indicating whether CitizenUnit limit doubling is enabled.
        /// </summary>
        [XmlElement("DoubleUnits")]
        public bool XMLDoubleUnits { get => CitizenDeserialize.DoubleLimit; set => CitizenDeserialize.DoubleLimit = value; }

        /// <summary>
        /// Gets or sets a value indicating whether invalid units should be checked for and fixed on load.
        /// </summary>
        [XmlElement("CheckUnits")]
        public bool XMLCheckUnits { get => CitizenDeserialize.s_checkUnits; set => CitizenDeserialize.s_checkUnits = value; }

        /// <summary>
        /// Gets or sets a value indicating whether to perform a full reset of the CitizenUnit array on load.
        /// </summary>
        [XmlElement("ResetUnits")]
        public bool XMLResetUnits { get => LoadLevelComplete.NukeAll; set => LoadLevelComplete.NukeAll = value; }

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