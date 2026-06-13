using System;
using System.Collections.Generic;

namespace GPOyun.Core
{
    /// <summary>
    /// A simple Inversion of Control (IoC) Container.
    /// Replaces tight-coupled Singletons to enable isolated Unit Testing.
    /// </summary>
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            if (_services.ContainsKey(typeof(T)))
            {
                UnityEngine.Debug.LogWarning($"[ServiceLocator] Service {typeof(T).Name} is already registered. Overwriting.");
            }
            _services[typeof(T)] = service;
        }

        public static T Get<T>()
        {
            if (_services.TryGetValue(typeof(T), out object service))
            {
                return (T)service;
            }
            UnityEngine.Debug.LogError($"[ServiceLocator] Service {typeof(T).Name} not found! Did you forget to register it?");
            return default;
        }

        public static void Unregister<T>()
        {
            if (_services.ContainsKey(typeof(T)))
            {
                _services.Remove(typeof(T));
            }
        }

        public static void ClearAll()
        {
            _services.Clear();
        }
    }
}
