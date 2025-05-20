using Android.Views;

using Linux_Mint.Service;

[assembly: Dependency(typeof(Linux_Mint.Platforms.Android.KeepScreenOnService))]

namespace Linux_Mint.Platforms.Android
{
    public class KeepScreenOnService : IKeepScreenOnService
    {
        public void KeepScreenOn()
        {
            var window = Platform.CurrentActivity?.Window;
            window?.AddFlags(WindowManagerFlags.KeepScreenOn);
        }

        public void AllowScreenOff()
        {
            var window = Platform.CurrentActivity?.Window;
            window?.ClearFlags(WindowManagerFlags.KeepScreenOn);
        }
    }
}