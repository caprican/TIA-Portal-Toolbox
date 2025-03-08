using System.Globalization;

using DocumentFormat.OpenXml;

namespace TiaPortalToolbox.Doc.Contracts.Builders;

public interface IPageBuilder
{
    internal OpenXmlElement[] Build(CultureInfo culture);
}
