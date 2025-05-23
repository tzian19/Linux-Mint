using CommunityToolkit.Maui;

using Linux_Mint.Service;

using Microsoft.Extensions.Logging;

using Plugin.Maui.ImageCropper;

using SkiaSharp.Views.Maui.Controls.Hosting;

namespace Linux_Mint;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        DependencyService.Get<IKeepScreenOnService>()?.KeepScreenOn();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseImageCropper()
            .ConfigureFonts( fonts =>
            {
                fonts.AddFont( "OpenSans-Regular.ttf" , "OpenSansRegular" );
                fonts.AddFont( "OpenSans-Semibold.ttf" , "OpenSansSemibold" );
                fonts.AddFont( "fontello.ttf" , "Fontello" );
                fonts.AddFont( "NotoSans-ExtraBold.ttf" , "NotoSansExtraBold" );
            } ).UseMauiCommunityToolkit();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        // 🟢 THE EVENT-DRIVEN FIX (High Performance / Zero Lag) 🟢
        // This replaces the while-loop. It only fires when Android tries to redraw the screen.
        Microsoft.Maui.Handlers.ToolbarHandler.Mapper.AppendToMapping( "CleanBurgerColorFix" , ( handler , view ) =>
        {
#if ANDROID
            var toolbar = handler.PlatformView;

            // 1. Create a helper function to paint it green
            void ForceGreenColor()
            {
                if ( toolbar?.NavigationIcon != null )
                {
                    var greenColor = Android.Graphics.Color.ParseColor("#217D41");
                    toolbar.NavigationIcon.SetColorFilter(
                        new Android.Graphics.PorterDuffColorFilter( greenColor , Android.Graphics.PorterDuff.Mode.SrcIn )
                    );
                }
            }

            // 2. Apply it immediately when the page first loads
            ForceGreenColor();

            // 3. THE CULPRIT KILLER: 
            // Listen for the native Android 'LayoutChange' event. 
            // Whenever Android tries to redraw or animate the toolbar, this fires instantly and repaints it green.
            toolbar.LayoutChange += ( sender , e ) =>
            {
                ForceGreenColor();
            };
#endif
        } );

        return builder.Build();
    }
}