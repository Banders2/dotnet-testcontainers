namespace DotNet.Testcontainers.Configurations.Images
{
  using System.Collections.Generic;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A Dockerfile configuration.
  /// </summary>
  internal interface IImageFromDockerfileConfiguration
  {
    /// <summary>
    /// Gets a value indicating whether an existing image is removed or not.
    /// </summary>
    bool DeleteIfExists { get; }

    /// <summary>
    /// Gets the Dockerfile.
    /// </summary>
    [NotNull]
    string Dockerfile { get; }

    /// <summary>
    /// Gets the Labels.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> Labels { get; }

    /// <summary>
    /// Gets the Dockerfile directory.
    /// </summary>
    [NotNull]
    string DockerfileDirectory { get; }

    /// <summary>
    /// Gets the Docker image.
    /// </summary>
    [NotNull]
    IDockerImage Image { get; }
  }
}
