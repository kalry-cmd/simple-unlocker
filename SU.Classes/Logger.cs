using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace SU.Classes;

internal class Logger
{
    private const string LogPattern = "%timestamp [%thread] %-5level %logger - %message%newline";

    private static readonly Hierarchy hierarchy = (Hierarchy)LogManager.GetRepository();

    private static readonly ColoredConsoleAppender coloredConsoleAppender = new ColoredConsoleAppender();

    public static MemoryAppender MemoryAppender = new MemoryAppender();

    public static void Init()
    {
        var patternLayout = new PatternLayout(LogPattern);
        patternLayout.ActivateOptions();

        coloredConsoleAppender.Layout = patternLayout;

        coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
        {
            Level = Level.Error,
            ForeColor = ColoredConsoleAppender.Colors.Red
        });
        coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
        {
            Level = Level.Warn,
            ForeColor = ColoredConsoleAppender.Colors.Yellow
        });
        coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
        {
            Level = Level.Info,
            ForeColor = ColoredConsoleAppender.Colors.White
        });
        coloredConsoleAppender.AddMapping(new ColoredConsoleAppender.LevelColors
        {
            Level = Level.Debug,
            ForeColor = ColoredConsoleAppender.Colors.Green
        });

        coloredConsoleAppender.ActivateOptions();

        hierarchy.Root.AddAppender(coloredConsoleAppender);

        MemoryAppender.Layout = patternLayout;
        MemoryAppender.ActivateOptions();
        hierarchy.Root.AddAppender(MemoryAppender);

        hierarchy.Configured = true;
    }
}
