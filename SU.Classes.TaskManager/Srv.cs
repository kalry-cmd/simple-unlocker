using System;
using System.Runtime.InteropServices;

namespace SU.Classes.TaskManager;

internal class Srv
{
	public string serviceName { get; set; }

	public string serviceDisplayName { get; set; }

	public string serviceStatus { get; set; }

	public string serviceStartType { get; set; }

	public void Delete()
	{
		IntPtr intPtr = Utils.OpenSCManager(null, null, 983103u);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidOperationException("Не удалось открыть менеджер служб.");
		}
		try
		{
			IntPtr intPtr2 = Utils.OpenService(intPtr, serviceName, 65572u);
			if (intPtr2 == IntPtr.Zero)
			{
				throw new InvalidOperationException("Не удалось открыть службу.");
			}
			try
			{
				if (!Utils.DeleteService(intPtr2))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					throw new InvalidOperationException($"Не удалось удалить службу. Код ошибки: {lastWin32Error}");
				}
			}
			finally
			{
				Utils.CloseServiceHandle(intPtr2);
			}
		}
		finally
		{
			Utils.CloseServiceHandle(intPtr);
		}
	}
}
