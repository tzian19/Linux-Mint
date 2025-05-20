using Linux_Mint.Service;

using Microsoft.Extensions.Logging;

namespace Linux_Mint;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {

        DependencyService.Get<IKeepScreenOnService>()?.KeepScreenOn();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts( fonts =>
            {
                fonts.AddFont( "OpenSans-Regular.ttf" , "OpenSansRegular" );
                fonts.AddFont( "OpenSans-Semibold.ttf" , "OpenSansSemibold" );
                fonts.AddFont( "fontello.ttf" , "Fontello" );
                fonts.AddFont( "NotoSans-ExtraBold.ttf" , "NotoSansExtraBold" );
            } );

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
