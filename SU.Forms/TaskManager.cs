using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using SU.Classes;
using SU.Classes.TaskManager;
using SU.Forms.TaskMgr;
using SU.Properties;

namespace SU.Forms;

public class TaskManager : Form
{
	private static ILog logger = LogManager.GetLogger(typeof(TaskManager));

	private readonly PerformanceCounter CPU = new PerformanceCounter("Processor", "% Processor Time", "_Total");

	private readonly PerformanceCounter RAM = new PerformanceCounter("Memory", "Available MBytes");

	private TMCore taskMgrCore;

	private List<Prc> runningProcess = new List<Prc>();

	private ServiceController sc = new ServiceController();

	private IContainer components;

	private StatusStrip ManagerStatusStrip;

	private MenuStrip ManagerMenu;

	private ToolStripMenuItem MenuStrip;

	private ToolStripMenuItem BackToMainMenuStrip;

	private ToolStripMenuItem ExitStrip;

	private ToolStrip ManagerMenuStrip;

	private TabControl ManagerMode;

	private TabPage TaskMgrPage;

	private TabPage ServicesPage;

	private ToolStripButton RefreshMenuBtn;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripButton TerminateMenuBtn;

	private ToolStripButton SuspendMenuBtn;

	private ToolStripButton ResumeMenuBtn;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripButton CriticalMenuBtn;

	private ToolStripButton UncriticalMenuBtn;

	private ToolStripStatusLabel CPUSSLabel;

	private ToolStripStatusLabel RAMSSLabel;

	private ToolStripStatusLabel ProcessesSSLabel;

	private ListView TMView;

	private ColumnHeader ProcessColumn;

	private ColumnHeader IdColumn;

	private ColumnHeader StatusColumn;

	private ColumnHeader UserColumn;

	private ColumnHeader CriticalColumn;

	private ColumnHeader DescriptionColumn;

	private ListView ServiceView;

	private ColumnHeader srvName;

	private ColumnHeader srvDescription;

	private ColumnHeader srvStatus;

	private ColumnHeader srvType;

	private System.Windows.Forms.Timer PrcTimer;

	private System.Windows.Forms.Timer CPUTimer;

	private System.Windows.Forms.Timer RAMTimer;

	private ContextMenuStrip ProcessContextMenu;

	private ToolStripMenuItem deletePrcMItem;

	private ToolStripMenuItem suspendPrcMItem;

	private ToolStripMenuItem criticalPrcMItem;

	private ToolStripSeparator separatorMItem;

	private ToolStripMenuItem addonMItem;

	private ToolStripMenuItem antiGDIMItem;

	private ToolStripMenuItem prcPathMItem;

	private ToolStripMenuItem infoPrcMItem;

	private ToolStripMenuItem addonStripItem;

	private ToolStripMenuItem antiGDIStripItem;

	private ToolStripMenuItem settingsStripItem;

	private ToolStripMenuItem autoCheckPrcStripItem;

	private ToolStripMenuItem topMostStripItem;

	private ToolStripMenuItem noHiddenFormStripItem;

	private ToolStripMenuItem uncriticalPrcStripItem;

	private System.Windows.Forms.Timer TopMostTimer;

	private System.Windows.Forms.Timer CheckMinimaze;

	private ToolStripMenuItem runStripItem;

	private ContextMenuStrip ServiceContextMenu;

	private ToolStripMenuItem runSrvMItem;

	private ToolStripMenuItem stopSrvMItem;

	private ToolStripMenuItem deleteSrvMItem;

	private ToolStripStatusLabel addonStripLabel;

	private ListViewItem ToListViewItem(Prc prc)
	{
		ListViewItem listViewItem = new ListViewItem(new string[6]
		{
			prc.ProcessName,
			prc.ProcessID.ToString(),
			prc.ProcessState,
			prc.ProcessOwner,
			prc.ProcessCritical.ToString(),
			prc.ProcessDescription
		});
		string text = prc.ProcessOwner?.ToLower();
		string obj = prc.ProcessState.ToLower();
		if (text != null)
		{
			if (text.Contains("service"))
			{
				listViewItem.BackColor = Color.SkyBlue;
			}
			if (text.Contains("система"))
			{
				listViewItem.BackColor = Color.Aqua;
			}
		}
		if (prc.ProcessCritical == true)
		{
			listViewItem.BackColor = Color.Orange;
		}
		if (obj == "suspended")
		{
			listViewItem.BackColor = Color.Gray;
		}
		listViewItem.Tag = prc;
		return listViewItem;
	}

	private async Task GetProcesses()
	{
		logger.Info((object)"Получение процессов");
		TMView.BeginUpdate();
		TMView.Items.Clear();
		runningProcess = await taskMgrCore.GetListProcesses();
		foreach (Prc item in runningProcess)
		{
			TMView.Items.Add(ToListViewItem(item));
		}
		TMView.EndUpdate();
		logger.Info((object)$"Процессов в TMView: {TMView.Items.Count}");
	}

	private async Task GetServices()
	{
		logger.Info((object)"Получение служб");
		ServiceView.BeginUpdate();
		ServiceView.Items.Clear();
		foreach (Srv item in await taskMgrCore.GetListServicesAsync())
		{
			ServiceView.Items.Add(new ListViewItem(new string[4] { item.serviceName, item.serviceDisplayName, item.serviceStatus, item.serviceStartType })
			{
				Tag = item
			});
		}
		ServiceView.EndUpdate();
	}

