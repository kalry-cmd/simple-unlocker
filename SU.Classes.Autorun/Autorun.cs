using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace SU.Classes.Autorun;

internal class Autorun
{
	public static AutorunElem[] GetRegistryAutoRun(RegistryPaths path, RegistryView view = RegistryView.Default)
	{
		List<AutorunElem> list = new List<AutorunElem>();
		RegistryHive[] array = new RegistryHive[2]
		{
			RegistryHive.CurrentUser,
			RegistryHive.LocalMachine
		};
		foreach (RegistryHive registryHive in array)
		{
			using RegistryKey registryKey = RegistryKey.OpenBaseKey(registryHive, view);
			if ((uint)path > 1u)
			{
				if (path != RegistryPaths.Winlogon)
				{
					continue;
				}
				using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", writable: true))
				{
					string[] array2 = new string[2] { "Shell", "Userinit" };
					foreach (string name in array2)
					{
						if (registryKey2.GetValue(name) == null)
						{
							continue;
						}
						string[] array3 = registryKey2.GetValue(name).ToString().Trim(' ')
							.Split(',');
						foreach (string text in array3)
						{
							if (text != "")
							{
								list.Add(new AutorunElem
								{
									Name = name,
									Path = text,
									Hive = registryHive,
									RegPath = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon",
									Type = AutorunType.Winlogon
								});
							}
						}
					}
				}
				continue;
			}
			string text2 = ((path == RegistryPaths.Run) ? "Software\\Microsoft\\Windows\\CurrentVersion\\Run" : "Software\\Microsoft\\Windows\\CurrentVersion\\RunOnce");
			try
			{
				using RegistryKey registryKey3 = registryKey.OpenSubKey(text2, writable: true);
				string[] array2 = registryKey3.GetValueNames();
				foreach (string name2 in array2)
				{
					list.Add(new AutorunElem
					{
						Name = name2,
						Type = AutorunType.Registry,
						Hive = registryHive,
						Path = registryKey3.GetValue(name2).ToString(),
						RegPath = text2
					});
				}
			}
			catch
			{
			}
		}
		return list.ToArray();
	}

	public static AutorunElem[] GetShellAutorun()
	{
		List<AutorunElem> list = new List<AutorunElem>();
		string[] array = new string[2]
		{
			Environment.GetFolderPath(Environment.SpecialFolder.Startup),
			Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup)
		};
		foreach (string path in array)
		{
			string[] array2 = (from k in Directory.GetFiles(path)
				where !k.EndsWith(".ini")
				select k).ToArray();
			foreach (string path2 in array2)
			{
				list.Add(new AutorunElem
				{
					Name = Path.GetFileName(path2),
					Path = path,
					Type = AutorunType.ShellFolder
				});
			}
		}
		return list.ToArray();
	}
}
