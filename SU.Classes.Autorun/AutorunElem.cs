using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;

namespace SU.Classes.Autorun;

internal class AutorunElem
{
	public string Name { get; set; }

	public AutorunType Type { get; set; }

	public RegistryHive Hive { get; set; }

	public string RegPath { get; set; }

	public string Path { get; set; }

	public void Remove()
	{
		switch (Type)
		{
		case AutorunType.Registry:
		{
			using RegistryKey registryKey3 = RegistryKey.OpenBaseKey(Hive, Utils.RegView).OpenSubKey(RegPath, writable: true);
			registryKey3.DeleteValue(Name);
			break;
		}
		case AutorunType.Winlogon:
		{
			using RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
			using (RegistryKey registryKey2 = registryKey.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Winlogon", writable: true))
			{
				string[] array = registryKey2.GetValue(Name).ToString().Trim(' ')
					.Split(',');
				string text = "";
				string[] array2 = array;
				foreach (string text2 in array2)
				{
					if (!Path.ToLower().Contains(text2.ToLower()))
					{
						text = ((!(text == "")) ? (text + ", " + text2) : (text + text2));
					}
				}
				registryKey2.SetValue(Name, text);
				registryKey2.Close();
			}
			registryKey.Close();
			break;
		}
		case AutorunType.ShellFolder:
			try
			{
				File.Delete(Path + "\\" + Name);
				break;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Test", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				break;
			}
		case AutorunType.TaskScheduler:
			break;
		}
	}
}
