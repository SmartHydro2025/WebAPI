namespace SmartHydro_API.Interface;
using SmartHydro_API.Models;

public interface ISensorReadingStore
{
    void Update(SensorReading reading);
    List<SensorReading> GetAll();
    SensorReading GetByMac(string mac);

    
    

}
