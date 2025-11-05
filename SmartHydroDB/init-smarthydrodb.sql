USE master;
GO
 
--create the conference database oonly if it does not already exist
 
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'SmartHydroDB')
BEGIN
	CREATE DATABASE SmartHydroDB;
END
GO
 
--Switch to smartHydro database
USE SmartHydroDB;
GO
 
CREATE TABLE AiEvents(
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Mac VARCHAR(255),
    Sensor VARCHAR(255),
    Message VARCHAR(255)
);
GO
 
CREATE TABLE HardwareStatuses(
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Mac VARCHAR(255),
    GrowLightStatus VARCHAR(255),
    ExtractorFanStatus VARCHAR(255),
    CirculationFanStatus VARCHAR(255),
    CirculationPumpStatus VARCHAR(255),
    NutrientPumpStatus VARCHAR(255),
    WaterPumpStatus VARCHAR(255),
    DateTime DATETIME DEFAULT CURRENT_TIMESTAMP
);
GO
 
CREATE TABLE SensorReadings(
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    Mac VARCHAR(255),
    Temperature FLOAT,
    Humidity FLOAT,
    LightLevel FLOAT,
    PhLevel FLOAT,
    EcLevel FLOAT,
    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
);
GO

CREATE TABLE TentInformation(
    ID INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
    tentName VARCHAR(255),
    tentLocation VARCHAR(255),
    Mac VARCHAR(255),
    networkName VARCHAR(255) 
);
GO




