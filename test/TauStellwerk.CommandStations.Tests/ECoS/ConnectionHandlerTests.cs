// <copyright file="ConnectionHandlerTests.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Net;
using System.Net.Sockets;
using System.Text;
using FluentAssertions;
using FluentResults.Extensions.FluentAssertions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using TauStellwerk.CommandStations;
using TauStellwerk.CommandStations.ECoS;

namespace TauStellwerk.Test.ECoS;

public class ConnectionHandlerTests
{
    private const int Port = 16547;

    private static readonly IPAddress _ip = IPAddress.Loopback;

    private TestTcpListener _tcpListener = null!; // Created in the SetUp

    private ECosConnectionHandler _connectionHandler = null!; // Created in the SetUp

    [SetUp]
    public async Task SetUp()
    {
        var logger = Substitute.For<ILogger<CommandStationBase>>();
        _tcpListener = new TestTcpListener();
        var listenerTask = _tcpListener.Start();
        _connectionHandler = new ECosConnectionHandler(_ip, Port, logger);
        await listenerTask;
    }

    [TearDown]
    public void Teardown()
    {
        _tcpListener.Stop();
    }

    [Test]
    public async Task SendCommandTest()
    {
        const string command = "set(1016,func[0,1])";
        const string response = "<REPLY set(1016,func[0,1])>\r\n<END 0 (OK)>\r\n";

        var task = _connectionHandler.SendCommandAsync(command);
        await _tcpListener.Send(response);
        var result = await task;
        await Task.Delay(10);
        var receivedCommand = Encoding.UTF8.GetString(_tcpListener.Data.ToArray());

        result.Should().BeSuccess();
        result.Value.ErrorCode.Should().Be(0);
        receivedCommand.Should().BeEquivalentTo(command);
    }

    [Test]
    public async Task ResponseLineSplitTest()
    {
        var task = _connectionHandler.SendCommandAsync("set(1016,func[0,1])");
        await _tcpListener.Send("<REPLY set(1016,func[0,1])>\r\n");
        await _tcpListener.Send("ä\r\n");
        await _tcpListener.Send("<END 0 (OK)>\r\n");
        var result = await task;

        result.Should().BeSuccess();
        var response = result.Value;
        response.Command.Should().Be("set(1016,func[0,1])");
        response.Content.Should().Be("ä");
        response.ErrorCode.Should().Be(0);
        response.ErrorMessage.Should().Be("(OK)");
    }

    [Test]
    public async Task ResponseSplitCharacterTest()
    {
        var task = _connectionHandler.SendCommandAsync("set(1,1)");
        await _tcpListener.Send("<REPLY set(1,1)>\r\n");
        await _tcpListener.Send(new byte[] { 0b110_00011 });
        await Task.Delay(50); // Ensure the incomplete byte was actually read.
        await _tcpListener.Send(new byte[] { 0b101_00100, 0b0000_1101, 0b0000_1010 });
        await _tcpListener.Send("<END 0 (OK)>\r\n");
        var result = await task;

        result.Value.Content.Should().Be("ä");
    }

    [Test]
    public async Task ResponseSplitLineEndTest()
    {
        var task = _connectionHandler.SendCommandAsync("set(1016,func[0,1])");
        await _tcpListener.Send("<REPLY set(1016,func[0,1])>");
        await Task.Delay(50);
        await _tcpListener.Send("\r");
        await Task.Delay(50);
        await _tcpListener.Send("\n");
        await Task.Delay(50);
        await _tcpListener.Send("<END 0 (OK)>\r\n");
        var result = await task;

        result.Should().BeSuccess();
        var response = result.Value;
        response.Command.Should().Be("set(1016,func[0,1])");
        response.Content.Should().Be(string.Empty);
        response.ErrorCode.Should().Be(0);
        response.ErrorMessage.Should().Be("(OK)");
    }

    private class TestTcpListener
    {
        private bool _shouldRun = true;

        private TcpClient? _client;

        public List<byte> Data { get; } = new();

        public async Task Start()
        {
            TcpListener listener = new(_ip, Port);
            listener.Start();
            _client = await listener.AcceptTcpClientAsync();
            listener.Stop();

            _ = Task.Run(HandleReceivedData);
        }

        public async Task Send(string message)
        {
            await Send(Encoding.UTF8.GetBytes(message));
        }

        public async Task Send(byte[] bytes)
        {
            if (_client == null)
            {
                throw new InvalidOperationException();
            }

            await _client.GetStream().WriteAsync(bytes);
        }

        public void Stop()
        {
            _shouldRun = false;
        }

        private async void HandleReceivedData()
        {
            if (_client != null)
            {
                var stream = _client.GetStream();
                var buffer = new byte[1024];
                while (_shouldRun)
                {
                    var readBytes = await stream.ReadAsync(buffer.AsMemory(0, 1024), CancellationToken.None);
                    Data.AddRange(buffer[0..readBytes]);
                }
            }
        }
    }
}