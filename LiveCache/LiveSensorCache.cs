using SmartHydro_API.Models;
using SmartHydro_API;

namespace SmartHydro_API.LiveCache
{
    public class LiveSensorCache
    {
        //(Witt, 2023) thread-safe collections ensure that data access is synchronized to prevent data inconsistencies.
        // Dictionary to store readings, keyed by MAC address.
        private readonly Dictionary<string, SensorReading> _cache = new();
        private readonly object _lock = new();

        // Updates or adds a sensor reading in the cache.
        public void Update(SensorReading reading)
        {
            if (string.IsNullOrWhiteSpace(reading.Mac)) return;

            var normalizedMac = reading.Mac.Trim().ToUpper();

            lock (_lock)
            {
                _cache[reading.Mac] = reading;
            }
        }

        // Gets the latest reading for a specific MAC address.
        public virtual SensorReading? GetLatest(string mac)
        {
            var normalizedMac = mac.Trim().ToUpper();

            lock (_lock)
            {
                return _cache.TryGetValue(mac, out var reading) ? reading : null;
            }
        }

        // Returns a list of the latest readings from all devices.
        public List<SensorReading> GetAllLatest()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }
    }
}

/*
REFERENCES
=====================
Witt, B. 2023. Thread Safety C#. [Online]. Available at:  https://medium.com/@wgyxxbf/thread-security-in-c-547e5f7cfe2b

 */
