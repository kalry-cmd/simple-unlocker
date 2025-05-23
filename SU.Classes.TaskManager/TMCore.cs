using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace SU.Classes.TaskManager;

internal class TMCore
{
	public delegate void ProcessHandler(uint processId);

	private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private List<Prc> processes = new List<Prc>();

	private List<Process> oldProcesses = new List<Process>();

	private ManagementEventWatcher startWatch;

	private ManagementEventWatcher stopWatch;

	public IEnumerable<Process> CurrentRunProcess => Process.GetProcesses().Intersect(oldProcesses, new ProcessComparerMemoryUsage()).ToList();

	public List<Prc> Processes
	{
		get
		{
			processes = Task.Run(() => GetListProcesses()).Result;
			return processes;
		}
		private set
		{
			processes = value;
		}
	}

	public event ProcessHandler NotifyStartNewProcess;

	public event ProcessHandler NotifyStopProcess;

	public event Action NotifyToUpdate;

	private void KillTask(Prc item, bool Uncritical = false)
	{
		try
		{
			if (Uncritical)
			{
				CriticalProcess(Process.GetProcessById(item.ProcessID), 0);
			}
			Process.GetProcessById(item.ProcessID).Kill();
			logger.Info((object)$"Был завершен процесс {item.ProcessID}");
		}
		catch
		{
			MessageBox.Show("Произошла ошибка при попытке убить процесс.", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public async Task<Prc> ToPrcClass(Process prc)
	{
		Prc prc2 = new Prc
		{
			ProcessName = prc.ProcessName,
			ProcessID = prc.Id
		};
		Prc prc3 = prc2;
		prc3.ProcessOwner = await GetProcessUser(prc);
		Prc prc4 = prc2;
		prc4.ProcessState = await GetProcessState(prc);
		prc2.ProcessCritical = IsProcessCritical(prc);
		Prc convertPrc = prc2;
		try
		{
			prc2 = convertPrc;
			prc2.ProcessDescription = await GetProcessDescription(prc);
		}
		catch
		{
			convertPrc.ProcessDescription = "";
		}
		return convertPrc;
	}

	public void SuspendProcess(int ProcessID)
	{
		using Process process = Process.GetProcessById(ProcessID);
		try
		{
			Utils.NtSuspendProcess(process.Handle);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public void ResumeProcess(int ProcessID)
	{
		using Process process = Process.GetProcessById(ProcessID);
		try
		{
			Utils.NtResumeProcess(process.Handle);
		}
		catch (Exception ex)
		{
			MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public async Task<string> GetProcessUser(Process process)
	{
		return await Task.Run(delegate
		{
			IntPtr TokenHandle = IntPtr.Zero;
			try
			{
				Utils.OpenProcessToken(process.Handle, 8u, out TokenHandle);
				return new WindowsIdentity(TokenHandle).Name?.Split('\\').Last();
			}
			catch
			{
				return (string)null;
			}
			finally
			{
				Utils.CloseHandle(TokenHandle);
			}
		});
	}

	public async Task<string> GetProcessState(Process process)
	{
		return await Task.Run(() => (process.Threads.Count <= 0 || process.Threads[0].ThreadState != ThreadState.Wait || process.Threads[0].WaitReason != ThreadWaitReason.Suspended) ? "Working" : "Suspended");
	}

	public async Task<string> GetProcessDescription(Process prc)
	{
		try
		{
			return await Task.Run(() => prc.MainModule.FileVersionInfo.FileDescription);
		}
		catch
		{
			return null;
		}
	}

	public void CriticalProcess(Process process, int isCritical)
	{
		int processInformationClass = 29;
		try
		{
			Utils.NtSetInformationProcess(process.Handle, processInformationClass, ref isCritical, 4);
		}
		catch (Exception ex)
		{
			logger.Error((object)ex.Message);
			MessageBox.Show(ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	public void TerminateProcesses(ListView prcList, bool UncriticalProcess)
	{
		foreach (int selectedIndex in prcList.SelectedIndices)
		{
			Prc prc = (Prc)prcList.Items[selectedIndex].Tag;
			if (prc.ProcessCritical == true)
			{
				if (UncriticalProcess)
				{
					KillTask(prc, Uncritical: true);
				}
				else if (MessageBox.Show("Процесс '" + prc.ProcessName + "' является критическим. Вы действительно хотите завершить его?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
				{
					KillTask(prc);
				}
			}
			else
			{
				KillTask(prc);
			}
		}
	}

	public bool? IsProcessCritical(Process pr)
	{
		try
		{
			uint pi = 0u;
			if (Utils.NtQueryInformationProcess(pr.Handle, 29u, ref pi, 4, out var pSize) != 0 || pSize != 4)
			{
				return null;
			}
			return pi != 0;
		}
		catch
		{
			return null;
		}
	}

	public async Task<List<Prc>> GetListProcesses()
	{
		logger.Debug((object)"Получение списка процессов");
		processes = new List<Prc>();
		Process[] array = Process.GetProcesses();
		oldProcesses = array.ToList();
		Process[] array2 = array;
		foreach (Process prc in array2)
		{
			List<Prc> list = processes;
			list.Add(await ToPrcClass(prc));
		}
		logger.Debug((object)$"Успешно! Было получено {processes.Count} процессов");
		return processes;
	}

	public List<Srv> GetListServices()
	{
		logger.Debug((object)"Получение списка служб");
		List<Srv> list = (from service in ServiceController.GetServices()
			where !string.IsNullOrWhiteSpace(service.ServiceName)
			select new Srv
			{
				serviceName = service.ServiceName,
				serviceDisplayName = service.DisplayName,
				serviceStatus = GetServiceStatus(service)
			}).ToList();
		logger.Debug((object)$"Успешно! Было получено {list.Count} служб");
		return list;
	}

	public async Task<List<Srv>> GetListServicesAsync()
	{
		logger.Debug((object)"Получение списка служб");
		ServiceController[] services = await Task.Run(() => ServiceController.GetServices());
		List<Srv> list = await Task.Run(() => (from service in services
			where !string.IsNullOrWhiteSpace(service.ServiceName)
			select new Srv
			{
				serviceName = service.ServiceName,
				serviceDisplayName = service.DisplayName,
				serviceStatus = GetServiceStatus(service),
				serviceStartType = service.StartType.ToString()
			}).ToList());
		logger.Debug((object)$"Успешно! Было получено {list.Count} служб");
		return list;
	}

	public string GetServiceStatus(ServiceController service)
	{
		try
		{
			return service.Status.ToString();
		}
		catch (Exception)
		{
			return "Unknown";
		}
	}

	public List<Process> UpdateProcessList()
	{
		logger.Debug((object)"Обновление списка процессов");
		List<Process> list = Process.GetProcesses().ToList().Except(oldProcesses, new ProcessComparer())
			.ToList();
		oldProcesses.AddRange(list);
		logger.Debug((object)"Обновление информации о процессах");
		if (list.Count == 0)
		{
			return null;
		}
		Prc[] result = Task.WhenAll(list.Select(ToPrcClass)).Result;
		processes.AddRange(result);
		logger.Debug((object)$"Успешно обновлено {list.Count} процессов!");
		return list;
	}

	public async Task<List<Process>> UpdateProcessListAsync()
	{
		logger.Debug((object)"Обновление списка процессов");
		List<Process> first = (await Task.Run(() => Process.GetProcesses().ToList())).ToList();
		List<Process> newProcesses = first.Except(oldProcesses, new ProcessComparer()).ToList();
		oldProcesses.AddRange(newProcesses);
		logger.Debug((object)"Обновление информации о процессах");
		if (newProcesses.Count == 0)
		{
			return null;
		}
		Prc[] collection = await Task.WhenAll(newProcesses.Select(ToPrcClass));
		processes.AddRange(collection);
		logger.Debug((object)$"Успешно обновлено {newProcesses.Count} процессов!");
		return newProcesses;
	}

	public async Task<Prc> UpdateProcessInfo(int prcID)
	{
		using Process p = Process.GetProcessById(prcID);
		return await ToPrcClass(p);
	}

	public void DeleteProcess(int pid)
	{
		if (processes.Any((Prc x) => x.ProcessID == pid))
		{
			processes.Remove(processes.Find((Prc x) => x.ProcessID == pid));
		}
		if (oldProcesses.Any((Process x) => x.Id == pid))
		{
			oldProcesses.Remove(oldProcesses.Find((Process x) => x.Id == pid));
		}
	}

	public void StartProcessEvent(object sender, EventArrivedEventArgs e)
	{
		this.NotifyStartNewProcess?.Invoke(Convert.ToUInt32(e.NewEvent.Properties["ProcessId"].Value));
	}

	public void StopProcessEvent(object sender, EventArrivedEventArgs e)
	{
		this.NotifyStopProcess?.Invoke(Convert.ToUInt32(e.NewEvent.Properties["ProcessId"].Value));
		DeleteProcess(Convert.ToInt32(e.NewEvent.Properties["ProcessId"].Value));
	}

	public void InitEventsProcess()
	{
		logger.Info((object)"Инициализация ивентов на запуск и остановку процесса");
		startWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStartTrace WITHIN 1"));
		startWatch.EventArrived += StartProcessEvent;
		startWatch.Start();
		stopWatch = new ManagementEventWatcher(new WqlEventQuery("SELECT * FROM Win32_ProcessStopTrace"));
		stopWatch.EventArrived += StopProcessEvent;
		stopWatch.Start();
	}

	public void EndEventsProcess()
	{
		logger.Info((object)"Остановка ивентов на запуск и остановку процесса");
		startWatch.Stop();
		stopWatch.Stop();
	}
}
