namespace DocumentFormat.OpenXml.Wordprocessing;

public static class RunExtensions
{
    public static void SetStyle(this Run run, string? styleId)
    {
        if (styleId is null) return;

        run.RunProperties ??= new RunProperties();
        run.RunProperties.RunStyle ??= new RunStyle();
        run.RunProperties.RunStyle.Val = styleId;
    }

    public static RunProperties GetOrCreateProperties(this Run run)
    {
        run.RunProperties ??= new RunProperties();
        return run.RunProperties;
    }
}
