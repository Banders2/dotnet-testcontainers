namespace DotNet.Testcontainers.Tests.Unit
{
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class TestcontainersVolumeTest : IClassFixture<VolumeFixture>, IAsyncLifetime
  {
    private readonly VolumeFixture volumeFixture;

    private const string Destination = "/tmp/";

    private ITestcontainersContainer testcontainer1;

    private ITestcontainersContainer testcontainer2;

    public TestcontainersVolumeTest(VolumeFixture volumeFixture)
    {
      this.volumeFixture = volumeFixture;
    }

    public Task InitializeAsync()
    {
      var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithResourceReaperSessionId(volumeFixture.ResourceReaperSessionId)
        .WithImage("alpine")
        .WithEntrypoint("/bin/sh", "-c")
        .WithCommand("touch /tmp/$(uname -n) && tail -f /dev/null")
        .WithVolumeMount(volumeFixture.Volume.Name, Destination);

      this.testcontainer1 = testcontainersBuilder
        .WithResourceReaperSessionId(volumeFixture.ResourceReaperSessionId)
        .WithHostname(nameof(this.testcontainer1))
        .Build();

      this.testcontainer2 = testcontainersBuilder
        .WithResourceReaperSessionId(volumeFixture.ResourceReaperSessionId)
        .WithHostname(nameof(this.testcontainer2))
        .Build();

      return Task.WhenAll(this.testcontainer1.StartAsync(), this.testcontainer2.StartAsync());
    }

    public Task DisposeAsync()
    {
      return Task.CompletedTask;
    }

    [Fact]
    public async Task WithVolumeMount()
    {
      Assert.Equal(0, (await this.testcontainer1.ExecAsync(new[] { "test", "-f", Path.Combine(Destination, nameof(this.testcontainer2)) })).ExitCode);
      Assert.Equal(0, (await this.testcontainer2.ExecAsync(new[] { "test", "-f", Path.Combine(Destination, nameof(this.testcontainer1)) })).ExitCode);
    }
  }
}
