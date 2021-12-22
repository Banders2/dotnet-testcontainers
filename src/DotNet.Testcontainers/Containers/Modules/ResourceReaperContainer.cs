namespace DotNet.Testcontainers.Containers
{
  using DotNet.Testcontainers.Configurations;
  using Microsoft.Extensions.Logging;

  public class ResourceReaperContainer : TestcontainersContainer
  {
    protected ResourceReaperContainer(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }
  }
}
