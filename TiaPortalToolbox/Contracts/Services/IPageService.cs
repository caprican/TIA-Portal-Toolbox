﻿using System.Windows.Controls;

namespace TiaPortalToolbox.Contracts.Services;

public interface IPageService
{
    Type GetPageType(string? key);

    Page? GetPage(string? key);
}
