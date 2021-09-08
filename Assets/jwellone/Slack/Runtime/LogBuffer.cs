using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace jwellone.Slack
{
    [Flags]
    public enum LogFlags
    {
        None = 0x0,
        Log = 0x1 << 0,
        Warning = 0x1 << 1,
        Error = 0x1 << 2,
        Assert = 0x1 << 3,
        Exception = 0x1 << 4,
        StackTrace = 0x1 << 5,
        All = 0xffffff
    };

    public interface ILogBuffer
    {
        bool Enabled { get; set; }
        int Capacity { get; set; }
        LogFlags Flags { get; }
        string Text { get; }
        byte[] Bytes { get; }

        void SetEnableFlg(LogFlags flags);
        void SetDisableFlg(LogFlags flags);
    }

    public class LogBuffer : ILogBuffer
    {
        private static readonly LogFlags[] s_flags = new LogFlags[] { LogFlags.Error, LogFlags.Assert, LogFlags.Warning, LogFlags.Log, LogFlags.Exception };
        private readonly object m_locakObj = new object();
        private bool m_enabled;
        private Queue<string> m_queue;

        public bool Enabled
        {
            get => m_enabled;
            set
            {
                if (m_enabled == value)
                {
                    return;
                }

                m_enabled = value;
                if (m_enabled)
                {
                    Application.logMessageReceivedThreaded += OnLogMessageReceivedThreaded;
                }
                else
                {
                    Application.logMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
                }
            }
        }

        public int Capacity
        {
            get;
            set;
        }

        public LogFlags Flags
        {
            get;
            private set;
        } = LogFlags.All;

        public string Text
        {
            get => GetString();
        }

        public byte[] Bytes
        {
            get => Encoding.UTF8.GetBytes(GetString());
        }

        public LogBuffer(int capacity = 32)
        {
            m_queue = new Queue<string>();
            Capacity = capacity;
            m_enabled = false;
            Enabled = true;
        }

        ~LogBuffer()
        {
            Enabled = false;
        }

        public void SetEnableFlg(LogFlags flags)
		{
            Flags |= flags;
        }

        public void SetDisableFlg(LogFlags flags)
        {
            Flags &= ~flags;
        }

        private string GetString()
        {
            lock (m_locakObj)
            {
                var sb = new StringBuilder();
                foreach (var target in m_queue)
                {
                    sb.AppendLine(target);
                }
                return sb.ToString();
            }
        }

        private void OnLogMessageReceivedThreaded(string logString, string stackTrace, LogType type)
        {
            lock (m_locakObj)
            {
                if ((Flags & s_flags[(int)type]) == 0)
                {
                    return;
                }

                while (m_queue.Count > Capacity)
                {
                    m_queue.Dequeue();
                }

                var sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("[MM/dd HH:mm:ss]"));
                sb.Append(logString);

                if ((Flags & LogFlags.StackTrace) != 0)
                {
                    sb.Append("\n");
                    sb.Append(stackTrace);
                }

                m_queue.Enqueue(sb.ToString());
            }
        }
    }
}
