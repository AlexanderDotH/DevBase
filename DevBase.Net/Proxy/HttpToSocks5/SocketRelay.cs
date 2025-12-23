using System.Buffers;
using System.Net.Sockets;

namespace DevBase.Net.Proxy.HttpToSocks5;

/// <summary>
/// High-performance bidirectional socket relay using async I/O.
/// </summary>
internal sealed class SocketRelay
{
    private static readonly ArrayPool<byte> BufferPool = ArrayPool<byte>.Shared;
    private const int BufferSize = 81920;

    private readonly SocketAsyncEventArgs _receiveArgs;
    private readonly SocketAsyncEventArgs _sendArgs;
    private readonly Socket _source;
    private readonly Socket _target;
    private readonly byte[] _buffer;

    private bool _receiving = true;
    private int _received;
    private int _sendingOffset;
    
    public SocketRelay? Other { get; set; }
    
    private volatile bool _disposed;
    private volatile bool _shouldDispose;

    private SocketRelay(Socket source, Socket target)
    {
        _source = source;
        _target = target;
        _buffer = BufferPool.Rent(BufferSize);
        
        _receiveArgs = new SocketAsyncEventArgs { UserToken = this };
        _sendArgs = new SocketAsyncEventArgs { UserToken = this };
        
        _receiveArgs.SetBuffer(_buffer, 0, BufferSize);
        _sendArgs.SetBuffer(_buffer, 0, BufferSize);
        
        _receiveArgs.Completed += OnAsyncOperationCompleted;
        _sendArgs.Completed += OnAsyncOperationCompleted;
    }

    private void Cleanup()
    {
        if (_disposed) return;
        
        _disposed = _shouldDispose = true;

        if (Other != null)
        {
            Other._shouldDispose = true;
            Other = null;
        }

        _source.TryDispose();
        _target.TryDispose();
        
        _receiveArgs.TryDispose();
        _sendArgs.TryDispose();
        
        BufferPool.Return(_buffer);
    }

    private void Process()
    {
        try
        {
            while (true)
            {
                if (_shouldDispose)
                {
                    Cleanup();
                    return;
                }

                if (_receiving)
                {
                    _receiving = false;
                    _sendingOffset = -1;

                    if (_source.ReceiveAsync(_receiveArgs))
                        return;
                }
                else
                {
                    if (_sendingOffset == -1)
                    {
                        _received = _receiveArgs.BytesTransferred;
                        _sendingOffset = 0;

                        if (_received == 0)
                        {
                            _shouldDispose = true;
                            continue;
                        }
                    }
                    else
                    {
                        _sendingOffset += _sendArgs.BytesTransferred;
                    }

                    if (_sendingOffset != _received)
                    {
                        _sendArgs.SetBuffer(_buffer, _sendingOffset, _received - _sendingOffset);

                        if (_target.SendAsync(_sendArgs))
                            return;
                    }
                    else
                    {
                        _receiving = true;
                    }
                }
            }
        }
        catch
        {
            Cleanup();
        }
    }

    private static void OnAsyncOperationCompleted(object? sender, SocketAsyncEventArgs e)
    {
        SocketRelay relay = (SocketRelay)e.UserToken!;
        relay.Process();
    }

    public static void RelayBidirectionally(Socket s1, Socket s2)
    {
        SocketRelay relayOne = new SocketRelay(s1, s2);
        SocketRelay relayTwo = new SocketRelay(s2, s1);

        relayOne.Other = relayTwo;
        relayTwo.Other = relayOne;

        Task.Run(relayOne.Process);
        Task.Run(relayTwo.Process);
    }
}
