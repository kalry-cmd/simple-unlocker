using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using Microsoft.Win32;

namespace SU.Classes.UR;

internal class Associations
{
	private static ILog logger = LogManager.GetLogger(typeof(Associations));

	public static List<FileType> GetFileTypes()
	{
		List<FileType> list = new List<FileType>();
		foreach (string item in from a in Registry.ClassesRoot.GetSubKeyNames()
			where !a.StartsWith(".")
			select a)
		{
			using RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(item);
			if (registryKey == null)
			{
				continue;
			}
			using RegistryKey registryKey2 = registryKey.OpenSubKey("shell");
			if (registryKey2 != null)
			{
				list.Add(CreateFileType(item, registryKey, registryKey2));
			}
		}
		logger.Info((object)$"FileTypes Count: {list.Count}");
		return list;
	}

	public static List<Association> GetAssocs()
	{
		List<Association> list = new List<Association>();
		foreach (string item in from a in Registry.ClassesRoot.GetSubKeyNames()
			where a.StartsWith(".")
			select a)
		{
			using RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(item);
			if (registryKey == null)
			{
				continue;
			}
			object value = registryKey.GetValue("");
			if (value == null)
			{
				continue;
			}
			using RegistryKey registryKey2 = Registry.ClassesRoot.OpenSubKey(value.ToString());
			if (registryKey2 != null)
			{
				using RegistryKey shell = registryKey2.OpenSubKey("shell");
				list.Add(CreateAssociation(item, value.ToString(), registryKey2, shell));
			}
		}
		logger.Info((object)$"Associations Count: {list.Count}");
		return list;
	}

	private static Association CreateAssociation(string AKey, string defaultKeyName, RegistryKey fileKey, RegistryKey shell)
	{
		return new Association
		{
			Name = AKey,
			Type = CreateFileType(defaultKeyName, fileKey, shell)
		};
	}

	public static Association CreateAssociation(string AKey, FileType fType, bool createInRegistry = false)
	{
		if (createInRegistry)
		{
			using RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(AKey);
			registryKey.SetValue("", fType.Name);
		}
		return new Association
		{
			Name = AKey,
			Type = fType
		};
	}

	public static void UpdateAssocFileType(Association assoc, FileType fileType)
	{
		using RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(assoc.Name, writable: true);
		registryKey.SetValue("", fileType.Name);
	}

	public static FileType CreateFileType(string fileTypeName, RegistryKey fileKey, RegistryKey shell)
	{
		return new FileType
		{
			Name = fileTypeName,
			Description = fileKey?.GetValue("")?.ToString(),
			DefaultIcon = fileKey.OpenSubKey("DefaultIcon")?.GetValue("")?.ToString(),
			ShellOpenCommand = CreateFileCommand(shell, "open"),
			ShellRunAsCommand = CreateFileCommand(shell, "runas"),
			ShellRunAsUserCommand = CreateFileCommand(shell, "runasuser")
		};
	}

	public static FileType CreateFileType(string fileTypeName, string description, string defaultIcon, FileCommand open, FileCommand runas, FileCommand runasuser)
	{
		using (RegistryKey registryKey = Registry.ClassesRoot.CreateSubKey(fileTypeName))
		{
			registryKey.SetValue("", description);
			registryKey.CreateSubKey("DefaultIcon").SetValue("", defaultIcon);
			using RegistryKey registryKey2 = registryKey.CreateSubKey("shell");
			FileCommand[] array = new FileCommand[3] { open, runas, runasuser };
			foreach (FileCommand fileCommand in array)
			{
				if (fileCommand == null || (fileCommand.DefaultCommand != null && fileCommand.IsolatedCommand != null && fileCommand.DelegateExecute != null))
				{
					continue;
				}
				using RegistryKey registryKey3 = registryKey2.CreateSubKey(fileCommand.Name).CreateSubKey("command");
				if (fileCommand.DefaultCommand != null)
				{
					registryKey3.SetValue("", fileCommand.DefaultCommand);
				}
				if (fileCommand.IsolatedCommand != null)
				{
					registryKey3.SetValue("IsolatedCommand", fileCommand.IsolatedCommand);
				}
				if (fileCommand.DelegateExecute != null)
				{
					registryKey3.SetValue("DelegateExecute", fileCommand.DelegateExecute);
				}
			}
		}
		return new FileType
		{
			Name = fileTypeName,
			Description = description,
			DefaultIcon = defaultIcon,
			ShellOpenCommand = open,
			ShellRunAsCommand = runas,
			ShellRunAsUserCommand = runasuser
		};
	}

