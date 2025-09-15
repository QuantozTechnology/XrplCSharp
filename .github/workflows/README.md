# GitHub Package Publishing Workflow

This repository includes a GitHub Actions workflow that automatically publishes NuGet packages to GitHub Packages (and optionally to NuGet.org) when a new release is created.

## How it works

The workflow is triggered when:
- A new release is created on GitHub

## What it does

1. **Extracts version from release tag** - The version is automatically extracted from the Git tag
2. **Sets up .NET environment** - Installs .NET 6, 7, 8, and 9
3. **Restores dependencies** - Downloads all required NuGet packages
4. **Builds the solution** - Compiles all projects in Release configuration
5. **Runs tests** - Executes all unit tests to ensure quality
6. **Updates package versions** - Sets the package version to match the release tag
7. **Creates NuGet packages** - Packs the following projects:
   - Xrpl.AddressCodec
   - Xrpl.BinaryCodec
   - Xrpl.Keypairs
   - Xrpl (main package)
8. **Publishes packages** - Uploads packages to GitHub Packages and optionally to NuGet.org

## Setup Instructions

### 1. GitHub Packages (Automatic)
No additional setup required. The workflow uses the built-in `GITHUB_TOKEN` which has the necessary permissions.

### 2. NuGet.org (Optional)
To also publish to NuGet.org:

1. Go to your repository's **Settings** ? **Secrets and variables** ? **Actions**
2. Click **New repository secret**
3. Name: `NUGET_API_KEY`
4. Value: Your NuGet.org API key
   - Get your API key from [nuget.org/account/apikeys](https://www.nuget.org/account/apikeys)
   - Create a new key with "Push new packages and package versions" scope

## Creating a Release

1. Go to your repository on GitHub
2. Click **Releases** ? **Create a new release**
3. Create a new tag (e.g., `v1.0.0`, `2.1.0`, etc.)
4. Fill in the release title and description
5. Click **Publish release**

The workflow will automatically trigger and publish your packages!

## Package Locations

### GitHub Packages
Packages will be available at:
- `https://nuget.pkg.github.com/YOUR_USERNAME/index.json`

### NuGet.org (if configured)
Packages will be available at:
- `https://www.nuget.org/packages/Xrpl.AddressCodec/`
- `https://www.nuget.org/packages/Xrpl.BinaryCodec/`
- `https://www.nuget.org/packages/Xrpl.Keypairs/`
- `https://www.nuget.org/packages/Xrpl/`

## Using GitHub Packages

To use packages from GitHub Packages, add this to your `nuget.config`:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="github" value="https://nuget.pkg.github.com/YOUR_USERNAME/index.json" />
  </packageSources>
  <packageSourceCredentials>
    <github>
      <add key="Username" value="YOUR_USERNAME" />
      <add key="ClearTextPassword" value="YOUR_PERSONAL_ACCESS_TOKEN" />
    </github>
  </packageSourceCredentials>
</configuration>
```

## Workflow File Location

The workflow is located at: `.github/workflows/publish-packages.yml`

## Monitoring

You can monitor the workflow execution in the **Actions** tab of your repository. Each release will create a new workflow run that you can view for logs and status.