namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Linq;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Configurations.Images;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <inheritdoc cref="IImageFromDockerfileBuilder" />
  [PublicAPI]
  public sealed class ImageFromDockerfileBuilder : IImageFromDockerfileBuilder
  {
    private readonly IImageFromDockerfileConfiguration configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    public ImageFromDockerfileBuilder()
      : this(new ImageFromDockerfileConfiguration())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileBuilder" /> class.
    /// </summary>
    /// <param name="configuration">The Dockerfile configuration.</param>
    private ImageFromDockerfileBuilder(IImageFromDockerfileConfiguration configuration)
    {
      this.configuration = configuration;
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithName(string name)
    {
      return this.WithName(new DockerImage(name));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithName(IDockerImage name)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(name, this.configuration.Dockerfile, this.configuration.DockerfileDirectory, this.configuration.DeleteIfExists, this.configuration.Labels));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDockerfile(string dockerfile)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, dockerfile, this.configuration.DockerfileDirectory, this.configuration.DeleteIfExists, this.configuration.Labels));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, this.configuration.Dockerfile, dockerfileDirectory, this.configuration.DeleteIfExists, this.configuration.Labels));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists)
    {
      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, this.configuration.Dockerfile, this.configuration.DockerfileDirectory, deleteIfExists, this.configuration.Labels));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithLabel(string labelName, string value)
    {
      var tmpLabels = this.configuration.Labels.ToDictionary(kv => kv.Key, kv => kv.Value);

      if (tmpLabels.ContainsKey(labelName))
      {
        tmpLabels[labelName] = value;
      }
      else
      {
        tmpLabels.Add(labelName, value);
      }

      return new ImageFromDockerfileBuilder(
        new ImageFromDockerfileConfiguration(this.configuration.Image, this.configuration.Dockerfile, this.configuration.DockerfileDirectory, this.configuration.DeleteIfExists, tmpLabels));
    }

    /// <inheritdoc />
    public IImageFromDockerfileBuilder WithResourceReaperSessionId(Guid? resourceReaperSessionId)
    {
      return this.WithLabel(ResourceReaper.ResourceReaperSessionLabel, resourceReaperSessionId?.ToString("D"));
    }

    /// <inheritdoc />
    public Task<string> Build()
    {
      var client = new TestcontainersClient();
      return client.BuildAsync(this.configuration);
    }
  }
}
