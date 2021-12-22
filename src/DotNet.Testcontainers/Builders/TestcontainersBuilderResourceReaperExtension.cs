namespace DotNet.Testcontainers.Builders
{
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;

  /// <summary>
  /// This class applies the extended Testcontainer configurations for the resource reaper.
  /// </summary>
  [PublicAPI]
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
        .WithBindMount(configuration.HostDockerSocketPath, "/var/run/docker.sock", AccessMode.ReadOnly);
    }
  }
}
