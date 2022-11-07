using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace OrderService.Extensions
{
    public class MyDiagnosticObserver : IObserver<KeyValuePair<string, object>>
    {
        private readonly ILogger<MyDiagnosticObserver> _logger;
        public MyDiagnosticObserver(ILogger<MyDiagnosticObserver> logger)
        {
            _logger = logger;
        }

        public void OnCompleted()
        {

        }

        public void OnError(Exception error)
        {
            _logger.LogError(error.Message);
        }
        public void OnNext(KeyValuePair<string, object> pair)
        {
            _logger.LogInformation($"{pair.Key}, {pair.Value}");
        }
    }
}
