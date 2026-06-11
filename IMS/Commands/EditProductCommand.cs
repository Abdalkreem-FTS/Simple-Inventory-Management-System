using IMS.Core;
using IMS.UI;
using Spectre.Console;

namespace IMS.Commands;

/// <summary>
/// Looks up a product and lets the user update any combination of
/// name, price, and quantity. Fields left blank are not changed.
/// </summary>
public sealed class EditProductCommand : ICommand
{
    public string Label => "[yellow]*[/]  Edit product";

    public bool Execute(Inventory inventory)
    {
        ConsoleUi.PrintSectionHeading("Edit Product");

        var name = ConsoleUi.PromptRequired("Product name to edit");
        var product = inventory.Find(name);

        if (product is null)
        {
            ConsoleUi.PrintFailure($"Product [white]{Markup.Escape(name)}[/] not found.");
            
            return true;
        }

        ConsoleUi.PrintProductLine(product);

        var newName = ConsoleUi.OptionalStringPrompt("New name",       product.Name);
        var newPrice = ConsoleUi.OptionalDecimalPrompt("New price ($)", product.Price);
        var newQuantity = ConsoleUi.OptionalIntPrompt("New quantity",      product.Quantity);

        if (newName is null && newPrice is null && newQuantity is null)
        {
            ConsoleUi.PrintInfo("No changes made.");
            
            return true;
        }

        var result = inventory.Edit(product.Name, newName, newPrice, newQuantity);

        if (result.IsSuccess)
        {
            ConsoleUi.PrintSuccess("Product updated successfully.");
        }
        else
        {
            ConsoleUi.PrintFailure(result.Error!);
        }

        return true;
    }
}