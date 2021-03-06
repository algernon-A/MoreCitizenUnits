using System;
using System.IO;
using System.Xml.Serialization;


namespace MoreCitizenUnits
{
    /// <summary>
    /// Global mod settings.
    /// </summary>
	[XmlRoot("MoreCitizenUnits")]
    public class ModSettings
    {
        // Settings file name.
        [XmlIgnore]
        private static readonly string SettingsFileName = Path.Combine(ColossalFramework.IO.DataLocation.localApplicationData, "MoreCitizenUnits.xml");

        // Perform a full reset of the CitizenUnit array on load.
        [XmlIgnore]
        internal static bool nukeAll = false;


        // Language.
        [XmlElement("Language")]
        public string Language
        {
            get => Translations.CurrentLanguage;

            set => Translations.CurrentLanguage = value;
        }

        [XmlElement("DoubleUnits")]
        public bool XMLDoubleUnits { get => CitizenDeserialze.DoubleLimit; set => CitizenDeserialze.DoubleLimit = value; }

        [XmlElement("CheckUnits")]
        public bool XMLCheckUnits { get => CitizenDeserialze.checkUnits; set => CitizenDeserialze.checkUnits = value; }

        [XmlElement("ResetUnits")]
        public bool XMLResetUnits { get => nukeAll; set => nukeAll = value; }


        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void Load()
        {
            try
            {
                // Check to see if configuration file exists.
                if (File.Exists(SettingsFileName))
                {
                    // Read it.
                    using (StreamReader reader = new StreamReader(SettingsFileName))
                    {
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                        if (!(xmlSerializer.Deserialize(reader) is ModSettings settingsFile))
                        {
                            Logging.Error("couldn't deserialize settings file");
                        }
                    }
                }
                else
                {
                    Logging.Message("no settings file found");
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception reading XML settings file");
            }
        }


        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void Save()
        {
            try
            {
                // Pretty straightforward.  Serialisation is within GBRSettingsFile class.
                using (StreamWriter writer = new StreamWriter(SettingsFileName))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(ModSettings));
                    xmlSerializer.Serialize(writer, new ModSettings());
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception saving XML settings file");
            }
        }
    }
}