	private static FileCommand CreateFileCommand(RegistryKey shell, string commandName)
	{
		if (shell == null)
		{
			return null;
		}
		RegistryKey registryKey = shell.OpenSubKey(commandName);
		if (registryKey == null)
		{
			return null;
		}
		return new FileCommand
		{
			DefaultCommand = registryKey.OpenSubKey("command")?.GetValue("")?.ToString(),
			IsolatedCommand = registryKey.OpenSubKey("command")?.GetValue("IsolatedCommand")?.ToString(),
			DelegateExecute = registryKey.OpenSubKey("command")?.GetValue("DelegateExecute")?.ToString()
		};
	}

	public static FileCommand CreateFileCommand(string name, string defaultCommand, string isolateCommand, string delegateCommand)
	{
		if (string.IsNullOrWhiteSpace(defaultCommand) && string.IsNullOrWhiteSpace(isolateCommand) && string.IsNullOrWhiteSpace(delegateCommand))
		{
			return null;
		}
		return new FileCommand
		{
			Name = name,
			DefaultCommand = ((!string.IsNullOrWhiteSpace(defaultCommand)) ? defaultCommand : null),
			IsolatedCommand = ((!string.IsNullOrWhiteSpace(isolateCommand)) ? isolateCommand : null),
			DelegateExecute = ((!string.IsNullOrWhiteSpace(delegateCommand)) ? delegateCommand : null)
		};
	}

	public static FileType UpdateFileType(FileType fileType, string defaultIcon, string description, FileCommand open, FileCommand runas, FileCommand runasuser)
	{
		using (RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(fileType.Name, writable: true))
		{
			registryKey.CreateSubKey("DefaultIcon").SetValue("", defaultIcon);
			registryKey.SetValue("", description);
			using RegistryKey registryKey2 = registryKey.CreateSubKey("shell");
			FileCommand[] array = new FileCommand[3] { open, runas, runasuser };
			foreach (FileCommand fileCommand in array)
			{
				if (fileCommand == null || (fileCommand.DefaultCommand == null && fileCommand.IsolatedCommand == null && fileCommand.DelegateExecute == null))
				{
					try
					{
						registryKey2.DeleteSubKeyTree(fileCommand.Name, throwOnMissingSubKey: false);
					}
					catch (Exception ex)
					{
						logger.Warn((object)(ex.Message + " - FileType: " + fileType.Name));
					}
				}
				else
				{
					using RegistryKey registryKey3 = registryKey2.CreateSubKey(fileCommand.Name).CreateSubKey("command");
					registryKey3.SetValue("", fileCommand.DefaultCommand ?? "");
					registryKey3.SetValue("IsolatedCommand", fileCommand.IsolatedCommand ?? "");
					registryKey3.SetValue("DelegateExecute", fileCommand.DelegateExecute ?? "");
				}
			}
		}
		fileType.DefaultIcon = defaultIcon;
		fileType.Description = description;
		fileType.ShellOpenCommand = open;
		fileType.ShellRunAsCommand = runas;
		fileType.ShellRunAsUserCommand = runasuser;
		return fileType;
	}

	public static void RemoveFileType(FileType fileType)
	{
		try
		{
			Registry.ClassesRoot.DeleteSubKeyTree(fileType.Name);
		}
		catch (Exception ex)
		{
			logger.Error((object)(ex.Message + " - FileType: " + fileType.Name));
		}
	}

	public static void RemoveAssociation(Association assoc)
	{
		Console.WriteLine(assoc.Name);
		Registry.ClassesRoot.DeleteSubKeyTree(assoc.Name, throwOnMissingSubKey: false);
		try
		{
			using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\ApplicationAssociationToasts", writable: true))
			{
				registryKey.DeleteValue(assoc.Type.Name + "_" + assoc.Name, throwOnMissingValue: false);
			}
			using RegistryKey registryKey2 = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\FileExts", writable: true);
			registryKey2.DeleteSubKeyTree(assoc.Name, throwOnMissingSubKey: false);
		}
		catch (Exception ex)
		{
			logger.Warn((object)(ex.Message + " - Association: " + assoc.Name));
		}
	}
}
