﻿using System.Globalization;

namespace TiaPortalToolbox.Doc.Contracts.Builders;

public interface IDocumentBuilder
{
    public Task CreateDocument(List<Core.Models.ProjectTree.Object> projetItems, List<Core.Models.ProjectTree.Object> derivedItems, CultureInfo culture, string path);
    public void Save();


}
