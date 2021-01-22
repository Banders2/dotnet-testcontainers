namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Containers.Modules.Misc;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for the resource reaper.
  /// </summary>
  public static class TestcontainersBuilderResourceReaperExtension
  {
    public static ITestcontainersBuilder<T> WithResourceReaper<T>(this ITestcontainersBuilder<T> builder, ResourceReaperContainerConfiguration configuration)
      where T : ResourceReaperContainer
    {
      return builder
        .WithName(configuration.Name)
        .WithImage(configuration.Image)
        .WithAutoRemove(true)
        .WithCleanUp(false)
        .WithPortBinding(configuration.Port, configuration.DefaultPort)
        .WithExposedPort(configuration.DefaultPort)
        .WithWaitStrategy(configuration.WaitStrategy)
        .WithMount(configuration.HostDockerSocketPath, "/var/run/docker.sock");
    }
  }
}
