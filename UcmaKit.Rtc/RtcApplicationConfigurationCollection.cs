using System.Collections.Generic;
using System.Configuration;

namespace ISI.Rtc
{

    [ConfigurationCollection(typeof(RtcApplicationConfigurationElement))]
    public class RtcApplicationConfigurationCollection : ConfigurationElementCollection, IEnumerable<RtcApplicationConfigurationElement>
    {

        protected override ConfigurationElement CreateNewElement()
        {
            return new RtcApplicationConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RtcApplicationConfigurationElement)element).ApplicationId;
        }

        public new IEnumerator<RtcApplicationConfigurationElement> GetEnumerator()
        {
            var e = base.GetEnumerator();
            while (e.MoveNext())
                yield return (RtcApplicationConfigurationElement)e.Current;
        }

    }

}
