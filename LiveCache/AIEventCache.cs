namespace SmartHydro_API.LiveCache
{
    public class AIEventCache
    {

        //Stores the AI events, MAC address is the key
        private readonly Dictionary<string, AiEvent> _cache = new();
        //Thread safety
        private readonly object _lock = new();

        //Constantly updates the cache of the live data from the board. 
        public void Update(AiEvent events)
        {
           //Ignores any events that have no MAC address
            if (string.IsNullOrWhiteSpace(events.Mac))
            {
                return;
            }

            // Normalize the MAC address for consistent keying.
            var normalizedMac = events.Mac.Trim().ToUpper();

            //prevent concurrent access issues
            lock (_lock)
            {
                _cache[events.Mac] = events;
            }
        }

        // Retrieves the latest event for a specific MAC address.
        public AiEvent? GetLatest(string mac)
        {
            var normalizedMac = mac.Trim().ToUpper();

            lock (_lock)
            {
                return _cache.TryGetValue(mac, out var reading) ? reading : null;
            }
        }

        // Returns a list of the latest events from all devices.
        public List<AiEvent> GetAllLatest()
        {
            lock (_lock)
            {
                return _cache.Values.ToList();
            }
        }
    }

}
