using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SU.Classes;
using SU.Classes.Autorun;
using SU.Forms.Startup;
using SU.Properties;
using TaskScheduler;

namespace SU.Forms;

public class AutorunView : Form
{
	private TaskCharp task;

	private IContainer components;

	private MenuStrip URMenu;

	private ToolStripMenuItem MenuStrip;

	private ToolStripMenuItem BackToMainMenuStrip;

	private ToolStripMenuItem ExitStrip;

	private TabControl ARTab;

	private TabPage RegistryPage;

	private TabPage ARFolderPage;

	private TabControl RegTab;

	private TabPage RegRunPage;

	private TabPage RegRunOncePage;

	private TabPage RegWinlogonPage;

	private TabPage ARTPage;

	private ListView RunRegView;

	private ColumnHeader nameRegRunCol;

	private ColumnHeader valueRegRunCol;

	private ListView RunOnceRegView;

	private ColumnHeader nameRunOnceRegCol;

	private ColumnHeader valueRunOnceRegCol;

	private ListView WinlogonRegView;

	private ColumnHeader nameWinlogonRegCol;

	private ColumnHeader valueWinlogonRegCol;

	private ListView ShellFolderView;

	private ColumnHeader FileName;

	private ColumnHeader PathFile;

	private ContextMenuStrip AutorunMenuStrip;

	private ToolStripMenuItem delToolStripContext;

	private ToolStripMenuItem locateToolStripContext;

	private ToolStripMenuItem refreshToolStripContext;

	private ListView TaskShedView;

	private ColumnHeader taskViewName;

	private ColumnHeader destTaskView;

	private ToolStrip TaskShedToolStrip;

	private ToolStripButton backTaskStripBtn;

	private ToolStripButton deleteTaskStripBtn;

	private ToolStripTextBox toolStripTextBox1;

	private ImageList TaskShedViewImg;

	private ContextMenuStrip TaskShedContextMenu;

	private ToolStripMenuItem aboutTaskStripBtn;

	private ToolStripMenuItem deleteTaskContextBtn;

	public AutorunView()
	{
		InitializeComponent();
		if (!Program.isSaveMode)
		{
			task = new TaskCharp();
			TaskShedView.Items.Clear();
			TaskShedViewImg.Images.Clear();
			task.ActOnStart += delegate
			{
				TaskShedView.Items.Clear();
				TaskShedViewImg.Images.Clear();
			};
			task.ActOnFolder += delegate(ITaskFolder e)
			{
				ListViewItem listViewItem = new ListViewItem(" " + e.Name, TaskShedView.Items.Count);
				TaskShedViewImg.Images.Add(Resources.folder);
				listViewItem.SubItems.Add(e.Path);
				listViewItem.SubItems.Add("Папка");
				TaskShedView.Items.Add(listViewItem);
			};
			task.ActOnTask += delegate(IRegisteredTask t)
			{
				ListViewItem listViewItem = new ListViewItem(t.Name ?? "", TaskShedView.Items.Count);
				TaskShedViewImg.Images.Add(Resources.task);
				listViewItem.SubItems.Add(t.Path);
				listViewItem.SubItems.Add("Задача");
				TaskShedView.Items.Add(listViewItem);
				AutoSizeColumnList(TaskShedView);
			};
			task.ActOnProgress += delegate
			{
				toolStripTextBox1.Text = task.current.Path;
			};
			task.EnumAllTasks();
		}
		else
		{
			ARTab.TabPages[2].Dispose();
		}
	}

	private void AutoSizeColumnList(ListView listView)
	{
		listView.BeginUpdate();
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
		foreach (ColumnHeader column in listView.Columns)
		{
			dictionary.Add(column.Index, column.Width);
		}
		listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
		foreach (ColumnHeader column2 in listView.Columns)
		{
			if (dictionary.TryGetValue(column2.Index, out var value))
			{
				column2.Width = Math.Max(value, column2.Width);
			}
			else
			{
				column2.Width = Math.Max(50, column2.Width);
			}
		}
		listView.EndUpdate();
	}

	private ListView GetCurrentListView()
	{
		switch (ARTab.SelectedIndex)
		{
		case 0:
			switch (RegTab.SelectedIndex)
			{
			case 0:
				return RunRegView;
			case 1:
				return RunOnceRegView;
			case 2:
				return WinlogonRegView;
			}
			break;
		case 1:
			return ShellFolderView;
		}
		return null;
	}

