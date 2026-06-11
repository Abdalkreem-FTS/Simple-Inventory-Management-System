using IMS.Core;
using IMS.UI;
using Spectre.Console;

namespace IMS.Commands;

/// <summary>Prompts the user for product details and adds it to the inventory.</summary>
public sealed class AddProductCommand : ICommand
{
    public string Label => "[steelblue1]+[/]  Add product";

    public bool Execute(Inventory inventory)
    {
        ConsoleUi.PrintSectionHeading("Add Product");

        var name = ConsoleUi.PromptRequired("Name");

        var price = AnsiConsole.Prompt(
            new TextPrompt<decimal>("  [grey46]›[/] [white]Price ($):[/]")
                .PromptStyle("white")
                .Validate(v => v >= 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Price must be ≥ 0.[/]")));

        var quantity = AnsiConsole.Prompt(
            new TextPrompt<int>("  [grey46]›[/] [white]Quantity:[/]")
                .PromptStyle("white")
                .Validate(v => v >= 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Quantity must be ≥ 0.[/]")));

        var result = inventory.Add(new Product(name, price, quantity));

        if (result.IsSuccess)
        {
            ConsoleUi.PrintSuccess($"[white]{Markup.Escape(name)}[/] added to inventory.");
        }
        else
        {
            ConsoleUi.PrintFailure(result.Error!);
        }

        return true;
    }
}