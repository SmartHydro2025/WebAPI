namespace SmartHydro_API.LiveCache
{
    public class LiveHardwareStatusCache
    {
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
