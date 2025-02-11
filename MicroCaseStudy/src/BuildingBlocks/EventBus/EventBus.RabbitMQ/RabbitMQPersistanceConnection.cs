using System.Net.Sockets;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ;

public class RabbitMQPersistanceConnection : IDisposable
{
    private readonly IConnectionFactory _connectionFactory;
    private readonly int retryCount;
    private IConnection connection;
    private object lock_object = new object();
    
    private bool _disposed;


    public bool IsConnected => connection != null && connection.IsOpen;

    public RabbitMQPersistanceConnection(IConnectionFactory connectionFactory, int retryCount = 5)
    {
        this._connectionFactory = connectionFactory;
        this.retryCount = retryCount;
    }

    public IModel CreateModel()
    {
        return connection.CreateModel();
    }

    #region IDisposable

    public void Dispose()
    {
        _disposed = true;
        connection.Dispose();
    }

    #endregion

    public bool TryConnect()
    {
        lock (lock_object)
        {
            var policy = Policy.Handle<SocketException>()
                .Or<BrokerUnreachableException>()
                .WaitAndRetry(retryCount, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (ex, time) => { });

            policy.Execute(() => { connection = _connectionFactory.CreateConnection(); });

            if (IsConnected)
            {
                connection.ConnectionShutdown += Connection_ConnectionShutdown;
                connection.CallbackException += Connection_CallbackException;
                connection.ConnectionBlocked += Connection_ConnectionBlocked;
                
                // log
                return true;
            }

            return false;
        }
    }

    private void Connection_ConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
    {
        // log Connection_ConnectionShutdown
        
        if (_disposed) return;
        
        TryConnect();
    }

    private void Connection_CallbackException(object? sender, CallbackExceptionEventArgs e)
    {
        // log Connection_ConnectionShutdown
        
        if (_disposed) return;
        
        TryConnect();
    }

    private void Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        // log Connection_ConnectionShutdown
        
        if (_disposed) return;
        
        TryConnect();
    }
    
}