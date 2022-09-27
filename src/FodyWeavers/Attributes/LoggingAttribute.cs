﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using Rougamo;
using Rougamo.Context;

namespace FodyWeavers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class LoggingAttribute: MoAttribute
    {
        private readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        public override void OnEntry(MethodContext context)
        {
            Console.WriteLine("执行方法 {0}() 开始, 参数：{1}.", context.Method.Name, JsonConvert.SerializeObject(context.Arguments));
            //context.Data = DateTime.Now;
            //var methodName = GetMethodName(context);
            //_logger.Info("执行方法 {0}() 开始, 参数：{1}.", methodName, JsonConvert.SerializeObject(context.Arguments));
        }

        public override void OnException(MethodContext context)
        {
            //var methodName = GetMethodName(context);
            //_logger.Error(context.Exception, $"执行方法 {methodName}() 异常.");
            Console.WriteLine("执行方法 {0}() 异常, {1}.", context.Method.Name, context.Exception.Message);
        }

        public override void OnExit(MethodContext context)
        {
            //var methodName = GetMethodName(context);
            //var duration = DateTime.Now - (DateTime)context.Data;
            //_logger.Info("执行方法 {0}() 结束, 耗时: {1} ms.", methodName, duration.TotalMilliseconds);
            Console.WriteLine("执行方法 {0}() 结束.", context.Method.Name);
        }

        public override void OnSuccess(MethodContext context)
        {
            //var methodName = GetMethodName(context);
            //_logger.Info("执行方法 {0}() 成功.", methodName);
            Console.WriteLine("执行方法 {0}() 成功.", context.Method.Name);
        }

        private string GetMethodName(MethodContext context) => context.Method.DeclaringType?.FullName + "." + context.Method.Name;
    }
}
