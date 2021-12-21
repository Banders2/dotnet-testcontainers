namespace DotNet.Testcontainers.Configurations.Images
{
  using System.Collections.Generic;
  using System.IO;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Images;

  /// <inheritdoc cref="IImageFromDockerfileConfiguration" />
  internal sealed class ImageFromDockerfileConfiguration : IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    public ImageFromDockerfileConfiguration()
      : this(CreateDockerImage())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageFromDockerfileConfiguration" /> class.
    /// </summary>
    /// <param name="image">The Docker image.</param>
    /// <param name="dockerfile">The Dockerfile.</param>
    /// <param name="dockerfileDirectory">The Dockerfile directory.</param>
    /// <param name="deleteIfExists">A value indicating whether an existing image is removed or not.</param>
    public ImageFromDockerfileConfiguration(
      IDockerImage image,
      string dockerfile = "Dockerfile",
      string dockerfileDirectory = ".",
      bool deleteIfExists = true,
      IReadOnlyDictionary<string, string> labels = null)
    {
      this.DeleteIfExists = deleteIfExists;
      this.Dockerfile = dockerfile;
      this.DockerfileDirectory = dockerfileDirectory;
      this.Image = image;
      this.Labels = labels ?? DefaultLabels.Instance;
    }

    /// <inheritdoc />
    public bool DeleteIfExists { get; }

    /// <inheritdoc />
    public string Dockerfile { get; }

    /// <inheritdoc />
    public string DockerfileDirectory { get; }

    /// <inheritdoc />
    public IDockerImage Image { get; }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, string> Labels { get; }

    private static IDockerImage CreateDockerImage()
    {
      return new DockerImage("Testcontainers", Path.GetRandomFileName().Substring(0, 8), string.Empty);
    }
  }
}
