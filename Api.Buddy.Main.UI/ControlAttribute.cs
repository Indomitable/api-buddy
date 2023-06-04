using System;

namespace Api.Buddy.Main.UI;

public sealed class ControlAttribute: Attribute
{
    public Type Control { get; }

    public ControlAttribute(Type control)
    {
        Control = control;
    }
}
