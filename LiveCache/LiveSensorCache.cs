// In SmartHydro_API/LiveCache/LiveSensorCache.cs
using SmartHydro_API;

namespace SmartHydro_API.LiveCache
{
    public class LiveSensorCache
    {
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
        public SensorReading? GetLatest(string mac)
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
