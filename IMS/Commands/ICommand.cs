using IMS.Core;

namespace IMS.Commands;

/// <summary>
/// Represents a single action the user can take from the main menu.
/// Decouples the menu (which knows only labels) from the logic (which
/// knows only the inventory), so adding a new feature means adding one
/// new class rather than editing the switch in Program.cs.
/// </summary>
public interface ICommand
{
    /// <summary>The label shown in the interactive menu.</summary>
    string Label { get; }

    /// <summary>
    /// Runs the command against the given <paramref name="inventory"/>.
    /// Returns <c>true</c> to keep the application running,
    /// <c>false</c> to signal that the app should exit.
    /// </summary>
    bool Execute(Inventory inventory);
}