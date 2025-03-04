using Microsoft.AspNetCore.Mvc;

namespace DNBarbershop.Extensions
{
    public static class ActionResultExtensions
    {
        public static async Task<T> GetResultAsync<T>(this IActionResult result)
        {
            if (result is JsonResult jsonResult)
            {
                return (T)jsonResult.Value;
            }

            throw new InvalidOperationException("The result is not a JsonResult.");
        }
    }
}
