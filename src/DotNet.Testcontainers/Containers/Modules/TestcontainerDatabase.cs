namespace DotNet.Testcontainers.Containers
{
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Configurations.Containers;
  using DotNet.Testcontainers.Containers.Modules;
  using JetBrains.Annotations;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// This class represents an extended configured Testcontainer for databases.
  /// </summary>
  public abstract class TestcontainerDatabase : HostedServiceContainer, IDatabaseScript
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="TestcontainerDatabase" /> class.
    /// </summary>
    /// <param name="configuration">The Testcontainers configuration.</param>
    /// <param name="logger">The logger.</param>
    protected TestcontainerDatabase(ITestcontainersConfiguration configuration, ILogger logger)
      : base(configuration, logger)
    {
    }

    /// <summary>
    /// Gets the database connection string.
    /// </summary>
    [PublicAPI]
    public abstract string ConnectionString { get; }

    /// <summary>
    /// Gets or sets the database.
    /// </summary>
    [PublicAPI]
    public virtual string Database { get; set; }

    /// <summary>
    /// Creates a path to a temporary script file.
    /// </summary>
    /// <returns>A path to a temporary script file.</returns>
    public virtual string GetTempScriptFile()
    {
      return Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    }

    /// <summary>
    /// Executes a bash script in the database container.
    /// </summary>
    /// <param name="scriptContent">The content of the bash script to be executed.</param>
    /// <returns>Task that completes when the script has been executed.</returns>
    public virtual async Task<ExecResult> ExecScriptAsync(string scriptContent)
    {
      var tempScriptFile = this.GetTempScriptFile();

      await this.CopyFileAsync(tempScriptFile, Encoding.UTF8.GetBytes(scriptContent), 493 /* 755 */)
        .ConfigureAwait(false);

      return await this.ExecAsync(new[] { "/bin/sh", "-c", tempScriptFile })
        .ConfigureAwait(false);
    }
  }
}
