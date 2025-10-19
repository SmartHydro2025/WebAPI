namespace SmartHydro_API.LiveCache
{
    public class LiveImageCache
    {

        private readonly Dictionary<string, CameraImage> _cache = new();
        private readonly object _lock = new();

        public void Update(CameraImage image)
        {
            if (string.IsNullOrWhiteSpace(image.Mac)) return;

            var normalizedMac = image.Mac.Trim().ToUpper();

            lock (_lock)
            {
                _cache[normalizedMac] = image;
            }
        }

        public CameraImage? GetLatest(string mac)
        {
            var normalizedMac = mac.Trim().ToUpper();

            lock (_lock)
            {
                return _cache.TryGetValue(normalizedMac, out var image) ? image : null;
            }
        }
    }
}