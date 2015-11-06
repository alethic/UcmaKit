using System.Collections.Generic;
using System.Configuration;

namespace UcmaKit.Rtc
{

    [ConfigurationCollection(typeof(RtcEndpointConfigurationElement))]
    public class RtcEndpointConfigurationCollection : ConfigurationElementCollection, IEnumerable<RtcEndpointConfigurationElement>
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new RtcEndpointConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RtcEndpointConfigurationElement)element).Uri;
        }

        public new IEnumerator<RtcEndpointConfigurationElement> GetEnumerator()
        {
            var e = base.GetEnumerator();
            while (e.MoveNext())
                yield return (RtcEndpointConfigurationElement)e.Current;
        }

    }

}
