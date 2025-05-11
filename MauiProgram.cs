using Microsoft.Extensions.Logging;

namespace Linux_Mint;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts( fonts =>
            {
                fonts.AddFont( "OpenSans-Regular.ttf" , "OpenSansRegular" );
                fonts.AddFont( "OpenSans-Semibold.ttf" , "OpenSansSemibold" );
                fonts.AddFont( "fontello.ttf" , "Fontello" );
            } );

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
