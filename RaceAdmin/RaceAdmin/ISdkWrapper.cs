﻿using iRacingSdkWrapper;
using System;

namespace RaceAdmin
{
    /// <summary>
    /// ISdkWrapper allows us to inject test implementations of the iRacingSdkWrapper.
    /// </summary>
    public interface ISdkWrapper
    {
        void AddSessionInfoUpdateHandler(EventHandler<SdkWrapper.SessionInfoUpdatedEventArgs> handler);

        void AddTelemetryUpdateHandler(EventHandler<ITelemetryUpdatedEvent> handler);

        void SetTelemetryUpdateFrequency(int updateFrequency);

        void Start();

        void Stop();

        ITelemetryValue<T> GetTelemetryValue<T>(string name);
    }
}
