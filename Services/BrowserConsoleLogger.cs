using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace ExistHelper.Services
{
    public static class BrowserConsoleLogger
    {
        public static ValueTask LogAsync(IJSRuntime js, string message)
            => js.InvokeVoidAsync("console.log", message);

        public static ValueTask WarnAsync(IJSRuntime js, string message)
            => js.InvokeVoidAsync("console.warn", message);

        public static ValueTask ErrorAsync(IJSRuntime js, string message)
            => js.InvokeVoidAsync("console.error", message);

        public static ValueTask InfoAsync(IJSRuntime js, string message)
            => js.InvokeVoidAsync("console.info", message);

        public static ValueTask DebugAsync(IJSRuntime js, string message)
            => js.InvokeVoidAsync("console.debug", message);
    }
}