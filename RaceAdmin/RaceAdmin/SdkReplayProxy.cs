﻿using iRacingSdkWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace RaceAdmin
{
    class SdkReplayProxy : ISdkWrapper
    {
        private BinaryReader reader;
        private List<EventHandler<SdkWrapper.SessionInfoUpdatedEventArgs>> sessionInfoUpdateHandlers;
        private List<EventHandler<SdkWrapper.TelemetryUpdatedEventArgs>> telemetryUpdateHandlers;

        public SdkReplayProxy(BinaryReader reader)
        {
            this.reader = reader;
            this.sessionInfoUpdateHandlers = new List<EventHandler<SdkWrapper.SessionInfoUpdatedEventArgs>>();
            this.telemetryUpdateHandlers = new List<EventHandler<SdkWrapper.TelemetryUpdatedEventArgs>>();
        }

        public void AddSessionInfoUpdateHandler(EventHandler<SdkWrapper.SessionInfoUpdatedEventArgs> handler)
        {
            sessionInfoUpdateHandlers.Add(handler);
        }

        public void AddTelemetryUpdateHandler(EventHandler<SdkWrapper.TelemetryUpdatedEventArgs> handler)
        {
            telemetryUpdateHandlers.Add(handler);
        }

        public ITelemetryValue<T> GetTelemetryValue<T>(string name)
        {
            throw new NotImplementedException();
        }

        public void SetTelemetryUpdateFrequency(int updateFrequency)
        {
            // ignored for now
        }

        public void Start(bool record)
        {
            ReplayLoop loop = new ReplayLoop(reader, sessionInfoUpdateHandlers);
            Thread t = new Thread(new ThreadStart(loop.ThreadProc));
            t.IsBackground = true;
            t.Start();
        }

        public void Stop()
        {
            reader.Close(); // probably not threadsafe (well, not threadsafe)
        }
    }

    public class ReplayLoop
    {
        private BinaryReader reader;
        private List<EventHandler<SdkWrapper.SessionInfoUpdatedEventArgs>> sessionInfoUpdateHandlers;

        public ReplayLoop(BinaryReader reader, List<EventHandler<SdkWrapper.SessionInfoUpdatedEventArgs>> sessionInfoUpdateHandlers)
        {
            this.reader = reader;
            this.sessionInfoUpdateHandlers = sessionInfoUpdateHandlers;
        }

        public void ThreadProc()
        {
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                int recordType = reader.ReadInt32();
                switch (recordType)
                {
                    case 1:
                        DoSessionInfoUpdate();
                        break;
                    default:
                        break;
                }
                Thread.Sleep(50);
            }
        }

        private void DoSessionInfoUpdate()
        {
            var timestamp = reader.ReadDouble();
            var yaml = ReadUTF8(reader);
            var args = new SdkWrapper.SessionInfoUpdatedEventArgs(yaml, timestamp);
            foreach(var handler in sessionInfoUpdateHandlers)
            {
                handler.Invoke(this, args);
            }
        }

        private string ReadUTF8(BinaryReader reader)
        {
            byte[] data = ReadByteArray(reader);
            return System.Text.Encoding.UTF8.GetString(data);
        }

        private byte[] ReadByteArray(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            return reader.ReadBytes(length);
        }
    }
}