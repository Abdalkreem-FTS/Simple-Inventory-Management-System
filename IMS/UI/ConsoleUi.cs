using IMS.Commands;
using IMS.Core;
using Spectre.Console;

namespace IMS.UI;

/// <summary>
/// Central home for every terminal rendering and input helper.
/// No business logic lives here — only how things look and how input is collected.
/// </summary>
public static class ConsoleUi
{
    // ── Layout ────────────────────────────────────────────────────────────────

    /// <summary>
    /// Clears the screen and redraws the application header.
    /// Called at the start of every screen transition.
    /// </summary>
    public static void RenderHeader()
    {
        AnsiConsole.Clear();

        AnsiConsole.Write(
            new FigletText("Inventory")
                .Centered()
                .Color(Color.SteelBlue1));

        AnsiConsole.Write(
            new Rule("[grey46]Simple Inventory Management System[/]")
                .RuleStyle("grey23")
                .Centered());

        AnsiConsole.WriteLine();
    }

    /// <summary>Prints a steelblue section heading, e.g. "Add Product".</summary>
    public static void PrintSectionHeading(string title)
        => AnsiConsole.MarkupLine($"  [steelblue1]{Markup.Escape(title)}[/]\n");

    /// <summary>
    /// Renders a one-line product summary used in Edit and Delete
    /// to confirm which product was found before the user commits.
    /// </summary>
    public static void PrintProductLine(Product product)
        => AnsiConsole.MarkupLine(
            $"\n  Found: [white]{Markup.Escape(product.Name)}[/]  " +
            $"[steelblue1_1]${product.Price:F2}[/]  " +
            $"qty [white]{product.Quantity}[/]\n");
    
    public static void PrintSuccess(string markup)
        => AnsiConsole.MarkupLine($"\n  [green]✓[/]  [white]{markup}[/]\n");

    public static void PrintFailure(string markup)
        => AnsiConsole.MarkupLine($"\n  [red]✗[/]  [white]{markup}[/]\n");

    public static void PrintInfo(string markup)
        => AnsiConsole.MarkupLine($"\n  [steelblue1]›[/]  [grey]{markup}[/]\n");

    public static void Pause()
    {
        AnsiConsole.MarkupLine("  [grey23]Press any key to continue…[/]");
        
        Console.ReadKey(intercept: true);
    }
    
    /// <summary>
    /// Renders products as a rounded, color-coded table.
    /// Quantity is green (≥ 5), yellow (1–4), or red (0).
    /// </summary>
    public static void RenderTable(IReadOnlyList<Product> products)
    {
        if (products.Count == 0)
        {
            PrintInfo("No products in inventory yet.");
            return;
        }

        var table = new Table()
            .Border(TableBorder.Rounded)
            .BorderColor(Color.Grey46)
            .AddColumn(new TableColumn("[grey46]#[/]").RightAligned())
            .AddColumn(new TableColumn("[steelblue1]Product[/]"))
            .AddColumn(new TableColumn("[steelblue1]Price[/]").RightAligned())
            .AddColumn(new TableColumn("[steelblue1]Qty[/]").RightAligned())
            .Expand();

        for (var i = 0; i < products.Count; i++)
        {
            var p        = products[i];
            var qtyColor = p.Quantity == 0 ? "red" : p.Quantity < 5 ? "yellow" : "green";

            table.AddRow(
                $"[grey46]{i + 1}[/]",
                $"[white]{Markup.Escape(p.Name)}[/]",
                $"[steelblue1_1]${p.Price:F2}[/]",
                $"[{qtyColor}]{p.Quantity}[/]"
            );
        }

        AnsiConsole.Write(table);
    }
    
    /// <summary>
    /// Renders the interactive arrow-key menu and returns the chosen command.
    /// The inventory is passed in so the live stock count can be shown.
    /// </summary>
    public static ICommand ShowMenu(IReadOnlyList<ICommand> commands, Inventory inventory)
    {
        var count    = inventory.GetProducts().Count;
        var subtitle = count == 0
            ? "[grey46]inventory is empty[/]"
            : $"[grey46]{count} product{(count == 1 ? "" : "s")} in stock[/]";

        AnsiConsole.MarkupLine($"  {subtitle}\n");

        return AnsiConsole.Prompt(
            new SelectionPrompt<ICommand>()
                .Title("  [grey46]What would you like to do?[/]")
                .HighlightStyle(new Style(foreground: Color.SteelBlue1))
                .PageSize(10)
                .UseConverter(c => c.Label)
                .AddChoices(commands));
    }
    
    /// <summary>Prompts for a required non-empty string.</summary>
    public static string PromptRequired(string label)
        => AnsiConsole.Prompt(
            new TextPrompt<string>($"  [grey46]›[/] [white]{label}:[/]")
                .PromptStyle("white")
                .Validate(v => string.IsNullOrWhiteSpace(v)
                    ? ValidationResult.Error("[red]This field cannot be empty.[/]")
                    : ValidationResult.Success()));

    /// <summary>
    /// Prompts for an optional string update.
    /// An empty response keeps the current value (returns <c>null</c>).
    /// </summary>
    public static string? OptionalStringPrompt(string label, string current)
    {
        var raw = AnsiConsole.Prompt(
            new TextPrompt<string>(
                $"  [grey46]›[/] [white]{label}[/] [grey46](leave blank to keep «{Markup.Escape(current)}»):[/]")
                .PromptStyle("white")
                .AllowEmpty());

        return string.IsNullOrWhiteSpace(raw) ? null : raw.Trim();
    }

    /// <summary>
    /// Prompts for an optional <see cref="decimal"/> update.
    /// Uses Spectre's typed prompt so invalid input is rejected with a clear message.
    /// An empty response keeps the current value (returns <c>null</c>).
    /// </summary>
    public static decimal? OptionalDecimalPrompt(string label, decimal current)
    {
        var raw = AnsiConsole.Prompt(
            new TextPrompt<string>(
                $"  [grey46]›[/] [white]{label}[/] [grey46](leave blank to keep «{current:F2}»):[/]")
                .PromptStyle("white")
                .AllowEmpty());

        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (decimal.TryParse(raw, out var value) && value >= 0)
        {
            return value;
        }
        
        PrintFailure("Invalid price — must be a number ≥ 0. No changes applied to price.");
        
        return null;
    }

    /// <summary>
    /// Prompts for an optional <see cref="int"/> update.
    /// Uses <see cref="int.TryParse(string?, out int)"/> so invalid input is rejected with a clear message.
    /// An empty response keeps the current value (returns <c>null</c>).
    /// </summary>
    public static int? OptionalIntPrompt(string label, int current)
    {
        var raw = AnsiConsole.Prompt(
            new TextPrompt<string>(
                $"  [grey46]›[/] [white]{label}[/] [grey46](leave blank to keep «{current}»):[/]")
                .PromptStyle("white")
                .AllowEmpty());

        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        if (int.TryParse(raw, out var value) && value >= 0)
        {
            return value;
        }
        
        PrintFailure("Invalid quantity — must be a whole number ≥ 0. No changes applied to quantity.");
        
        return null;

    }
}