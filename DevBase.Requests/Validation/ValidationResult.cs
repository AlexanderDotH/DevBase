namespace DevBase.Requests.Validation;

/// <summary>
/// Represents the result of a validation operation.
/// </summary>
public readonly struct ValidationResult
{
    public bool IsValid { get; }
    public string? ErrorMessage { get; }

    private ValidationResult(bool isValid, string? errorMessage)
    {
        IsValid = isValid;
        ErrorMessage = errorMessage;
    }

    public static ValidationResult Success() => new(true, null);
    public static ValidationResult Fail(string message) => new(false, message);

    public void ThrowIfInvalid()
    {
        if (!IsValid)
            throw new Exceptions.ValidationException("Url", ErrorMessage ?? "Validation failed");
    }
}
