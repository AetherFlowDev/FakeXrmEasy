using NUnit.Framework;

namespace AetherFlow.FakeXrmEasy.Plugins.Tests.FakeContextTests
{
    public class FakeTracingServiceTests
    {
        [Test]
        public void When_a_trace_is_dumped_it_should_return_right_traces()
        {
            var tracingService = new XrmFakedTracingService();

            var trace1 = "This is one trace";
            var trace2 = "This is a second trace";

            tracingService.Trace(trace1);
            tracingService.Trace(trace2);

            Assert.AreEqual(tracingService.DumpTrace(), trace1 + "\r\n" + trace2 + "\r\n");
        }
    }
}