using System.Diagnostics;
using System.Reflection;

using TiaPortalToolbox.Contracts.Services;

namespace TiaPortalToolbox.Services;

public class ApplicationInfoService : IApplicationInfoService
{
    public Version GetVersion()
    {
        //if (WindowsVersionHelper.HasPackageIdentity)
        //{
        //    // Packaged application
        //    // Set the app version in ToolsAssist.Packaging > Package.appxmanifest > Packaging > PackageVersion
        //    var packageVersion = Package.Current.Id.Version;
        //    return new Version(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        //}

        // Set the app version in ToolsAssist > Properties > Package > PackageVersion
        string assemblyLocation = Assembly.GetExecutingAssembly().Location;
        var version = FileVersionInfo.GetVersionInfo(assemblyLocation).FileVersion;
        return new Version(version);
    }
}