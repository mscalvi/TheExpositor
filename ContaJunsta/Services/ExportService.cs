using Microsoft.JSInterop;

namespace ContaJunsta.Services;

public class ExportService
{
    private readonly IJSRuntime _js;
    public ExportService(IJSRuntime js) => _js = js;

    public Task SaveTextAsync(string filename, string content) =>
        _js.InvokeVoidAsync("ContaJunstaFiles.saveText", filename, content).AsTask();

    // helpers STATIC (para usar como ExportService.CentsToPtbr(...))
    public static string CentsToPtbr(int cents) => (cents / 100.0).ToString("N2");

    public static string CsvEscape(string s)
    {
        if (s is null) return "";
        var needQuotes = s.Contains(',') || s.Contains('"') || s.Contains('\n') || s.Contains('\r');
        if (!needQuotes) return s;
        return "\"" + s.Replace("\"", "\"\"") + "\"";
    }
}
