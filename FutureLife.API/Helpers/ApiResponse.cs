namespace FutureLife.API.Helpers;

public static class ApiResponse
{
    public static object Success(object? data = null, string message = "Success")
        => new { success = true, data, message };

    public static object Success<T>(T? data, string message = "Success")
        => new { success = true, data, message };

    public static object Fail(string message = "An error occurred")
        => new { success = false, message };
}
