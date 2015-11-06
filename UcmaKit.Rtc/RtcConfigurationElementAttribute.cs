using System;

namespace UcmaKit.Rtc
{

    /// <summary>
    /// Indicates the configuration element type to instantiate and parse configuration for the decorated type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RtcConfigurationElementAttribute : Attribute
    {

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        public RtcConfigurationElementAttribute(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="type"></param>
        public RtcConfigurationElementAttribute(Type type)
            : this(type, null)
        {

        }

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="name"></param>
        public RtcConfigurationElementAttribute(string name)
            : this(null, name)
        {

        }

        /// <summary>
        /// Type of the <see cref="ConfigurationElement"/> to create.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Element name to map to configuration element type.
        /// </summary>
        public string Name { get; set; }

    }

}
