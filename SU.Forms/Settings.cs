using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SU.Properties;

namespace SU.Forms;

public class Settings : Form
{
	private IContainer components;

	private GroupBox mainBox;

	private CheckBox RandomWindowName;

	private CheckBox AlwaysOnTop;

	private Button doneBtn;

	private Button cancelBtn;

	private CheckBox AutoUpdate;

	private Label label1;

	private GroupBox actionBox;

	private CheckBox MainMenuClose;

	private CheckBox CloseApp;

	public Settings()
	{
		InitializeComponent();
		RandomWindowName.Checked = SU.Properties.Settings.Default.RandomWindowName;
		AlwaysOnTop.Checked = SU.Properties.Settings.Default.AlwaysOnTop;
		AutoUpdate.Checked = SU.Properties.Settings.Default.AutoCheckUpdate;
		MainMenuClose.Checked = SU.Properties.Settings.Default.CloseMainMenuOnAction;
		CloseApp.Checked = SU.Properties.Settings.Default.CloseAppOnClick;
		if (!MainMenuClose.Checked)
		{
			CloseApp.Checked = false;
			CloseApp.Enabled = false;
		}
	}

	private void RandomWindowName_CheckedChanged(object sender, EventArgs e)
	{
		SU.Properties.Settings.Default.RandomWindowName = RandomWindowName.Checked;
	}

	private void AlwaysOnTop_CheckedChanged(object sender, EventArgs e)
	{
		SU.Properties.Settings.Default.AlwaysOnTop = AlwaysOnTop.Checked;
	}

	private void AutoUpdate_CheckedChanged(object sender, EventArgs e)
	{
		SU.Properties.Settings.Default.AutoCheckUpdate = AutoUpdate.Checked;
	}

	private void MainMenuClose_CheckedChanged(object sender, EventArgs e)
	{
		if (!MainMenuClose.Checked)
		{
			CloseApp.Checked = false;
			CloseApp.Enabled = false;
		}
		else
		{
			CloseApp.Enabled = true;
		}
		SU.Properties.Settings.Default.CloseMainMenuOnAction = MainMenuClose.Checked;
	}

	private void CloseApp_CheckedChanged(object sender, EventArgs e)
	{
		SU.Properties.Settings.Default.CloseAppOnClick = CloseApp.Checked;
	}

