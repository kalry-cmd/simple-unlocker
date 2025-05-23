using System;
using System.Windows.Forms;
using log4net;
using SU.Classes;
using SU.Forms;

namespace SU;

internal static class Program
{
	public static bool AutoUpdateClickNo;

	public static bool isSaveMode;

	[STAThread]
	private static void Main()
	{
		Logger.Init();
		AppDomain.CurrentDomain.UnhandledException += delegate(object a, UnhandledExceptionEventArgs b)
		{
			LogManager.GetLogger("Unhandled Exception").Fatal((object)((Exception)b.ExceptionObject).Message);
		};
		isSaveMode = Utils.GetSystemMetrics(67u) != 0;
		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault(defaultValue: false);
		Application.Run(new SU.Forms.MainMenu());
	}
}