	private void GetAutorunList(AutorunType type)
	{
		switch (type)
		{
		case AutorunType.Registry:
		{
			RunRegView.Items.Clear();
			RunOnceRegView.Items.Clear();
			WinlogonRegView.Items.Clear();
			AutorunElem[] shellAutorun = Autorun.GetRegistryAutoRun(RegistryPaths.Run, Utils.RegView);
			foreach (AutorunElem autorunElem2 in shellAutorun)
			{
				ListViewItem listViewItem2 = new ListViewItem(new string[2] { autorunElem2.Name, autorunElem2.Path });
				listViewItem2.Tag = autorunElem2;
				RunRegView.Items.Add(listViewItem2);
			}
			shellAutorun = Autorun.GetRegistryAutoRun(RegistryPaths.RunOnce, Utils.RegView);
			foreach (AutorunElem autorunElem3 in shellAutorun)
			{
				ListViewItem listViewItem3 = new ListViewItem(new string[2] { autorunElem3.Name, autorunElem3.Path });
				listViewItem3.Tag = autorunElem3;
				RunOnceRegView.Items.Add(listViewItem3);
			}
			shellAutorun = Autorun.GetRegistryAutoRun(RegistryPaths.Winlogon, Utils.RegView);
			foreach (AutorunElem autorunElem4 in shellAutorun)
			{
				ListViewItem listViewItem4 = new ListViewItem(new string[2] { autorunElem4.Name, autorunElem4.Path });
				listViewItem4.Tag = autorunElem4;
				WinlogonRegView.Items.Add(listViewItem4);
			}
			break;
		}
		case AutorunType.ShellFolder:
		{
			ShellFolderView.Items.Clear();
			AutorunElem[] shellAutorun = Autorun.GetShellAutorun();
			foreach (AutorunElem autorunElem in shellAutorun)
			{
				ListViewItem listViewItem = new ListViewItem(new string[2] { autorunElem.Name, autorunElem.Path });
				listViewItem.Tag = autorunElem;
				if ((autorunElem.Path == "explorer.exe" && autorunElem.Name == "Shell") || (autorunElem.Path.ToLower() == "c:\\windows\\system32\\userinit.exe" && autorunElem.Name == "Userinit"))
				{
					listViewItem.BackColor = Color.LightGreen;
				}
				ShellFolderView.Items.Add(listViewItem);
			}
			break;
		}
		case AutorunType.Winlogon:
		case AutorunType.TaskScheduler:
			break;
		}
	}

	private void Autorun_Load(object sender, EventArgs e)
	{
		GetAutorunList(AutorunType.Registry);
		GetAutorunList(AutorunType.ShellFolder);
	}

