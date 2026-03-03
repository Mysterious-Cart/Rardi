using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using CHKS.Entity;

namespace CHKS.Services;


/// <summary>
/// This service manages the connection to the InventoryNotificationHub and provides methods to send and receive messages related to inventory changes.
/// It allows components to subscribe to inventory change notifications and ensures proper connection management, including starting, stopping, and disposing of the hub connection.
/// </summary>
public class InventoryNotificationHubConnectionService : IAsyncDisposable
{

    private readonly NavigationManager navigationManager;

    HubConnection _hubConnection;
    public event Action<StockLogs> OnProductChanged;

    public InventoryNotificationHubConnectionService(NavigationManager navigationManager)
    {
        this.navigationManager = navigationManager;
    }

    public async Task StartConnection()
    {

        if (_hubConnection != null) return;

        var url = navigationManager.ToAbsoluteUri("/inventorylogs").ToString();

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();

        _hubConnection.On<StockLogs>("ReceiveMessage", message => OnProductChanged?.Invoke(message));

        await _hubConnection.StartAsync();
    }


    public async Task SendChangesLog(StockLogs logs)
    {
        if (_hubConnection == null) return;

        await _hubConnection.SendAsync("SendMessage", logs);
    }

    public async Task StopConnection()
    {
        if (_hubConnection == null) return;

        await _hubConnection.StopAsync();
        await _hubConnection.DisposeAsync();
        _hubConnection = null;
    }

    public async ValueTask DisposeAsync()
    {
        GC.SuppressFinalize(this);
        await StopConnection();
    }
}