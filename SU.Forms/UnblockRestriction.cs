using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using SU.Classes;
using SU.Classes.UR;
using SU.Forms.UR;

namespace SU.Forms;

public class UnblockRestriction : Form
{
	private List<FileType> _fileTypes;

	private bool _AutoRestrictDel;

	private IContainer components;

	private MenuStrip URMenu;

	private ToolStripMenuItem MenuStrip;

	private ToolStripMenuItem AddonMenuStrip;

	private StatusStrip URStatus;

	private TabControl URControl;

	private TabPage URPage;

	private TabControl URTypeControl;

	private TabPage URScanPage;

	private TabPage URManualPage;

	private TabPage AssocPage;

	private ToolStripStatusLabel StatusText;

	private GroupBox ResultBox;

	private ListView ResultView;

	private ColumnHeader RestrictionName;

	private ColumnHeader DescriptionRestricion;

	private ColumnHeader PathRestriction;

	private Panel AURControlPanel;

	private Button StartScanButton;

	private CheckBox AURCheckbox;

	private ToolStripMenuItem BackToMainMenuStrip;

	private ToolStripMenuItem ExitStrip;

	private ToolStripMenuItem CreateAssocStrip;

	private ContextMenuStrip ScanResultContext;

	private ToolStripMenuItem URToolMenuStrip;

	private ToolStripMenuItem OpenRegistryToolStripMenu;

	private GroupBox BasicRestrictionsGroupBox;

	private ListView BasicRestrictionList;

	private ColumnHeader BRMColumn;

	private ColumnHeader BRDColumn;

	private Panel MURPanel;

	private Button ManualUnlockBtn;

	private CheckBox SelectAllBox;

	private ToolStripMenuItem CreateFileTypeStrip;

	private ContextMenuStrip AssocContextList;

	private ToolStripMenuItem deleteAssocMenuItem;

	private ToolStripMenuItem changeAssocMenuItem;

	private ToolStripMenuItem refreshAssocMenuItem;

	private TabControl AssocControl;

	private TabPage AssocTab;

	private GroupBox AssocBox;

	private ListView AssocView;

	private ColumnHeader AssocNameColumn;

	private ColumnHeader AssocFileTypeColumn;

	private ColumnHeader ShellOpenCommandColumn;

	private TabPage FileTypesTab;

	private GroupBox FileTypesBox;

	private ListView FileTypeView;

	private ColumnHeader fTypeNameColumn;

	private ColumnHeader fTypeDescriptionColumn;

	private ColumnHeader fTypeOpenDefaultColumn;

	private ColumnHeader fTypeDefaultIconColumn;

	private ContextMenuStrip FileTypeContext;

	private ToolStripMenuItem delFTItem;

	private ToolStripMenuItem chgFTItem;

	private ToolStripMenuItem refFTItem;

	private void GetListAssocs()
	{
		AssocView.Items.Clear();
		foreach (Association assoc in Associations.GetAssocs())
		{
			ListViewItem value = new ListViewItem(new string[3]
			{
				assoc.Name,
				assoc.Type.Name,
				assoc.Type?.ShellOpenCommand?.DefaultCommand
			})
			{
				Tag = assoc
			};
			AssocView.Items.Add(value);
		}
	}

	private void GetListFileTypes()
	{
		FileTypeView.Items.Clear();
		foreach (FileType fileType in _fileTypes)
		{
			ListViewItem value = new ListViewItem(new string[4]
			{
				fileType.Name,
				fileType?.DefaultIcon,
				fileType?.Description,
				fileType?.ShellOpenCommand?.DefaultCommand
			})
			{
				Tag = fileType
			};
			FileTypeView.Items.Add(value);
		}
	}

	public UnblockRestriction()
	{
		InitializeComponent();
		_fileTypes = Associations.GetFileTypes();
	}

