using System;

namespace API.DTOs;

public class APIResponse
{
    public bool? IsSuccess { get; init; }
    public int? StatusCode { get; init; }
    public string? Title { get; init; }
    public string? Message { get; init; }
    public string? Details { get; init; }
    public bool? IsHtmlEnabled { get; init; }
    public bool? DisplayByDefault { get; init; }
    public bool? ShowWithToastr { get; init; }
    public IEnumerable<string>? Errors { get; init; }

    public APIResponse(
        bool isSuccess,
        int statusCode,
        string? title = null,
        string? message = null,
        string? details = null,
        bool isHtmlEnabled = false,
        bool displayByDefault = false,
        bool showWithToastr = false,
        IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        StatusCode = statusCode;
        Title = title ?? GetDefaultTitle(statusCode);
        Message = message ?? GetDefaultMessage(statusCode);
        Details = details;
        IsHtmlEnabled = isHtmlEnabled;
        DisplayByDefault = displayByDefault;
        ShowWithToastr = showWithToastr;
        Errors = errors;
    }

    private static string GetDefaultTitle(int statusCode)
    {
        return statusCode switch
        {
            200 => "Success",
            201 => "Created",
            400 => "Bad Request",
            401 => "Unauthorized",
            403 => "Forbidden",
            404 => "Not Found",
            500 => "Internal Server Error",
            _ => "Error"
        };
    }

    private string GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            200 => "The request was successful.",
            201 => "The resource was created successfully.",
            400 => "The request could not be understood or was missing required parameters.",
            401 => "Authentication failed or user does not have permissions for the desired action.",
            403 => "Access to the requested resource is forbidden.",
            404 => "The requested resource could not be found.",
            500 => "An error occurred on the server.",
            _ => "An unexpected error occurred."
        };
    }
}
