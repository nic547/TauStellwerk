using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TauStellwerk.Commands.ECoS;

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
        _tcpListener = new TestTcpListener();
        var listenerTask = _tcpListener.Start();
        _connectionHandler = new ECosConnectionHandler(_ip, Port);
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
        var receivedCommand = Encoding.UTF8.GetString(_tcpListener.Data.ToArray());

        result.ErrorCode.Should().Be(0);
        receivedCommand.Should().BeEquivalentTo(command);
    }

    [Test]
    public async Task ResponseLineSplitTest()
    {
        var task = _connectionHandler.SendCommandAsync("set(1016,func[0,1])");
        await _tcpListener.Send("<REPLY set(1016,func[0,1])>\r\n");
        await _tcpListener.Send("<END 0 (OK)>\r\n");
        var result = await task;

        result.Command.Should().Be("set(1016,func[0,1])");
        result.Content.Should().Be(string.Empty);
        result.ErrorCode.Should().Be(0);
        result.ErrorMessage.Should().Be("(OK)");
    }

    [Test]
    public async Task ResponseSplitCharacterTest()
    {
        var task = _connectionHandler.SendCommandAsync("set(1,1)");
        await _tcpListener.Send("<REPLY set(1,1)>\r\n");
        await _tcpListener.Send(new byte[] { 0b110_00011 });
        await _tcpListener.Send(new byte[] { 0b101_00100, 0b0000_1101, 0b0000_1010 });
        await _tcpListener.Send("<END 0 (OK)>\r\n");
        var result = await task;

        result.Content.Should().Be("ä\r\n");
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
                    var readBytes = await stream.ReadAsync(buffer, 0, 1024);
                    Data.AddRange(buffer[0..readBytes]);
                }
            }
        }
    }
}