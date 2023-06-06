namespace Api.Buddy.Main.Logic.Models.Request;

public record QueryParam(int Index, string Name, string Value, bool Selected = true);
