namespace DotNet.Testcontainers.Clients
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using DotNet.Testcontainers.Configurations;

  internal sealed class DefaultLabels : ReadOnlyDictionary<string, string>
  {
    private DefaultLabels()
      : base(new Dictionary<string, string>
      {
        { TestcontainersClient.TestcontainersLabel, bool.TrueString },
        { ResourceReaper.ResourceReaperSessionLabel, ResourceReaper.DefaultSessionId.ToString("D") },
      })
    {
    }

    public static IReadOnlyDictionary<string, string> Instance { get; }
      = new DefaultLabels();
  }
}
