using NLog;
using NLog.LayoutRenderers;
using System.Diagnostics;
using System.Text;

namespace OrderService.Extensions
{
    [LayoutRenderer("my-trace")]
    public class MyTraceLayoutRenderer : LayoutRenderer
    {
        protected override void Append(StringBuilder builder, LogEventInfo logEvent)
        {
            if (Activity.Current != null)
            {
                builder.Append(Activity.Current.TraceId.ToString());
            }
            else
            {
                var traceId = CallContext.GetData<string>("traceid");
                builder.Append(traceId);
            }
        }
    }
}
