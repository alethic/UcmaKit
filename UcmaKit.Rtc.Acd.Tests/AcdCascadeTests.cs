using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ISI.Rtc.Acd.Tests
{

    [TestClass]
    public class AcdCascadeTests
    {

        static AcdAction group1 = new AcdParallel()
        {
            new AcdDebug("Agent 1"),
        };

        static AcdAction group2 = new AcdParallel()
        {
            new AcdDebug("Agent 2"),
            new AcdDebug("Agent 3"),
        };

        static AcdAction group3 = new AcdParallel()
        {
            new AcdDebug("Agent 4"),
            new AcdDebug("Agent 5"),
            new AcdDebug("Agent 6"),
        };

        /// <summary>
        /// Ring each group in parallel, but with an increasing time before each
        /// </summary>
        static AcdAction cascade = new AcdCascade()
        {
            new AcdCascadeLevel(TimeSpan.FromSeconds(00), group1),
            new AcdCascadeLevel(TimeSpan.FromSeconds(10), group2),
            new AcdCascadeLevel(TimeSpan.FromSeconds(20), group3),
        };

        [TestMethod]
        public void CascadeTest()
        {
            cascade.Execute(null, null, CancellationToken.None).Wait();
        }

    }

}