	private void StartScanButton_Click(object sender, EventArgs e)
	{
		_AutoRestrictDel = AURCheckbox.Checked;
		ResultView.MultiSelect = !_AutoRestrictDel;
		ResultView.Items.Clear();
		ListViewItem restriction2;
		foreach (Restriction Basic in RestrictionList.BasicRestrictions)
		{
			Parallel.ForEach(Basic.RestrictionHive, delegate(RegistryHive hive)
			{
				using RegistryKey registryKey2 = RegistryKey.OpenBaseKey(hive, RegistryView.Default);
				using RegistryKey registryKey3 = registryKey2.OpenSubKey(Basic.RestrictionKey);
				if (registryKey3 != null)
				{
					object value = registryKey3.GetValue(Basic.RestrictionName);
					if (value != null && ((int)value != (int)Basic.DisableValue || (int)value == 1))
					{
						restriction2 = new ListViewItem(new string[3]
						{
							Basic.RestrictionName,
							Basic.RestrictionDescription,
							registryKey2.Name + "\\" + Basic.RestrictionKey
						})
						{
							Tag = Basic
						};
						if (_AutoRestrictDel)
						{
							Basic.Unlock(Delete: true);
							restriction2.BackColor = Color.LightGreen;
						}
						ResultView.Items.Add(restriction2);
					}
				}
			});
		}
		Restriction[] disallowApps = RestrictionList.GetDisallowApps();
		foreach (Restriction restriction in disallowApps)
		{
			restriction2 = new ListViewItem(new string[3]
			{
				restriction.RestrictionName,
				restriction.RestrictionDescription,
				"HKEY_CURRENT_USER\\" + restriction.RestrictionKey
			})
			{
				Tag = restriction
			};
			if (_AutoRestrictDel)
			{
				restriction.Unlock(Delete: true);
				restriction2.BackColor = Color.LightGreen;
			}
			ResultView.Items.Add(restriction2);
		}
		disallowApps = RestrictionList.GetDebuggerApps();
		foreach (Restriction restriction3 in disallowApps)
		{
			restriction2 = new ListViewItem(new string[3]
			{
				restriction3.RestrictionName,
				restriction3.RestrictionDescription,
				"HKEY_LOCAL_MACHINE\\" + restriction3.RestrictionKey
			})
			{
				Tag = restriction3
			};
			if (_AutoRestrictDel)
			{
				restriction3.Unlock(Delete: true);
				restriction2.BackColor = Color.LightGreen;
			}
			ResultView.Items.Add(restriction2);
		}
		using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout"))
		{
			if (registryKey.GetValue("Scancode Map") != null)
			{
				Restriction restriction4 = new Restriction();
				restriction4.RestrictionName = "Scancode Map";
				restriction4.RestrictionDescription = "Scancode Map это ключ в реестре который позволяет переназначать клавишы или отключить их.";
				restriction4.RestrictionHive = new RegistryHive[1] { RegistryHive.LocalMachine };
				restriction4.RestrictionKey = "SYSTEM\\CurrentControlSet\\Control\\Keyboard Layout";
				Restriction restriction5 = restriction4;
				restriction2 = new ListViewItem(new string[3]
				{
					restriction5.RestrictionName,
					restriction5.RestrictionDescription,
					"HKEY_LOCAL_MACHINE\\" + restriction5.RestrictionKey
				})
				{
					Tag = restriction5
				};
				if (_AutoRestrictDel)
				{
					restriction5.Unlock(Delete: true);
					restriction2.BackColor = Color.LightGreen;
				}
				ResultView.Items.Add(restriction2);
			}
		}
		StatusText.Text = (_AutoRestrictDel ? $"Состояние: Сканирование и разблокировка ограничений успешно завершена! [{ResultView.Items.Count}]" : "Состояние: Сканирование успешно завершено!");
	}

