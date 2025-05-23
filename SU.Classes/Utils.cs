using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using log4net;
using Microsoft.Win32;
using SU.Properties;

namespace SU.Classes;

internal class Utils
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct SHELLEXECUTEINFO
	{
		public int cbSize;

		public uint fMask;

		public IntPtr hwnd;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpVerb;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpFile;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpParameters;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpDirectory;

		public int nShow;

		public IntPtr hInstApp;

		public IntPtr lpIDList;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string lpClass;

		public IntPtr hkeyClass;

		public uint dwHotKey;

		public IntPtr hIcon;

		public IntPtr hProcess;
	}

	public struct PROCESS_INFORMATION
	{
		public IntPtr hProcess;

		public IntPtr hThread;

		public uint dwProcessId;

		public uint dwThreadId;
	}

	public struct SECURITY_ATTRIBUTES
	{
		public int Length;

		public IntPtr lpSecurityDescriptor;

		public bool bInheritHandle;

		internal int nLength;
	}

	public struct STARTUPINFO
	{
		public int cb;

		public string lpReserved;

		public string lpDesktop;

		public string lpTitle;

		public uint dwX;

		public uint dwY;

		public uint dwXSize;

		public uint dwYSize;

		public uint dwXCountChars;

		public uint dwYCountChars;

		public uint dwFillAttribute;

		public uint dwFlags;

		public short wShowWindow;

		public short cbReserved2;

		public IntPtr lpReserved2;

		public IntPtr hStdInput;

		public IntPtr hStdOutput;

		public IntPtr hStdError;
	}

	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	private struct STARTUPINFOEX
	{
		public STARTUPINFO StartupInfo;

		public IntPtr lpAttributeList;
	}

	private enum TOKEN_TYPE
	{
		TokenPrimary = 1,
		TokenImpersonation
	}

	private enum SECURITY_IMPERSONATION_LEVEL
	{
		SecurityAnonymous,
		SecurityIdentification,
		SecurityImpersonation,
		SecurityDelegation
	}

	[Flags]
	public enum ProcessAccessFlags : uint
	{
		All = 0x1F0FFFu,
		Terminate = 1u,
		CreateThread = 2u,
		VirtualMemoryOperation = 8u,
		VirtualMemoryRead = 0x10u,
		VirtualMemoryWrite = 0x20u,
		DuplicateHandle = 0x40u,
		CreateProcess = 0x80u,
		SetQuota = 0x100u,
		SetInformation = 0x200u,
		QueryInformation = 0x400u,
		QueryLimitedInformation = 0x1000u,
		Synchronize = 0x100000u
	}

	[Flags]
	private enum HANDLE_FLAGS : uint
	{
		None = 0u,
		INHERIT = 1u,
		PROTECT_FROM_CLOSE = 2u
	}

	[Flags]
	public enum PROCESS_CREATION_FLAGS : uint
	{
		DEBUG_PROCESS = 1u,
		DEBUG_ONLY_THIS_PROCESS = 2u,
		CREATE_SUSPENDED = 4u,
		DETACHED_PROCESS = 8u,
		CREATE_NEW_CONSOLE = 0x10u,
		NORMAL_PRIORITY_CLASS = 0x20u,
		IDLE_PRIORITY_CLASS = 0x40u,
		HIGH_PRIORITY_CLASS = 0x80u,
		REALTIME_PRIORITY_CLASS = 0x100u,
		CREATE_NEW_PROCESS_GROUP = 0x200u,
		CREATE_UNICODE_ENVIRONMENT = 0x400u,
		CREATE_SEPARATE_WOW_VDM = 0x800u,
		CREATE_SHARED_WOW_VDM = 0x1000u,
		CREATE_FORCEDOS = 0x2000u,
		BELOW_NORMAL_PRIORITY_CLASS = 0x4000u,
		ABOVE_NORMAL_PRIORITY_CLASS = 0x8000u,
		INHERIT_PARENT_AFFINITY = 0x10000u,
		INHERIT_CALLER_PRIORITY = 0x20000u,
		CREATE_PROTECTED_PROCESS = 0x40000u,
		EXTENDED_STARTUPINFO_PRESENT = 0x80000u,
		PROCESS_MODE_BACKGROUND_BEGIN = 0x100000u,
		PROCESS_MODE_BACKGROUND_END = 0x200000u,
		CREATE_SECURE_PROCESS = 0x400000u,
		CREATE_BREAKAWAY_FROM_JOB = 0x1000000u,
		CREATE_PRESERVE_CODE_AUTHZ_LEVEL = 0x2000000u,
		CREATE_DEFAULT_ERROR_MODE = 0x4000000u,
		CREATE_NO_WINDOW = 0x8000000u,
		PROFILE_USER = 0x10000000u,
		PROFILE_KERNEL = 0x20000000u,
		PROFILE_SERVER = 0x40000000u,
		CREATE_IGNORE_SYSTEM_DEFAULT = 0x80000000u
	}

	private static ILog logger = LogManager.GetLogger(typeof(Utils));

	public static readonly RegistryView RegView = (Environment.Is64BitOperatingSystem ? RegistryView.Registry64 : RegistryView.Registry32);

	public static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);

	public static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);

	public const uint SWP_NOSIZE = 1u;

	public const uint SWP_NOMOVE = 2u;

	public const uint TOPMOST_FLAGS = 3u;

	public const int EM_SETCUEBANNER = 5377;

	private const uint DefaultMbrSize = 1048576u;

	public const int SW_SHOW = 5;

	public const uint SEE_MASK_INVOKEIDLIST = 12u;

	public const int TOKEN_DUPLICATE = 2;

	public const uint MAXIMUM_ALLOWED = 33554432u;

	public const int CREATE_NEW_CONSOLE = 16;

	public const int IDLE_PRIORITY_CLASS = 64;

	public const int NORMAL_PRIORITY_CLASS = 32;

	public const int HIGH_PRIORITY_CLASS = 128;

	public const int REALTIME_PRIORITY_CLASS = 256;

	public const uint SC_MANAGER_ALL_ACCESS = 983103u;

	public const uint SERVICE_STOP = 32u;

	public const uint SERVICE_QUERY_STATUS = 4u;

	public const uint DELETE = 65536u;

	[DllImport("shell32.dll", CharSet = CharSet.Auto)]
	private static extern bool ShellExecuteEx(ref SHELLEXECUTEINFO lpExecInfo);

	[DllImport("ntdll.dll", PreserveSig = false)]
	public static extern void NtSuspendProcess(IntPtr processHandle);

	[DllImport("ntdll.dll", PreserveSig = false, SetLastError = true)]
	public static extern void NtResumeProcess(IntPtr processHandle);

	[DllImport("ntdll.dll", SetLastError = true)]
	public static extern int NtQueryInformationProcess(IntPtr hProcess, uint pic, ref uint pi, int cb, out int pSize);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern bool OpenProcessToken(IntPtr ProcessHandle, uint DesiredAccess, out IntPtr TokenHandle);

	[DllImport("ntdll.dll", SetLastError = true)]
	public static extern int NtSetInformationProcess(IntPtr hProcess, int processInformationClass, ref int processInformation, int processInformationLength);

	[DllImport("kernel32")]
	public static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern bool ReadFile(IntPtr hFile, [Out] byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

	[DllImport("user32.dll")]
	public static extern int GetSystemMetrics(uint Index);

	[DllImport("kernel32")]
	public static extern bool WriteFile(IntPtr hfile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberBytesWritten, IntPtr lpOverlapped);

	[DllImport("kernel32.dll", SetLastError = true)]
	[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
	[SuppressUnmanagedCodeSecurity]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CloseHandle(IntPtr hObject);

	[DllImport("user32.dll", CharSet = CharSet.Auto)]
	public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern IntPtr OpenSCManager(string machineName, string databaseName, uint dwAccess);

	[DllImport("advapi32.dll", SetLastError = true)]
	public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

	[DllImport("advapi32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool DeleteService(IntPtr hService);

	[DllImport("advapi32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	public static extern bool CloseServiceHandle(IntPtr hSCObject);

	[DllImport("kernel32.dll")]
	private static extern uint WTSGetActiveConsoleSessionId();

	[DllImport("advapi32.dll", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Ansi, SetLastError = true)]
	public static extern bool CreateProcessAsUser(IntPtr hToken, string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandle, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

	[DllImport("kernel32.dll")]
	private static extern bool ProcessIdToSessionId(uint dwProcessId, ref uint pSessionId);

	[DllImport("advapi32.dll")]
	public static extern bool DuplicateTokenEx(IntPtr ExistingTokenHandle, uint dwDesiredAccess, ref SECURITY_ATTRIBUTES lpThreadAttributes, int TokenType, int ImpersonationLevel, ref IntPtr DuplicateTokenHandle);

	[DllImport("kernel32.dll")]
	private static extern IntPtr OpenProcess(uint dwDesiredAccess, bool bInheritHandle, uint dwProcessId);

	[DllImport("advapi32", SetLastError = true)]
	[SuppressUnmanagedCodeSecurity]
	private static extern bool OpenProcessToken(IntPtr ProcessHandle, int DesiredAccess, ref IntPtr TokenHandle);

	[DllImport("kernel32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes, ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFOEX lpStartupInfo, out PROCESS_INFORMATION lpProcessInformation);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

	[DllImport("kernel32.dll", SetLastError = true)]
	public static extern uint WaitForSingleObject(IntPtr handle, uint milliseconds);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool UpdateProcThreadAttribute(IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, IntPtr lpValue, IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool InitializeProcThreadAttributeList(IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool SetHandleInformation(IntPtr hObject, HANDLE_FLAGS dwMask, HANDLE_FLAGS dwFlags);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool DuplicateHandle(IntPtr hSourceProcessHandle, IntPtr hSourceHandle, IntPtr hTargetProcessHandle, ref IntPtr lpTargetHandle, uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwOptions);

	public static string RandomString(int size, bool lowerCase = false)
	{
		Random random = new Random();
		StringBuilder stringBuilder = new StringBuilder(size);
		char c = (lowerCase ? 'a' : 'A');
		for (int i = 0; i < size; i++)
		{
			char value = (char)random.Next(c, c + 26);
			stringBuilder.Append(value);
		}
		if (!lowerCase)
		{
			return stringBuilder.ToString();
		}
		return stringBuilder.ToString().ToLower();
	}

	public static void RunFile(string FilePath, string Arguments, bool UAC, bool Hidden, bool WaitForExit)
	{
		Process process = new Process();
		ProcessStartInfo processStartInfo = new ProcessStartInfo
		{
			WindowStyle = (Hidden ? ProcessWindowStyle.Hidden : ProcessWindowStyle.Normal),
			FileName = FilePath,
			Arguments = Arguments
		};
		if (UAC)
		{
			processStartInfo.Verb = "runas";
		}
		logger.Debug((object)$"Запуск файла: \"{FilePath}\" с аргументами \"{Arguments} | Run as Admin: {UAC}, Hidden: {Hidden}, Wait for Exit: {WaitForExit}.");
		process.StartInfo = processStartInfo;
		process.Start();
		if (WaitForExit)
		{
			process.WaitForExit();
		}
	}

	public static byte[] ReadMBR(string PhysicalDrive)
	{
		byte[] array = new byte[1048576];
		IntPtr intPtr = CreateFile(PhysicalDrive, 268435456u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
		if (!ReadFile(intPtr, array, 1048576u, out var _, IntPtr.Zero))
		{
			CloseHandle(intPtr);
			throw new Exception("Не удалось прочитать MBR [" + PhysicalDrive + "]");
		}
		CloseHandle(intPtr);
		logger.Debug((object)$"Чтение MBR на {PhysicalDrive} (Length: {array.Length})");
		return array;
	}

	public static bool WriteMBR(byte[] MBRHex, string PhysicalDrive)
	{
		IntPtr intPtr = CreateFile(PhysicalDrive, 268435456u, 3u, IntPtr.Zero, 3u, 0u, IntPtr.Zero);
		if (!WriteFile(intPtr, MBRHex, 1048576u, out var _, IntPtr.Zero))
		{
			CloseHandle(intPtr);
			return false;
		}
		CloseHandle(intPtr);
		logger.Debug((object)$"Запись MBR на {PhysicalDrive} (Length: {MBRHex.Length})");
		return true;
	}

	public static string GetExecutablePathFromCommand(string command)
	{
		string pattern = "\"?([A-Za-z]:+(\\\\[\\w\\s.()]+)+\\.\\w+)\"?";
		Match match = Regex.Match(command, pattern);
		if (match.Success)
		{
			return match.Groups[1].Value;
		}
		return null;
	}

	public static bool ShowFileProperties(string FilePath)
	{
		SHELLEXECUTEINFO lpExecInfo = default(SHELLEXECUTEINFO);
		lpExecInfo.cbSize = Marshal.SizeOf(lpExecInfo);
		lpExecInfo.lpVerb = "properties";
		lpExecInfo.lpFile = FilePath;
		lpExecInfo.nShow = 5;
		lpExecInfo.fMask = 12u;
		return ShellExecuteEx(ref lpExecInfo);
	}

	public static void CloseForm(FormClosingEventArgs e)
	{
		string text = e.CloseReason.ToString();
		if (!(text == "UserClosing"))
		{
			if (text == "None" && Settings.Default.CloseMainMenuOnAction)
			{
				Application.OpenForms["MainMenu"].Show();
			}
		}
		else if (Settings.Default.CloseAppOnClick)
		{
			Environment.Exit(1);
		}
		else if (Settings.Default.CloseMainMenuOnAction)
		{
			Application.OpenForms["MainMenu"].Show();
		}
	}

	public static int GetRandomNumber(int min, int max)
	{
		return new Random().Next(min, max);
	}

	public static void CheckUpdate(Form form, AsyncCompletedEventHandler dFC, bool InformateUser)
	{
		logger.Info((object)"Запуск проверки обновлений.");
		string filename = "http://simpleunlocker.ds1nc.ru/release/version.xml";
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			Version version = new Version(Application.ProductVersion);
			Version version2 = new Version(xmlDocument.GetElementsByTagName("simpleunlocker")[0].InnerText);
			if (version < version2)
			{
				logger.Info((object)$"Была найдена новая версия SimpleUnlocker [{version2}], показываю MessageBox пользователю.");
				if (MessageBox.Show(form, $"Доступна новая версия SimpleUnlocker [{version2}]\nВы хотите обновить SimpleUnlocker сейчас?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Version version3 = new Version(xmlDocument.GetElementsByTagName("simpleunlocker_updater")[0].InnerText);
					if (new Version(FileVersionInfo.GetVersionInfo("bin\\su_updater.exe").FileVersion) < version3)
					{
						string uriString = "https://ds1nc.ru/updates/simpleunlocker/updater/su_updater.zip";
						logger.Info((object)"Запуск скачивание нового апдейтера.");
						ServicePointManager.Expect100Continue = true;
						ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
						WebClient webClient = new WebClient();
						webClient.DownloadFileCompleted += dFC;
						webClient.DownloadFileAsync(new Uri(uriString), "temp\\updater.temp");
					}
					else
					{
						RunFile("bin\\su_updater.exe", $"{version} {Path.GetFileName(Application.ExecutablePath)}", UAC: true, Hidden: false, WaitForExit: false);
						Environment.Exit(1);
					}
				}
				else
				{
					Program.AutoUpdateClickNo = true;
				}
			}
			else
			{
				logger.Info((object)"Провека была успешно завершена! У пользователя последняя версия SimpleUnlocker.");
				if (InformateUser)
				{
					MessageBox.Show(form, "Вы используете последнюю версию SimpleUnlocker", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				}
			}
		}
		catch (Exception ex)
		{
			logger.Error((object)ex.Message);
		}
	}
}
