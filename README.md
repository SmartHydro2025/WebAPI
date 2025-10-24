# 🌿 Smart Hydro Web API 🌿
---
### Disclamier: Some code has been used from previous SMart Hydro Group. Smart Hydro 2025 has decide to adapt it and write our own code to make developers' lives easier.

## What Smart Hydro 2025 Web API is built on.

1.  **Receiving Data**:
---

The Web API collects the sensor readings, hardware statuses and AI events that come FROM the hydroponics tent.

2.  **Sending Data**:
   ---

   The web api then sends commands TO the hydroponics to turn the different components (light 💡, pumps ⛽ and fans ❄️) on or off.


## How it works

1. **Android Device**: A hydroponics tent (that has the ESP32) reads its sensors (temp, pH, etc.) or checks its hardware status (light is "ON").

2. **Publish to MQTT**: The device packages this data into a JSON string and publishes it to a specific MQTT topic (like sensor_readings or hardware_status).

3. **Receive from MQTT**: The ```MqttService.cs``` is always running in the background, subscribed to those topics. It instantly receives the JSON message.

4. **Process**: The MqttService's ```OnMessageReceived``` method figures out which topic the message came from (e.g., sensor_readings).

5. **Cache (to obtaine live data)**: The service immediately updates an in-memory methods like ```LiveSensorCache.cs``` or ```LiveHardwareStatusCache.cs```. This cache holds only the latest data from each tent, making it extremely fast to read.

6. **Database**: After updating the cache, the service also saves the data to the Database ```SmartHydroDbContext.cs```. This builds a complete history of all readings.

7. **Android Device** : When a user's app wants to see the current temperature, it calls a Controller endpoint. This controller reads directly from the super-fast Live Cache and returns the value instantly.

## Why both Cache and Database

### Cache
The cache provides speed and responsiveness for your application. It acts as a super-fast, in-memory (RAM) "whiteboard" that holds only the latest data from each device. When a user wants to see the current temperature or hardware status, the API controllers read this live data instantly from the cache instead of running a slower, more complex query on the database. This gives the app a real-time feel and dramatically reduces the workload on the database, preventing bottlenecks.


### Database
The database serves as the system's permanent, long-term memory (persistence). While the cache only knows what's happening right now, the database is the complete historical "filing cabinet" that safely stores every single reading and event over time. This historical record is essential for persistence (surviving an API restart) and for powerful features like viewing data graphs, analyzing trends, and running reports, which a volatile, live-only cache cannot do.
