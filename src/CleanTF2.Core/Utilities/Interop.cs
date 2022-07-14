using System.Runtime.InteropServices;

namespace CleanTF2.Core.Utilities
{
    public class Interop : IInterop
    {
        public bool IsOSPlatform(OSPlatform osPlatform) => RuntimeInformation.IsOSPlatform(osPlatform);
    }
}
