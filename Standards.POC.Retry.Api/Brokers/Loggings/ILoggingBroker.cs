using System;

namespace Standards.POC.Retry.Api.Brokers.Loggings
{
    public interface ILoggingBroker
    {
        void LogInformation(string message);
    }
}
