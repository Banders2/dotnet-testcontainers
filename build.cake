#tool nuget:?package=dotnet-sonarscanner&version=5.4.0

#addin nuget:?package=Cake.Sonar&version=1.1.29

#addin nuget:?package=Cake.Git&version=2.0.0

#load ".cake-scripts/parameters.cake"

readonly var param = BuildParameters.Instance(Context);

Setup(context =>
{
  var toClean = param.Paths.Directories.ToClean;

  foreach (var project in param.Projects.All)
  {
    toClean.Add("build");
  }

  Information("Building version {0} of .NET Testcontainers ({1}@{2})", param.Version, param.Branch, param.Sha);
});

Teardown(context =>
{
});

Task("Clean")
  .Does(() =>
{
  var deleteDirectorySettings = new DeleteDirectorySettings();
  deleteDirectorySettings.Recursive = true;
  deleteDirectorySettings.Force = true;

  foreach (var directory in param.Paths.Directories.ToClean)
  {
    if (DirectoryExists(directory))
    {
      DeleteDirectory(directory, deleteDirectorySettings);
    }
  }
});

Task("Restore-NuGet-Packages")
  .Does(() =>
{
  DotNetRestore(param.Solution, new DotNetRestoreSettings
  {
    Verbosity = param.Verbosity
  });
});

Task("Build-Information")
  .Does(() =>
{
  foreach (var project in param.Projects.All)
  {
    Console.WriteLine($"{project.Name} [{project.Path.GetDirectory()}]");
  }
});

Task("Build")
  .Does(() =>
{
  DotNetBuild(param.Solution, new DotNetBuildSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    ArgumentCustomization = args => args
      .Append($"/p:TreatWarningsAsErrors={param.IsReleaseBuild.ToString()}")
  });
});

Task("Tests")
  .Does(() =>
{
  foreach(var testProject in param.Projects.OnlyTests)
  {
    DotNetTest(testProject.Path.FullPath, new DotNetTestSettings
    {
      Configuration = param.Configuration,
      Verbosity = param.Verbosity,
      NoRestore = true,
      NoBuild = true,
      Loggers = new[] { "trx" },
      Filter = param.TestFilter,
      ResultsDirectory = param.Paths.Directories.TestResults,
      ArgumentCustomization = args => args
        .Append("/p:Platform=AnyCPU")
        .Append("/p:CollectCoverage=true")
        .Append("/p:CoverletOutputFormat=opencover")
        .Append($"/p:CoverletOutput=\"{MakeAbsolute(param.Paths.Directories.TestCoverage)}/\"")
    });
  }
});

Task("Sonar-Begin")
  .Does(() =>
{
  SonarBegin(new SonarBeginSettings
  {
    Url = param.SonarQubeCredentials.Url,
    Key = param.SonarQubeCredentials.Key,
    Login = param.SonarQubeCredentials.Token,
    Organization = param.SonarQubeCredentials.Organization,
    Branch = param.IsPullRequest ? null : param.Branch, // A pull request analysis can not have the branch analysis parameter 'sonar.branch.name'.
    UseCoreClr = true,
    Silent = true,
    Version = param.Version.Substring(0, 5),
    PullRequestProvider = "GitHub",
    PullRequestGithubEndpoint = "https://api.github.com/",
    PullRequestGithubRepository = "HofmeisterAn/dotnet-testcontainers",
    PullRequestKey = param.IsPullRequest && System.Int32.TryParse(param.PullRequestId, out var id) ? id : (int?)null,
    PullRequestBranch = param.SourceBranch,
    PullRequestBase = param.TargetBranch,
    VsTestReportsPath = $"{MakeAbsolute(param.Paths.Directories.TestResults)}/*.trx",
    OpenCoverReportsPath = $"{MakeAbsolute(param.Paths.Directories.TestCoverage)}/*.opencover.xml"
  });
});

Task("Sonar-End")
  .Does(() =>
{
  SonarEnd(new SonarEndSettings
  {
    Login = param.SonarQubeCredentials.Token,
    UseCoreClr = true
  });
});

Task("Create-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  DotNetPack(param.Projects.Testcontainers.Path.FullPath, new DotNetPackSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    NoBuild = true,
    IncludeSymbols = true,
    OutputDirectory = param.Paths.Directories.NugetRoot,
    ArgumentCustomization = args => args
      .Append("/p:Platform=AnyCPU")
      .Append("/p:SymbolPackageFormat=snupkg")
      .Append($"/p:Version={param.Version}")
  });
});

Task("Publish-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  foreach(var package in GetFiles($"{param.Paths.Directories.NugetRoot}/*.(nupkg|snupkgs)"))
  {
    DotNetNuGetPush(package.FullPath, new DotNetNuGetPushSettings
    {
      Source = param.NuGetCredentials.Source,
      ApiKey = param.NuGetCredentials.ApiKey
    });
  }
});

Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("Build")
  .IsDependentOn("Tests");

Task("Sonar")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("Sonar-Begin")
  .IsDependentOn("Build")
  .IsDependentOn("Tests")
  .IsDependentOn("Sonar-End");

Task("Publish")
  .IsDependentOn("Create-NuGet-Packages")
  .IsDependentOn("Publish-NuGet-Packages");

RunTarget(param.Target);
