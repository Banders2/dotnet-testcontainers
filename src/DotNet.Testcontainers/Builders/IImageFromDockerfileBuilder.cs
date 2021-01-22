namespace DotNet.Testcontainers.Builders
{
  using System;
  using System.Threading.Tasks;
  using Configurations;
  using DotNet.Testcontainers.Images;
  using JetBrains.Annotations;

  /// <summary>
  /// A fluent Docker image builder.
  /// </summary>
  public interface IImageFromDockerfileBuilder
  {
    /// <summary>
    /// Sets the name of the Docker image.
    /// </summary>
    /// <param name="name">Docker image name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithName(string name);

    /// <summary>
    /// Sets the name of the Docker image.
    /// </summary>
    /// <param name="name">Docker image name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithName(IDockerImage name);

    /// <summary>
    /// Sets the name of the Dockerfile.
    /// </summary>
    /// <param name="dockerfile">Dockerfile name.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithDockerfile(string dockerfile);

    /// <summary>
    /// Sets the base directory of the Dockerfile.
    /// </summary>
    /// <param name="dockerfileDirectory">Dockerfile base directory.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithDockerfileDirectory(string dockerfileDirectory);

    /// <summary>
    /// If true, Testcontainer will remove the existing Docker image. Otherwise, Testcontainer will keep the Docker image.
    /// </summary>
    /// <param name="deleteIfExists">True, Testcontainer will remove the Docker image. Otherwise, Testcontainer will keep it.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithDeleteIfExists(bool deleteIfExists);

    /// <summary>
    /// Add a label to the resulting image.
    /// </summary>
    /// <param name="labelName">Name of the label.</param>
    /// <param name="value">Value of the label.</param>
    /// <returns>A configured instance of <see cref="IImageFromDockerfileBuilder" />.</returns>
    [PublicAPI]
    IImageFromDockerfileBuilder WithLabel(string labelName, string value);

    /// <summary>
    /// Sets the resource reaper session id for this image.
    /// The <see cref="ResourceReaper"/> will make sure to delete the image after the tests have finished if it was not deleted explicitly.
    /// </summary>
    /// <param name="resourceReaperSessionId">The session id of the <see cref="ResourceReaper"/> instance.</param>
    [PublicAPI]
    IImageFromDockerfileBuilder WithResourceReaperSessionId(Guid? resourceReaperSessionId);

    /// <summary>
    /// Builds the instance of <see cref="IImageFromDockerfileBuilder" /> with the given configuration.
    /// </summary>
    /// <returns>FullName of the created image.</returns>
    [PublicAPI]
    Task<string> Build();
  }
}
