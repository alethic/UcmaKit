using System;
using System.ComponentModel;
using System.Configuration;

namespace UcmaKit.Rtc
{

    public class RtcApplicationConfigurationElement : ConfigurationElement
    {

        /// <summary>
        /// Gets the implementing type of the platform.
        /// </summary>
        [ConfigurationProperty("type")]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type Type
        {
            get { return (Type)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("autoProvision", DefaultValue = true)]
        public bool AutoProvision
        {
            get { return (bool)this["autoProvision"]; }
            set { this["autoProvision"] = value; }
        }

        /// <summary>
        /// Gets the ApplicationId for the platform.
        /// </summary>
        [ConfigurationProperty("applicationId")]
        public string ApplicationId
        {
            get { return (string)this["applicationId"]; }
            set { this["applicationId"] = value; }
        }

        /// <summary>
        /// Gets the Gruu for the platform. Only applicable when using manual provisioning.
        /// </summary>
        [ConfigurationProperty("gruu")]
        public string Gruu
        {
            get { return (string)this["gruu"]; }
            set { this["gruu"] = value; }
        }

        /// <summary>
        /// Gets the UserAgent for the platform.
        /// </summary>
        [ConfigurationProperty("userAgent")]
        public string UserAgent
        {
            get { return (string)this["userAgent"]; }
            set { this["userAgent"] = value; }
        }

        /// <summary>
        /// Gets the localhost for the platform. Only applicable when using manual provisioning.
        /// </summary>
        [DefaultValue(null)]
        [ConfigurationProperty("localhost", DefaultValue = null)]
        public string Localhost
        {
            get { return (string)this["localhost"]; }
            set { this["localhost"] = value; }
        }

        /// <summary>
        /// Gets the port for the platform. Only applicable when using manual provisioning.
        /// </summary>
        [ConfigurationProperty("port")]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }

        /// <summary>
        /// Gets the certificate thumbprint to use. Only applicable when using manual provisioning.
        /// </summary>
        [ConfigurationProperty("certificateThumbprint")]
        public string CertificateThumbprint
        {
            get { return (string)this["certificateThumbprint"]; }
            set { this["certificateThumbprint"] = value; }
        }

        /// <summary>
        /// Gets the certificate subject name to use. Only applicable when using manual provisioning.
        /// </summary>
        [ConfigurationProperty("certificateSubjectName")]
        public string CertificateSubjectName
        {
            get { return (string)this["certificateSubjectName"]; }
            set { this["certificateSubjectName"] = value; }
        }

        /// <summary>
        /// 
        /// Gets the Registrar. Only applicable when using manual provisioning.
        /// </summary>
        [ConfigurationProperty("registrar")]
        public string Registrar
        {
            get { return (string)this["registrar"]; }
            set { this["registrar"] = value; }
        }

        /// <summary>
        /// Gets the port of the Registrar. Only applicable when using manual provisioning.
        /// </summary>
        [ConfigurationProperty("registrarPort")]
        public int RegistrarPort
        {
            get { return (int)this["registrarPort"]; }
            set { this["registrarPort"] = value; }
        }

        /// <summary>
        /// Endpoints to be configured.
        /// </summary>
        [ConfigurationProperty("endpoints")]
        public RtcEndpointConfigurationCollection Endpoints
        {
            get { return (RtcEndpointConfigurationCollection)this["endpoints"]; }
        }

    }

}