	private void doneBtn_Click(object sender, EventArgs e)
	{
		SU.Properties.Settings.Default.Save();
		if (MessageBox.Show("Требуется перезапуск программы. Вы хотите перезапустить программу сейчас", "", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.Yes)
		{
			Application.Restart();
		}
		else
		{
			Close();
		}
	}

	private void cancelBtn_Click(object sender, EventArgs e)
	{
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
		this.mainBox = new System.Windows.Forms.GroupBox();
		this.AutoUpdate = new System.Windows.Forms.CheckBox();
		this.RandomWindowName = new System.Windows.Forms.CheckBox();
		this.AlwaysOnTop = new System.Windows.Forms.CheckBox();
		this.doneBtn = new System.Windows.Forms.Button();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.label1 = new System.Windows.Forms.Label();
		this.actionBox = new System.Windows.Forms.GroupBox();
		this.CloseApp = new System.Windows.Forms.CheckBox();
		this.MainMenuClose = new System.Windows.Forms.CheckBox();
		this.mainBox.SuspendLayout();
		this.actionBox.SuspendLayout();
		base.SuspendLayout();
		this.mainBox.Controls.Add(this.AutoUpdate);
		this.mainBox.Controls.Add(this.RandomWindowName);
		this.mainBox.Controls.Add(this.AlwaysOnTop);
		this.mainBox.Location = new System.Drawing.Point(12, 45);
		this.mainBox.Name = "mainBox";
		this.mainBox.Size = new System.Drawing.Size(341, 90);
		this.mainBox.TabIndex = 0;
		this.mainBox.TabStop = false;
		this.mainBox.Text = "Основные";
		this.AutoUpdate.AutoSize = true;
		this.AutoUpdate.Location = new System.Drawing.Point(6, 18);
		this.AutoUpdate.Name = "AutoUpdate";
		this.AutoUpdate.Size = new System.Drawing.Size(288, 17);
		this.AutoUpdate.TabIndex = 2;
		this.AutoUpdate.Text = "Автоматически проверять обновления при запуске";
		this.AutoUpdate.UseVisualStyleBackColor = true;
		this.AutoUpdate.CheckedChanged += new System.EventHandler(AutoUpdate_CheckedChanged);
		this.RandomWindowName.AutoSize = true;
		this.RandomWindowName.Location = new System.Drawing.Point(6, 41);
		this.RandomWindowName.Name = "RandomWindowName";
		this.RandomWindowName.Size = new System.Drawing.Size(269, 17);
		this.RandomWindowName.TabIndex = 1;
		this.RandomWindowName.Text = "Рандомное название окна при каждом запуске";
		this.RandomWindowName.UseVisualStyleBackColor = true;
		this.RandomWindowName.CheckedChanged += new System.EventHandler(RandomWindowName_CheckedChanged);
		this.AlwaysOnTop.AutoSize = true;
		this.AlwaysOnTop.Location = new System.Drawing.Point(6, 64);
		this.AlwaysOnTop.Name = "AlwaysOnTop";
		this.AlwaysOnTop.Size = new System.Drawing.Size(143, 17);
		this.AlwaysOnTop.TabIndex = 0;
		this.AlwaysOnTop.Text = "Быть поверх всех окон";
		this.AlwaysOnTop.UseVisualStyleBackColor = true;
		this.AlwaysOnTop.CheckedChanged += new System.EventHandler(AlwaysOnTop_CheckedChanged);
		this.doneBtn.Location = new System.Drawing.Point(12, 218);
		this.doneBtn.Name = "doneBtn";
		this.doneBtn.Size = new System.Drawing.Size(75, 23);
		this.doneBtn.TabIndex = 2;
		this.doneBtn.Text = "Применить";
		this.doneBtn.UseVisualStyleBackColor = true;
		this.doneBtn.Click += new System.EventHandler(doneBtn_Click);
		this.cancelBtn.Location = new System.Drawing.Point(278, 218);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(75, 23);
		this.cancelBtn.TabIndex = 3;
		this.cancelBtn.Text = "Отмена";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Segoe UI", 18f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(109, 13);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(133, 32);
		this.label1.TabIndex = 4;
		this.label1.Text = "Настройки";
		this.actionBox.Controls.Add(this.CloseApp);
		this.actionBox.Controls.Add(this.MainMenuClose);
		this.actionBox.Location = new System.Drawing.Point(12, 141);
		this.actionBox.Name = "actionBox";
		this.actionBox.Size = new System.Drawing.Size(341, 71);
		this.actionBox.TabIndex = 5;
		this.actionBox.TabStop = false;
		this.actionBox.Text = "Поведение";
		this.CloseApp.AutoSize = true;
		this.CloseApp.Location = new System.Drawing.Point(6, 42);
		this.CloseApp.Name = "CloseApp";
		this.CloseApp.Size = new System.Drawing.Size(267, 17);
		this.CloseApp.TabIndex = 1;
		this.CloseApp.Text = "Закрывать программу при нажатии на крестик";
		this.CloseApp.UseVisualStyleBackColor = true;
		this.CloseApp.CheckedChanged += new System.EventHandler(CloseApp_CheckedChanged);
		this.MainMenuClose.AutoSize = true;
		this.MainMenuClose.Location = new System.Drawing.Point(6, 19);
		this.MainMenuClose.Name = "MainMenuClose";
		this.MainMenuClose.Size = new System.Drawing.Size(272, 17);
		this.MainMenuClose.TabIndex = 0;
		this.MainMenuClose.Text = "Закрывать главное меню при запуске действия";
		this.MainMenuClose.UseVisualStyleBackColor = true;
		this.MainMenuClose.CheckedChanged += new System.EventHandler(MainMenuClose_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(365, 252);
		base.Controls.Add(this.actionBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.cancelBtn);
		base.Controls.Add(this.doneBtn);
		base.Controls.Add(this.mainBox);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Settings";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.mainBox.ResumeLayout(false);
		this.mainBox.PerformLayout();
		this.actionBox.ResumeLayout(false);
		this.actionBox.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
