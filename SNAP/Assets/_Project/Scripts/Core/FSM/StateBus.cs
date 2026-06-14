using System.Collections.Generic;
using UnityEngine;

namespace GPOyun.Core.FSM
{
    /// <summary>
    /// Central nervous system for all UI and System Mini Machines.
    /// Routes events (like Global_ForceClose) to all registered machines.
    /// </summary>
    public static class StateBus
    {
        private static List<IMiniStateMachine> _machines = new List<IMiniStateMachine>();

        public static void Register(IMiniStateMachine machine)
        {
            if (!_machines.Contains(machine))
            {
                _machines.Add(machine);
                Debug.Log($"[StateBus] Registered Machine: {machine.MachineName}");
            }
        }

        public static void Unregister(IMiniStateMachine machine)
        {
            _machines.Remove(machine);
        }

        public static void Emit(string eventName)
        {
            Debug.Log($"[StateBus] Emitting Event: {eventName}");
            // Create a copy to prevent collection modification during enumeration
            var targets = new List<IMiniStateMachine>(_machines);
            foreach (var m in targets)
            {
                m.OnEvent(eventName);
            }
        }
    }
}
