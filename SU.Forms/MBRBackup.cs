using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Management;
using System.Windows.Forms;
using SU.Classes;

namespace SU.Forms;

public class MBRBackup : Form
{
	private IContainer components;

	private MenuStrip menuToolStrip;

	private ToolStripMenuItem menuItem;

	private ToolStripMenuItem backMenuItem;

	private ToolStripMenuItem exitProgramItem;

	private GroupBox MBRBox;

	private Button backupMBRBtn;

	private Button restoreMBRBtn;

	private ComboBox DiskBox;

	private Label label1;

	public MBRBackup()
	{
		InitializeComponent();
	}

	private void DiskBox_DropDown(object sender, EventArgs e)
	{
		DiskBox.Items.Clear();
		using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT DeviceID FROM Win32_DiskDrive");
		foreach (ManagementObject item in managementObjectSearcher.Get())
		{
			DiskBox.Items.Add((string)item["DeviceID"]);
		}
	}

	private void restoreMBRBtn_Click(object sender, EventArgs e)
	{
		using OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.Filter = "SimpleUnlocker MBR Backup|*.suMBR";
		if (DiskBox.SelectedItem == null || openFileDialog.ShowDialog() != DialogResult.OK || MessageBox.Show($"Вы действительно хотите восстановить MBR [{DiskBox.SelectedItem}] из файла? Если был выбран неверный файл восстановления то MBR будет поврежден. Вы делате это на свой страх и риск.", "Внимание!", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) != DialogResult.Yes)
		{
			return;
		}
		using BinaryReader binaryReader = new BinaryReader(File.OpenRead(openFileDialog.FileName));
		if (Utils.WriteMBR(binaryReader.ReadBytes((int)binaryReader.BaseStream.Length), DiskBox.SelectedItem.ToString()))
		{
			MessageBox.Show($"MBR [{DiskBox.SelectedItem}] был успешно восстановлен из файла!", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}
		else
		{
			MessageBox.Show($"MBR [{DiskBox.SelectedItem}] не был восстановлен из файла!", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
		}
	}

	private void backupMBRBtn_Click(object sender, EventArgs e)
	{
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "SimpleUnlocker MBR Backup|*.suMBR";
		saveFileDialog.DefaultExt = "suMBR";
		if (DiskBox.SelectedItem != null && saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(File.OpenWrite(saveFileDialog.FileName)))
			{
				binaryWriter.Write(Utils.ReadMBR(DiskBox.SelectedItem.ToString()));
				binaryWriter.Flush();
				MessageBox.Show($"Резеврная копия MBR [{DiskBox.SelectedItem}] была успешно создана!", "", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
				return;
			}
		}
	}

	private void backMenuItem_Click(object sender, EventArgs e)
	{
		Dispose();
		Close();
	}

	private void exitProgramItem_Click(object sender, EventArgs e)
	{
		Environment.Exit(0);
	}

	private void MBRBackup_FormClosing(object sender, FormClosingEventArgs e)
	{
		Utils.CloseForm(e);
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
		this.menuToolStrip = new System.Windows.Forms.MenuStrip();
		this.menuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.backMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.exitProgramItem = new System.Windows.Forms.ToolStripMenuItem();
		this.MBRBox = new System.Windows.Forms.GroupBox();
		this.DiskBox = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.restoreMBRBtn = new System.Windows.Forms.Button();
		this.backupMBRBtn = new System.Windows.Forms.Button();
		this.menuToolStrip.SuspendLayout();
		this.MBRBox.SuspendLayout();
		base.SuspendLayout();
		this.menuToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.menuItem });
		this.menuToolStrip.Location = new System.Drawing.Point(0, 0);
		this.menuToolStrip.Name = "menuToolStrip";
		this.menuToolStrip.Size = new System.Drawing.Size(271, 24);
		this.menuToolStrip.TabIndex = 1;
		this.menuToolStrip.Text = "menuStrip1";
		this.menuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.backMenuItem, this.exitProgramItem });
		this.menuItem.Name = "menuItem";
		this.menuItem.Size = new System.Drawing.Size(53, 20);
		this.menuItem.Text = "Меню";
		this.backMenuItem.Name = "backMenuItem";
		this.backMenuItem.Size = new System.Drawing.Size(221, 22);
		this.backMenuItem.Text = "Вернуться в главное меню";
		this.backMenuItem.Click += new System.EventHandler(backMenuItem_Click);
		this.exitProgramItem.Name = "exitProgramItem";
		this.exitProgramItem.Size = new System.Drawing.Size(221, 22);
		this.exitProgramItem.Text = "Выйти из программы";
		this.exitProgramItem.Click += new System.EventHandler(exitProgramItem_Click);
		this.MBRBox.Controls.Add(this.DiskBox);
		this.MBRBox.Controls.Add(this.label1);
		this.MBRBox.Controls.Add(this.restoreMBRBtn);
		this.MBRBox.Controls.Add(this.backupMBRBtn);
		this.MBRBox.Location = new System.Drawing.Point(12, 27);
		this.MBRBox.Name = "MBRBox";
		this.MBRBox.Size = new System.Drawing.Size(243, 109);
		this.MBRBox.TabIndex = 2;
		this.MBRBox.TabStop = false;
		this.MBRBox.Text = "MBR Backup";
		this.DiskBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.DiskBox.FormattingEnabled = true;
		this.DiskBox.Location = new System.Drawing.Point(53, 19);
		this.DiskBox.Name = "DiskBox";
		this.DiskBox.Size = new System.Drawing.Size(180, 21);
		this.DiskBox.TabIndex = 2;
		this.DiskBox.DropDown += new System.EventHandler(DiskBox_DropDown);
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(10, 22);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(37, 13);
		this.label1.TabIndex = 1;
		this.label1.Text = "Диск:";
		this.restoreMBRBtn.Location = new System.Drawing.Point(10, 46);
		this.restoreMBRBtn.Name = "restoreMBRBtn";
		this.restoreMBRBtn.Size = new System.Drawing.Size(223, 23);
		this.restoreMBRBtn.TabIndex = 0;
		this.restoreMBRBtn.Text = "Восстановить из файла";
		this.restoreMBRBtn.UseVisualStyleBackColor = true;
		this.restoreMBRBtn.Click += new System.EventHandler(restoreMBRBtn_Click);
		this.backupMBRBtn.Location = new System.Drawing.Point(10, 75);
		this.backupMBRBtn.Name = "backupMBRBtn";
		this.backupMBRBtn.Size = new System.Drawing.Size(223, 23);
		this.backupMBRBtn.TabIndex = 1;
		this.backupMBRBtn.Text = "Сделать backup";
		this.backupMBRBtn.UseVisualStyleBackColor = true;
		this.backupMBRBtn.Click += new System.EventHandler(backupMBRBtn_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(271, 145);
		base.Controls.Add(this.MBRBox);
		base.Controls.Add(this.menuToolStrip);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "MBRBackup";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(MBRBackup_FormClosing);
		this.menuToolStrip.ResumeLayout(false);
		this.menuToolStrip.PerformLayout();
		this.MBRBox.ResumeLayout(false);
		this.MBRBox.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
