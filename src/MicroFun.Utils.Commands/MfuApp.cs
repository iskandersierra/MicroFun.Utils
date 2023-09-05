using MicroFun.Utils.Commands.FileSystem;
using Spectre.Console;
using Spectre.Console.Cli;

namespace MicroFun.Utils.Commands;

public static class MfuApp
{
    public static CommandApp CreateApp()
    {
        var app = new CommandApp();

        app.Configure(mfu =>
        {
            mfu.SetApplicationName("mfu");

#if DEBUG
            mfu.SetExceptionHandler(ex =>
            {
                AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything);
                return -1;
            });
#endif

            mfu.AddCommand<RandCommand>("rand")
                .WithDescription("Generate random data")
                .WithExample("rand")
                .WithExample("rand 32")
                .WithExample("rand 32 --hex")
                ;

            mfu.AddBranch("file", file =>
            {
                file.AddCommand<FileStatsCommand>("stats")
                    .WithDescription("Get file stats")
                    .WithExample("file stats")
                    .WithExample("file stats C:")
                    .WithExample("file stats C: *.cs")
                    .WithExample("file stats C: *.sys --no-recurse")
                    .WithExample("file stats C: --no-recurse --sort max")
                    .WithExample("file stats C: --no-recurse --sort max --ascending")
                    ;
            });
        });

        return app;
    }
}
