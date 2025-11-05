using SmartHydro_API.Models;

namespace SmartHydro_API.LiveCache
{
    public class LiveTentInformationCache
    {
        //(Witt, 2023) thread-safe collections ensure that data access is synchronized to prevent data inconsistencies.
        private readonly Dictionary<string, TentInformation> _cache = new();
        private readonly object _lock = new();

        // Updates or adds tent information to the cache.
        public void Update(TentInformation reading)
        {
            if (string.IsNullOrWhiteSpace(reading.Mac)) return;

            var normalizedMac = reading.Mac.Trim().ToUpper();

            //(Witt, 2023)
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

/*
REFERENCES
=====================
Witt, B. 2023. Thread Safety C#. [Online]. Available at:  https://medium.com/@wgyxxbf/thread-security-in-c-547e5f7cfe2b

 */
