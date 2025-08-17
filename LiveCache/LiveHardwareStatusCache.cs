namespace SmartHydro_API.LiveCache
{
    public class LiveHardwareStatusCache
    {
        private readonly Dictionary<string, HardwareReading> _cache = new();
        private readonly object _lock = new();
        public HardwareReading? GetLatest(string mac)
        {
            var normalizedMac = mac.Trim().ToUpper();

            lock (_lock)
            {
                return _cache.TryGetValue(mac, out var reading) ? reading : null;
            }
        }

        public List<HardwareReading> GetAllLatest()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }

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
