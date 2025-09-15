# GitHub Package Publishing Workflow

This repository includes a GitHub Actions workflow that automatically publishes NuGet packages to GitHub Packages (and optionally to NuGet.org) when a new release is created.

## How it works

The workflow is triggered when:
- A new release is created on GitHub

## What it does

1. **Extracts version from release tag** - The version is automatically extracted from the Git tag
2. **Sets up .NET environment** - Installs .NET 8, and 9
3. **Restores dependencies** - Downloads all required NuGet packages
4. **Builds the solution** - Compiles all projects in Release configuration
5. **Runs tests** - Executes all unit tests (⚠️ **Test failures will NOT block the release**)
6. **Updates package versions** - Sets the package version to match the release tag
7. **Creates NuGet packages** - Packs the following projects:
   - Xrpl.AddressCodec
   - Xrpl.BinaryCodec
   - Xrpl.Keypairs
   - Xrpl (main package)
8. **Publishes packages** - Uploads packages to GitHub Packages and optionally to NuGet.org

## ⚠️ Important Note About Tests

**The workflow is configured to ignore test failures and continue with the release process.** This means:

- Tests will still run and their results will be visible in the workflow logs
- If tests fail, the workflow will continue and publish packages anyway
- This is useful for development scenarios where you have known failing tests that shouldn't block releases
- **For production releases, consider fixing failing tests before creating a release**

To change this behavior and make test failures block releases, remove the `continue-on-error: true` line from the "Run tests" step in the workflow file.

## Setup Instructions

### 1. GitHub Packages (Automatic)
No additional setup required. The workflow uses the built-in `GITHUB_TOKEN` which has the necessary permissions.

## Creating a Release

1. Go to your repository on GitHub
2. Click **Releases** → **Create a new release**
3. Create a new tag (e.g., `v1.0.0`, `2.1.0`, etc.)
4. Fill in the release title and description
5. Click **Publish release**

The workflow will automatically trigger and publish your packages, even if some tests fail!

## Package Locations

### GitHub Packages
Packages will be available at:
- `https://nuget.pkg.github.com/YOUR_USERNAME/index.json`

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

You can monitor the workflow execution in the **Actions** tab of your repository. Each release will create a new workflow run that you can view for logs and status. Note that even if tests fail, the workflow will show as successful if the packages are published correctly.