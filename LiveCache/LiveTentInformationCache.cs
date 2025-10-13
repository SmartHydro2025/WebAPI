namespace SmartHydro_API.LiveCache
{
    public class LiveTentInformationCache
    {
        private readonly Dictionary<string, TentInformation> _cache = new();
        private readonly object _lock = new();

        // Updates or adds tent information to the cache.
        public void Update(TentInformation reading)
        {
            if (string.IsNullOrWhiteSpace(reading.Mac)) return;

            var normalizedMac = reading.Mac.Trim().ToUpper();

            lock (_lock)
            {
                _cache[reading.Mac] = reading;
            }
        }

        // Returns a list of all tent information objects in the cache.
        public List<TentInformation> GetAll()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }
    }
}