	private void ShowContext(ListView list, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && list.FocusedItem != null && list.FocusedItem.Bounds.Contains(e.Location))
		{
			AutorunMenuStrip.Show(Cursor.Position);
		}
	}

	private void delToolStripContext_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem selectedItem in GetCurrentListView().SelectedItems)
		{
			((AutorunElem)selectedItem.Tag).Remove();
		}
		GetAutorunList(AutorunType.Registry);
		GetAutorunList(AutorunType.ShellFolder);
	}

	private void RunRegView_MouseClick(object sender, MouseEventArgs e)
	{
		ShowContext(RunRegView, e);
	}

	private void RunOnceRegView_MouseClick(object sender, MouseEventArgs e)
	{
		ShowContext(RunOnceRegView, e);
	}

	private void WinlogonRegView_MouseClick(object sender, MouseEventArgs e)
	{
		ShowContext(WinlogonRegView, e);
	}

	private void ShellFolderView_MouseClick(object sender, MouseEventArgs e)
	{
		ShowContext(ShellFolderView, e);
	}

	private void locateToolStripContext_Click(object sender, EventArgs e)
	{
		ListView currentListView = GetCurrentListView();
		Utils.RunFile("explorer.exe", "/n,/select, \"" + Utils.GetExecutablePathFromCommand((currentListView != ShellFolderView) ? ((AutorunElem)currentListView.FocusedItem.Tag).Path : (((AutorunElem)currentListView.FocusedItem.Tag).Path + "\\" + ((AutorunElem)currentListView.FocusedItem.Tag).Name)) + "\"", UAC: false, Hidden: false, WaitForExit: false);
	}

	private void refreshToolStripContext_Click(object sender, EventArgs e)
	{
		GetAutorunList(AutorunType.Registry);
		GetAutorunList(AutorunType.ShellFolder);
	}

	private void AutorunView_FormClosing(object sender, FormClosingEventArgs e)
	{
		Utils.CloseForm(e);
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

	private void TaskShedView_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (TaskShedView.SelectedIndices.Count <= 0 || e.Button != MouseButtons.Left)
		{
			return;
		}
		string text = TaskShedView.FocusedItem.SubItems[2].Text;
		if (!(text == "Папка"))
		{
			if (text == "Задача")
			{
				using (TaskAbout taskAbout = new TaskAbout(task.GetTask(TaskShedView.FocusedItem.SubItems[0].Text, TaskShedView.FocusedItem.SubItems[1].Text)))
				{
					taskAbout.ShowDialog();
				}
			}
		}
		else
		{
			task.EnumAllTasks(TaskShedView.FocusedItem.SubItems[1].Text);
		}
	}

	private void aboutTaskStripBtn_Click(object sender, EventArgs e)
	{
		if (TaskShedView.SelectedIndices.Count > 0 && TaskShedView.FocusedItem.SubItems[2].Text == "Задача")
		{
			using (TaskAbout taskAbout = new TaskAbout(task.GetTask(TaskShedView.FocusedItem.SubItems[0].Text, TaskShedView.FocusedItem.SubItems[1].Text)))
			{
				taskAbout.ShowDialog();
			}
		}
	}

	private void deleteTaskContextBtn_Click(object sender, EventArgs e)
	{
		if (TaskShedView.SelectedIndices.Count > 0 && TaskShedView.FocusedItem.SubItems[2].Text == "Задача")
		{
			task.current.DeleteTask(TaskShedView.FocusedItem.Text, 0);
			task.EnumAllTasks(task.current.Path);
		}
	}

	private void backTaskStripBtn_Click(object sender, EventArgs e)
	{
		task.EnumAllTasks(task.parent.Path);
	}

	private void deleteTaskStripBtn_Click(object sender, EventArgs e)
	{
		if (TaskShedView.SelectedIndices.Count > 0 && TaskShedView.FocusedItem.SubItems[2].Text == "Задача")
		{
			task.current.DeleteTask(TaskShedView.FocusedItem.Text, 0);
			task.EnumAllTasks(task.current.Path);
		}
	}

	private void TaskShedView_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right)
		{
			ListViewItem focusedItem = TaskShedView.FocusedItem;
			if (focusedItem != null && focusedItem.Bounds.Contains(e.Location))
			{
				TaskShedContextMenu.Show(Cursor.Position);
			}
		}
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
		this.URMenu = new System.Windows.Forms.MenuStrip();
		this.MenuStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.BackToMainMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.ExitStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.ARTab = new System.Windows.Forms.TabControl();
		this.RegistryPage = new System.Windows.Forms.TabPage();
		this.RegTab = new System.Windows.Forms.TabControl();
		this.RegRunPage = new System.Windows.Forms.TabPage();
		this.RunRegView = new System.Windows.Forms.ListView();
		this.nameRegRunCol = new System.Windows.Forms.ColumnHeader();
		this.valueRegRunCol = new System.Windows.Forms.ColumnHeader();
		this.RegRunOncePage = new System.Windows.Forms.TabPage();
		this.RunOnceRegView = new System.Windows.Forms.ListView();
		this.nameRunOnceRegCol = new System.Windows.Forms.ColumnHeader();
		this.valueRunOnceRegCol = new System.Windows.Forms.ColumnHeader();
		this.RegWinlogonPage = new System.Windows.Forms.TabPage();
		this.WinlogonRegView = new System.Windows.Forms.ListView();
		this.nameWinlogonRegCol = new System.Windows.Forms.ColumnHeader();
		this.valueWinlogonRegCol = new System.Windows.Forms.ColumnHeader();
		this.ARFolderPage = new System.Windows.Forms.TabPage();
		this.ShellFolderView = new System.Windows.Forms.ListView();
		this.FileName = new System.Windows.Forms.ColumnHeader();
		this.PathFile = new System.Windows.Forms.ColumnHeader();
		this.ARTPage = new System.Windows.Forms.TabPage();
		this.TaskShedView = new System.Windows.Forms.ListView();
		this.taskViewName = new System.Windows.Forms.ColumnHeader();
		this.destTaskView = new System.Windows.Forms.ColumnHeader();
		this.TaskShedViewImg = new System.Windows.Forms.ImageList(this.components);
		this.TaskShedToolStrip = new System.Windows.Forms.ToolStrip();
		this.backTaskStripBtn = new System.Windows.Forms.ToolStripButton();
		this.deleteTaskStripBtn = new System.Windows.Forms.ToolStripButton();
		this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
		this.AutorunMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.delToolStripContext = new System.Windows.Forms.ToolStripMenuItem();
		this.locateToolStripContext = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripContext = new System.Windows.Forms.ToolStripMenuItem();
		this.TaskShedContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.aboutTaskStripBtn = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteTaskContextBtn = new System.Windows.Forms.ToolStripMenuItem();
		this.URMenu.SuspendLayout();
		this.ARTab.SuspendLayout();
		this.RegistryPage.SuspendLayout();
		this.RegTab.SuspendLayout();
		this.RegRunPage.SuspendLayout();
		this.RegRunOncePage.SuspendLayout();
		this.RegWinlogonPage.SuspendLayout();
		this.ARFolderPage.SuspendLayout();
		this.ARTPage.SuspendLayout();
		this.TaskShedToolStrip.SuspendLayout();
		this.AutorunMenuStrip.SuspendLayout();
		this.TaskShedContextMenu.SuspendLayout();
		base.SuspendLayout();
		this.URMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.MenuStrip });
		this.URMenu.Location = new System.Drawing.Point(0, 0);
		this.URMenu.Name = "URMenu";
		this.URMenu.Size = new System.Drawing.Size(800, 24);
		this.URMenu.TabIndex = 2;
		this.URMenu.Text = "menuStrip1";
		this.MenuStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.BackToMainMenuStrip, this.ExitStrip });
		this.MenuStrip.Name = "MenuStrip";
		this.MenuStrip.Size = new System.Drawing.Size(53, 20);
		this.MenuStrip.Text = "Меню";
		this.BackToMainMenuStrip.Name = "BackToMainMenuStrip";
		this.BackToMainMenuStrip.Size = new System.Drawing.Size(215, 22);
		this.BackToMainMenuStrip.Text = "Вернутся в главное меню";
		this.BackToMainMenuStrip.Click += new System.EventHandler(BackToMainMenuStrip_Click);
		this.ExitStrip.Name = "ExitStrip";
		this.ExitStrip.Size = new System.Drawing.Size(215, 22);
		this.ExitStrip.Text = "Выход";
		this.ExitStrip.Click += new System.EventHandler(ExitStrip_Click);
		this.ARTab.Controls.Add(this.RegistryPage);
		this.ARTab.Controls.Add(this.ARFolderPage);
		this.ARTab.Controls.Add(this.ARTPage);
		this.ARTab.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ARTab.Location = new System.Drawing.Point(0, 24);
		this.ARTab.Name = "ARTab";
		this.ARTab.SelectedIndex = 0;
		this.ARTab.Size = new System.Drawing.Size(800, 426);
		this.ARTab.TabIndex = 3;
		this.RegistryPage.Controls.Add(this.RegTab);
		this.RegistryPage.Location = new System.Drawing.Point(4, 22);
		this.RegistryPage.Name = "RegistryPage";
		this.RegistryPage.Padding = new System.Windows.Forms.Padding(3);
		this.RegistryPage.Size = new System.Drawing.Size(792, 400);
		this.RegistryPage.TabIndex = 0;
		this.RegistryPage.Text = "Реестр";
		this.RegistryPage.UseVisualStyleBackColor = true;
		this.RegTab.Controls.Add(this.RegRunPage);
		this.RegTab.Controls.Add(this.RegRunOncePage);
		this.RegTab.Controls.Add(this.RegWinlogonPage);
		this.RegTab.Dock = System.Windows.Forms.DockStyle.Fill;
		this.RegTab.Location = new System.Drawing.Point(3, 3);
		this.RegTab.Name = "RegTab";
		this.RegTab.SelectedIndex = 0;
		this.RegTab.Size = new System.Drawing.Size(786, 394);
		this.RegTab.TabIndex = 0;
		this.RegRunPage.Controls.Add(this.RunRegView);
		this.RegRunPage.Location = new System.Drawing.Point(4, 22);
		this.RegRunPage.Name = "RegRunPage";
		this.RegRunPage.Size = new System.Drawing.Size(778, 368);
		this.RegRunPage.TabIndex = 0;
		this.RegRunPage.Text = "Run";
		this.RegRunPage.UseVisualStyleBackColor = true;
		this.RunRegView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.nameRegRunCol, this.valueRegRunCol });
		this.RunRegView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.RunRegView.FullRowSelect = true;
		this.RunRegView.GridLines = true;
		this.RunRegView.HideSelection = false;
		this.RunRegView.Location = new System.Drawing.Point(0, 0);
		this.RunRegView.Name = "RunRegView";
		this.RunRegView.Size = new System.Drawing.Size(778, 368);
		this.RunRegView.TabIndex = 0;
		this.RunRegView.UseCompatibleStateImageBehavior = false;
		this.RunRegView.View = System.Windows.Forms.View.Details;
		this.RunRegView.MouseClick += new System.Windows.Forms.MouseEventHandler(RunRegView_MouseClick);
		this.nameRegRunCol.Text = "Значение";
		this.nameRegRunCol.Width = 193;
		this.valueRegRunCol.Text = "Путь";
		this.valueRegRunCol.Width = 566;
		this.RegRunOncePage.Controls.Add(this.RunOnceRegView);
		this.RegRunOncePage.Location = new System.Drawing.Point(4, 22);
		this.RegRunOncePage.Name = "RegRunOncePage";
		this.RegRunOncePage.Size = new System.Drawing.Size(778, 368);
		this.RegRunOncePage.TabIndex = 2;
		this.RegRunOncePage.Text = "RunOnce";
		this.RegRunOncePage.UseVisualStyleBackColor = true;
		this.RunOnceRegView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.nameRunOnceRegCol, this.valueRunOnceRegCol });
		this.RunOnceRegView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.RunOnceRegView.FullRowSelect = true;
		this.RunOnceRegView.GridLines = true;
		this.RunOnceRegView.HideSelection = false;
		this.RunOnceRegView.Location = new System.Drawing.Point(0, 0);
		this.RunOnceRegView.Name = "RunOnceRegView";
		this.RunOnceRegView.Size = new System.Drawing.Size(778, 368);
		this.RunOnceRegView.TabIndex = 3;
		this.RunOnceRegView.UseCompatibleStateImageBehavior = false;
		this.RunOnceRegView.View = System.Windows.Forms.View.Details;
		this.RunOnceRegView.MouseClick += new System.Windows.Forms.MouseEventHandler(RunOnceRegView_MouseClick);
		this.nameRunOnceRegCol.Text = "Значение";
		this.nameRunOnceRegCol.Width = 193;
		this.valueRunOnceRegCol.Text = "Путь";
		this.valueRunOnceRegCol.Width = 566;
		this.RegWinlogonPage.Controls.Add(this.WinlogonRegView);
		this.RegWinlogonPage.Location = new System.Drawing.Point(4, 22);
		this.RegWinlogonPage.Margin = new System.Windows.Forms.Padding(0);
		this.RegWinlogonPage.Name = "RegWinlogonPage";
		this.RegWinlogonPage.Size = new System.Drawing.Size(778, 368);
		this.RegWinlogonPage.TabIndex = 1;
		this.RegWinlogonPage.Text = "Winlogon";
		this.RegWinlogonPage.UseVisualStyleBackColor = true;
		this.WinlogonRegView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.nameWinlogonRegCol, this.valueWinlogonRegCol });
		this.WinlogonRegView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.WinlogonRegView.FullRowSelect = true;
		this.WinlogonRegView.GridLines = true;
		this.WinlogonRegView.HideSelection = false;
		this.WinlogonRegView.Location = new System.Drawing.Point(0, 0);
		this.WinlogonRegView.Name = "WinlogonRegView";
		this.WinlogonRegView.Size = new System.Drawing.Size(778, 368);
		this.WinlogonRegView.TabIndex = 1;
		this.WinlogonRegView.UseCompatibleStateImageBehavior = false;
		this.WinlogonRegView.View = System.Windows.Forms.View.Details;
		this.WinlogonRegView.MouseClick += new System.Windows.Forms.MouseEventHandler(WinlogonRegView_MouseClick);
		this.nameWinlogonRegCol.Text = "Значение";
		this.nameWinlogonRegCol.Width = 193;
		this.valueWinlogonRegCol.Text = "Путь";
		this.valueWinlogonRegCol.Width = 566;
		this.ARFolderPage.Controls.Add(this.ShellFolderView);
		this.ARFolderPage.Location = new System.Drawing.Point(4, 22);
		this.ARFolderPage.Name = "ARFolderPage";
		this.ARFolderPage.Padding = new System.Windows.Forms.Padding(3);
		this.ARFolderPage.Size = new System.Drawing.Size(792, 400);
		this.ARFolderPage.TabIndex = 1;
		this.ARFolderPage.Text = "Папка автозагрузки";
		this.ARFolderPage.UseVisualStyleBackColor = true;
		this.ShellFolderView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.FileName, this.PathFile });
		this.ShellFolderView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ShellFolderView.FullRowSelect = true;
		this.ShellFolderView.GridLines = true;
		this.ShellFolderView.HideSelection = false;
		this.ShellFolderView.Location = new System.Drawing.Point(3, 3);
		this.ShellFolderView.Name = "ShellFolderView";
		this.ShellFolderView.Size = new System.Drawing.Size(786, 394);
		this.ShellFolderView.TabIndex = 1;
		this.ShellFolderView.UseCompatibleStateImageBehavior = false;
		this.ShellFolderView.View = System.Windows.Forms.View.Details;
		this.ShellFolderView.MouseClick += new System.Windows.Forms.MouseEventHandler(ShellFolderView_MouseClick);
		this.FileName.Text = "Имя файла";
		this.FileName.Width = 193;
		this.PathFile.Text = "Путь";
		this.PathFile.Width = 566;
		this.ARTPage.Controls.Add(this.TaskShedView);
		this.ARTPage.Controls.Add(this.TaskShedToolStrip);
		this.ARTPage.Location = new System.Drawing.Point(4, 22);
		this.ARTPage.Name = "ARTPage";
		this.ARTPage.Size = new System.Drawing.Size(792, 400);
		this.ARTPage.TabIndex = 2;
		this.ARTPage.Text = "Планировщик заданий";
		this.ARTPage.UseVisualStyleBackColor = true;
		this.TaskShedView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.taskViewName, this.destTaskView });
		this.TaskShedView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.TaskShedView.FullRowSelect = true;
		this.TaskShedView.HideSelection = false;
		this.TaskShedView.Location = new System.Drawing.Point(0, 23);
		this.TaskShedView.Name = "TaskShedView";
		this.TaskShedView.Size = new System.Drawing.Size(792, 377);
		this.TaskShedView.SmallImageList = this.TaskShedViewImg;
		this.TaskShedView.TabIndex = 1;
		this.TaskShedView.UseCompatibleStateImageBehavior = false;
		this.TaskShedView.View = System.Windows.Forms.View.Details;
		this.TaskShedView.MouseClick += new System.Windows.Forms.MouseEventHandler(TaskShedView_MouseClick);
		this.TaskShedView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(TaskShedView_MouseDoubleClick);
		this.taskViewName.Text = "Имя";
		this.taskViewName.Width = 205;
		this.destTaskView.Text = "Расположение";
		this.destTaskView.Width = 557;
		this.TaskShedViewImg.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
		this.TaskShedViewImg.ImageSize = new System.Drawing.Size(16, 16);
		this.TaskShedViewImg.TransparentColor = System.Drawing.Color.Transparent;
		this.TaskShedToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.backTaskStripBtn, this.deleteTaskStripBtn, this.toolStripTextBox1 });
		this.TaskShedToolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
		this.TaskShedToolStrip.Location = new System.Drawing.Point(0, 0);
		this.TaskShedToolStrip.Name = "TaskShedToolStrip";
		this.TaskShedToolStrip.Size = new System.Drawing.Size(792, 23);
		this.TaskShedToolStrip.TabIndex = 0;
		this.TaskShedToolStrip.Text = "toolStrip1";
		this.backTaskStripBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.backTaskStripBtn.Image = SU.Properties.Resources.left;
		this.backTaskStripBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.backTaskStripBtn.Margin = new System.Windows.Forms.Padding(5, 1, 0, 2);
		this.backTaskStripBtn.Name = "backTaskStripBtn";
		this.backTaskStripBtn.Size = new System.Drawing.Size(23, 20);
		this.backTaskStripBtn.Text = "Назад";
		this.backTaskStripBtn.Click += new System.EventHandler(backTaskStripBtn_Click);
		this.deleteTaskStripBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
		this.deleteTaskStripBtn.Image = SU.Properties.Resources.delete;
		this.deleteTaskStripBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
		this.deleteTaskStripBtn.Name = "deleteTaskStripBtn";
		this.deleteTaskStripBtn.Size = new System.Drawing.Size(23, 20);
		this.deleteTaskStripBtn.Text = "Удалить";
		this.deleteTaskStripBtn.Click += new System.EventHandler(deleteTaskStripBtn_Click);
		this.toolStripTextBox1.Font = new System.Drawing.Font("Segoe UI", 9f);
		this.toolStripTextBox1.Margin = new System.Windows.Forms.Padding(10, 0, 1, 0);
		this.toolStripTextBox1.Name = "toolStripTextBox1";
		this.toolStripTextBox1.Size = new System.Drawing.Size(500, 23);
		this.AutorunMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.delToolStripContext, this.locateToolStripContext, this.refreshToolStripContext });
		this.AutorunMenuStrip.Name = "AutorunMenuStrip";
		this.AutorunMenuStrip.Size = new System.Drawing.Size(207, 70);
		this.delToolStripContext.Name = "delToolStripContext";
		this.delToolStripContext.Size = new System.Drawing.Size(206, 22);
		this.delToolStripContext.Text = "Удалить";
		this.delToolStripContext.Click += new System.EventHandler(delToolStripContext_Click);
		this.locateToolStripContext.Name = "locateToolStripContext";
		this.locateToolStripContext.Size = new System.Drawing.Size(206, 22);
		this.locateToolStripContext.Text = "Открыть расположение";
		this.locateToolStripContext.Click += new System.EventHandler(locateToolStripContext_Click);
		this.refreshToolStripContext.Name = "refreshToolStripContext";
		this.refreshToolStripContext.Size = new System.Drawing.Size(206, 22);
		this.refreshToolStripContext.Text = "Обновить";
		this.refreshToolStripContext.Click += new System.EventHandler(refreshToolStripContext_Click);
		this.TaskShedContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.aboutTaskStripBtn, this.deleteTaskContextBtn });
		this.TaskShedContextMenu.Name = "TaskShedContextMenu";
		this.TaskShedContextMenu.Size = new System.Drawing.Size(126, 48);
		this.aboutTaskStripBtn.Name = "aboutTaskStripBtn";
		this.aboutTaskStripBtn.Size = new System.Drawing.Size(180, 22);
		this.aboutTaskStripBtn.Text = "Свойства";
		this.aboutTaskStripBtn.Click += new System.EventHandler(aboutTaskStripBtn_Click);
		this.deleteTaskContextBtn.Name = "deleteTaskContextBtn";
		this.deleteTaskContextBtn.Size = new System.Drawing.Size(180, 22);
		this.deleteTaskContextBtn.Text = "Удалить";
		this.deleteTaskContextBtn.Click += new System.EventHandler(deleteTaskContextBtn_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.ARTab);
		base.Controls.Add(this.URMenu);
		this.MinimumSize = new System.Drawing.Size(600, 400);
		base.Name = "AutorunView";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(AutorunView_FormClosing);
		base.Load += new System.EventHandler(Autorun_Load);
		this.URMenu.ResumeLayout(false);
		this.URMenu.PerformLayout();
		this.ARTab.ResumeLayout(false);
		this.RegistryPage.ResumeLayout(false);
		this.RegTab.ResumeLayout(false);
		this.RegRunPage.ResumeLayout(false);
		this.RegRunOncePage.ResumeLayout(false);
		this.RegWinlogonPage.ResumeLayout(false);
		this.ARFolderPage.ResumeLayout(false);
		this.ARTPage.ResumeLayout(false);
		this.ARTPage.PerformLayout();
		this.TaskShedToolStrip.ResumeLayout(false);
		this.TaskShedToolStrip.PerformLayout();
		this.AutorunMenuStrip.ResumeLayout(false);
		this.TaskShedContextMenu.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
