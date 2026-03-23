using System.Net.Sockets;
using Microsoft.AspNetCore.SignalR;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles();

app.MapHub<TelemetryHub>("/telemetryHub");
app.MapGet("/", () => Results.File(Path.Combine(app.Environment.ContentRootPath, "wwwroot/page.html"), "text/html"));


var droneState = new DroneTelemetry();

Task.Run(async () =>
{
    using var udp = new UdpClient(8080);
    Console.WriteLine("UDP Listener active on port 8080...");

    var hubContext = app.Services.GetRequiredService<IHubContext<TelemetryHub>>();

    while (true)
    {
        var result = await udp.ReceiveAsync();
        if (result.Buffer.Length == 20) 
        {
            droneState.Roll = BitConverter.ToSingle(result.Buffer, 0);
            droneState.Pitch = BitConverter.ToSingle(result.Buffer, 4);
            droneState.Yaw = BitConverter.ToSingle(result.Buffer, 8);
            droneState.Altitude = BitConverter.ToSingle(result.Buffer, 12);
            droneState.Battery = BitConverter.ToSingle(result.Buffer, 16);

            await hubContext.Clients.All.SendAsync("ReceiveTelemetry", droneState);

        }
    }
});

app.MapGet("/api/data", () => droneState);


app.Run("http://localhost:5000");

class DroneTelemetry
{
    public float Roll { get; set; }
    public float Pitch { get; set; }
    public float Yaw { get; set; }
    public float Altitude { get; set; }
    public float Battery { get; set; }
}

class TelemetryHub : Hub { }

