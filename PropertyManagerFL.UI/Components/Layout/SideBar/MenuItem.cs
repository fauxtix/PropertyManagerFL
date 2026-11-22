namespace PropertyManagerFL.UI.Components.Layout.SideBar;

/// <summary>Optional badge shown at the end of a menu item.</summary>
public sealed class Suffix
{
    public string Title { get; }
    public string Class { get; }

    public Suffix(string title, string cssClass)
    {
        Title = title;
        Class = cssClass;
    }
}

/// <summary>
/// Menu model shared by <c>SidebarComponent</c> and the recursive
/// <c>MenuItemComponent</c>. Mirrors the reference repo's sidebarpro model.
/// </summary>
public sealed class MenuItem
{
    private const int ItemHeight = 40;

    public string? Href { get; set; }
    public string? Title { get; set; }
    public string? Icon { get; set; }
    public Suffix? Suffix { get; set; }
    public List<MenuItem>? ChildMenuItems { get; set; }
    public bool IsActive { get; set; }

    private bool _isOpened;

    public bool IsOpened
    {
        get => _isOpened;
        set
        {
            _isOpened = value;
            if (!value && ChildMenuItems is not null)
            {
                foreach (var child in ChildMenuItems)
                {
                    child.IsOpened = false;
                }
            }
        }
    }

    public bool HasChildMenuItems => ChildMenuItems is { Count: > 0 };

    public MenuItem(
        string? href = null,
        string? title = null,
        string? icon = null,
        Suffix? suffix = null,
        List<MenuItem>? childMenuItems = null)
    {
        Href = href;
        Title = title;
        Icon = icon;
        Suffix = suffix;
        ChildMenuItems = childMenuItems;
    }

    /// <summary>Height used by the inline style of the animated submenu.</summary>
    public int CalcSubMenuHeight()
    {
        if (!HasChildMenuItems || !IsOpened)
        {
            return 0;
        }

        var height = 0;
        foreach (var child in ChildMenuItems!)
        {
            height += child.CalcSubMenuHeight() + ItemHeight;
        }

        return height;
    }

    /// <summary>
    /// True when this item (or one of its descendants) points at the current route.
    /// </summary>
    public bool MatchesPath(string relativePath)
    {
        var target = (Href ?? string.Empty).Trim('/');
        var current = relativePath.Trim('/');

        var isMatch = target.Length == 0
            ? current.Length == 0
            : current.Equals(target, StringComparison.OrdinalIgnoreCase) ||
              current.StartsWith(target + "/", StringComparison.OrdinalIgnoreCase);

        return isMatch || ChildMenuItems?.Any(child => child.MatchesPath(relativePath)) == true;
    }
}
