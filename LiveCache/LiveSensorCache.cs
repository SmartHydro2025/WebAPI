// In SmartHydro_API/LiveCache/LiveSensorCache.cs
using SmartHydro_API;

namespace SmartHydro_API.LiveCache
{
    public class LiveSensorCache
    {
        private readonly Dictionary<string, SensorReading> _cache = new();
        private readonly object _lock = new();

        public void Update(SensorReading reading)
        {
            if (string.IsNullOrWhiteSpace(reading.Mac)) return;

            var normalizedMac = reading.Mac.Trim().ToUpper();

            lock (_lock)
            {
                _cache[reading.Mac] = reading;
            }
        }

        public SensorReading? GetLatest(string mac)
        {
            var normalizedMac = mac.Trim().ToUpper();

            lock (_lock)
            {
                return _cache.TryGetValue(mac, out var reading) ? reading : null;
            }
        }

        public List<SensorReading> GetAllLatest()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }
    }
}
