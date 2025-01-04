using Siemens.Engineering;
using Siemens.Engineering.Multiuser;

using TiaPortalToolbox.Core.Contrats.Services;

namespace TiaPortalToolbox.Openness.Services;

public class OpennessService : IOpennessService
{
    private readonly List<string>? engineeringVersions;
    private Version? engineeringVersion;
    internal protected TiaPortal? TiaPortal { get; set; }
    internal protected Project? Project { get; set; }
    internal protected MultiuserProject? MultiuserProject { get; set; }
    public List<string>? EngineeringVersions => engineeringVersions;
    public Version? EngineeringVersion => engineeringVersion;

    public OpennessService()
    {
        engineeringVersions = Helpers.Resolver.GetEngineeringVersions();
    }

    public void Initialize(string? engineeringVersion, string? opennessVersion)
    {
        if(string.IsNullOrEmpty(engineeringVersion) || string.IsNullOrEmpty(opennessVersion))
        {
            this.engineeringVersion = null;
            return;
        }

        if (Helpers.Resolver.GetAssemblyPath(engineeringVersion, opennessVersion) != null)
        {
            this.engineeringVersion = new Version(engineeringVersion);
        }
        else
        {
            this.engineeringVersion = null;
        }
    }

    public List<string>? GetOpennessVersions(string? engineeringVersion = null)
    {
        if (!string.IsNullOrEmpty(engineeringVersion))
        {
            return Helpers.Resolver.GetAssemblies(engineeringVersion);
        }

        return null;
    }

    public List<Models.TiaProcess> GetProcesses()
    {
        var tiaProcesses = new List<Models.TiaProcess>();
        foreach (var process in Siemens.Engineering.TiaPortal.GetProcesses())
        {
            tiaProcesses.Add(new Models.TiaProcess(process.Id, process.ProjectPath));
        }
        return tiaProcesses;
    }

    public void Start()
    {
        TiaPortal ??= new TiaPortal(TiaPortalMode.WithUserInterface);

        TiaPortal.Confirmation += TiaPortal_Confirmation;
        TiaPortal.Authentication += TiaPortal_Authentication;
        TiaPortal.Notification += TiaPortal_Notification;
        TiaPortal.Disposed += TiaPortal_Disposed;
    }

    public void Connect(Models.TiaProcess process)
    {
        TiaPortal = TiaPortal.GetProcess(process.Id).Attach();

        Project = TiaPortal.Projects.FirstOrDefault(f => f.IsPrimary);
        MultiuserProject = TiaPortal?.LocalSessions.FirstOrDefault(p => p.Project.IsPrimary)?.Project;

        //projectPath = Project?.Path ?? MultiuserProject?.Path;

        //referenceLanguage = Project?.LanguageSettings.ReferenceLanguage.Culture ?? MultiuserProject?.LanguageSettings.ReferenceLanguage.Culture ?? null;
        //if (Project?.LanguageSettings?.ActiveLanguages != null)
        //{
        //    foreach (var lang in Project.LanguageSettings.ActiveLanguages)
        //    {
        //        projectLanguages ??= new List<CultureInfo>();
        //        projectLanguages.Add(lang.Culture);
        //    }
        //}
        //else if (MultiuserProject?.LanguageSettings?.ActiveLanguages != null)
        //{
        //    foreach (var lang in MultiuserProject.LanguageSettings.ActiveLanguages)
        //    {
        //        projectLanguages ??= new List<CultureInfo>();
        //        projectLanguages.Add(lang.Culture);
        //    }
        //}

        //if (Project != null || MultiuserProject != null) ProjectOpened?.Invoke(this, EventArgs.Empty);
    }

    public void Close()
    {
        if (TiaPortal != null)
        {
            TiaPortal.Confirmation -= TiaPortal_Confirmation;
            TiaPortal.Authentication -= TiaPortal_Authentication;
            TiaPortal.Notification -= TiaPortal_Notification;
            TiaPortal.Disposed -= TiaPortal_Disposed;

            TiaPortal.Dispose();
            TiaPortal = null;
        }
    }

    private void TiaPortal_Disposed(object sender, System.EventArgs e)
    {
        if (TiaPortal != null)
        {
            TiaPortal.Confirmation -= TiaPortal_Confirmation;
            TiaPortal.Authentication -= TiaPortal_Authentication;
            TiaPortal.Notification -= TiaPortal_Notification;
            TiaPortal.Disposed -= TiaPortal_Disposed;

            TiaPortal.Dispose();
            TiaPortal = null;
        }
    }

    private void TiaPortal_Notification(object sender, NotificationEventArgs e)
    {

    }

    private void TiaPortal_Authentication(object sender, AuthenticationEventArgs e)
    {

    }

    private void TiaPortal_Confirmation(object sender, ConfirmationEventArgs e)
    {

    }
}
