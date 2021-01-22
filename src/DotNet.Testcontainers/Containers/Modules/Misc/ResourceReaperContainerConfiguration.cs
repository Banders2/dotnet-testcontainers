namespace DotNet.Testcontainers.Containers.Modules.Misc
{
  using System;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using JetBrains.Annotations;

  public class ResourceReaperContainerConfiguration
  {
    /// <summary>
    /// See <see cref="Image"/>
    /// </summary>
    private const string DefaultImage = "ghcr.io/psanetra/ryuk:2021.12.20";

    public ResourceReaperContainerConfiguration(string image = DefaultImage, Guid? sessionId = null, string name = null, int port = 0, int defaultPort = 8080)
    {
      this.SessionId = sessionId ?? Guid.NewGuid();
      this.Name = name ?? $"testcontainers-ryuk-{this.SessionId:D}";
      this.Image = image;
      this.Port = port;
      this.DefaultPort = defaultPort;
      this.WaitStrategy = new WaitForContainerUnix().UntilPortIsAvailable(defaultPort);
    }

    /// <summary>
    /// Gets or sets the reaper container image.
    /// Must be compatible to ryuk and support pruning on SIGINT and SIGTERM signals.
    /// As of ryuk 0.3.3 pruning is not supported by the original ryuk image. By default the fork ghcr.io/psanetra/ryuk:2021.12.20 is used.
    /// </summary>
    [PublicAPI]
    public string Image { get; }

    /// <summary>
    /// Gets the session id of the resource reaper instance.
    /// </summary>
    [PublicAPI]
    public Guid SessionId { get; }

    /// <summary>
    /// Gets the container name.
    /// </summary>
    [PublicAPI]
    public string Name { get; }

    /// <summary>
    /// Gets the container port.
    /// </summary>
    /// <remarks>
    /// Corresponds to the default port of the hosted service.
    /// </remarks>
    [PublicAPI]
    public int DefaultPort { get; }

    /// <summary>
    /// Gets or sets the host port.
    /// </summary>
    /// <remarks>
    /// Is bond to <see cref="DefaultPort" />.
    /// </remarks>
    [PublicAPI]
    public int Port { get; set; }

    /// <summary>
    /// Gets the wait strategy.
    /// </summary>
    /// <remarks>
    /// Uses <see cref="Wait.ForUnixContainer" /> as default value.
    /// </remarks>
    [PublicAPI]
    public IWaitForContainerOS WaitStrategy { get; }

    /// <summary>
    /// Gets or sets the path to the Docker socket on the host.
    /// </summary>
    [PublicAPI]
    public string HostDockerSocketPath { get; set; } = "/var/run/docker.sock";
  }
}
