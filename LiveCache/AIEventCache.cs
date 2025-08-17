namespace SmartHydro_API.LiveCache
{
    public class AIEventCache
    {
        private readonly Dictionary<string, AiEvent> _cache = new();
        private readonly object _lock = new();

        public void Update(AiEvent events)
        {
            if (string.IsNullOrWhiteSpace(events.Mac)) return;

            var normalizedMac = events.Mac.Trim().ToUpper();

            lock (_lock)
            {
                _cache[events.Mac] = events;
            }
        }

        public AiEvent? GetLatest(string mac)
        {
            var normalizedMac = mac.Trim().ToUpper();

            lock (_lock)
            {
                return _cache.TryGetValue(mac, out var reading) ? reading : null;
            }
        }

        public List<AiEvent> GetAllLatest()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }
    }

}
