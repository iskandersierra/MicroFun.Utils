using Spectre.Console.Cli;

namespace MicroFun.Utils.Commands;

public static class MfuApp
{
    public static CommandApp CreateApp()
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.SetApplicationName("mfu");

            config.AddCommand<RandCommand>("rand");
        });

        return app;
    }
}
