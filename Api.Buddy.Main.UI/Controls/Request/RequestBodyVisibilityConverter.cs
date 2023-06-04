using Api.Buddy.Main.Logic.Models;
using Avalonia.Data.Converters;

namespace Api.Buddy.Main.UI.Controls.Request;

public class RequestBodyVisibilityConverter: FuncValueConverter<HttpMethod, bool>
{
    public RequestBodyVisibilityConverter() : base((method) => method != HttpMethod.GET)
    {
    }
}
