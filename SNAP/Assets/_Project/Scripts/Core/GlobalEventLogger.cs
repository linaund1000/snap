using System.Collections.Generic;
using UnityEngine;

namespace GPOyun.Core
{
    /// <summary>
    /// Logs human-readable global town events (Emojis, Greetings, Gossips) for the Journal Feed.
    /// </summary>
    public static class GlobalEventLogger
    {
        private static List<string> _events = new List<string>();

        public static event System.Action<string> OnEventLogged;

        public static void Log(string text)
        {
            string timeStr = System.TimeSpan.FromSeconds(Time.time).ToString(@"mm\:ss");
            _events.Insert(0, $"[{timeStr}] {text}"); // Newest first
            if (_events.Count > 50)
            {
                _events.RemoveAt(_events.Count - 1);
            }
            OnEventLogged?.Invoke(text);
        }

        public static List<string> GetRecentEvents()
        {
            return _events;
        }
    }
}
