using System.Globalization;
using System.Reflection;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;

using Microsoft.Win32;

using TiaPortalToolbox.Core.Models;

namespace TiaPortalToolbox.Core.Helpers;

/// <summary>
/// Definition of helper functionality to resolve api dll, modules and options
/// </summary>
public static class Resolver
{
    /// <summary>
    /// Required min version of engineering dll
    /// </summary>
    public const string StrRequiredVersion = "V15.0";
    private const string BasePath = @"SOFTWARE\Siemens\Automation\Openness";
    private const string SiemensEngineeringAssemblyName = "Siemens.Engineering";
    private const string SiemensEngineeringAddInAssemblyName = "Siemens.Engineering.AddIn";
    private const string ReferencedHmiAssembly = "Siemens.Engineering.Hmi";
    private const string ModuleBaseInterface = "IOpennessBaseModule";
    private const string ModuleInterface = "IOpennessModule";
    private static string? assemblyPath = null;
    private static string? assemblyPathHmi = null;


    //public static Assembly? OnResolve(object sender, ResolveEventArgs args)
    //{
    //    var assemblyName = new AssemblyName(args.Name);
    //    if (!assemblyName.Name.StartsWith(ReferencedAssembly))
    //    {
    //        return null;
    //    }

    //    using var regBaseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
    //    using var opennessBaseKey = regBaseKey.OpenSubKey(BasePath);

    //    var opennessVersionBaseKey = opennessBaseKey.GetSubKeyNames().ToList();
    //    opennessVersionBaseKey.RemoveAll((key) =>
    //    {
    //        var subKey = opennessBaseKey.OpenSubKey(key);
    //        var t = subKey.GetSubKeyNames();
    //        var rst = subKey.GetSubKeyNames()?.Contains("PublicAPI");
    //        subKey.Close();
    //        return rst != true;
    //    });

    //    using var registryKeyLatestTiaVersion = opennessBaseKey?.OpenSubKey(opennessVersionBaseKey.Last());

    //    var requestedVersionOfAssembly = assemblyName.Version.ToString();

    //    using var assemblyVersionSubKey = registryKeyLatestTiaVersion?.OpenSubKey("PublicAPI")?.OpenSubKey(requestedVersionOfAssembly);
    //    var siemensEngineeringAssemblyPath = assemblyVersionSubKey?.GetValue(ReferencedAssembly).ToString();

    //    if (siemensEngineeringAssemblyPath == null || !File.Exists(siemensEngineeringAssemblyPath))
    //    {
    //        return null;
    //    }

    //    var assembly = Assembly.LoadFrom(siemensEngineeringAssemblyPath);
    //    return assembly;
    //}

