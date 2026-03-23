# Real-Time Drone Telemetry Pipeline

A full-stack, cross-language simulation demonstrating real-time hardware telemetry transmission from a bare-metal flight controller to a web-based Ground Control Station (GCS). 

This project simulates a high-frequency UDP data stream (representing physical sensors) being packed in Rust, transmitted over a network, and consumed asynchronously by a C# .NET Core backend, which then pushes live updates to a JavaScript frontend via WebSockets.

## 🏗️ System Architecture

This project is split into two distinct environments to mimic a real-world aerospace pipeline:

1. **The Flight Controller (Rust):** - Simulates a bare-metal microcontroller reading IMU and battery sensors.
   - Packs floating-point physics data (Roll, Pitch, Yaw, Altitude, Battery) into raw Little-Endian byte arrays.
   - Broadcasts the packed 20-byte payload over a UDP socket at 10Hz to minimize network overhead.
   - *Language/Tools:* Rust, `std::net::UdpSocket`, `rand`

2. **The Ground Control Station (C# & JS):**
   - An ASP.NET Core Web API runs a background `async` task to catch incoming UDP packets without blocking the main execution thread.
   - Safely parses and unpacks the raw binary bytes back into usable `float` values.
   - Uses **SignalR (WebSockets)** to instantly push the deserialized telemetry to connected web clients.
   - The frontend implements a **Watchdog Timer** failsafe, which visually alerts the user if the hardware connection is interrupted or goes silent.
   - *Language/Tools:* C# 11, .NET Core Minimal APIs, SignalR, HTML/CSS/JS

## 🚀 How to Run the Simulation

To run this project locally, you will need two separate terminal windows.

### 1. Start the Ground Control Station (C#)
Navigate to the C# project directory and start the web server:
```bash
cd GroundStationWeb
dotnet run
```

### 2. Start the Drone Flight Controller (Rust) ###
Open a second terminal, navigate to the Rust project directory, and start the telemetry broadcast:
```bash
cd drone_sim
cargo run
```
### 3. View the Dashboard ###
Open your web browser and navigate to ```http://localhost:5000```. You will see the WebSocket connection establish and live telemetry data begin streaming immediately.

If you terminate the Rust application (```Ctrl+C```), the frontend watchdog timer will trigger within 1000ms and display a critical connection loss warning.