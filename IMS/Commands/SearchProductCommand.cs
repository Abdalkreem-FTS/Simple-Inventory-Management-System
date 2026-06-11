using IMS.Core;
using IMS.UI;
using Spectre.Console;

namespace IMS.Commands;

/// <summary>
/// Searches for a product by exact name and renders it as a single-row table
/// so the display is consistent with <see cref="ViewProductsCommand"/>.
/// </summary>
public sealed class SearchProductCommand : ICommand
{
    public string Label => "[grey46]?[/]  Search product";

    public bool Execute(Inventory inventory)
    {
        ConsoleUi.PrintSectionHeading("Search Product");

        var name = ConsoleUi.PromptRequired("Product name");
        var product = inventory.Find(name);

        if (product is null)
        {
            ConsoleUi.PrintFailure($"No product found matching [white]{Markup.Escape(name)}[/].");
            
            return true;
        }

        AnsiConsole.WriteLine();
        ConsoleUi.RenderTable([product]);
        AnsiConsole.WriteLine();

        return true;
    }
}