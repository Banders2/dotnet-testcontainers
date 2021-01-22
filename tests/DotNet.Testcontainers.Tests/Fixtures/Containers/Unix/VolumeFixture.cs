namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Volumes;
  using Xunit;

  public sealed class VolumeFixture : IAsyncLifetime
  {
    public Guid ResourceReaperSessionId { get; } = Guid.NewGuid();

    private ResourceReaper ResourceReaper { get; set; }

    public IDockerVolume Volume { get; private set; }

    public async Task InitializeAsync()
    {
      this.ResourceReaper = await ResourceReaper.StartNew(sessionId: this.ResourceReaperSessionId);

      this.Volume = new TestcontainersVolumeBuilder()
        .WithResourceReaperSessionId(this.ResourceReaperSessionId)
        .WithName("test-volume")
        .Build();

      await this.Volume.CreateAsync();
    }

    public async Task DisposeAsync()
    {
      if (this.ResourceReaper != null)
      {
        await this.ResourceReaper.DisposeAsync();
      }
    }
  }
}
