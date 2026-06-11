namespace IMS.Core;

/// <summary>
/// Represents the outcome of an inventory operation.
/// Replaces the mix of bool returns and thrown exceptions with a single,
/// consistent return type that carries both the success state and
/// an optional human-readable error message.
/// </summary>
public readonly record struct OperationResult
{
    public bool IsSuccess { get; init; }
    public string? Error   { get; init; }

    private OperationResult(bool isSuccess, string? error)
    {
        IsSuccess = isSuccess;
        Error     = error;
    }

    /// <summary>Creates a successful result.</summary>
    public static OperationResult Ok() => new(true, null);

    /// <summary>Creates a failed result with a reason.</summary>
    public static OperationResult Fail(string error) => new(false, error);
}