﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace TriangleCollector.Models
{
    public class WebSocketAdapter : IClientWebSocket
    {
        private readonly ILogger<WebSocketAdapter> _logger;
        private readonly ClientWebSocket _client;

        public WebSocketAdapter(ILogger<WebSocketAdapter> logger, ClientWebSocket client)
        {
            _logger = logger;
            _client = client;
        }

        public WebSocketState State => _client.State;

        public async Task<WebSocketReceiveResult> ReceiveAsync(MemoryStream ms, ArraySegment<byte> buffer, CancellationToken cancellationToken)
        {
            WebSocketReceiveResult result = null;

            do
            {
                try
                {
                    result = await _client.ReceiveAsync(buffer, CancellationToken.None);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                catch (WebSocketException ex)
                {
                    _logger.LogError(ex.Message);
                    _logger.LogError(ex.InnerException.Message);
                }
            }
            while (!result.EndOfMessage && !cancellationToken.IsCancellationRequested);
            ms.Seek(0, SeekOrigin.Begin);
            return result;
        }

        public async Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType type, bool endOfMessage, CancellationToken cancellationToken)
        {
            await _client.SendAsync(buffer, type, endOfMessage, cancellationToken);
        }
    }
}