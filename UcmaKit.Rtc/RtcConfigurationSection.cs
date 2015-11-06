using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace UcmaKit.Rtc
{

    public class RtcConfigurationSection : ConfigurationSection
    {

        /// <summary>
        /// Default section name for ISI RTC platform.
        /// </summary>
        const string DEFAULT_SECTION_NAME = "isi.rtc";

        /// <summary>
        /// Gets the specified platform configuration section.
        /// </summary>
        /// <param name="configurationSectionName"></param>
        /// <returns></returns>
        public static RtcConfigurationSection GetSection(string configurationSectionName)
        {
            return (RtcConfigurationSection)ConfigurationManager.GetSection(configurationSectionName);
        }

        /// <summary>
        /// Gets the default platform configuration.
        /// </summary>
        public static RtcConfigurationSection Default
        {
            get { return GetSection(DEFAULT_SECTION_NAME); }
        }

        /// <summary>
        /// Applications to be configured.
        /// </summary>
        [ConfigurationProperty("applications")]
        public RtcApplicationConfigurationCollection Applications
        {
            get { return (RtcApplicationConfigurationCollection)this["applications"]; }
        }

    }

}
