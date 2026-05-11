namespace Financa.Domain.Common;

public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public T? Value { get; private set; }
    public string? Error { get; private set; }
    public string? ErrorCode { get; private set; }

    private Result() { }

    public static Result<T> Success(T value) =>
        new() { IsSuccess = true, Value = value };

    public static Result<T> Failure(string error, string? errorCode = null) =>
        new() { IsSuccess = false, Error = error, ErrorCode = errorCode };

    public bool IsFailure => !IsSuccess;
}

public class Result
{
    public bool IsSuccess { get; private set; }
    public string? Error { get; private set; }
    public string? ErrorCode { get; private set; }

    private Result() { }

    public static Result Success() => new() { IsSuccess = true };

    public static Result Failure(string error, string? errorCode = null) =>
        new() { IsSuccess = false, Error = error, ErrorCode = errorCode };

    public bool IsFailure => !IsSuccess;
}

public static class ErrorCodes
{
    public const string InvalidCredentials = "AUTH_INVALID_CREDENTIALS";
    public const string UserNotFound       = "AUTH_USER_NOT_FOUND";
    public const string UserInactive       = "AUTH_USER_INACTIVE";
    public const string EmailAlreadyExists = "AUTH_EMAIL_EXISTS";
    public const string ValidationError    = "VALIDATION_ERROR";
    public const string PasswordMismatch   = "AUTH_PASSWORD_MISMATCH";
    public const string InvalidCurrentPass = "AUTH_INVALID_CURRENT_PASSWORD";
    public const string Unauthorized       = "AUTH_UNAUTHORIZED";
    public const string InvalidResetCode   = "AUTH_INVALID_RESET_CODE";
    public const string ExpiredResetCode   = "AUTH_EXPIRED_RESET_CODE";
}
