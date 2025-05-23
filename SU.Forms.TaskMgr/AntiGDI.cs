using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SU.Classes;
using SU.Properties;

namespace SU.Forms.TaskMgr;

public class AntiGDI : Form
{
	private List<int> ProcessID = new List<int>();

	private IContainer components;

	private GroupBox mainBox;

	private ComboBox prcBox;

	private Label label1;

	private Button cancelBtn;

	private Button runBtn;

	private Label label2;

	private void GetProcesses()
	{
		ProcessID = new List<int>();
		prcBox.Items.Clear();
		Process[] processes = Process.GetProcesses();
		foreach (Process process in processes)
		{
			if (process.Threads[0].ThreadState == ThreadState.Wait && process.Threads[0].WaitReason == ThreadWaitReason.Suspended)
			{
				prcBox.Items.Add($"Process: {process.ProcessName}.exe | ID: {process.Id} | Suspended");
			}
			else
			{
				prcBox.Items.Add($"Process: {process.ProcessName}.exe | ID: {process.Id}");
			}
			ProcessID.Add(process.Id);
			process.Dispose();
		}
	}

	public AntiGDI()
	{
		InitializeComponent();
	}

	private void prcBox_DropDown(object sender, EventArgs e)
	{
		GetProcesses();
	}

	private void runBtn_Click(object sender, EventArgs e)
	{
		Utils.RunFile("bin\\AntiGDI_Injector.exe", $"{ProcessID[prcBox.SelectedIndex]}", UAC: true, Hidden: false, WaitForExit: false);
	}

	private void cancelBtn_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void AntiGDI_Load(object sender, EventArgs e)
	{
		if (SU.Properties.Settings.Default.RandomWindowName)
		{
			string text = Utils.RandomString(10, lowerCase: true);
			Text = text;
		}
		else
		{
			Text = "SimpleUnlocker [AntiGDI]";
		}
		base.TopMost = SU.Properties.Settings.Default.AlwaysOnTop;
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
		this.mainBox = new System.Windows.Forms.GroupBox();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.runBtn = new System.Windows.Forms.Button();
		this.prcBox = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.mainBox.SuspendLayout();
		base.SuspendLayout();
		this.mainBox.Controls.Add(this.cancelBtn);
		this.mainBox.Controls.Add(this.runBtn);
		this.mainBox.Controls.Add(this.prcBox);
		this.mainBox.Controls.Add(this.label1);
		this.mainBox.Location = new System.Drawing.Point(12, 41);
		this.mainBox.Name = "mainBox";
		this.mainBox.Size = new System.Drawing.Size(414, 75);
		this.mainBox.TabIndex = 1;
		this.mainBox.TabStop = false;
		this.mainBox.Text = "Главная";
		this.cancelBtn.Location = new System.Drawing.Point(319, 46);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(75, 23);
		this.cancelBtn.TabIndex = 3;
		this.cancelBtn.Text = "Отмена";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.runBtn.Location = new System.Drawing.Point(9, 46);
		this.runBtn.Name = "runBtn";
		this.runBtn.Size = new System.Drawing.Size(75, 23);
		this.runBtn.TabIndex = 2;
		this.runBtn.Text = "Старт";
		this.runBtn.UseVisualStyleBackColor = true;
		this.runBtn.Click += new System.EventHandler(runBtn_Click);
		this.prcBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.prcBox.FormattingEnabled = true;
		this.prcBox.Location = new System.Drawing.Point(80, 15);
		this.prcBox.Name = "prcBox";
		this.prcBox.Size = new System.Drawing.Size(314, 21);
		this.prcBox.TabIndex = 1;
		this.prcBox.DropDown += new System.EventHandler(prcBox_DropDown);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(6, 16);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(68, 17);
		this.label1.TabIndex = 0;
		this.label1.Text = "Процесс:";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Segoe UI", 18f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.Location = new System.Drawing.Point(166, 6);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(97, 32);
		this.label2.TabIndex = 2;
		this.label2.Text = "AntiGDI";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(438, 125);
		base.ControlBox = false;
		base.Controls.Add(this.label2);
		base.Controls.Add(this.mainBox);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.Name = "AntiGDI";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.Load += new System.EventHandler(AntiGDI_Load);
		this.mainBox.ResumeLayout(false);
		this.mainBox.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
