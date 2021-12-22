namespace DotNet.Testcontainers.Configurations
{
  using System;
  using System.IO;
  using System.Net.Sockets;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging;

  public class ResourceReaper : IAsyncDisposable
  {
    public const string ResourceReaperSessionLabel = TestcontainersClient.TestcontainersLabel + ".resource-reaper-session";

    public static readonly Guid DefaultSessionId = Guid.NewGuid();
    private static SemaphoreSlim defaultLock = new SemaphoreSlim(1, 1);
    private static ResourceReaper defaultInstance;

    private static readonly ILogger Logger = TestcontainersSettings.Logger;

    private readonly ResourceReaperContainerConfiguration resourceReaperContainerConfiguration;
    private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

    private ResourceReaperContainer resourceReaperContainer;

    private bool disposed;

    private ResourceReaper(ResourceReaperContainerConfiguration resourceReaperContainerConfiguration = null, Guid? sessionId = null)
    {
      this.resourceReaperContainerConfiguration = resourceReaperContainerConfiguration ?? new ResourceReaperContainerConfiguration();
      this.SessionId = sessionId ?? Guid.NewGuid();
    }

    public static async Task<ResourceReaper> StartNew(ResourceReaperContainerConfiguration resourceReaperContainerConfiguration = null, Guid? sessionId = null)
    {
      var resourceReaper = new ResourceReaper(resourceReaperContainerConfiguration, sessionId);
      await resourceReaper.StartAsync().ConfigureAwait(false);
      return resourceReaper;
    }

    public static async Task<ResourceReaper> GetOrStartDefaultAsync()
    {
      if (defaultInstance != null && !defaultInstance.disposed)
      {
        return defaultInstance;
      }

      await defaultLock.WaitAsync();

      try
      {
        if (defaultInstance != null && !defaultInstance.disposed)
        {
          return defaultInstance;
        }

        defaultInstance = await StartNew(sessionId: DefaultSessionId);

        return defaultInstance;
      }
      finally
      {
        defaultLock.Release();
      }
    }

    public Guid SessionId { get; }

    private async Task StartAsync()
    {
      if (this.resourceReaperContainer != null)
      {
        return;
      }

      this.resourceReaperContainer = new TestcontainersBuilder<ResourceReaperContainer>()
        .WithResourceReaper(this.resourceReaperContainerConfiguration)
        .Build();

      await this.resourceReaperContainer.StartAsync().ConfigureAwait(false);

#pragma warning disable 4014
      Task.Run(() => this.MaintainRyukConnection(this.cancellationTokenSource.Token));
#pragma warning restore 4014
    }

    private async Task MaintainRyukConnection(CancellationToken cancellationToken)
    {
      var hostname = this.resourceReaperContainer.Hostname;
      var port = this.resourceReaperContainer.GetMappedPublicPort(this.resourceReaperContainerConfiguration.DefaultPort);
      var sessionLabelSent = false;

      while (!cancellationToken.IsCancellationRequested)
      {
        try
        {
          using (var tcpClient = new TcpClient())
          {
            await tcpClient.ConnectAsync(hostname, port).ConfigureAwait(false);

            var stream = tcpClient.GetStream();

            if (!sessionLabelSent)
            {
              await this.SendSessionLabel(stream, cancellationToken).ConfigureAwait(false);
              sessionLabelSent = true;
            }

            while (!cancellationToken.IsCancellationRequested)
            {
              var buffer = new byte[1024];
              await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false);
            }
          }
        }
        catch (OperationCanceledException e)
        {
          Logger.LogTrace(e, "Lost connection to resource reaper container.");
        }
        catch (Exception e)
        {
          Logger.LogError(e, "Lost connection to resource reaper container.");
          await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken).ConfigureAwait(false);
        }
      }
    }

    private async Task SendSessionLabel(Stream stream, CancellationToken cancellationToken)
    {
      var messageBytes = Encoding.UTF8.GetBytes($"label={ResourceReaperSessionLabel}={this.SessionId:D}\n");
      await stream.WriteAsync(messageBytes, 0, messageBytes.Length, cancellationToken);
      await stream.FlushAsync(cancellationToken);

      var streamReader = new StreamReader(stream, Encoding.UTF8);

      while (!cancellationToken.IsCancellationRequested && !string.Equals("ack", await streamReader.ReadLineAsync(), StringComparison.OrdinalIgnoreCase))
      {
      }
    }

    public async ValueTask DisposeAsync()
    {
      if (this.disposed)
      {
        return;
      }

      this.disposed = true;

      try
      {
        this.cancellationTokenSource.Cancel();
      }
      catch (Exception e)
      {
        Logger.LogError(e, $"Could not cancel {nameof(this.cancellationTokenSource)}.");
      }

      this.cancellationTokenSource.Dispose();

      if (this.resourceReaperContainer != null)
      {
        await this.resourceReaperContainer.DisposeAsync();
      }
    }
  }
}
