using Serilog.Core;
using Serilog.Events;
using System.Diagnostics;

namespace JobScheduling.Extensions
{
    /// <summary>
    /// 客製化 Log 內容
    /// </summary>
    public class LogEnricher : ILogEventEnricher
    {
        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            #region 只留 Controller Name 不要 Namespace
            //if (logEvent.Properties.TryGetValue("SourceContext", out LogEventPropertyValue sourceContext))
            //{
            //    var controllerName = sourceContext.ToString().Replace("\"", "").Split('.').Last().Replace("Controllers", "");
            //    logEvent.AddPropertyIfAbsent(new LogEventProperty("ControllerName", new ScalarValue(controllerName)));
            //} 
            #endregion


            #region 讓log多帶上ClassName、MethodName和LineNumber三個屬性
            if (logEvent.Properties.ContainsKey(Constants.SourceContextPropertyName))
            {
                //增加 ClassName
                var sourceContext = ((ScalarValue)logEvent.Properties[Constants.SourceContextPropertyName]).Value?.ToString() ?? "";
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("ClassName", sourceContext));

                var methodName = "";
                var lineNumber = 0;
                var callerFrame = GetCallerStackFrame(sourceContext);
                if (callerFrame != null)
                {
                    methodName = callerFrame.GetMethod()?.Name;
                    lineNumber = callerFrame.GetFileLineNumber();
                }
                //增加 MethodName
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("MethodName", methodName));
                //增加 LineNumber
                logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty("LineNumber", lineNumber));

            }
            #endregion

        }

        private static StackFrame? GetCallerStackFrame(string className)
        {
            var trace = new StackTrace(true);
            var frames = trace.GetFrames();
            var callerFrame = frames?.FirstOrDefault(f => f.GetMethod()?.DeclaringType?.FullName == className);

            return callerFrame;
        }

    }
}
