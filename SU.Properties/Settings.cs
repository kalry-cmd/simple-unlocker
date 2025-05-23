using System.CodeDom.Compiler;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace SU.Properties;

[CompilerGenerated]
[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "16.10.0.0")]
internal sealed class Settings : ApplicationSettingsBase
{
	private static Settings defaultInstance = (Settings)SettingsBase.Synchronized(new Settings());

	public static Settings Default => defaultInstance;

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool AlwaysOnTop
	{
		get
		{
			return (bool)this["AlwaysOnTop"];
		}
		set
		{
			this["AlwaysOnTop"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool RandomWindowName
	{
		get
		{
			return (bool)this["RandomWindowName"];
		}
		set
		{
			this["RandomWindowName"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool CloseMainMenuOnAction
	{
		get
		{
			return (bool)this["CloseMainMenuOnAction"];
		}
		set
		{
			this["CloseMainMenuOnAction"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool CloseAppOnClick
	{
		get
		{
			return (bool)this["CloseAppOnClick"];
		}
		set
		{
			this["CloseAppOnClick"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("True")]
	public bool AutoCheckUpdate
	{
		get
		{
			return (bool)this["AutoCheckUpdate"];
		}
		set
		{
			this["AutoCheckUpdate"] = value;
		}
	}

	[UserScopedSetting]
	[DebuggerNonUserCode]
	[DefaultSettingValue("False")]
	public bool AutoRenameFile
	{
		get
		{
			return (bool)this["AutoRenameFile"];
		}
		set
		{
			this["AutoRenameFile"] = value;
		}
	}
}
