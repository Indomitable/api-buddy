namespace Api.Buddy.Main.Logic.Models.Request;

public class QueryParam : KeyValueParam
{
    public QueryParam(int index, string name, string value, bool selected) : base(index, name, value, selected)
    {
    }
}
