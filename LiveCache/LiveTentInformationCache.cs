namespace SmartHydro_API.LiveCache
{
    public class LiveTentInformationCache
    {
        private readonly Dictionary<string, TentInformation> _cache = new();
        private readonly object _lock = new();

        public void Update(TentInformation reading)
        {
            if (string.IsNullOrWhiteSpace(reading.Mac)) return;

            var normalizedMac = reading.Mac.Trim().ToUpper();

            lock (_lock)
            {
                _cache[reading.Mac] = reading;
            }
        }

        public List<TentInformation> GetAll()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }
    }
}
