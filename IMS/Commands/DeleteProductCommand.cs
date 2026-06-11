using IMS.Core;
using IMS.UI;
using Spectre.Console;

namespace IMS.Commands;

/// <summary>
/// Looks up a product, shows it to the user, asks for confirmation,
/// then removes it from the inventory.
/// </summary>
public sealed class DeleteProductCommand : ICommand
{
    public string Label => "[red]-[/]  Delete product";

    public bool Execute(Inventory inventory)
    {
        ConsoleUi.PrintSectionHeading("Delete Product");

        var name = ConsoleUi.PromptRequired("Product name to delete");
        var product = inventory.Find(name);

        if (product is null)
        {
            ConsoleUi.PrintFailure($"Product [white]{Markup.Escape(name)}[/] not found.");
            
            return true;
        }

        ConsoleUi.PrintProductLine(product);

        var confirmed = AnsiConsole.Prompt(
            new ConfirmationPrompt(
                $"  [grey46]Delete [white]{Markup.Escape(product.Name)}[/]? This cannot be undone.[/]")
            {
                DefaultValue = false,
            });

        if (!confirmed)
        {
            ConsoleUi.PrintInfo("Deletion cancelled.");
            
            return true;
        }

        var result = inventory.Remove(product.Name);

        if (result.IsSuccess)
        {
            ConsoleUi.PrintSuccess($"[white]{Markup.Escape(product.Name)}[/] removed from inventory.");
        }
        else
        {
            ConsoleUi.PrintFailure(result.Error!);
        }

        return true;
    }
}