	private void ResultView_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && ResultView.FocusedItem != null && ResultView.FocusedItem.Bounds.Contains(e.Location) && !_AutoRestrictDel)
		{
			ScanResultContext.Show(Cursor.Position);
		}
	}

	private void UnblockRestriction_Load(object sender, EventArgs e)
	{
		foreach (Restriction basicRestriction in RestrictionList.BasicRestrictions)
		{
			ListViewItem value = new ListViewItem(new string[2] { basicRestriction.RestrictionName, basicRestriction.RestrictionDescription })
			{
				Tag = basicRestriction
			};
			BasicRestrictionList.Items.Add(value);
		}
		GetListAssocs();
		GetListFileTypes();
	}

	private void BasicRestrictionList_ItemChecked(object sender, ItemCheckedEventArgs e)
	{
		ManualUnlockBtn.Enabled = BasicRestrictionList.CheckedItems.Count > 0;
	}

	private void SelectAllBox_CheckedChanged(object sender, EventArgs e)
	{
		foreach (ListViewItem item in BasicRestrictionList.Items)
		{
			item.Checked = SelectAllBox.Checked;
		}
	}

	private void URToolMenuStrip_Click(object sender, EventArgs e)
	{
		int count = ResultView.SelectedItems.Count;
		foreach (ListViewItem selectedItem in ResultView.SelectedItems)
		{
			((Restriction)selectedItem.Tag).Unlock(Delete: true);
			selectedItem.Remove();
		}
		StatusText.Text = $"Состояние: Ограничения успешно разблокированны [{count}]";
	}

	private void ManualUnlockBtn_Click(object sender, EventArgs e)
	{
		foreach (ListViewItem checkedItem in BasicRestrictionList.CheckedItems)
		{
			((Restriction)checkedItem.Tag).Unlock(Delete: true);
		}
		StatusText.Text = "Состояние: Успех!";
	}

	private void UnblockRestriction_FormClosed(object sender, FormClosedEventArgs e)
	{
		GC.Collect();
	}

	private void AssocView_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left || AssocView.SelectedItems.Count <= 0)
		{
			return;
		}
		using AssociationEdit associationEdit = new AssociationEdit((Association)AssocView.FocusedItem.Tag, _fileTypes);
		if (associationEdit.ShowDialog() == DialogResult.OK)
		{
			AssocView.Items[AssocView.FocusedItem.Index] = new ListViewItem(new string[3]
			{
				associationEdit._assoc.Name,
				associationEdit._assoc.Type.Name,
				associationEdit._assoc.Type?.ShellOpenCommand?.DefaultCommand
			})
			{
				Tag = associationEdit._assoc
			};
			AssocView.Update();
			StatusText.Text = "Состояние: Успешно изменена ассоциация " + associationEdit._assoc.Name;
		}
	}

	private void CreateAssocStrip_Click(object sender, EventArgs e)
	{
		using AssociationBackup associationBackup = new AssociationBackup(_fileTypes);
		if (associationBackup.ShowDialog() == DialogResult.OK)
		{
			StatusText.Text = "Состояние: Успешно создана ассоциация " + associationBackup._assoc.Name;
			GetListAssocs();
		}
	}

	private void AssocView_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && AssocView.FocusedItem != null && AssocView.FocusedItem.Bounds.Contains(e.Location))
		{
			AssocContextList.Show(Cursor.Position);
		}
	}

	private void deleteAssocMenuItem_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Внимание! Данное действие является необратимым!\nВы точно хотите удалить данные ассоциации?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		foreach (ListViewItem selectedItem in AssocView.SelectedItems)
		{
			Associations.RemoveAssociation((Association)selectedItem.Tag);
		}
		StatusText.Text = $"Состояние: Удаление ассоциаций прошло успешно! [{AssocView.SelectedItems.Count}]";
		GetListAssocs();
	}

	private void changeAssocMenuItem_Click(object sender, EventArgs e)
	{
		using AssociationEdit associationEdit = new AssociationEdit((Association)AssocView.FocusedItem.Tag, _fileTypes);
		if (associationEdit.ShowDialog() == DialogResult.OK)
		{
			AssocView.Items[AssocView.FocusedItem.Index] = new ListViewItem(new string[3]
			{
				associationEdit._assoc.Name,
				associationEdit._assoc.Type.Name,
				associationEdit._assoc.Type?.ShellOpenCommand?.DefaultCommand
			})
			{
				Tag = associationEdit._assoc
			};
			AssocView.Update();
			StatusText.Text = "Состояние: Успешно изменена ассоциация " + associationEdit._assoc.Name;
		}
	}

	private void refreshAssocMenuItem_Click(object sender, EventArgs e)
	{
		GetListAssocs();
	}

	private void CreateFileTypeStrip_Click(object sender, EventArgs e)
	{
		using FileTypeBackup fileTypeBackup = new FileTypeBackup(_fileTypes);
		if (fileTypeBackup.ShowDialog() == DialogResult.OK)
		{
			StatusText.Text = "Состояние: Успешно создан тип файла " + fileTypeBackup._createdFileType.Name;
			_fileTypes = Associations.GetFileTypes();
			GetListFileTypes();
		}
	}

	private void FileTypeView_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (e.Button != MouseButtons.Left || FileTypeView.SelectedItems.Count <= 0)
		{
			return;
		}
		using FileTypeEdit fileTypeEdit = new FileTypeEdit((FileType)FileTypeView.FocusedItem.Tag);
		if (fileTypeEdit.ShowDialog() == DialogResult.OK)
		{
			FileTypeView.Items[FileTypeView.FocusedItem.Index] = new ListViewItem(new string[4]
			{
				fileTypeEdit._fileType.Name,
				fileTypeEdit._fileType.DefaultIcon,
				fileTypeEdit._fileType.Description,
				fileTypeEdit._fileType.ShellOpenCommand.DefaultCommand
			})
			{
				Tag = fileTypeEdit._fileType
			};
			FileTypeView.Update();
			StatusText.Text = "Состояние: Успешно изменён тип файла " + fileTypeEdit._fileType.Name;
		}
	}

	private void FileTypeView_MouseClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Right && FileTypeView.FocusedItem != null && FileTypeView.FocusedItem.Bounds.Contains(e.Location))
		{
			FileTypeContext.Show(Cursor.Position);
		}
	}

	private void delFTItem_Click(object sender, EventArgs e)
	{
		if (MessageBox.Show("Внимание! Данное действие является необратимым!\nВы точно хотите удалить данные типы файлов?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		foreach (ListViewItem selectedItem in FileTypeView.SelectedItems)
		{
			Associations.RemoveFileType((FileType)selectedItem.Tag);
		}
		StatusText.Text = $"Состояние: Удаление типов файлов прошло успешно! [{FileTypeView.SelectedItems.Count}]";
		_fileTypes = Associations.GetFileTypes();
		GetListFileTypes();
	}

	private void chgFTItem_Click(object sender, EventArgs e)
	{
		using FileTypeEdit fileTypeEdit = new FileTypeEdit((FileType)FileTypeView.FocusedItem.Tag);
		if (fileTypeEdit.ShowDialog() == DialogResult.OK)
		{
			FileTypeView.Items[FileTypeView.FocusedItem.Index] = new ListViewItem(new string[4]
			{
				fileTypeEdit._fileType.Name,
				fileTypeEdit._fileType.DefaultIcon,
				fileTypeEdit._fileType.Description,
				fileTypeEdit._fileType.ShellOpenCommand.DefaultCommand
			})
			{
				Tag = fileTypeEdit._fileType
			};
			FileTypeView.Update();
			StatusText.Text = "Состояние: Успешно изменён тип файла " + fileTypeEdit._fileType.Name;
		}
	}

	private void refFTItem_Click(object sender, EventArgs e)
	{
		_fileTypes = Associations.GetFileTypes();
		GetListFileTypes();
	}

	private void OpenRegistryToolStripMenu_Click(object sender, EventArgs e)
	{
		using RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Applets\\Regedit");
		Process[] processesByName = Process.GetProcessesByName("regedit");
		for (int i = 0; i < processesByName.Length; i++)
		{
			processesByName[i].Kill();
		}
		registryKey.SetValue("LastKey", ResultView.FocusedItem.SubItems[2].Text ?? "");
		Process.Start("regedit.exe");
	}

	private void ExitStrip_Click(object sender, EventArgs e)
	{
		Environment.Exit(0);
	}

	private void UnblockRestriction_FormClosing(object sender, FormClosingEventArgs e)
	{
		Utils.CloseForm(e);
	}

	private void BackToMainMenuStrip_Click(object sender, EventArgs e)
	{
		Dispose();
		Close();
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
		this.AddonMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.CreateAssocStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.CreateFileTypeStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.URStatus = new System.Windows.Forms.StatusStrip();
		this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
		this.URControl = new System.Windows.Forms.TabControl();
		this.URPage = new System.Windows.Forms.TabPage();
		this.URTypeControl = new System.Windows.Forms.TabControl();
		this.URScanPage = new System.Windows.Forms.TabPage();
		this.ResultBox = new System.Windows.Forms.GroupBox();
		this.ResultView = new System.Windows.Forms.ListView();
		this.RestrictionName = new System.Windows.Forms.ColumnHeader();
		this.DescriptionRestricion = new System.Windows.Forms.ColumnHeader();
		this.PathRestriction = new System.Windows.Forms.ColumnHeader();
		this.AURControlPanel = new System.Windows.Forms.Panel();
		this.StartScanButton = new System.Windows.Forms.Button();
		this.AURCheckbox = new System.Windows.Forms.CheckBox();
		this.URManualPage = new System.Windows.Forms.TabPage();
		this.BasicRestrictionsGroupBox = new System.Windows.Forms.GroupBox();
		this.BasicRestrictionList = new System.Windows.Forms.ListView();
		this.BRMColumn = new System.Windows.Forms.ColumnHeader();
		this.BRDColumn = new System.Windows.Forms.ColumnHeader();
		this.MURPanel = new System.Windows.Forms.Panel();
		this.SelectAllBox = new System.Windows.Forms.CheckBox();
		this.ManualUnlockBtn = new System.Windows.Forms.Button();
		this.AssocPage = new System.Windows.Forms.TabPage();
		this.AssocControl = new System.Windows.Forms.TabControl();
		this.AssocTab = new System.Windows.Forms.TabPage();
		this.AssocBox = new System.Windows.Forms.GroupBox();
		this.AssocView = new System.Windows.Forms.ListView();
		this.AssocNameColumn = new System.Windows.Forms.ColumnHeader();
		this.AssocFileTypeColumn = new System.Windows.Forms.ColumnHeader();
		this.ShellOpenCommandColumn = new System.Windows.Forms.ColumnHeader();
		this.FileTypesTab = new System.Windows.Forms.TabPage();
		this.FileTypesBox = new System.Windows.Forms.GroupBox();
		this.FileTypeView = new System.Windows.Forms.ListView();
		this.fTypeNameColumn = new System.Windows.Forms.ColumnHeader();
		this.fTypeDefaultIconColumn = new System.Windows.Forms.ColumnHeader();
		this.fTypeDescriptionColumn = new System.Windows.Forms.ColumnHeader();
		this.fTypeOpenDefaultColumn = new System.Windows.Forms.ColumnHeader();
		this.ScanResultContext = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.URToolMenuStrip = new System.Windows.Forms.ToolStripMenuItem();
		this.OpenRegistryToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
		this.AssocContextList = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteAssocMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.changeAssocMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshAssocMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.FileTypeContext = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.delFTItem = new System.Windows.Forms.ToolStripMenuItem();
		this.chgFTItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refFTItem = new System.Windows.Forms.ToolStripMenuItem();
		this.URMenu.SuspendLayout();
		this.URStatus.SuspendLayout();
		this.URControl.SuspendLayout();
		this.URPage.SuspendLayout();
		this.URTypeControl.SuspendLayout();
		this.URScanPage.SuspendLayout();
		this.ResultBox.SuspendLayout();
		this.AURControlPanel.SuspendLayout();
		this.URManualPage.SuspendLayout();
		this.BasicRestrictionsGroupBox.SuspendLayout();
		this.MURPanel.SuspendLayout();
		this.AssocPage.SuspendLayout();
		this.AssocControl.SuspendLayout();
		this.AssocTab.SuspendLayout();
		this.AssocBox.SuspendLayout();
		this.FileTypesTab.SuspendLayout();
		this.FileTypesBox.SuspendLayout();
		this.ScanResultContext.SuspendLayout();
		this.AssocContextList.SuspendLayout();
		this.FileTypeContext.SuspendLayout();
		base.SuspendLayout();
		this.URMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.MenuStrip, this.AddonMenuStrip });
		this.URMenu.Location = new System.Drawing.Point(0, 0);
		this.URMenu.Name = "URMenu";
		this.URMenu.Size = new System.Drawing.Size(955, 24);
		this.URMenu.TabIndex = 1;
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
		this.AddonMenuStrip.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.CreateAssocStrip, this.CreateFileTypeStrip });
		this.AddonMenuStrip.Name = "AddonMenuStrip";
		this.AddonMenuStrip.Size = new System.Drawing.Size(107, 20);
		this.AddonMenuStrip.Text = "Дополнительно";
		this.CreateAssocStrip.Name = "CreateAssocStrip";
		this.CreateAssocStrip.Size = new System.Drawing.Size(252, 22);
		this.CreateAssocStrip.Text = "Создать резервную ассоциацию";
		this.CreateAssocStrip.Click += new System.EventHandler(CreateAssocStrip_Click);
		this.CreateFileTypeStrip.Name = "CreateFileTypeStrip";
		this.CreateFileTypeStrip.Size = new System.Drawing.Size(252, 22);
		this.CreateFileTypeStrip.Text = "Создать резервный тип файла";
		this.CreateFileTypeStrip.Click += new System.EventHandler(CreateFileTypeStrip_Click);
		this.URStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.StatusText });
		this.URStatus.Location = new System.Drawing.Point(0, 549);
		this.URStatus.Name = "URStatus";
		this.URStatus.Size = new System.Drawing.Size(955, 24);
		this.URStatus.SizingGrip = false;
		this.URStatus.TabIndex = 2;
		this.StatusText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
		this.StatusText.Name = "StatusText";
		this.StatusText.Size = new System.Drawing.Size(264, 19);
		this.StatusText.Text = "Состояние: Ожидание действий пользователя";
		this.URControl.Controls.Add(this.URPage);
		this.URControl.Controls.Add(this.AssocPage);
		this.URControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.URControl.Location = new System.Drawing.Point(0, 24);
		this.URControl.Name = "URControl";
		this.URControl.SelectedIndex = 0;
		this.URControl.Size = new System.Drawing.Size(955, 525);
		this.URControl.TabIndex = 3;
		this.URPage.Controls.Add(this.URTypeControl);
		this.URPage.Location = new System.Drawing.Point(4, 22);
		this.URPage.Name = "URPage";
		this.URPage.Padding = new System.Windows.Forms.Padding(3);
		this.URPage.Size = new System.Drawing.Size(947, 499);
		this.URPage.TabIndex = 0;
		this.URPage.Text = "Разблокировка ограничений";
		this.URPage.UseVisualStyleBackColor = true;
		this.URTypeControl.Controls.Add(this.URScanPage);
		this.URTypeControl.Controls.Add(this.URManualPage);
		this.URTypeControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.URTypeControl.Location = new System.Drawing.Point(3, 3);
		this.URTypeControl.Name = "URTypeControl";
		this.URTypeControl.SelectedIndex = 0;
		this.URTypeControl.Size = new System.Drawing.Size(941, 493);
		this.URTypeControl.TabIndex = 0;
		this.URScanPage.Controls.Add(this.ResultBox);
		this.URScanPage.Controls.Add(this.AURControlPanel);
		this.URScanPage.Location = new System.Drawing.Point(4, 22);
		this.URScanPage.Name = "URScanPage";
		this.URScanPage.Padding = new System.Windows.Forms.Padding(3);
		this.URScanPage.Size = new System.Drawing.Size(933, 467);
		this.URScanPage.TabIndex = 0;
		this.URScanPage.Text = "Сканирование";
		this.URScanPage.UseVisualStyleBackColor = true;
		this.ResultBox.Controls.Add(this.ResultView);
		this.ResultBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ResultBox.Location = new System.Drawing.Point(3, 3);
		this.ResultBox.Name = "ResultBox";
		this.ResultBox.Size = new System.Drawing.Size(927, 434);
		this.ResultBox.TabIndex = 6;
		this.ResultBox.TabStop = false;
		this.ResultBox.Text = "Результаты сканирования";
		this.ResultView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.RestrictionName, this.DescriptionRestricion, this.PathRestriction });
		this.ResultView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ResultView.FullRowSelect = true;
		this.ResultView.GridLines = true;
		this.ResultView.HideSelection = false;
		this.ResultView.Location = new System.Drawing.Point(3, 16);
		this.ResultView.Name = "ResultView";
		this.ResultView.Size = new System.Drawing.Size(921, 415);
		this.ResultView.TabIndex = 0;
		this.ResultView.UseCompatibleStateImageBehavior = false;
		this.ResultView.View = System.Windows.Forms.View.Details;
		this.ResultView.MouseClick += new System.Windows.Forms.MouseEventHandler(ResultView_MouseClick);
		this.RestrictionName.Text = "Ограничение";
		this.RestrictionName.Width = 159;
		this.DescriptionRestricion.Text = "Описание ограничения";
		this.DescriptionRestricion.Width = 422;
		this.PathRestriction.Text = "Путь";
		this.PathRestriction.Width = 330;
		this.AURControlPanel.Controls.Add(this.StartScanButton);
		this.AURControlPanel.Controls.Add(this.AURCheckbox);
		this.AURControlPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.AURControlPanel.Location = new System.Drawing.Point(3, 437);
		this.AURControlPanel.Name = "AURControlPanel";
		this.AURControlPanel.Size = new System.Drawing.Size(927, 27);
		this.AURControlPanel.TabIndex = 0;
		this.StartScanButton.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.StartScanButton.Location = new System.Drawing.Point(770, 1);
		this.StartScanButton.Name = "StartScanButton";
		this.StartScanButton.Size = new System.Drawing.Size(157, 25);
		this.StartScanButton.TabIndex = 1;
		this.StartScanButton.Text = "Начать сканирование";
		this.StartScanButton.UseVisualStyleBackColor = true;
		this.StartScanButton.Click += new System.EventHandler(StartScanButton_Click);
		this.AURCheckbox.AutoSize = true;
		this.AURCheckbox.Location = new System.Drawing.Point(3, 6);
		this.AURCheckbox.Name = "AURCheckbox";
		this.AURCheckbox.Size = new System.Drawing.Size(258, 17);
		this.AURCheckbox.TabIndex = 0;
		this.AURCheckbox.Text = "Автоматическая разблокировка ограничений";
		this.AURCheckbox.UseVisualStyleBackColor = true;
		this.URManualPage.Controls.Add(this.BasicRestrictionsGroupBox);
		this.URManualPage.Controls.Add(this.MURPanel);
		this.URManualPage.Location = new System.Drawing.Point(4, 22);
		this.URManualPage.Name = "URManualPage";
		this.URManualPage.Padding = new System.Windows.Forms.Padding(3);
		this.URManualPage.Size = new System.Drawing.Size(933, 467);
		this.URManualPage.TabIndex = 1;
		this.URManualPage.Text = "Ручная разблокировка";
		this.URManualPage.UseVisualStyleBackColor = true;
		this.BasicRestrictionsGroupBox.Controls.Add(this.BasicRestrictionList);
		this.BasicRestrictionsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BasicRestrictionsGroupBox.Location = new System.Drawing.Point(3, 3);
		this.BasicRestrictionsGroupBox.Name = "BasicRestrictionsGroupBox";
		this.BasicRestrictionsGroupBox.Size = new System.Drawing.Size(927, 434);
		this.BasicRestrictionsGroupBox.TabIndex = 8;
		this.BasicRestrictionsGroupBox.TabStop = false;
		this.BasicRestrictionsGroupBox.Text = "Список ограничений";
		this.BasicRestrictionList.CheckBoxes = true;
		this.BasicRestrictionList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.BRMColumn, this.BRDColumn });
		this.BasicRestrictionList.Dock = System.Windows.Forms.DockStyle.Fill;
		this.BasicRestrictionList.FullRowSelect = true;
		this.BasicRestrictionList.GridLines = true;
		this.BasicRestrictionList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
		this.BasicRestrictionList.HideSelection = false;
		this.BasicRestrictionList.Location = new System.Drawing.Point(3, 16);
		this.BasicRestrictionList.Name = "BasicRestrictionList";
		this.BasicRestrictionList.Size = new System.Drawing.Size(921, 415);
		this.BasicRestrictionList.TabIndex = 0;
		this.BasicRestrictionList.UseCompatibleStateImageBehavior = false;
		this.BasicRestrictionList.View = System.Windows.Forms.View.Details;
		this.BasicRestrictionList.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(BasicRestrictionList_ItemChecked);
		this.BRMColumn.Text = "Ограничение";
		this.BRMColumn.Width = 159;
		this.BRDColumn.Text = "Описание ограничения";
		this.BRDColumn.Width = 599;
		this.MURPanel.Controls.Add(this.SelectAllBox);
		this.MURPanel.Controls.Add(this.ManualUnlockBtn);
		this.MURPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.MURPanel.Location = new System.Drawing.Point(3, 437);
		this.MURPanel.Name = "MURPanel";
		this.MURPanel.Size = new System.Drawing.Size(927, 27);
		this.MURPanel.TabIndex = 7;
		this.SelectAllBox.AutoSize = true;
		this.SelectAllBox.Location = new System.Drawing.Point(3, 6);
		this.SelectAllBox.Name = "SelectAllBox";
		this.SelectAllBox.Size = new System.Drawing.Size(91, 17);
		this.SelectAllBox.TabIndex = 2;
		this.SelectAllBox.Text = "Выбрать все";
		this.SelectAllBox.UseVisualStyleBackColor = true;
		this.SelectAllBox.CheckedChanged += new System.EventHandler(SelectAllBox_CheckedChanged);
		this.ManualUnlockBtn.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.ManualUnlockBtn.Enabled = false;
		this.ManualUnlockBtn.Location = new System.Drawing.Point(672, 1);
		this.ManualUnlockBtn.Name = "ManualUnlockBtn";
		this.ManualUnlockBtn.Size = new System.Drawing.Size(255, 25);
		this.ManualUnlockBtn.TabIndex = 1;
		this.ManualUnlockBtn.Text = "Разблокировать выбранные ограничения";
		this.ManualUnlockBtn.UseVisualStyleBackColor = true;
		this.ManualUnlockBtn.Click += new System.EventHandler(ManualUnlockBtn_Click);
		this.AssocPage.Controls.Add(this.AssocControl);
		this.AssocPage.Location = new System.Drawing.Point(4, 22);
		this.AssocPage.Name = "AssocPage";
		this.AssocPage.Padding = new System.Windows.Forms.Padding(3);
		this.AssocPage.Size = new System.Drawing.Size(947, 499);
		this.AssocPage.TabIndex = 1;
		this.AssocPage.Text = "Восстановление ассоциаций";
		this.AssocPage.UseVisualStyleBackColor = true;
		this.AssocControl.Controls.Add(this.AssocTab);
		this.AssocControl.Controls.Add(this.FileTypesTab);
		this.AssocControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.AssocControl.Location = new System.Drawing.Point(3, 3);
		this.AssocControl.Name = "AssocControl";
		this.AssocControl.SelectedIndex = 0;
		this.AssocControl.Size = new System.Drawing.Size(941, 493);
		this.AssocControl.TabIndex = 0;
		this.AssocTab.Controls.Add(this.AssocBox);
		this.AssocTab.Location = new System.Drawing.Point(4, 22);
		this.AssocTab.Name = "AssocTab";
		this.AssocTab.Padding = new System.Windows.Forms.Padding(3);
		this.AssocTab.Size = new System.Drawing.Size(933, 467);
		this.AssocTab.TabIndex = 0;
		this.AssocTab.Text = "Ассоциации";
		this.AssocTab.UseVisualStyleBackColor = true;
		this.AssocBox.Controls.Add(this.AssocView);
		this.AssocBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.AssocBox.Location = new System.Drawing.Point(3, 3);
		this.AssocBox.Name = "AssocBox";
		this.AssocBox.Size = new System.Drawing.Size(927, 461);
		this.AssocBox.TabIndex = 1;
		this.AssocBox.TabStop = false;
		this.AssocBox.Text = "Ассоциации";
		this.AssocView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[3] { this.AssocNameColumn, this.AssocFileTypeColumn, this.ShellOpenCommandColumn });
		this.AssocView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.AssocView.FullRowSelect = true;
		this.AssocView.GridLines = true;
		this.AssocView.HideSelection = false;
		this.AssocView.Location = new System.Drawing.Point(3, 16);
		this.AssocView.Name = "AssocView";
		this.AssocView.Size = new System.Drawing.Size(921, 442);
		this.AssocView.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.AssocView.TabIndex = 0;
		this.AssocView.UseCompatibleStateImageBehavior = false;
		this.AssocView.View = System.Windows.Forms.View.Details;
		this.AssocView.MouseClick += new System.Windows.Forms.MouseEventHandler(AssocView_MouseClick);
		this.AssocView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(AssocView_MouseDoubleClick);
		this.AssocNameColumn.Text = "Расширение файла";
		this.AssocNameColumn.Width = 130;
		this.AssocFileTypeColumn.Text = "Тип файла";
		this.AssocFileTypeColumn.Width = 189;
		this.ShellOpenCommandColumn.Text = "Команда";
		this.ShellOpenCommandColumn.Width = 455;
		this.FileTypesTab.Controls.Add(this.FileTypesBox);
		this.FileTypesTab.Location = new System.Drawing.Point(4, 22);
		this.FileTypesTab.Name = "FileTypesTab";
		this.FileTypesTab.Padding = new System.Windows.Forms.Padding(3);
		this.FileTypesTab.Size = new System.Drawing.Size(933, 467);
		this.FileTypesTab.TabIndex = 1;
		this.FileTypesTab.Text = "Типы файлов";
		this.FileTypesTab.UseVisualStyleBackColor = true;
		this.FileTypesBox.Controls.Add(this.FileTypeView);
		this.FileTypesBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.FileTypesBox.Location = new System.Drawing.Point(3, 3);
		this.FileTypesBox.Name = "FileTypesBox";
		this.FileTypesBox.Size = new System.Drawing.Size(927, 461);
		this.FileTypesBox.TabIndex = 0;
		this.FileTypesBox.TabStop = false;
		this.FileTypesBox.Text = "Типы файлов";
		this.FileTypeView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[4] { this.fTypeNameColumn, this.fTypeDefaultIconColumn, this.fTypeDescriptionColumn, this.fTypeOpenDefaultColumn });
		this.FileTypeView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.FileTypeView.FullRowSelect = true;
		this.FileTypeView.GridLines = true;
		this.FileTypeView.HideSelection = false;
		this.FileTypeView.Location = new System.Drawing.Point(3, 16);
		this.FileTypeView.Name = "FileTypeView";
		this.FileTypeView.Size = new System.Drawing.Size(921, 442);
		this.FileTypeView.Sorting = System.Windows.Forms.SortOrder.Ascending;
		this.FileTypeView.TabIndex = 1;
		this.FileTypeView.UseCompatibleStateImageBehavior = false;
		this.FileTypeView.View = System.Windows.Forms.View.Details;
		this.FileTypeView.MouseClick += new System.Windows.Forms.MouseEventHandler(FileTypeView_MouseClick);
		this.FileTypeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(FileTypeView_MouseDoubleClick);
		this.fTypeNameColumn.Text = "Тип файла";
		this.fTypeNameColumn.Width = 181;
		this.fTypeDefaultIconColumn.Text = "Иконка";
		this.fTypeDefaultIconColumn.Width = 210;
		this.fTypeDescriptionColumn.Text = "Описание";
		this.fTypeDescriptionColumn.Width = 191;
		this.fTypeOpenDefaultColumn.Text = "Команда по умолчанию";
		this.fTypeOpenDefaultColumn.Width = 332;
		this.ScanResultContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.URToolMenuStrip, this.OpenRegistryToolStripMenu });
		this.ScanResultContext.Name = "ScanResultContext";
		this.ScanResultContext.Size = new System.Drawing.Size(239, 48);
		this.URToolMenuStrip.Name = "URToolMenuStrip";
		this.URToolMenuStrip.Size = new System.Drawing.Size(238, 22);
		this.URToolMenuStrip.Text = "Разблокировать ограничение";
		this.URToolMenuStrip.Click += new System.EventHandler(URToolMenuStrip_Click);
		this.OpenRegistryToolStripMenu.Name = "OpenRegistryToolStripMenu";
		this.OpenRegistryToolStripMenu.Size = new System.Drawing.Size(238, 22);
		this.OpenRegistryToolStripMenu.Text = "Открыть в реестре";
		this.OpenRegistryToolStripMenu.Click += new System.EventHandler(OpenRegistryToolStripMenu_Click);
		this.AssocContextList.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.deleteAssocMenuItem, this.changeAssocMenuItem, this.refreshAssocMenuItem });
		this.AssocContextList.Name = "AssocContextList";
		this.AssocContextList.Size = new System.Drawing.Size(129, 70);
		this.deleteAssocMenuItem.Name = "deleteAssocMenuItem";
		this.deleteAssocMenuItem.Size = new System.Drawing.Size(128, 22);
		this.deleteAssocMenuItem.Text = "Удалить";
		this.deleteAssocMenuItem.Click += new System.EventHandler(deleteAssocMenuItem_Click);
		this.changeAssocMenuItem.Name = "changeAssocMenuItem";
		this.changeAssocMenuItem.Size = new System.Drawing.Size(128, 22);
		this.changeAssocMenuItem.Text = "Изменить";
		this.changeAssocMenuItem.Click += new System.EventHandler(changeAssocMenuItem_Click);
		this.refreshAssocMenuItem.Name = "refreshAssocMenuItem";
		this.refreshAssocMenuItem.Size = new System.Drawing.Size(128, 22);
		this.refreshAssocMenuItem.Text = "Обновить";
		this.refreshAssocMenuItem.Click += new System.EventHandler(refreshAssocMenuItem_Click);
		this.FileTypeContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.delFTItem, this.chgFTItem, this.refFTItem });
		this.FileTypeContext.Name = "AssocContextList";
		this.FileTypeContext.Size = new System.Drawing.Size(129, 70);
		this.delFTItem.Name = "delFTItem";
		this.delFTItem.Size = new System.Drawing.Size(128, 22);
		this.delFTItem.Text = "Удалить";
		this.delFTItem.Click += new System.EventHandler(delFTItem_Click);
		this.chgFTItem.Name = "chgFTItem";
		this.chgFTItem.Size = new System.Drawing.Size(128, 22);
		this.chgFTItem.Text = "Изменить";
		this.chgFTItem.Click += new System.EventHandler(chgFTItem_Click);
		this.refFTItem.Name = "refFTItem";
		this.refFTItem.Size = new System.Drawing.Size(128, 22);
		this.refFTItem.Text = "Обновить";
		this.refFTItem.Click += new System.EventHandler(refFTItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(955, 573);
		base.Controls.Add(this.URControl);
		base.Controls.Add(this.URStatus);
		base.Controls.Add(this.URMenu);
		base.MainMenuStrip = this.URMenu;
		this.MinimumSize = new System.Drawing.Size(600, 400);
		base.Name = "UnblockRestriction";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(UnblockRestriction_FormClosing);
		base.FormClosed += new System.Windows.Forms.FormClosedEventHandler(UnblockRestriction_FormClosed);
		base.Load += new System.EventHandler(UnblockRestriction_Load);
		this.URMenu.ResumeLayout(false);
		this.URMenu.PerformLayout();
		this.URStatus.ResumeLayout(false);
		this.URStatus.PerformLayout();
		this.URControl.ResumeLayout(false);
		this.URPage.ResumeLayout(false);
		this.URTypeControl.ResumeLayout(false);
		this.URScanPage.ResumeLayout(false);
		this.ResultBox.ResumeLayout(false);
		this.AURControlPanel.ResumeLayout(false);
		this.AURControlPanel.PerformLayout();
		this.URManualPage.ResumeLayout(false);
		this.BasicRestrictionsGroupBox.ResumeLayout(false);
		this.MURPanel.ResumeLayout(false);
		this.MURPanel.PerformLayout();
		this.AssocPage.ResumeLayout(false);
		this.AssocControl.ResumeLayout(false);
		this.AssocTab.ResumeLayout(false);
		this.AssocBox.ResumeLayout(false);
		this.FileTypesTab.ResumeLayout(false);
		this.FileTypesBox.ResumeLayout(false);
		this.ScanResultContext.ResumeLayout(false);
		this.AssocContextList.ResumeLayout(false);
		this.FileTypeContext.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
