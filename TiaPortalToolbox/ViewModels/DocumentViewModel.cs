using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Media.Animation;

using CommunityToolkit.Mvvm.ComponentModel;

using Markdig;

using Microsoft.VisualBasic.Logging;

using TiaPortalToolbox.Contracts.ViewModels;
using TiaPortalToolbox.Contracts.Views;
using TiaPortalToolbox.Core.Contracts.Services;
using TiaPortalToolbox.Core.Models;
using TiaPortalToolbox.Doc.Contracts.Services;

namespace TiaPortalToolbox.ViewModels;

public class DocumentViewModel(IShellWindow shellWindow, IDialogCoordinator dialogCoordinator, IOpennessService opennessService,
                               IPlcService plcService, IMarkdownService markdownService) : ObservableObject, INavigationAware
{
    private readonly IShellWindow shellWindow = shellWindow;
    private readonly IDialogCoordinator dialogCoordinator = dialogCoordinator;
    private readonly IOpennessService opennessService = opennessService;
    private readonly IPlcService plcService = plcService;
    private readonly IMarkdownService markdownService = markdownService;

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

    public Core.Models.ProjectTree.Plc.Object? PlcBlock
    {
        get => plcBlock;
        set => SetProperty(ref plcBlock, value);
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

        ProjectLangages = opennessService.ProjectLanguages;
        EditingLanguage = opennessService.EditingLanguage;
        ReferenceLanguage = opennessService.ReferenceLanguage;
    }

    private void OnSelectedItemChanged(object sender, Core.Models.ProjectTree.Object e)
    {
        Task.Run(async () =>
        {
            switch (e)
            {
                case Core.Models.ProjectTree.Plc.Object plcItem:
                    await plcService.GetMetaDataBlockAsync(plcItem);

                    PlcBlock = plcItem;

                    Title = ReferenceLanguage is not null ? plcItem.Title?[ReferenceLanguage] : null;
                    Author = ReferenceLanguage is not null ? plcItem.Author?[ReferenceLanguage] : null;
                    Library = ReferenceLanguage is not null ? plcItem.Library?[ReferenceLanguage] : null;
                    Family = ReferenceLanguage is not null ? plcItem.Family?[ReferenceLanguage] : null;

                    ShortDescription = ReferenceLanguage is not null ? plcItem.Comment?[ReferenceLanguage] ?? plcItem.Function?[ReferenceLanguage] : null;
                    
                    Logs = ReferenceLanguage is not null ? plcItem.Logs?[ReferenceLanguage] : null;

                    InterfaceMembers = ReferenceLanguage is not null ? plcItem.Members?[ReferenceLanguage] : null;

                    Description = ReferenceLanguage is not null ? plcItem.Description?[ReferenceLanguage] : null;



                    var pipeline = new MarkdownPipelineBuilder()
                                            .UseAdvancedExtensions()
                                            .UseFigures()
                                            .Build();
                    var document = Markdig.Markdown.Parse(Description, pipeline);

                    markdownService.CreateDocX(document, "");

                    break;
            }
        });
    }


    public void LoadCompleted()
    {

    }
}
