namespace SmartHydro_API.Interface
{
    public interface ISensorReadingStore
    {
        void Update(SensorReading reading);
        List<SensorReading> GetAll();
        SensorReading GetByMac(string mac);

        
        

    }
}
