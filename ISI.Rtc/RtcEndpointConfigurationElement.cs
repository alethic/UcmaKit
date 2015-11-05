using System.Configuration;

namespace ISI.Rtc
{

    public class RtcEndpointConfigurationElement : ConfigurationElement
    {

        [ConfigurationProperty("uri")]
        public string Uri
        {
            get { return (string)this["uri"]; }
            set { this["uri"] = value; }
        }

        [ConfigurationProperty("phoneUri")]
        public string PhoneUri
        {
            get { return (string)this["phoneUri"]; }
            set { this["phoneUri"] = value; }
        }

        [ConfigurationProperty("isDefaultRoutingEndpoint", DefaultValue = true)]
        public bool IsDefaultRoutingEndpoint
        {
            get { return (bool)this["isDefaultRoutingEndpoint"]; }
            set { this["isDefaultRoutingEndpoint"] = value;}
        }

    }

}
