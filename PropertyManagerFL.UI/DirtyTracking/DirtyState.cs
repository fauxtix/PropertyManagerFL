using System.Text.Json;
using System.Text.Json.Serialization;

namespace PropertyManagerFL.UI.DirtyTracking;

/// <summary>
/// Snapshot-based dirty tracking for edit dialogs.
/// </summary>
/// <remarks>
/// Each dialog captures a deep clone of its model when it opens. Closing compares the
/// current model with that clone, so a stale "original" record from a previous dialog
/// can no longer produce a false positive - and a real edit can no longer be missed.
/// </remarks>
public static class DirtyState
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.General)
    {
        ReferenceHandler = ReferenceHandler.IgnoreCycles
    };

    /// <summary>
    /// Creates an isolated deep clone of the current model state.
    /// </summary>
    public static T? Snapshot<T>(T? model) where T : class
    {
        if (model is null)
            return null;

        var json = JsonSerializer.Serialize(model, Options);
        return JsonSerializer.Deserialize<T>(json, Options);
    }

    /// <summary>
    /// Returns true when the current model differs from the captured snapshot.
    /// A missing snapshot on either side is treated as a change.
    /// </summary>
    public static bool HasChanges<T>(T? current, T? snapshot) where T : class
    {
        if (current is null || snapshot is null)
            return current is not null || snapshot is not null;

        return !string.Equals(
            JsonSerializer.Serialize(current, Options),
            JsonSerializer.Serialize(snapshot, Options),
            StringComparison.Ordinal);
    }
}