	private void DeleteProcess(uint processId)
	{
		foreach (ListViewItem item in TMView.Items)
		{
			Prc prc = (Prc)item.Tag;
			if (Convert.ToUInt32(prc.ProcessID) == processId)
			{
				logger.Info((object)("Процесс: " + prc.ProcessName + " был завершён"));
				TMView.Items.Remove(item);
				TMView.Update();
				break;
			}
		}
	}

	private void StartNewProcess(uint processId)
	{
		List<Process> list = taskMgrCore.UpdateProcessList();
		if (list == null || list.Count == 0)
		{
			return;
		}
		foreach (Process prc in list)
		{
			Prc result = Task.Run(async () => await taskMgrCore.ToPrcClass(prc)).Result;
			logger.Info((object)("Процесс: " + result.ProcessName + " был запущен"));
			TMView.Items.Add(ToListViewItem(result));
		}
	}

	public TaskManager()
	{
		InitializeComponent();
		ThreadPool.SetMinThreads(1000, 1000);
	}

	private void TaskManager_Shown(object sender, EventArgs e)
	{
		taskMgrCore = new TMCore();
		GetProcesses();
		GetServices();
		taskMgrCore.InitEventsProcess();
		taskMgrCore.NotifyStopProcess += DeleteProcess;
		taskMgrCore.NotifyStartNewProcess += StartNewProcess;
		CPUTimer.Enabled = true;
		RAMTimer.Enabled = true;
		PrcTimer.Enabled = true;
	}

	private async void RefreshMenuBtn_Click(object sender, EventArgs e)
	{
		RefreshMenuBtn.Enabled = false;
		await GetProcesses();
		RefreshMenuBtn.Enabled = true;
	}

	private void PrcTimer_Tick(object sender, EventArgs e)
	{
		ProcessesSSLabel.Text = $"Процессов: {Process.GetProcesses().Length}";
	}

	private void CPUTimer_Tick(object sender, EventArgs e)
	{
		CPUSSLabel.Text = $"CPU Usage: {Math.Round(CPU.NextValue())} %";
	}

	private void RAMTimer_Tick(object sender, EventArgs e)
	{
		RAMSSLabel.Text = $"RAM Free: {RAM.NextValue()} MB";
	}

	private void deletePrcMItem_Click(object sender, EventArgs e)
	{
		taskMgrCore.TerminateProcesses(TMView, uncriticalPrcStripItem.Checked);
	}

