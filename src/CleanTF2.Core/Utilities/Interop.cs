using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CleanTF2.Core.Utilities
{
    [ExcludeFromCodeCoverage]
    public class Interop : IInterop
    {
        public bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
    }
}
