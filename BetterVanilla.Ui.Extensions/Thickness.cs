namespace BetterVanilla.Ui.Extensions;

/// <summary>
/// Represents thickness values for margins and padding.
/// </summary>
public readonly struct Thickness
{
    public float Left { get; }
    public float Top { get; }
    public float Right { get; }
    public float Bottom { get; }

    public Thickness(float uniform) : this(uniform, uniform, uniform, uniform)
    {
    }

    public Thickness(float horizontal, float vertical) : this(horizontal, vertical, horizontal, vertical)
    {
    }

    public Thickness(float left, float top, float right, float bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public static Thickness Zero => new(0);

    /// <summary>
    /// Parses a thickness string. Formats: "10", "10,5", "10,5,10,5"
    /// </summary>
    public static Thickness Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return Zero;

        var parts = value.Split(',');
        return parts.Length switch
        {
            1 => new Thickness(float.Parse(parts[0].Trim())),
            2 => new Thickness(float.Parse(parts[0].Trim()), float.Parse(parts[1].Trim())),
            4 => new Thickness(
                float.Parse(parts[0].Trim()),
                float.Parse(parts[1].Trim()),
                float.Parse(parts[2].Trim()),
                float.Parse(parts[3].Trim())),
            _ => Zero
        };
    }

    public override string ToString() => $"{Left},{Top},{Right},{Bottom}";
}