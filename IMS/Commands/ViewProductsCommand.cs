using IMS.Core;
using IMS.UI;
using Spectre.Console;

namespace IMS.Commands;

/// <summary>Renders the full product list as a formatted table.</summary>
public sealed class ViewProductsCommand : ICommand
{
    public string Label => "[steelblue1]=[/]  View all products";

    public bool Execute(Inventory inventory)
    {
        ConsoleUi.PrintSectionHeading("All Products");
        ConsoleUi.RenderTable(inventory.GetProducts());
        AnsiConsole.WriteLine();

        return true;
    }
}