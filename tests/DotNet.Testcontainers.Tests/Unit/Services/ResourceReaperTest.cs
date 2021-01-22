namespace DotNet.Testcontainers.Tests.Unit.Services
{
  using System;
  using System.Diagnostics;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public class ResourceReaperTest
  {
    [Fact]
    public async Task ShouldReapContainerWhenDisposingResourceReaper()
    {
      // Given
      await using (var resourceReaper = await ResourceReaper.StartNew())
      {
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithResourceReaperSessionId(resourceReaper.SessionId)
          .WithImage("alpine")
          .WithCommand("/bin/sh", "-c", "tail -f /dev/null");

        await using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();
          var containerId = testcontainer.Id;

          // When
          await resourceReaper.DisposeAsync();

          // Then
          var dockerPsStdOut = await DockerPsAqNoTrunc();

          Assert.DoesNotContain(containerId, dockerPsStdOut);
        }
      }
    }

    [Fact]
    public async Task ShouldReapImageWhenDisposingResourceReaper()
    {
      var imageName = $"testimage_{Guid.NewGuid().ToString("D")}";

      // Given
      await using (var resourceReaper = await ResourceReaper.StartNew())
      {
        await new ImageFromDockerfileBuilder()
          .WithResourceReaperSessionId(resourceReaper.SessionId)
          .WithName(imageName)
          .WithDockerfileDirectory("Assets")
          .Build();

        Assert.Contains(imageName, await DockerImages());

        // When
        await resourceReaper.DisposeAsync();

        // Then
        Assert.DoesNotContain(imageName, await DockerImages());
      }
    }

    [Fact]
    public async Task ShouldReapNetworkWhenDisposingResourceReaper()
    {
      var networkName = $"testnetwork_{Guid.NewGuid().ToString("D")}";

      // Given
      await using (var resourceReaper = await ResourceReaper.StartNew())
      {
        await new TestcontainersNetworkBuilder()
          .WithResourceReaperSessionId(resourceReaper.SessionId)
          .WithName(networkName)
          .Build()
          .CreateAsync();

        Assert.Contains(networkName, await DockerNetworks());

        await resourceReaper.DisposeAsync();

        Assert.DoesNotContain(networkName, await DockerNetworks());
      }
    }

    [Fact]
    public async Task ShouldReapVolumeWhenDisposingResourceReaper()
    {
      var volumeName = $"testvolume_{Guid.NewGuid().ToString("D")}";

      // Given
      await using (var resourceReaper = await ResourceReaper.StartNew())
      {
        await new TestcontainersVolumeBuilder()
          .WithResourceReaperSessionId(resourceReaper.SessionId)
          .WithName(volumeName)
          .Build()
          .CreateAsync();

        Assert.Contains(volumeName, await DockerVolumes());

        await resourceReaper.DisposeAsync();

        Assert.DoesNotContain(volumeName, await DockerVolumes());
      }
    }

    private static async Task<string> DockerPsAqNoTrunc()
    {
      using var dockerPs = new Process { StartInfo = { FileName = "docker", Arguments = "images -aq --no-trunc" } };
      dockerPs.StartInfo.RedirectStandardOutput = true;
      Assert.True(dockerPs.Start());

      var dockerPsStdOut = await dockerPs.StandardOutput.ReadToEndAsync();
      return dockerPsStdOut;
    }

    private static async Task<string> DockerImages()
    {
      using var dockerImages = new Process { StartInfo = { FileName = "docker", Arguments = "images" } };
      dockerImages.StartInfo.RedirectStandardOutput = true;
      Assert.True(dockerImages.Start());

      var dockerImagesStdOut = await dockerImages.StandardOutput.ReadToEndAsync();
      return dockerImagesStdOut;
    }

    private static async Task<string> DockerNetworks()
    {
      using var dockerNetworks = new Process { StartInfo = { FileName = "docker", Arguments = "network ls" } };
      dockerNetworks.StartInfo.RedirectStandardOutput = true;
      Assert.True(dockerNetworks.Start());

      var dockerNetworksStdOut = await dockerNetworks.StandardOutput.ReadToEndAsync();
      return dockerNetworksStdOut;
    }

    private static async Task<string> DockerVolumes()
    {
      using var dockerVolumes = new Process { StartInfo = { FileName = "docker", Arguments = "volume ls" } };
      dockerVolumes.StartInfo.RedirectStandardOutput = true;
      Assert.True(dockerVolumes.Start());

      var dockerVolumesStdOut = await dockerVolumes.StandardOutput.ReadToEndAsync();
      return dockerVolumesStdOut;
    }
  }
}
