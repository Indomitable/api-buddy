using System;

namespace Api.Buddy.Main.UI.Models;

public interface IBody<out T>
{
    T Content { get; }

    bool IsReadOnly { get; }
}

public class EmptyBody : IBody<string>
{
    private EmptyBody()
    {
        Content = string.Empty;
        IsReadOnly = true;
    }

    public string Content { get; }
    public bool IsReadOnly { get; }

    private static EmptyBody? instance = null;

    public static EmptyBody Instance
    {
        get { return instance ??= new EmptyBody(); }
    }
}

public record TextBody(string Content, bool IsReadOnly) : IBody<string>;

public record BinaryBody(byte[] Content, bool IsReadOnly) : IBody<byte[]>;
