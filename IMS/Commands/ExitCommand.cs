using IMS.Core;
using IMS.UI;
using Spectre.Console;

namespace IMS.Commands;

/// <summary>
/// Signals the main loop to terminate.
/// Returns <c>false</c> from <see cref="Execute"/> — the only command that does so.
/// </summary>
public sealed class ExitCommand : ICommand
{
    public string Label => "[grey46]x[/]  Exit";

    public bool Execute(Inventory inventory)
    {
        ConsoleUi.RenderHeader();
        AnsiConsole.MarkupLine("  [grey46]Goodbye. All inventory data has been cleared.[/]\n");
        AnsiConsole.Cursor.Show();

        return false;
    }
}