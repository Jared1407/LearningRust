use std::net::UdpSocket;
use std::{thread, time::Duration};
use rand::Rng;

fn main() {
    let socket = UdpSocket::bind("127.0.0.1:0").unwrap();
    let target = "127.0.0.1:8080";
    println!("Drone Transmitting Complex Telemetry...");

    // Simulated Drone State
    let mut roll = 0.0f32; let mut pitch = 0.0f32; let mut yaw = 0.0f32;
    let mut alt = 100.0f32; let mut bat = 16.8f32; 
    let mut rng = rand::thread_rng();
    let mut n: f32;

    loop {
        n = rng.gen_range(-0.5..0.5);
        // Simulate some flight movement
        roll = (roll + 0.5) % 45.0; 
        pitch = (pitch + 0.2) % 30.0;
        yaw = (yaw + 1.0) % 360.0;
        alt += 0.1 * n;
        bat -= 0.001; // Battery draining

        // Pack 5 floats (20 bytes total)
        let mut buffer = Vec::new();
        buffer.extend_from_slice(&roll.to_le_bytes());
        buffer.extend_from_slice(&pitch.to_le_bytes());
        buffer.extend_from_slice(&yaw.to_le_bytes());
        buffer.extend_from_slice(&alt.to_le_bytes());
        buffer.extend_from_slice(&bat.to_le_bytes());

        socket.send_to(&buffer, target).unwrap();
        thread::sleep(Duration::from_millis(100)); // 10Hz updates
    }
}