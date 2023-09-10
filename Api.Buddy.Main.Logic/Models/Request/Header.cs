namespace Api.Buddy.Main.Logic.Models.Request;

public class Header: KeyValueParam
{
    public Header(int index, string name, string value, bool selected) : base(index, name, value, selected)
    {
    }
}