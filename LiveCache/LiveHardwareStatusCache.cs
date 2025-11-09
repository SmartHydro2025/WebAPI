using SmartHydro_API.Models;

namespace SmartHydro_API.LiveCache
{
    public class LiveHardwareStatusCache
    {
        //(Witt, 2023) thread-safe collections ensure that data access is synchronized to prevent data inconsistencies.
        private readonly Dictionary<string, HardwareReading> _cache = new();
        private readonly object _lock = new();

        // Gets the latest hardware status for a specific MAC address.
        public HardwareReading? GetLatest(string mac)
        {
            var normalizedMac = mac.Trim().ToUpper();

            lock (_lock)
            {
                return _cache.TryGetValue(mac, out var reading) ? reading : null;
            }
        }

        // Returns a list of the latest hardware statuses from all devices.
        public List<HardwareReading> GetAllLatest()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }

        // Updates or adds a hardware status reading in the cache.
        public void Update(HardwareReading reading)
        {
            if (string.IsNullOrWhiteSpace(reading.Mac)) return;

            var normalizedMac = reading.Mac.Trim().ToUpper();

            lock (_lock)
            {
                _cache[reading.Mac] = reading;
            }
        }
    }
}

/*
REFERENCES
=====================
Witt, B. 2023. Thread Safety C#. [Online]. Available at:  https://medium.com/@wgyxxbf/thread-security-in-c-547e5f7cfe2b

 */
