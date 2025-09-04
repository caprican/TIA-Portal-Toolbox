using System.Collections.ObjectModel;
using System.IO;
using System.Globalization;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Microsoft.Extensions.Options;

using TiaPortalOpenness.Contracts.Services;

using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.Doc.Contracts.Builders;

using TiaPortalOpenness.Models.ProjectTree.Plc;
using TiaPortalOpenness.Models;

namespace TiaPortalToolbox.ViewModels;

public class DocumentViewModel(IShellWindow shellWindow, IDialogCoordinator dialogCoordinator, IOpennessService opennessService,
                               IOptions<Doc.Models.DocumentSettings> settings,
                               IPlcService plcService, IDocumentBuilder documentBuilder) : ObservableObject, INavigationAware
{
    private readonly IShellWindow shellWindow = shellWindow;
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;
    private readonly IOpennessService opennessService = opennessService;
    private readonly IPlcService plcService = plcService;
    private readonly IDocumentBuilder documentBuilder = documentBuilder;

    private readonly Doc.Models.DocumentSettings? settings = settings?.Value;

    private ICommand? buildDocumentCommand;
    private ICommand? loadDocumentCommand;

    private List<CultureInfo>? projectLangages;
    private CultureInfo? referenceLanguage;
    private CultureInfo? editingLanguage;

    private TiaPortalOpenness.Models.ProjectTree.Plc.Object? plcBlock;

    private string? title;
    private string? author;
    private string? library;
    private string? family;
    private string? shortDescription;
    private string? description;
    private string? folderName;
    private List<Core.Models.PlcBlockLog>? logs;
    private List<InterfaceMember>? interfaceMembers;
    private ObservableCollection<TiaPortalOpenness.Models.ProjectTree.Plc.Object>? derivedTypes = null;
    private ObservableCollection<TiaPortalOpenness.Models.ProjectTree.Object>? plcObjects = null;

    public List<CultureInfo>? ProjectLangages
    {
        get => projectLangages;
        set => SetProperty(ref projectLangages, value);
    }

    public CultureInfo? ReferenceLanguage
    {
        get => referenceLanguage;
        set
        {
            SetProperty(ref referenceLanguage, value);
        }
    }

    public CultureInfo? EditingLanguage
    {
        get => editingLanguage;
        set
        {
            SetProperty(ref editingLanguage, value);
        }
    }

    public ICommand? BuildDocumentCommand => buildDocumentCommand ??= new AsyncRelayCommand(OnBuildDocument);
    public ICommand? LoadDocumentCommand => loadDocumentCommand ??= new AsyncRelayCommand<TiaPortalOpenness.Models.ProjectTree.Object?>(OnLoadDocument);
    public TiaPortalOpenness.Models.ProjectTree.Plc.Object? PlcBlock
    {
        get => plcBlock;
        set
        {
            SetProperty(ref plcBlock, value);

            Title = ReferenceLanguage is not null ? plcBlock?.Title?[ReferenceLanguage] : null;
            Author = ReferenceLanguage is not null ? plcBlock?.Author?[ReferenceLanguage] : null;
            Library = ReferenceLanguage is not null ? plcBlock?.Library?[ReferenceLanguage] : null;
            Family = ReferenceLanguage is not null ? plcBlock?.Family?[ReferenceLanguage] : null;

            ShortDescription = ReferenceLanguage is not null ? plcBlock?.Comment?[ReferenceLanguage] ?? plcBlock?.Function?[ReferenceLanguage] : null;

            Logs = ReferenceLanguage is not null ? plcBlock?.Logs?[ReferenceLanguage] : null;

            InterfaceMembers = ReferenceLanguage is not null ? plcBlock?.Members?[ReferenceLanguage] : null;

            Description = ReferenceLanguage is not null ? plcBlock?.Descriptions?[ReferenceLanguage] : null;
            
        }
    }
    public ObservableCollection<TiaPortalOpenness.Models.ProjectTree.Plc.Object>? DerivedTypes
    {
        get => derivedTypes;
        set => SetProperty(ref derivedTypes, value);
    }
    public ObservableCollection<TiaPortalOpenness.Models.ProjectTree.Object>? PlcObjects
    {
        get => plcObjects;
        set => SetProperty(ref plcObjects, value);
    }

    public string? Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }

    public string? Author
    {
        get => author;
        set => SetProperty(ref author, value);
    }

    public string? Library
    {
        get => library;
        set => SetProperty(ref library, value);
    }

    public string? Family
    {
        get => family;
        set => SetProperty(ref family, value);
    }

    public string? ShortDescription
    {
        get => shortDescription;
        set => SetProperty(ref shortDescription, value);
    }

    public List<Core.Models.PlcBlockLog>? Logs
    {
        get => logs;
        set => SetProperty(ref logs, value);
    }

    public List<InterfaceMember>? InterfaceMembers
    {
        get => interfaceMembers;
        set => SetProperty(ref interfaceMembers, value);
    }

    public string? Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    public string? FolderName 
    { 
        get => folderName; 
        set => SetProperty(ref folderName, value);
    }

    public void OnNavigatedFrom()
    {
        shellWindow.SelectedItemChanged -= OnSelectedItemChanged;
    }

    public void OnNavigatedTo(object parameter)
    {
        shellWindow.SelectedItemChanged += OnSelectedItemChanged;

        if(parameter is TiaPortalOpenness.Models.ProjectTree.Object item)
        {
            OnSelectedItemChanged(this, item);
        }

        ProjectLangages = opennessService.ProjectLanguages;
        EditingLanguage = opennessService.EditingLanguage;
        ReferenceLanguage = opennessService.ReferenceLanguage;
    }

    private void OnSelectedItemChanged(object sender, TiaPortalOpenness.Models.ProjectTree.Object? e)
    {
        LoadDocumentCommand?.Execute(e);
    }

    public void LoadCompleted()
    {

    }

    private async Task OnLoadDocument(TiaPortalOpenness.Models.ProjectTree.Object? e)
    {
        if (e is null) return;

        PlcObjects = [];
        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, Properties.Resources.DocumentPageLoadTitle, e.Name);
        progress.SetIndeterminate();

        FolderName = null;

        switch (e)
        {
            case TiaPortalOpenness.Models.ProjectTree.Plc.Blocks.Object plcObject:
                await GetPlcBlock(plcObject);
                PlcBlock = plcObject;
                PlcObjects?.Add(plcObject);
                break;
            case TiaPortalOpenness.Models.ProjectTree.Plc.Type plcType:
                await GetPlcBlock(plcType);
                PlcBlock = plcType;
                PlcObjects?.Add(plcType);
                break;
            case Item plcItem:
                FolderName = plcItem.Name;
                await RecursiveBlock(plcItem, progress);
                break;
        }

        await progress.CloseAsync();

    }

    private async Task RecursiveBlock(TiaPortalOpenness.Models.ProjectTree.Plc.Object plcObject, ProgressDialogController progressDialog)
    {
        if(plcObject.Items?.Count > 0)
        {
            foreach (var item in plcObject.Items)
            {
                switch(item)
                {
                    case TiaPortalOpenness.Models.ProjectTree.Plc.Blocks.Object plcBlock:
                        progressDialog.SetMessage(plcBlock.Name);
                        await GetPlcBlock(plcBlock);
                        PlcObjects?.Add(plcBlock);
                        break;
                    case Item plcItem:
                        await RecursiveBlock(plcItem, progressDialog);
                        break;
                }
            }
        }
    }

    private async Task GetPlcBlock(TiaPortalOpenness.Models.ProjectTree.Plc.Blocks.Object plcObject)
    {
        if (PlcObjects?.Any(a => a.Name == plcObject.Name) != true)
            await plcService.GetMetaDataBlockAsync(plcObject);

        if (plcObject?.Members?.First() is not null)
        {
            foreach (var member in plcObject.Members.First().Value.Where(w => !string.IsNullOrEmpty(w.DerivedType)))
            {
               
                if(PlcObjects?.Any(a => a.Name == member.DerivedType) == true)
                {
                        
                }
                else if (DerivedTypes?.Any(a => a.Name == member.DerivedType) != true)
                {
                    var derivedType = await plcService.GetItem(member.DerivedType!);
                    if (derivedType is not null)
                    {
                        await plcService.GetMetaDataBlockAsync(derivedType);
                        DerivedTypes ??= [];
                        DerivedTypes.Add(derivedType);

                        if(PlcObjects?.Any(a => a.Name == derivedType.Name) != true)
                        {
                            PlcObjects?.Add(derivedType);
                        }
                    }

                }
            }
        }
    }

    private async Task GetPlcBlock(TiaPortalOpenness.Models.ProjectTree.Plc.Type plcType)
    {
        if (PlcObjects?.Any(a => a.Name == plcType.Name) != true)
            await plcService.GetMetaDataBlockAsync(plcType);

        if (plcType?.Members?.First() is not null)
        {
            foreach (var member in plcType.Members.First().Value.Where(w => !string.IsNullOrEmpty(w.DerivedType)))
            {

                if (PlcObjects?.Any(a => a.Name == member.DerivedType) == true)
                {

                }
                else if (DerivedTypes?.Any(a => a.Name == member.DerivedType) != true)
                {
                    var derivedType = await plcService.GetItem(member.DerivedType!);
                    if (derivedType is not null)
                    {
                        await plcService.GetMetaDataBlockAsync(derivedType);
                        DerivedTypes ??= [];
                        DerivedTypes.Add(derivedType);

                        if (PlcObjects?.Any(a => a.Name == derivedType.Name) != true)
                        {
                            PlcObjects?.Add(derivedType);
                        }
                    }

                }
            }
        }
    }

    private async Task OnBuildDocument()
    {
        var projectName = $"{opennessService.ProjectName}_{FolderName ?? string.Empty}{PlcBlock?.Name ?? string.Empty}";

        List<TiaPortalOpenness.Models.ProjectTree.Object> projectItems = PlcObjects is null ? [] : [.. PlcObjects];
        List<TiaPortalOpenness.Models.ProjectTree.Object> derivedTypes = DerivedTypes is null? [] : [.. DerivedTypes];

        if(ReferenceLanguage is null) return;

        if(settings is null)
        {
            await dialogCoordinator.ShowMessageAsync(App.Current.MainWindow.DataContext, Properties.Resources.DocumentPageMessageErrorTitle, Properties.Resources.DocumentPageMessageErrorSettingsText);
            return;
        }

        settings.ProjectPath = opennessService.ProjectPath ?? "";
        settings.UserFolderPath = opennessService.UserFolder ?? "";

#if DEBUG
        settings.DocumentPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads", $"{projectName}_{ReferenceLanguage.Name}.docx");
#else
        var saveFileDialog = new SaveFileDialog()
        {
            Title = "Select the location to save the document",
            FileName = $"{projectName}_{ReferenceLanguage?.Name}.docx",
            Filter = "Word Document (*.docx)|*.docx",
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        };
        if (saveFileDialog.ShowDialog() != true)
        {
            settings.DocumentPath = saveFileDialog.FileName;
        }
#endif

        settings.Culture = ReferenceLanguage ?? CultureInfo.DefaultThreadCurrentUICulture;

        await documentBuilder.CreateDocument(projectItems, derivedTypes);
        documentBuilder.Save();

        await dialogCoordinator.ShowMessageAsync(App.Current.MainWindow.DataContext, Properties.Resources.DocumentPageMessageBuildedTitle, Properties.Resources.DocumentPageMessageBuildedText);

#if DEBUG
        System.Diagnostics.Process.Start(settings.DocumentPath);
#endif
    }
}
