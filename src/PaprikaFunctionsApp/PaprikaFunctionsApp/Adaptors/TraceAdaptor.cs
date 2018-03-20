using Microsoft.Azure.WebJobs.Host;
using PaprikaFunctionsApp.Common.Interfaces;
using System;

namespace PaprikaFunctionsApp.Adaptors
{
    class TraceAdaptor : ITraceWriter
    {
        private TraceWriter log;

        public TraceAdaptor(TraceWriter logger)
        {
            log = logger;
        }

        public void Error(string message, Exception ex = null, string source = null)
        {
            log.Error(message, ex, source);
        }

        public void Info(string message, string source = null)
        {
            log.Info(message, source);
        }

        public void Verbose(string message, string source = null)
        {
            log.Verbose(message, source);
        }

        public void Warning(string message, string source = null)
        {
            log.Warning(message, source);
        }
    }
}
