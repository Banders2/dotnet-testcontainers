namespace DotNet.Testcontainers.Containers.Modules.Misc
{
  using DotNet.Testcontainers.Configurations.Containers;
  using Microsoft.Extensions.Logging;

  public class ResourceReaperContainer : TestcontainersContainer
  {
    protected ResourceReaperContainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }
  }
}