    /// <summary>
    /// Resolve assembly by name
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static Assembly? OnResolve(object? sender, ResolveEventArgs args)
    {
        var assemblyName = new AssemblyName(args.Name);
        var path = "";

        path = assemblyName.Name switch
        {
            _ when assemblyName.Name.Equals(ReferencedHmiAssembly) => assemblyPathHmi,
            _ when assemblyName.Name.Equals(SiemensEngineeringAssemblyName) => assemblyPath,
            _ => null
        };

        if (!string.IsNullOrEmpty(path) && File.Exists(path))
        {
            try
            {
                //var opennessName = AssemblyName.GetAssemblyName(path);
                //var asm = Assembly.Load(opennessName);
                var asm = Assembly.LoadFrom(path);

                return asm;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        return null;
    }

    //public static Assembly? OnResolve(object? sender, ResolveEventArgs args)
    //{
    //    int index = args.Name.IndexOf(',');
    //    if (index == -1)
    //    {
    //        return null;
    //    }
    //    string name = args.Name.Substring(0, index) + ".dll";
    //    // Edit the following path according to your installed version of TIA Portal
    //    string path = Path.Combine(@$"C:\Program Files\Siemens\Automation\Portal V18\PublicAPI\V18\", name);
    //    string fullPath = Path.GetFullPath(path);
    //    if (File.Exists(fullPath))
    //    {
    //        return Assembly.LoadFrom(fullPath);
    //    }
    //    return null;
    //}



    /// <summary>
    /// Retrieve assembly by path and version
    /// </summary>
    /// <param name="version"></param>
    /// <returns></returns>
    public static List<string>? GetAssemblies(string? version)
    {
        var key = GetRegistryKey($@"{BasePath}\{version}");

        if (key != null)
        {
            try
            {
                var subKey = key.OpenSubKey("PublicAPI");
                if (subKey is not null)
                {
                    var subKeys = subKey.GetSubKeyNames().OrderBy(x => x).ToList();
                    
                    var result = (from item in subKeys
                                  where Convert.ToDecimal(item.Substring(0, 4), CultureInfo.InvariantCulture) >= Convert.ToDecimal(StrRequiredVersion.Substring(1, 4), CultureInfo.InvariantCulture)
                                  select item.Substring(0, 4)).ToList();

                    subKey.Dispose();

                    return result;
                }
            }
            finally
            {
                key.Dispose();
            }
        }

        return null;
    }

    /// <summary>
    /// Get version info from registry key
    /// </summary>
    /// <returns></returns>
    public static List<string> GetEngineeringVersions()
    {
        var key = GetRegistryKey(BasePath);

        if (key != null)
        {
            try
            {
                var names = key.GetSubKeyNames().OrderBy(x => x).ToList();

                foreach (var item in names.ToList())
                {
                    if(key.OpenSubKey(item)?.GetSubKeyNames().Contains("PublicAPI") == false)
                    {
                        names.Remove(item);
                    }
                }

                var result = (from item in names
                              where Convert.ToDecimal(item.Substring(0, 4), CultureInfo.InvariantCulture) >= Convert.ToDecimal(StrRequiredVersion.Substring(1, 4), CultureInfo.InvariantCulture)
                              select item.Substring(0, 4)).ToList();

                key.Dispose();

                return result;
            }
            finally
            {
                key.Dispose();
            }
        }

        return [];
    }

    private static RegistryKey? GetRegistryKey(string keyName)
    {
        var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        var key = baseKey.OpenSubKey(keyName);
        if (key is null)
        {
            baseKey.Dispose();
            baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Default);
            key = baseKey.OpenSubKey(keyName);
        }
        if (key is null)
        {
            baseKey.Dispose();
            baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            key = baseKey.OpenSubKey(keyName);
        }
        baseKey.Dispose();

        return key;
    }

    /// <summary>
    /// Retrieve the path from assembly by version
    /// </summary>
    /// <param name="version"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static string? GetAssemblyPath(string? version, string? assembly)
    {
        var libraries = GetOpennessLibraries();
        var portalVersion = new Version(version);
        var apiVersion = new Version(assembly);
        var libraryVersion = libraries.Where(e => e.TiaPortalVersion.Major == portalVersion.Major &&
                                                 e.TiaPortalVersion.Minor == portalVersion.Minor).SingleOrDefault(e => e.PublicApiVersion.Major == apiVersion.Major &&
                                                                                                                       e.PublicApiVersion.Minor == apiVersion.Minor);
        //_assemblyPath = libraryVersion is null ? string.Empty : libraryVersion.LibraryFilePath;
        assemblyPath = libraryVersion.LibraryFilePath;

        return assemblyPath;
    }

    /// <summary>
    /// Check if openness api is installed
    /// </summary>
    /// <returns></returns>
    public static bool IsOpennessInstalled()
    {
        var engineeringVersion = GetEngineeringVersions();

        var requiredVersion = (from version in engineeringVersion
                               where Convert.ToDecimal(version, CultureInfo.InvariantCulture) >= Convert.ToDecimal(StrRequiredVersion.Substring(1, 4), CultureInfo.InvariantCulture)
                               select version).FirstOrDefault();


        return !string.IsNullOrEmpty(requiredVersion);
    }

    /// <summary>
    /// Get installed openness libraries
    /// </summary>
    /// <returns></returns>
    public static IReadOnlyList<OpennessVersion> GetOpennessLibraries()
    {
        var opennessVersions = new List<OpennessVersion>();

        if (Environment.Is64BitOperatingSystem)
        {
            using var baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            using var registryKey = baseKey.OpenSubKey(BasePath, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);
            var tiaPortalVersions = registryKey?.GetSubKeyNames();
            if (tiaPortalVersions != null)
            {
                foreach (var tiaPortalVersion in tiaPortalVersions)
                {
                    using var publicApi = registryKey?.OpenSubKey(Path.Combine(tiaPortalVersion, "PublicAPI"), RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);
                    var publicApis = publicApi?.GetSubKeyNames();
                    if (publicApis != null)
                    {
                        foreach (var publicApiVersion in publicApis)
                        {
                            using var openness = publicApi?.OpenSubKey(publicApiVersion, RegistryKeyPermissionCheck.ReadSubTree, RegistryRights.ReadKey);
                            var library = openness?.GetValue(SiemensEngineeringAssemblyName) as string;

                            if (!string.IsNullOrWhiteSpace(library) && File.Exists(library))
                            {
                                var portalVersion = new Version(tiaPortalVersion);
                                var apiVersion = new Version(publicApiVersion);
                                var opennessVersion = new Models.OpennessVersion(portalVersion, library, apiVersion);
                                opennessVersions.Add(opennessVersion);
                            }
                        }
                    }
                }
            }
        }

        return opennessVersions.AsReadOnly();
    }

    public static string GetOpennessNamespace(Version tiaVersion)
    {
        return tiaVersion.Major switch
        {
            13 or 14 => "http://www.siemens.com/automation/Openness/SW/Interface/v1",
            15 => tiaVersion.Minor == 1 ? "http://www.siemens.com/automation/Openness/SW/Interface/v2" : "http://www.siemens.com/automation/Openness/SW/Interface/v3",
            16 => "http://www.siemens.com/automation/Openness/SW/Interface/v4",
            17 or 18 or 19 or 20 => "http://www.siemens.com/automation/Openness/SW/Interface/v5",
            _ => "http://www.siemens.com/automation/Openness/SW/Interface/v5",
        };
    }

    public static void SetWhiteList(Version? version)
    {
        var applicationStartupPath = AppContext.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
        //var key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
        string user = Environment.UserDomainName + "\\" + Environment.UserName;
        //RegistrySecurity rs = new RegistrySecurity();
        ////rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.WriteKey | RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.FullControl, AccessControlType.Allow));
        //rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.ReadKey | RegistryRights.Delete, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
        //rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.WriteKey | RegistryRights.ChangePermissions, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny));
        //rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.WriteKey, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));


        //using var key = GetRegistryKey($@"{BasePath}\{version?.ToString(2)}\Whitelist");
        //key?.SetAccessControl(rs);
        //using var whitelistKey = key?.OpenSubKey("Whitelist", true);

        SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
        NTAccount? account = sid.Translate(typeof(NTAccount)) as NTAccount;

        if (account is null) return;

        using var key = Registry.LocalMachine.OpenSubKey($@"{BasePath}\{version?.ToString(2)}\Whitelist", true);
        var rs = key?.GetAccessControl();
        rs?.AddAccessRule(new RegistryAccessRule(account, RegistryRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
        key?.SetAccessControl(rs!);


        RegistryKey? software;
        try
        {
            software = key?.OpenSubKey(AppDomain.CurrentDomain.FriendlyName)?.OpenSubKey("Entry", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            if( software is null )
            {
                software = key?.CreateSubKey(AppDomain.CurrentDomain.FriendlyName).CreateSubKey("Entry", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
            }
        }
        catch
        {
            software = key?.CreateSubKey(AppDomain.CurrentDomain.FriendlyName).CreateSubKey("Entry", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
        }

        
        DateTime lastWriteTimeUtc;
        HashAlgorithm hashAlgorithm = SHA256.Create();
        FileStream stream = File.OpenRead(applicationStartupPath);
        byte[] hash = hashAlgorithm.ComputeHash(stream);

        string convertedHash = Convert.ToBase64String(hash);
        software?.SetValue("FileHash", convertedHash);
        lastWriteTimeUtc = new FileInfo(applicationStartupPath).LastWriteTimeUtc;
        
        software?.SetValue("DateModified", lastWriteTimeUtc.ToString(@"yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        software?.SetValue("Path", applicationStartupPath);
    }

    private static void SetPermission()
    {
        //var id = AppDomain.CurrentDomain.Id;
        //RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).CreateSubKey(@"SOFTWARE\Classes\AppID\{CDCBCFCA-3CDC-436f-A4E2-0E02075250C2}", true);
        //key.SetValue("RunAs", "Interactive User");

        string user = Environment.UserDomainName + "\\" + Environment.UserName;

        RegistrySecurity rs = new RegistrySecurity();

        // Allow the current user to read and delete the key.
        rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.ReadKey | RegistryRights.Delete, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));

        // Prevent the current user from writing or changing the
        // permission set of the key. Note that if Delete permission
        // were not allowed in the previous access rule, denying
        // WriteKey permission would prevent the user from deleting the
        // key.
        rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.WriteKey | RegistryRights.ChangePermissions, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny));

        // Create the example key with registry security.
        RegistryKey rk = null;
        try
        {
            rk = Registry.CurrentUser.CreateSubKey("RegistryRightsExample", RegistryKeyPermissionCheck.Default, rs);
            Console.WriteLine("\r\nExample key created.");
            rk.SetValue("ValueName", "StringValue");
        }
        catch (Exception ex)
        {
            Console.WriteLine("\r\nUnable to create the example key: {0}", ex);
        }
        if (rk != null) rk.Close();

        rk = Registry.CurrentUser;

        RegistryKey rk2;

        // Open the key with read access.
        rk2 = rk.OpenSubKey("RegistryRightsExample", false);
        Console.WriteLine("\r\nRetrieved value: {0}", rk2.GetValue("ValueName"));
        rk2.Close();

        // Attempt to open the key with write access.
        try
        {
            rk2 = rk.OpenSubKey("RegistryRightsExample", true);
        }
        catch (SecurityException ex)
        {
            Console.WriteLine("\nUnable to write to the example key. Caught SecurityException: {0}", ex.Message);
        }
        if (rk2 != null) rk2.Close();

        // Attempt to change permissions for the key.
        try
        {
            rs = new RegistrySecurity();
            rs.AddAccessRule(new RegistryAccessRule(user, RegistryRights.WriteKey, InheritanceFlags.None, PropagationFlags.None, AccessControlType.Allow));
            rk2 = rk.OpenSubKey("RegistryRightsExample", false);
            rk2.SetAccessControl(rs);
            Console.WriteLine("\r\nExample key permissions were changed.");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine("\nUnable to change permissions for the example key. Caught UnauthorizedAccessException: {0}", ex.Message);
        }
        if (rk2 != null) rk2.Close();

        Console.WriteLine("\r\nPress Enter to delete the example key.");
        Console.ReadLine();

        try
        {
            rk.DeleteSubKey("RegistryRightsExample");
            Console.WriteLine("Example key was deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Unable to delete the example key: {0}", ex);
        }

        rk.Close();
    }
}
