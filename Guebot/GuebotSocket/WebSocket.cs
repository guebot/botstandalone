using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Net.Sockets;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GuebotLib;
using GuebotEntities;
using Newtonsoft.Json;

public class WebSocket
{
    private static UTF8Encoding encoding = new UTF8Encoding();
    private static Bot myBot;

    public static async Task Connect(string uri, Bot bot)
    {
        Thread.Sleep(1000);
        ClientWebSocket webSocket = null;
        myBot = bot;

        try
        {
            webSocket = new ClientWebSocket();
            await webSocket.ConnectAsync(new Uri(uri), CancellationToken.None);
            await Task.WhenAll(Receive(webSocket), Send(webSocket));
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception : {0}", ex.Message);
        }
        finally
        {
            if (webSocket != null)
                webSocket.Dispose();
            Console.WriteLine();
            Console.WriteLine("WebSocket: closed.");
        }
    }

    private static async Task Send(ClientWebSocket webSocket)
    {
        while (webSocket.State == WebSocketState.Open)
        {
            //Enviar estado al server
            JSONStatusEntity json = await myBot.StatusAsync();
            string stringtoSend = JsonConvert.SerializeObject(json);
            byte[] buffer = encoding.GetBytes(stringtoSend);
            await webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, false, CancellationToken.None);
            Console.WriteLine("JSON enviado: {0}", stringtoSend);
            //await Task.Delay(1000);
        }
    }

    private static async Task Receive(ClientWebSocket webSocket)
    {
        byte[] buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                string stringReceived = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                JSONMovementEntity move = JsonConvert.DeserializeObject<JSONMovementEntity>(stringReceived);
                myBot.Movement(move);
                //Enviar comando al robot para moverse
                Console.WriteLine("JSON recibido: {0}", stringReceived);
            }
        }
    }
}

