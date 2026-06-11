using IMS.Commands;
using IMS.Core;
using IMS.UI;
using Spectre.Console;

var inventory = new Inventory();
AnsiConsole.Cursor.Hide();

// All available commands in menu order.
// To add a new feature: create a class that implements ICommand, add it here.
IReadOnlyList<ICommand> commands =
[
    new AddProductCommand(),
    new ViewProductsCommand(),
    new EditProductCommand(),
    new DeleteProductCommand(),
    new SearchProductCommand(),
    new ExitCommand(),
];


// Main loop (Entry point)

var running = true;

while (running)
{
    ConsoleUi.RenderHeader();
    var command = ConsoleUi.ShowMenu(commands, inventory);

    ConsoleUi.RenderHeader();
    running = command.Execute(inventory);

    if (running)
    {
        ConsoleUi.Pause();
    }
}