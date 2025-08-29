using System.Globalization;

namespace TiaPortalToolbox.Core.Contracts.Builders;

public interface IPageBuilder
{
    //public void Build(CultureInfo culture);
    public string Build(CultureInfo culture);
}
