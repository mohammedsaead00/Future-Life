namespace FutureLife.API.Helpers;

public static class ApiResponse
{
    public static object Success(object? data = null, string message = "Success")
    {
        return new { success = true, data, message };
    }

    public static object Fail(string message = "An error occurred")
    {
        return new { success = false, message };
    }
}
