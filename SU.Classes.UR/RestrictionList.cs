using System;
using System.Collections.Generic;
using System.Reflection;
using log4net;
using Microsoft.Win32;

namespace SU.Classes.UR;

internal class RestrictionList
{
	private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static List<Restriction> BasicRestrictions = new List<Restriction>
	{
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System",
			RestrictionName = "DisableTaskMgr",
			RestrictionDescription = "Блокировка диспетчера задач",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System",
			RestrictionName = "DisableRegistryTools",
			RestrictionDescription = "Блокировка Редактора реестра",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Policies\\Microsoft\\Windows\\System",
			RestrictionName = "DisableCMD",
			RestrictionDescription = "Блокировка Командной строки",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Policies\\Microsoft\\MMC",
			RestrictionName = "RestrictToPermittedSnapins",
			RestrictionDescription = "Блокировка MMC",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoControlPanel",
			RestrictionDescription = "Блокировка панели управления и параметров Windows",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoRun",
			RestrictionDescription = "Блокировка окна выполнить",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[2]
			{
				RegistryHive.CurrentUser,
				RegistryHive.LocalMachine
			},
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoViewOnDrive",
			RestrictionDescription = "Блокировка доступа к диску из проводника",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[2]
			{
				RegistryHive.CurrentUser,
				RegistryHive.LocalMachine
			},
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoDrives",
			RestrictionDescription = "Скрытие диска из проводника",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoFind",
			RestrictionDescription = "Блокировка поиска в пуске",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoViewContextMenu",
			RestrictionDescription = "Блокировка контекстного меню",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoFolderOptions",
			RestrictionDescription = "Блокировка настройки папок",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoSecurityTab",
			RestrictionDescription = "Блокировка вкладки безопасность",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoFileMenu",
			RestrictionDescription = "Скрытие файла меню",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoClose",
			RestrictionDescription = "Блокировка возможности выключить компьютер через пуск",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoCommonGroups",
			RestrictionDescription = "Скрытие разделов из пуска",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "StartMenuLogOff",
			RestrictionDescription = "Скрытие выхода из системы в меню пуск",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\ActiveDesktop",
			RestrictionName = "NoChangingWallPaper",
			RestrictionDescription = "Запрет на смену обоев",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoWinKeys",
			RestrictionDescription = "Отключение горячих клавиш",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoSetTaskbar",
			RestrictionDescription = "Запрет на внесение изменений в настройки панели задач и меню \"Пуск\"",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System",
			RestrictionName = "DisableLockWorkstation",
			RestrictionDescription = "Предотвращение блокировки системы пользователем",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System",
			RestrictionName = "DisableChangePassword",
			RestrictionDescription = "Запрет на смену пароля",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoTrayContextMenu",
			RestrictionDescription = "Запрет меню, которые появляются при щелчке правой кнопкой мыши на панели задач",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.LocalMachine },
			RestrictionKey = "Software\\Policies\\Microsoft\\Windows\\System",
			RestrictionName = "DenyUsersFromMachGP",
			RestrictionDescription = "Пользователи не смогут вызывать обновление политики компьютера.",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.LocalMachine },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "HidePowerOptions",
			RestrictionDescription = "Команды выключения, перезагрузки, сна и т. д. будут удалены из меню \"Пуск\". Кнопка питания также удалена с экрана безопасности Windows.",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[2]
			{
				RegistryHive.LocalMachine,
				RegistryHive.CurrentUser
			},
			RestrictionKey = "Software\\Policies\\Microsoft\\Windows\\Explorer",
			RestrictionName = "DisableContextMenusInStart",
			RestrictionDescription = "Запрет пользователям открывать контекстные меню в меню \"Пуск\"",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.LocalMachine },
			RestrictionKey = "Software\\Policies\\Microsoft\\Windows NT\\SystemRestore",
			RestrictionName = "DisableSR",
			RestrictionDescription = "Восстановление системы отключится, и мастер восстановления системы будет недоступен.",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.LocalMachine },
			RestrictionKey = "Software\\Policies\\Microsoft\\Windows NT\\SystemRestore",
			RestrictionName = "DisableConfig",
			RestrictionDescription = "Возможность настройки восстановления системы с помощью защиты системы будет отключена.",
			DisableValue = 0
		},
		new Restriction
		{
			RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
			RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer",
			RestrictionName = "NoLogoff",
			RestrictionDescription = "Блокировка возможности выйти из системы",
			DisableValue = 0
		}
	};

	public static Restriction[] GetDisallowApps()
	{
		List<Restriction> list = new List<Restriction>();
		using RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun");
		if (registryKey == null)
		{
			return new Restriction[0];
		}
		string[] valueNames = registryKey.GetValueNames();
		foreach (string text in valueNames)
		{
			list.Add(new Restriction
			{
				RestrictionHive = new RegistryHive[1] { RegistryHive.CurrentUser },
				RestrictionKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer\\DisallowRun",
				RestrictionDescription = $"Блокировка приложения \"{registryKey.GetValue(text)}\" [DisallowRun]",
				RestrictionName = text
			});
		}
		return list.ToArray();
	}

	public static Restriction[] GetDebuggerApps()
	{
		List<Restriction> list = new List<Restriction>();
		using RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options");
		if (registryKey == null)
		{
			return list.ToArray();
		}
		string[] subKeyNames = registryKey.GetSubKeyNames();
		foreach (string text in subKeyNames)
		{
			try
			{
				using RegistryKey registryKey2 = registryKey.OpenSubKey(text);
				logger.Debug((object)("Getting " + text + " Debugger"));
				object value = registryKey2.GetValue("Debugger");
				if (value != null)
				{
					list.Add(new Restriction
					{
						RestrictionHive = new RegistryHive[1] { RegistryHive.LocalMachine },
						RestrictionKey = "SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Image File Execution Options\\" + text,
						RestrictionDescription = $"Обнаружен дебаггер приложения \"{text}\" ({value}) [Debugger]",
						RestrictionName = "Debugger"
					});
				}
			}
			catch (Exception ex)
			{
				logger.Error((object)("Не удалось получить список дебаггеров (" + ex.Message + ")"));
			}
		}
		return list.ToArray();
	}
}
