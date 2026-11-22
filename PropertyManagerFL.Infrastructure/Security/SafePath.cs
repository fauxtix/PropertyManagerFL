namespace PropertyManagerFL.Infrastructure.Security;

/// <summary>
/// Helpers to keep file operations inside a known root folder.
/// </summary>
public static class SafePath
{
    /// <summary>
    /// Returns only the final segment of a client-supplied file name.
    /// Rejects empty names, "." / ".." and names containing invalid file-name characters.
    /// </summary>
    public static string FileName(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Invalid file name.", nameof(value));
        }

        var fileName = Path.GetFileName(value);
        if (string.IsNullOrWhiteSpace(fileName) ||
            fileName is "." or ".." ||
            fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
        {
            throw new ArgumentException("Invalid file name.", nameof(value));
        }

        return fileName;
    }

    /// <summary>
    /// Returns a plain SQL identifier only when it matches <c>letter, digit or underscore</c>.
    /// Used for the generic lookup-table repository where identifiers cannot be parameterized.
    /// </summary>
    public static string SqlIdentifier(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) ||
            value.Any(c => !char.IsAsciiLetterOrDigit(c) && c != '_'))
        {
            throw new ArgumentException("Invalid SQL identifier.", nameof(value));
        }

        return value;
    }

    /// <summary>
    /// Combines a trusted root with client-supplied segments and verifies that the
    /// normalized result is still inside the root. Throws UnauthorizedAccessException otherwise.
    /// </summary>
    public static string CombineContained(string root, params string?[] parts)
    {
        if (string.IsNullOrWhiteSpace(root))
        {
            throw new ArgumentException("Invalid root path.", nameof(root));
        }

        var fullRoot = Path.TrimEndingDirectorySeparator(Path.GetFullPath(root)) + Path.DirectorySeparatorChar;
        var segments = new[] { root }.Concat(parts.Select(p => p ?? string.Empty)).ToArray();
        var combined = Path.GetFullPath(Path.Combine(segments));

        if (!combined.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("The path is outside the permitted root.");
        }

        return combined;
    }
}
