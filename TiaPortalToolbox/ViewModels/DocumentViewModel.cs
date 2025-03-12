using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.Core.Contracts.Services;
using TiaPortalToolbox.Doc.Contracts.Builders;

namespace TiaPortalToolbox.ViewModels;

public class DocumentViewModel(IShellWindow shellWindow, IDialogCoordinator dialogCoordinator, IOpennessService opennessService,
                               IPlcService plcService, IDocumentBuilder documentBuilder) : ObservableObject, INavigationAware
{
    private readonly IShellWindow shellWindow = shellWindow;
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;
    private readonly IOpennessService opennessService = opennessService;
    private readonly IPlcService plcService = plcService;
    private readonly IDocumentBuilder documentBuilder = documentBuilder;

    private ICommand? buildDocumentCommand;
    private ICommand? loadDocumentCommand;

    private List<CultureInfo>? projectLangages;
    private CultureInfo? referenceLanguage;
    private CultureInfo? editingLanguage;

    private Core.Models.ProjectTree.Plc.Object? plcBlock;

    private string? title;
    private string? author;
    private string? library;
    private string? family;
    private string? shortDescription;
    private string? description;
    private List<Core.Models.PlcBlockLog>? logs;
    private List<Core.Models.InterfaceMember>? interfaceMembers;
    private ObservableCollection<Core.Models.ProjectTree.Plc.Object>? derivedTypes = null;

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
    public ICommand? LoadDocumentCommand => loadDocumentCommand ??= new AsyncRelayCommand<Core.Models.ProjectTree.Object?>(OnLoadDocument);

    public Core.Models.ProjectTree.Plc.Object? PlcBlock
    {
        get => plcBlock;
        set => SetProperty(ref plcBlock, value);
    }
    public ObservableCollection<Core.Models.ProjectTree.Plc.Object>? DerivedTypes
    {
        get => derivedTypes;
        set => SetProperty(ref derivedTypes, value);
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

    public List<Core.Models.InterfaceMember>? InterfaceMembers
    {
        get => interfaceMembers;
        set => SetProperty(ref interfaceMembers, value);
    }

    public string? Description
    {
        get => description;
        set => SetProperty(ref description, value);
    }

    public void OnNavigatedFrom()
    {
        shellWindow.SelectedItemChanged -= OnSelectedItemChanged;
    }

    public void OnNavigatedTo(object parameter)
    {
        shellWindow.SelectedItemChanged += OnSelectedItemChanged;

        if(parameter is Core.Models.ProjectTree.Object item)
        {
            OnSelectedItemChanged(this, item);
        }

        ProjectLangages = opennessService.ProjectLanguages;
        EditingLanguage = opennessService.EditingLanguage;
        ReferenceLanguage = opennessService.ReferenceLanguage;
    }

    private void OnSelectedItemChanged(object sender, Core.Models.ProjectTree.Object? e)
    {
        LoadDocumentCommand?.Execute(e);
    }

    public void LoadCompleted()
    {

    }

    private async Task OnLoadDocument(Core.Models.ProjectTree.Object? e)
    {
        var progress = await dialogCoordinator.ShowProgressAsync(App.Current.MainWindow.DataContext, "", "");
        progress.SetIndeterminate();

        switch (e)
        {
            case Core.Models.ProjectTree.Plc.Object plcItem:
                await plcService.GetMetaDataBlockAsync(plcItem);
                PlcBlock = plcItem;

                if(PlcBlock?.Members?.First() is not null)
                {
                    foreach (var member in PlcBlock.Members.First().Value.Where(w => !string.IsNullOrEmpty(w.DerivedType)))
                    {
                        if(!string.IsNullOrEmpty(member.DerivedType))
                        {
                            var derivedType = await plcService.GetItem(member.DerivedType!);
                            if(derivedType is not null)
                            { 
                                await plcService.GetMetaDataBlockAsync(derivedType);
                                DerivedTypes ??= [];
                                DerivedTypes.Add(derivedType);
                            }

                        }
                    }
                }

                Title = ReferenceLanguage is not null ? plcItem.Title?[ReferenceLanguage] : null;
                Author = ReferenceLanguage is not null ? plcItem.Author?[ReferenceLanguage] : null;
                Library = ReferenceLanguage is not null ? plcItem.Library?[ReferenceLanguage] : null;
                Family = ReferenceLanguage is not null ? plcItem.Family?[ReferenceLanguage] : null;

                ShortDescription = ReferenceLanguage is not null ? plcItem.Comment?[ReferenceLanguage] ?? plcItem.Function?[ReferenceLanguage] : null;
                    
                Logs = ReferenceLanguage is not null ? plcItem.Logs?[ReferenceLanguage] : null;

                InterfaceMembers = ReferenceLanguage is not null ? plcItem.Members?[ReferenceLanguage] : null;

                Description = ReferenceLanguage is not null ? plcItem.Descriptions?[ReferenceLanguage] : null;

                break;
        }

        await progress.CloseAsync();

    }

    private async Task OnBuildDocument()
    {
        var projectName = $"SKF libraries {PlcBlock?.Name}";

        List<Core.Models.ProjectTree.Object> projectItems = [PlcBlock];
        List<Core.Models.ProjectTree.Object> derivedTypes = [];
        if(DerivedTypes?.Count > 0)
        {
            projectItems.AddRange(DerivedTypes);
            derivedTypes.AddRange(DerivedTypes);
        }

        if(ReferenceLanguage is null) return;

        await documentBuilder.CreateDocument(projectItems, derivedTypes, ReferenceLanguage, @$"C:/Users/capri/Downloads/{projectName}_{ReferenceLanguage.Name}.docx");
        documentBuilder.Save();

        await dialogCoordinator.ShowMessageAsync(App.Current.MainWindow.DataContext, "Document created", "The document has been created successfully");
    }
}
