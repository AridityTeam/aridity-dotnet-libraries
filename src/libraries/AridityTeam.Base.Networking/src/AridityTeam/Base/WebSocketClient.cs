using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AridityTeam.Base;

public class WebSocketClient
{
    private readonly ClientWebSocket _clientWebSocket = new();

    public async Task ConnectAsync(string uri)
    {
        await _clientWebSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
    }

    public async Task SendMessageAsync(string message)
    {
        var bytes = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<byte>(bytes);
        await _clientWebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async Task<string?> ReceiveMessageAsync()
    {
        var buffer = new ArraySegment<byte>(new byte[1024]);
        if (buffer.Array == null) return null;
        var result = await _clientWebSocket.ReceiveAsync(buffer, CancellationToken.None);
        return Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
    }

    public async Task DisconnectAsync()
    {
        await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
    }
}