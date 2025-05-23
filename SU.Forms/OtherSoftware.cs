using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SU.Classes;

namespace SU.Forms;

public class OtherSoftware : Form
{
	private class Software
	{
		public string Name { get; set; }

		public string Description { get; set; }

		public string Path { get; set; }
	}

	private IContainer components;

	private MenuStrip menuToolStrip;

	private ToolStripMenuItem menuItem;

	private ToolStripMenuItem backMenuItem;

	private ToolStripMenuItem exitProgramItem;

	private GroupBox osBox;

	private ListView osView;

	private ColumnHeader nameSoftwareColumn;

	private ColumnHeader descriptionSoftwareColumn;

	private List<Software> GetListSoftwares()
	{
		List<Software> list = new List<Software>();
		if (!Directory.Exists("othersoftware"))
		{
			return list;
		}
		string[] directories = Directory.GetDirectories("othersoftware");
		foreach (string text in directories)
		{
			IniFile iniFile = new IniFile(text + "\\software.ini");
			string name = iniFile.ReadINI("Software", "Name");
			string description = iniFile.ReadINI("Software", "Description");
			string text2 = iniFile.ReadINI("Software", "RunFile");
			list.Add(new Software
			{
				Name = name,
				Description = description,
				Path = text + "\\" + text2
			});
		}
		return list;
	}

	private void GetSoftwares()
	{
		List<Software> listSoftwares = GetListSoftwares();
		osView.Items.Clear();
		foreach (Software item in listSoftwares)
		{
			ListViewItem listViewItem = new ListViewItem(new string[2] { item.Name, item.Description });
			listViewItem.Tag = item;
			osView.Items.Add(listViewItem);
		}
	}

	public OtherSoftware()
	{
		InitializeComponent();
		GetSoftwares();
	}

	private void osView_MouseDoubleClick(object sender, MouseEventArgs e)
	{
		if (e.Button == MouseButtons.Left && osView.SelectedItems.Count > 0)
		{
			Utils.RunFile(((Software)osView.FocusedItem.Tag).Path, "", UAC: true, Hidden: false, WaitForExit: false);
			base.WindowState = FormWindowState.Minimized;
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

	private void OtherSoftware_FormClosing(object sender, FormClosingEventArgs e)
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
		this.osBox = new System.Windows.Forms.GroupBox();
		this.osView = new System.Windows.Forms.ListView();
		this.nameSoftwareColumn = new System.Windows.Forms.ColumnHeader();
		this.descriptionSoftwareColumn = new System.Windows.Forms.ColumnHeader();
		this.menuToolStrip.SuspendLayout();
		this.osBox.SuspendLayout();
		base.SuspendLayout();
		this.menuToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.menuItem });
		this.menuToolStrip.Location = new System.Drawing.Point(0, 0);
		this.menuToolStrip.Name = "menuToolStrip";
		this.menuToolStrip.Size = new System.Drawing.Size(800, 24);
		this.menuToolStrip.TabIndex = 2;
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
		this.osBox.Controls.Add(this.osView);
		this.osBox.Dock = System.Windows.Forms.DockStyle.Fill;
		this.osBox.Location = new System.Drawing.Point(0, 24);
		this.osBox.Name = "osBox";
		this.osBox.Size = new System.Drawing.Size(800, 426);
		this.osBox.TabIndex = 3;
		this.osBox.TabStop = false;
		this.osBox.Text = "Сторонние утилиты";
		this.osView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.nameSoftwareColumn, this.descriptionSoftwareColumn });
		this.osView.Dock = System.Windows.Forms.DockStyle.Fill;
		this.osView.FullRowSelect = true;
		this.osView.GridLines = true;
		this.osView.HideSelection = false;
		this.osView.Location = new System.Drawing.Point(3, 16);
		this.osView.MultiSelect = false;
		this.osView.Name = "osView";
		this.osView.Size = new System.Drawing.Size(794, 407);
		this.osView.TabIndex = 0;
		this.osView.UseCompatibleStateImageBehavior = false;
		this.osView.View = System.Windows.Forms.View.Details;
		this.osView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(osView_MouseDoubleClick);
		this.nameSoftwareColumn.Text = "Название";
		this.nameSoftwareColumn.Width = 181;
		this.descriptionSoftwareColumn.Text = "Описание";
		this.descriptionSoftwareColumn.Width = 607;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.osBox);
		base.Controls.Add(this.menuToolStrip);
		base.Name = "OtherSoftware";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(OtherSoftware_FormClosing);
		this.menuToolStrip.ResumeLayout(false);
		this.menuToolStrip.PerformLayout();
		this.osBox.ResumeLayout(false);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