	private void TMView_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && TMView.FocusedItem != null && TMView.FocusedItem.Bounds.Contains(e.Location))
		{
			Prc prc = (Prc)TMView.FocusedItem.Tag;
			suspendPrcMItem.Text = ((prc.ProcessState == "Suspended") ? "Разморозить" : "Заморозить");
			criticalPrcMItem.Text = ((prc.ProcessCritical == true) ? "Сделать не критическим" : "Сделать критическим");
			ProcessContextMenu.Show(Cursor.Position);
		}
	}

	private async void suspendPrcMItem_Click(object sender, EventArgs e)
	{
		Prc prc = (Prc)TMView.FocusedItem.Tag;
		if (prc.ProcessState == "Suspended")
		{
			taskMgrCore.ResumeProcess(prc.ProcessID);
		}
		else
		{
			taskMgrCore.SuspendProcess(prc.ProcessID);
		}
		ListView.ListViewItemCollection items = TMView.Items;
		int index = TMView.FocusedItem.Index;
		items[index] = ToListViewItem(await taskMgrCore.UpdateProcessInfo(prc.ProcessID));
	}

	private async void criticalPrcMItem_Click(object sender, EventArgs e)
	{
		Prc prc = (Prc)TMView.FocusedItem.Tag;
		using (Process process = Process.GetProcessById(prc.ProcessID))
		{
			taskMgrCore.CriticalProcess(process, (prc.ProcessCritical != true) ? 1 : 0);
		}
		ListView.ListViewItemCollection items = TMView.Items;
		int index = TMView.FocusedItem.Index;
		items[index] = ToListViewItem(await taskMgrCore.UpdateProcessInfo(prc.ProcessID));
	}

	private void prcPathMItem_Click(object sender, EventArgs e)
	{
		using Process process = Process.GetProcessById(((Prc)TMView.FocusedItem.Tag).ProcessID);
		Utils.RunFile("explorer.exe", "/n,/select, \"" + process.MainModule.FileName + "\"", UAC: false, Hidden: false, WaitForExit: false);
	}

	private void infoPrcMItem_Click(object sender, EventArgs e)
	{
		using Process process = Process.GetProcessById(((Prc)TMView.FocusedItem.Tag).ProcessID);
		Utils.ShowFileProperties(process.MainModule.FileName);
	}

	private void TerminateMenuBtn_Click(object sender, EventArgs e)
	{
		if (TMView.SelectedIndices.Count > 0)
		{
			taskMgrCore.TerminateProcesses(TMView, uncriticalPrcStripItem.Checked);
		}
	}

	private void TaskManager_FormClosing(object sender, FormClosingEventArgs e)
	{
		Dispose();
		taskMgrCore.EndEventsProcess();
		Utils.CloseForm(e);
	}

	private async void autoCheckPrcStripItem_Click(object sender, EventArgs e)
	{
		if (autoCheckPrcStripItem.Checked)
		{
			taskMgrCore.InitEventsProcess();
		}
		else
		{
			taskMgrCore.EndEventsProcess();
		}
		await GetProcesses();
	}

	private void topMostStripItem_Click(object sender, EventArgs e)
	{
		TopMostTimer.Enabled = topMostStripItem.Checked;
		if (!topMostStripItem.Checked && !SU.Properties.Settings.Default.AlwaysOnTop)
		{
			Utils.SetWindowPos(base.Handle, Utils.HWND_NOTOPMOST, 0, 0, 0, 0, 3u);
		}
	}

	private void TopMostTimer_Tick(object sender, EventArgs e)
	{
		Utils.SetWindowPos(base.Handle, Utils.HWND_TOPMOST, 0, 0, 0, 0, 3u);
	}

	private void noHiddenFormStripItem_Click(object sender, EventArgs e)
	{
		CheckMinimaze.Enabled = noHiddenFormStripItem.Checked;
	}

	private void CheckMinimaze_Tick(object sender, EventArgs e)
	{
		if (base.WindowState == FormWindowState.Minimized)
		{
			base.WindowState = FormWindowState.Normal;
		}
	}

	private async void SuspendMenuBtn_Click(object sender, EventArgs e)
	{
		if (TMView.SelectedIndices.Count <= 0)
		{
			return;
		}
		foreach (int selectedIndex in TMView.SelectedIndices)
		{
			Prc prc = (Prc)TMView.Items[selectedIndex].Tag;
			if (!prc.ProcessState.ToLower().Contains("suspended"))
			{
				taskMgrCore.SuspendProcess(prc.ProcessID);
				ListView.ListViewItemCollection items = TMView.Items;
				int index2 = TMView.FocusedItem.Index;
				items[index2] = ToListViewItem(await taskMgrCore.UpdateProcessInfo(prc.ProcessID));
			}
		}
	}

	private async void ResumeMenuBtn_Click(object sender, EventArgs e)
	{
		if (TMView.SelectedIndices.Count <= 0)
		{
			return;
		}
		foreach (int selectedIndex in TMView.SelectedIndices)
		{
			Prc prc = (Prc)TMView.Items[selectedIndex].Tag;
			if (prc.ProcessState.ToLower().Contains("suspended"))
			{
				taskMgrCore.ResumeProcess(prc.ProcessID);
				ListView.ListViewItemCollection items = TMView.Items;
				int index2 = TMView.FocusedItem.Index;
				items[index2] = ToListViewItem(await taskMgrCore.UpdateProcessInfo(prc.ProcessID));
			}
		}
	}

	private async void CriticalMenuBtn_Click(object sender, EventArgs e)
	{
		if (TMView.SelectedIndices.Count <= 0)
		{
			return;
		}
		foreach (int selectedIndex in TMView.SelectedIndices)
		{
			Prc prc = (Prc)TMView.Items[selectedIndex].Tag;
			try
			{
				using Process p = Process.GetProcessById(prc.ProcessID);
				taskMgrCore.CriticalProcess(p, 1);
				ListView.ListViewItemCollection items = TMView.Items;
				int index2 = TMView.FocusedItem.Index;
				items[index2] = ToListViewItem(await taskMgrCore.UpdateProcessInfo(prc.ProcessID));
			}
			catch (Exception ex)
			{
				logger.Error((object)ex);
			}
		}
	}

	private async void UncriticalMenuBtn_Click(object sender, EventArgs e)
	{
		if (TMView.SelectedIndices.Count <= 0)
		{
			return;
		}
		foreach (int selectedIndex in TMView.SelectedIndices)
		{
			Prc prc = (Prc)TMView.Items[selectedIndex].Tag;
			try
			{
				using Process p = Process.GetProcessById(prc.ProcessID);
				taskMgrCore.CriticalProcess(p, 0);
				ListView.ListViewItemCollection items = TMView.Items;
				int index2 = TMView.FocusedItem.Index;
				items[index2] = ToListViewItem(await taskMgrCore.UpdateProcessInfo(prc.ProcessID));
			}
			catch (Exception ex)
			{
				logger.Error((object)ex);
			}
		}
	}

	private void ServiceView_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && ServiceView.FocusedItem != null && ServiceView.FocusedItem.Bounds.Contains(e.Location))
		{
			ServiceContextMenu.Show(Cursor.Position);
		}
	}

	private void runSrvMItem_Click(object sender, EventArgs e)
	{
		Srv srv = (Srv)ServiceView.FocusedItem.Tag;
		sc.ServiceName = srv.serviceName;
		addonStripLabel.Text = "Запускаю службу \"" + srv.serviceName + "\"...";
		try
		{
			sc.Start();
			Task.Run(delegate
			{
				try
				{
					sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30.0));
				}
				catch (Exception ex2)
				{
					logger.Error((object)ex2.Message);
				}
			}).ContinueWith(delegate
			{
				GetServices();
				addonStripLabel.Text = "";
			}, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
		}
		catch (InvalidOperationException ex)
		{
			addonStripLabel.Text = "";
			logger.Error((object)ex.Message);
			MessageBox.Show("Не удалось запустить службу \"" + srv.serviceName + "\" по следующей причине: \"" + ex.Message + "\"", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void stopSrvMItem_Click(object sender, EventArgs e)
	{
		Srv srv = (Srv)ServiceView.FocusedItem.Tag;
		sc.ServiceName = srv.serviceName;
		addonStripLabel.Text = "Останавливаю службу \"" + srv.serviceName + "\"...";
		try
		{
			sc.Stop();
			Task.Run(delegate
			{
				try
				{
					sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(30.0));
				}
				catch (Exception ex2)
				{
					logger.Error((object)ex2.Message);
				}
			}).ContinueWith(delegate
			{
				GetServices();
				addonStripLabel.Text = "";
			}, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
		}
		catch (InvalidOperationException ex)
		{
			addonStripLabel.Text = "";
			logger.Error((object)ex.Message);
			MessageBox.Show("Не удалось остановить службу \"" + srv.serviceName + "\" по следующей причине: \"" + ex.Message + "\"", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void deleteSrvMItem_Click(object sender, EventArgs e)
	{
		Srv srv = (Srv)ServiceView.FocusedItem.Tag;
		sc.ServiceName = srv.serviceName;
		addonStripLabel.Text = "Останавливаю службу \"" + srv.serviceName + "\"...";
		try
		{
			sc.Stop();
			Task.Run(delegate
			{
				try
				{
					sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(30.0));
				}
				catch (Exception ex3)
				{
					logger.Error((object)ex3.Message);
				}
			}).ContinueWith(delegate
			{
				GetServices();
				addonStripLabel.Text = "";
			}, System.Threading.Tasks.TaskScheduler.FromCurrentSynchronizationContext());
		}
		catch (InvalidOperationException ex)
		{
			addonStripLabel.Text = "";
			logger.Error((object)ex.Message);
			MessageBox.Show("Не удалось остановить службу \"" + srv.serviceName + "\" по следующей причине: \"" + ex.Message + "\"", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		addonStripLabel.Text = "Удаление службы \"" + srv.serviceName + "\"...";
		try
		{
			srv.Delete();
		}
		catch (Exception ex2)
		{
			MessageBox.Show("Не удалось удалить службу \"" + srv.serviceName + "\". Причина: \"" + ex2.Message + "\"", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
		finally
		{
			GetServices();
			addonStripLabel.Text = "";
		}
	}

	private void runStripItem_Click(object sender, EventArgs e)
	{
		using Run run = new Run();
		run.StartPosition = FormStartPosition.Manual;
		run.Location = base.Location;
		run.ShowDialog();
	}

	private void BackToMainMenuStrip_Click(object sender, EventArgs e)
	{
		Dispose();
		Close();
	}

	private void ExitStrip_Click(object sender, EventArgs e)
	{
		Environment.Exit(0);
	}

	private async void TMView_KeyDown(object sender, KeyEventArgs e)
	{
		if (TMView.SelectedItems.Count <= 0)
		{
			return;
		}
		switch (e.KeyCode)
		{
		case Keys.Delete:
			taskMgrCore.TerminateProcesses(TMView, uncriticalPrcStripItem.Checked);
			break;
		case Keys.Pause:
			foreach (ListViewItem selectedItem in TMView.SelectedItems)
			{
				Prc prc = (Prc)selectedItem.Tag;
				if (prc.ProcessState.ToLower() == "suspended")
				{
					taskMgrCore.ResumeProcess(prc.ProcessID);
				}
				else
				{
					taskMgrCore.SuspendProcess(prc.ProcessID);
				}
				ListView.ListViewItemCollection items = TMView.Items;
				int index = selectedItem.Index;
				items[index] = ToListViewItem(await taskMgrCore.UpdateProcessInfo(prc.ProcessID));
			}
			break;
		}
		if (e.Modifiers != Keys.Control)
		{
			return;
		}
		switch (e.KeyCode)
		{
		case Keys.C:
			foreach (ListViewItem selectedItem2 in TMView.SelectedItems)
			{
				Prc prc3 = (Prc)selectedItem2.Tag;
				using Process p = Process.GetProcessById(prc3.ProcessID);
				taskMgrCore.CriticalProcess(p, (prc3.ProcessCritical != true) ? 1 : 0);
				ListView.ListViewItemCollection items = TMView.Items;
				int index = selectedItem2.Index;
				items[index] = ToListViewItem(await taskMgrCore.ToPrcClass(p));
			}
			break;
		case Keys.A:
		{
			foreach (ListViewItem selectedItem3 in TMView.SelectedItems)
			{
				Prc prc2 = (Prc)selectedItem3.Tag;
				Utils.RunFile("bin\\AntiGDI_Injector.exe", $"{prc2.ProcessID}", UAC: true, Hidden: false, WaitForExit: false);
			}
			break;
		}
		}
	}

	private void antiGDIStripItem_Click(object sender, EventArgs e)
	{
		new AntiGDI().ShowDialog();
	}

	private void antiGDIMItem_Click(object sender, EventArgs e)
	{
		Prc prc = (Prc)TMView.FocusedItem.Tag;
		Utils.RunFile("bin\\AntiGDI_Injector.exe", $"{prc.ProcessID}", UAC: true, Hidden: false, WaitForExit: false);
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SU.Forms.TaskManager));
		this.ManagerStatusStrip = new System.Windows.Forms.StatusStrip();
		this.CPUSSLabel = new System.Windows.Forms.ToolStripStatusLabel();
		this.RAMSSLabel = new System.Windows.Forms.ToolStripStatusLabel();
		this.ProcessesSSLabel = new System.Windows.Forms.ToolStripStatusLabel();
		this.addonStripLabel = new System.Windows.Forms.ToolStripStatusLabel();
		this.ManagerMenu = new System.Windows.Forms.MenuStrip();
		this.MenuStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.runStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.BackToMainMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.ExitStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.addonStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.antiGDIStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.settingsStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.autoCheckPrcStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.topMostStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.noHiddenFormStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.uncriticalPrcStripItem = new System.Windows.Forms.ToolStripMenuItem();
		this.ManagerMenuStrip = new System.Windows.Forms.ToolStrip();
		this.RefreshMenuBtn = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.TerminateMenuBtn = new System.Windows.Forms.ToolStripButton();
		this.SuspendMenuBtn = new System.Windows.Forms.ToolStripButton();
		this.ResumeMenuBtn = new System.Windows.Forms.ToolStripButton();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.CriticalMenuBtn = new System.Windows.Forms.ToolStripButton();
		this.UncriticalMenuBtn = new System.Windows.Forms.ToolStripButton();
		this.ManagerMode = new System.Windows.Forms.TabControl();
		this.TaskMgrPage = new System.Windows.Forms.TabPage();
		this.TMView = new System.Windows.Forms.ListView();
		this.ProcessColumn = new System.Windows.Forms.ColumnHeader();
		this.IdColumn = new System.Windows.Forms.ColumnHeader();
		this.StatusColumn = new System.Windows.Forms.ColumnHeader();
		this.UserColumn = new System.Windows.Forms.ColumnHeader();
		this.CriticalColumn = new System.Windows.Forms.ColumnHeader();
		this.DescriptionColumn = new System.Windows.Forms.ColumnHeader();
		this.ServicesPage = new System.Windows.Forms.TabPage();
		this.ServiceView = new System.Windows.Forms.ListView();
		this.srvName = new System.Windows.Forms.ColumnHeader();
		this.srvDescription = new System.Windows.Forms.ColumnHeader();
		this.srvStatus = new System.Windows.Forms.ColumnHeader();
		this.srvType = new System.Windows.Forms.ColumnHeader();
		this.PrcTimer = new System.Windows.Forms.Timer(this.components);
		this.CPUTimer = new System.Windows.Forms.Timer(this.components);
		this.RAMTimer = new System.Windows.Forms.Timer(this.components);
		this.ProcessContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deletePrcMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.suspendPrcMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.criticalPrcMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.separatorMItem = new System.Windows.Forms.ToolStripSeparator();
		this.addonMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.antiGDIMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.prcPathMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.infoPrcMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.TopMostTimer = new System.Windows.Forms.Timer(this.components);
		this.CheckMinimaze = new System.Windows.Forms.Timer(this.components);
		this.ServiceContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.runSrvMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.stopSrvMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteSrvMItem = new System.Windows.Forms.ToolStripMenuItem();
		this.ManagerStatusStrip.SuspendLayout();
		this.ManagerMenu.SuspendLayout();
		this.ManagerMenuStrip.SuspendLayout();
		this.ManagerMode.SuspendLayout();
		this.TaskMgrPage.SuspendLayout();
		this.ServicesPage.SuspendLayout();
		this.ProcessContextMenu.SuspendLayout();
		this.ServiceContextMenu.SuspendLayout();
		base.SuspendLayout();
		this.ManagerStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.CPUSSLabel, this.RAMSSLabel, this.ProcessesSSLabel, this.addonStripLabel });
		this.ManagerStatusStrip.Location = new System.Drawing.Point(0, 524);
		this.ManagerStatusStrip.Name = "ManagerStatusStrip";
		this.ManagerStatusStrip.Size = new System.Drawing.Size(950, 22);
		this.ManagerStatusStrip.TabIndex = 1;
		this.ManagerStatusStrip.Text = "statusStrip1";
		this.CPUSSLabel.Name = "CPUSSLabel";
		this.CPUSSLabel.Size = new System.Drawing.Size(87, 17);
		this.CPUSSLabel.Text = "CPU Usage: 0%";
		this.RAMSSLabel.Name = "RAMSSLabel";
		this.RAMSSLabel.Size = new System.Drawing.Size(101, 17);
		this.RAMSSLabel.Text = "RAM Usage: 0 MB";
		this.ProcessesSSLabel.Name = "ProcessesSSLabel";
		this.ProcessesSSLabel.Size = new System.Drawing.Size(80, 17);
		this.ProcessesSSLabel.Text = "Процессов: 0";
		this.addonStripLabel.Name = "addonStripLabel";
		this.addonStripLabel.Size = new System.Drawing.Size(0, 17);
		this.ManagerMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.MenuStrip, this.addonStripItem, this.settingsStripItem });
		this.ManagerMenu.Location = new System.Drawing.Point(0, 0);
		this.ManagerMenu.Name = "ManagerMenu";
		this.ManagerMenu.Size = new System.Drawing.Size(950, 24);
		this.ManagerMenu.TabIndex = 4;
		this.ManagerMenu.Text = "menuStrip1";
		this.MenuStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.runStripItem, this.BackToMainMenuStrip, this.ExitStrip });
		this.MenuStrip.Name = "MenuStrip";
		this.MenuStrip.Size = new System.Drawing.Size(53, 20);
		this.MenuStrip.Text = "Меню";
		this.runStripItem.Name = "runStripItem";
		this.runStripItem.Size = new System.Drawing.Size(215, 22);
		this.runStripItem.Text = "Выполнить";
		this.runStripItem.Click += new System.EventHandler(runStripItem_Click);
		this.BackToMainMenuStrip.Name = "BackToMainMenuStrip";
		this.BackToMainMenuStrip.Size = new System.Drawing.Size(215, 22);
		this.BackToMainMenuStrip.Text = "Вернутся в главное меню";
		this.BackToMainMenuStrip.Click += new System.EventHandler(BackToMainMenuStrip_Click);
		this.ExitStrip.Name = "ExitStrip";
		this.ExitStrip.Size = new System.Drawing.Size(215, 22);
		this.ExitStrip.Text = "Выход";
		this.ExitStrip.Click += new System.EventHandler(ExitStrip_Click);
		this.addonStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.antiGDIStripItem });
		this.addonStripItem.Name = "addonStripItem";
		this.addonStripItem.Size = new System.Drawing.Size(107, 20);
		this.addonStripItem.Text = "Дополнительно";
		this.antiGDIStripItem.Name = "antiGDIStripItem";
		this.antiGDIStripItem.Size = new System.Drawing.Size(115, 22);
		this.antiGDIStripItem.Text = "AntiGDI";
		this.antiGDIStripItem.Click += new System.EventHandler(antiGDIStripItem_Click);
		this.settingsStripItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.autoCheckPrcStripItem, this.topMostStripItem, this.noHiddenFormStripItem, this.uncriticalPrcStripItem });
		this.settingsStripItem.Name = "settingsStripItem";
		this.settingsStripItem.Size = new System.Drawing.Size(79, 20);
		this.settingsStripItem.Text = "Настройки";
		this.autoCheckPrcStripItem.Checked = true;
		this.autoCheckPrcStripItem.CheckOnClick = true;
		this.autoCheckPrcStripItem.CheckState = System.Windows.Forms.CheckState.Checked;
		this.autoCheckPrcStripItem.Name = "autoCheckPrcStripItem";
		this.autoCheckPrcStripItem.Size = new System.Drawing.Size(369, 22);
		this.autoCheckPrcStripItem.Text = "Автообновление списка процессов";
		this.autoCheckPrcStripItem.Click += new System.EventHandler(autoCheckPrcStripItem_Click);
		this.topMostStripItem.CheckOnClick = true;
		this.topMostStripItem.Name = "topMostStripItem";
		this.topMostStripItem.Size = new System.Drawing.Size(369, 22);
		this.topMostStripItem.Text = "Принудительно поверх всех окон";
		this.topMostStripItem.Click += new System.EventHandler(topMostStripItem_Click);
		this.noHiddenFormStripItem.CheckOnClick = true;
		this.noHiddenFormStripItem.Name = "noHiddenFormStripItem";
		this.noHiddenFormStripItem.Size = new System.Drawing.Size(369, 22);
		this.noHiddenFormStripItem.Text = "Предотвращение сворачивания окна";
		this.noHiddenFormStripItem.Click += new System.EventHandler(noHiddenFormStripItem_Click);
		this.uncriticalPrcStripItem.Checked = true;
		this.uncriticalPrcStripItem.CheckOnClick = true;
		this.uncriticalPrcStripItem.CheckState = System.Windows.Forms.CheckState.Checked;
		this.uncriticalPrcStripItem.Name = "uncriticalPrcStripItem";
		this.uncriticalPrcStripItem.Size = new System.Drawing.Size(369, 22);
		this.uncriticalPrcStripItem.Text = "Делать процесс не критическим при его завершении";
		this.ManagerMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.RefreshMenuBtn, this.toolStripSeparator1, this.TerminateMenuBtn, this.SuspendMenuBtn, this.ResumeMenuBtn, this.toolStripSeparator2, this.CriticalMenuBtn, this.UncriticalMenuBtn });
		this.ManagerMenuStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
		this.ManagerMenuStrip.Location = new System.Drawing.Point(0, 24);
		this.ManagerMenuStrip.Name = "ManagerMenuStrip";
		this.ManagerMenuStrip.Size = new System.Drawing.Size(950, 23);
		this.ManagerMenuStrip.TabIndex = 5;
		this.RefreshMenuBtn.Image = (System.Drawing.Image)resources.GetObject("RefreshMenuBtn.Image");
		this.RefreshMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.RefreshMenuBtn.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
		this.RefreshMenuBtn.Name = "RefreshMenuBtn";
		this.RefreshMenuBtn.Size = new System.Drawing.Size(81, 20);
		this.RefreshMenuBtn.Text = "Обновить";
		this.RefreshMenuBtn.Click += new System.EventHandler(RefreshMenuBtn_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
		this.TerminateMenuBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.TerminateMenuBtn.Image = (System.Drawing.Image)resources.GetObject("TerminateMenuBtn.Image");
		this.TerminateMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.TerminateMenuBtn.Name = "TerminateMenuBtn";
		this.TerminateMenuBtn.Size = new System.Drawing.Size(23, 20);
		this.TerminateMenuBtn.Text = "toolStripButton2";
		this.TerminateMenuBtn.ToolTipText = "Завершить процесс";
		this.TerminateMenuBtn.Click += new System.EventHandler(TerminateMenuBtn_Click);
		this.SuspendMenuBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.SuspendMenuBtn.Image = (System.Drawing.Image)resources.GetObject("SuspendMenuBtn.Image");
		this.SuspendMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.SuspendMenuBtn.Name = "SuspendMenuBtn";
		this.SuspendMenuBtn.Size = new System.Drawing.Size(23, 20);
		this.SuspendMenuBtn.Text = "Заморозить";
		this.SuspendMenuBtn.ToolTipText = "Заморозить процесс";
		this.SuspendMenuBtn.Click += new System.EventHandler(SuspendMenuBtn_Click);
		this.ResumeMenuBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.ResumeMenuBtn.Image = (System.Drawing.Image)resources.GetObject("ResumeMenuBtn.Image");
		this.ResumeMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.ResumeMenuBtn.Name = "ResumeMenuBtn";
		this.ResumeMenuBtn.Size = new System.Drawing.Size(23, 20);
		this.ResumeMenuBtn.Text = "Разморозить";
		this.ResumeMenuBtn.ToolTipText = "Разморозить процесс";
		this.ResumeMenuBtn.Click += new System.EventHandler(ResumeMenuBtn_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(6, 23);
		this.CriticalMenuBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.CriticalMenuBtn.Image = (System.Drawing.Image)resources.GetObject("CriticalMenuBtn.Image");
		this.CriticalMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.CriticalMenuBtn.Name = "CriticalMenuBtn";
		this.CriticalMenuBtn.Size = new System.Drawing.Size(23, 20);
		this.CriticalMenuBtn.Text = "Critical";
		this.CriticalMenuBtn.ToolTipText = "Сделать процесс критическим";
		this.CriticalMenuBtn.Click += new System.EventHandler(CriticalMenuBtn_Click);
		this.UncriticalMenuBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.UncriticalMenuBtn.Image = (System.Drawing.Image)resources.GetObject("UncriticalMenuBtn.Image");
		this.UncriticalMenuBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.UncriticalMenuBtn.Name = "UncriticalMenuBtn";
		this.UncriticalMenuBtn.Size = new System.Drawing.Size(23, 20);
		this.UncriticalMenuBtn.Text = "Uncritical";
		this.UncriticalMenuBtn.ToolTipText = "Сделать процесс некритическим";
		this.UncriticalMenuBtn.Click += new System.EventHandler(UncriticalMenuBtn_Click);
		this.ManagerMode.Controls.Add(this.TaskMgrPage);
		this.ManagerMode.Controls.Add(this.ServicesPage);
		this.ManagerMode.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ManagerMode.Location = new System.Drawing.Point(0, 47);
		this.ManagerMode.Name = "ManagerMode";
		this.ManagerMode.SelectedIndex = 0;
		this.ManagerMode.Size = new System.Drawing.Size(950, 477);
		this.ManagerMode.TabIndex = 6;
		this.TaskMgrPage.Controls.Add(this.TMView);
		this.TaskMgrPage.Location = new System.Drawing.Point(4, 22);
		this.TaskMgrPage.Name = "TaskMgrPage";
		this.TaskMgrPage.Padding = new System.Windows.Forms.Padding(3);
		this.TaskMgrPage.Size = new System.Drawing.Size(942, 451);
		this.TaskMgrPage.TabIndex = 0;
		this.TaskMgrPage.Text = "Процессы";
		this.TaskMgrPage.UseVisualStyleBackColor = true;
		this.TMView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[6] { this.ProcessColumn, this.IdColumn, this.StatusColumn, this.UserColumn, this.CriticalColumn, this.DescriptionColumn });
		this.TMView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.TMView.FullRowSelect = true;
		this.TMView.GridLines = true;
		this.TMView.HideSelection = false;
		this.TMView.Location = new System.Drawing.Point(3, 3);
		this.TMView.Name = "TMView";
		this.TMView.Size = new System.Drawing.Size(936, 445);
		this.TMView.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.TMView.TabIndex = 0;
		this.TMView.UseCompatibleStateImageBehavior = false;
		this.TMView.View = System.Windows.Forms.View.Details;
		this.TMView.KeyDown += new System.Windows.Forms.KeyEventHandler(TMView_KeyDown);
		this.TMView.MouseClick += new System.Windows.Forms.MouseEventHandler(TMView_MouseClick);
		this.ProcessColumn.Text = "Process";
		this.ProcessColumn.Width = 222;
		this.IdColumn.Text = "ID";
		this.IdColumn.Width = 70;
		this.StatusColumn.Text = "Status";
		this.StatusColumn.Width = 100;
		this.UserColumn.Text = "Username";
		this.UserColumn.Width = 153;
		this.CriticalColumn.Text = "Critical";
		this.DescriptionColumn.Text = "Description";
		this.DescriptionColumn.Width = 324;
		this.ServicesPage.Controls.Add(this.ServiceView);
		this.ServicesPage.Location = new System.Drawing.Point(4, 22);
		this.ServicesPage.Name = "ServicesPage";
		this.ServicesPage.Padding = new System.Windows.Forms.Padding(3);
		this.ServicesPage.Size = new System.Drawing.Size(942, 451);
		this.ServicesPage.TabIndex = 1;
		this.ServicesPage.Text = "Службы";
		this.ServicesPage.UseVisualStyleBackColor = true;
		this.ServiceView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] { this.srvName, this.srvDescription, this.srvStatus, this.srvType });
		this.ServiceView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ServiceView.FullRowSelect = true;
		this.ServiceView.GridLines = true;
		this.ServiceView.HideSelection = false;
		this.ServiceView.Location = new System.Drawing.Point(3, 3);
		this.ServiceView.Name = "ServiceView";
		this.ServiceView.Size = new System.Drawing.Size(936, 445);
		this.ServiceView.TabIndex = 1;
		this.ServiceView.UseCompatibleStateImageBehavior = false;
		this.ServiceView.View = System.Windows.Forms.View.Details;
		this.ServiceView.MouseClick += new System.Windows.Forms.MouseEventHandler(ServiceView_MouseClick);
		this.srvName.Text = "Имя службы";
		this.srvName.Width = 209;
		this.srvDescription.Text = "Отображаемое имя";
		this.srvDescription.Width = 468;
		this.srvStatus.Text = "Статус";
		this.srvStatus.Width = 100;
		this.srvType.Text = "Тип запуска";
		this.srvType.Width = 153;
		this.PrcTimer.Interval = 1000;
		this.PrcTimer.Tick += new System.EventHandler(PrcTimer_Tick);
		this.CPUTimer.Interval = 1000;
		this.CPUTimer.Tick += new System.EventHandler(CPUTimer_Tick);
		this.RAMTimer.Enabled = true;
		this.RAMTimer.Interval = 1000;
		this.RAMTimer.Tick += new System.EventHandler(RAMTimer_Tick);
		this.ProcessContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.deletePrcMItem, this.suspendPrcMItem, this.criticalPrcMItem, this.separatorMItem, this.addonMItem, this.prcPathMItem, this.infoPrcMItem });
		this.ProcessContextMenu.Name = "ProcessContextMenu";
		this.ProcessContextMenu.Size = new System.Drawing.Size(237, 142);
		this.deletePrcMItem.Name = "deletePrcMItem";
		this.deletePrcMItem.ShortcutKeyDisplayString = "Delete";
		this.deletePrcMItem.Size = new System.Drawing.Size(236, 22);
		this.deletePrcMItem.Text = "Завершить";
		this.deletePrcMItem.Click += new System.EventHandler(deletePrcMItem_Click);
		this.suspendPrcMItem.Name = "suspendPrcMItem";
		this.suspendPrcMItem.ShortcutKeyDisplayString = "Pause";
		this.suspendPrcMItem.Size = new System.Drawing.Size(236, 22);
		this.suspendPrcMItem.Text = "Заморозить";
		this.suspendPrcMItem.Click += new System.EventHandler(suspendPrcMItem_Click);
		this.criticalPrcMItem.Name = "criticalPrcMItem";
		this.criticalPrcMItem.ShortcutKeyDisplayString = "Ctrl+C";
		this.criticalPrcMItem.Size = new System.Drawing.Size(236, 22);
		this.criticalPrcMItem.Text = "Сделать критическим";
		this.criticalPrcMItem.Click += new System.EventHandler(criticalPrcMItem_Click);
		this.separatorMItem.Name = "separatorMItem";
		this.separatorMItem.Size = new System.Drawing.Size(233, 6);
		this.addonMItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.antiGDIMItem });
		this.addonMItem.Name = "addonMItem";
		this.addonMItem.Size = new System.Drawing.Size(236, 22);
		this.addonMItem.Text = "Дополнительно";
		this.antiGDIMItem.Name = "antiGDIMItem";
		this.antiGDIMItem.ShortcutKeyDisplayString = "Ctrl+A";
		this.antiGDIMItem.Size = new System.Drawing.Size(157, 22);
		this.antiGDIMItem.Text = "AntiGDI";
		this.antiGDIMItem.Click += new System.EventHandler(antiGDIMItem_Click);
		this.prcPathMItem.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.prcPathMItem.Name = "prcPathMItem";
		this.prcPathMItem.Size = new System.Drawing.Size(236, 22);
		this.prcPathMItem.Text = "Расположение файла";
		this.prcPathMItem.Click += new System.EventHandler(prcPathMItem_Click);
		this.infoPrcMItem.Font = new System.Drawing.Font("Segoe UI", 9f, System.Drawing.FontStyle.Bold);
		this.infoPrcMItem.Name = "infoPrcMItem";
		this.infoPrcMItem.Size = new System.Drawing.Size(236, 22);
		this.infoPrcMItem.Text = "Свойства";
		this.infoPrcMItem.Click += new System.EventHandler(infoPrcMItem_Click);
		this.TopMostTimer.Interval = 1;
		this.TopMostTimer.Tick += new System.EventHandler(TopMostTimer_Tick);
		this.CheckMinimaze.Interval = 1;
		this.CheckMinimaze.Tick += new System.EventHandler(CheckMinimaze_Tick);
		this.ServiceContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.runSrvMItem, this.stopSrvMItem, this.deleteSrvMItem });
		this.ServiceContextMenu.Name = "ServiceContextMenu";
		this.ServiceContextMenu.Size = new System.Drawing.Size(139, 70);
		this.runSrvMItem.Name = "runSrvMItem";
		this.runSrvMItem.Size = new System.Drawing.Size(138, 22);
		this.runSrvMItem.Text = "Запустить";
		this.runSrvMItem.Click += new System.EventHandler(runSrvMItem_Click);
		this.stopSrvMItem.Name = "stopSrvMItem";
		this.stopSrvMItem.Size = new System.Drawing.Size(138, 22);
		this.stopSrvMItem.Text = "Остановить";
		this.stopSrvMItem.Click += new System.EventHandler(stopSrvMItem_Click);
		this.deleteSrvMItem.Name = "deleteSrvMItem";
		this.deleteSrvMItem.Size = new System.Drawing.Size(138, 22);
		this.deleteSrvMItem.Text = "Удалить";
		this.deleteSrvMItem.Click += new System.EventHandler(deleteSrvMItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(950, 546);
		base.Controls.Add(this.ManagerMode);
		base.Controls.Add(this.ManagerMenuStrip);
		base.Controls.Add(this.ManagerMenu);
		base.Controls.Add(this.ManagerStatusStrip);
		this.MinimumSize = new System.Drawing.Size(600, 400);
		base.Name = "TaskManager";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(TaskManager_FormClosing);
		base.Shown += new System.EventHandler(TaskManager_Shown);
		this.ManagerStatusStrip.ResumeLayout(false);
		this.ManagerStatusStrip.PerformLayout();
		this.ManagerMenu.ResumeLayout(false);
		this.ManagerMenu.PerformLayout();
		this.ManagerMenuStrip.ResumeLayout(false);
		this.ManagerMenuStrip.PerformLayout();
		this.ManagerMode.ResumeLayout(false);
		this.TaskMgrPage.ResumeLayout(false);
		this.ServicesPage.ResumeLayout(false);
		this.ProcessContextMenu.ResumeLayout(false);
		this.ServiceContextMenu.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
