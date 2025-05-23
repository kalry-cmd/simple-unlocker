using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Ionic.Zip;
using log4net;
using SU.Classes;
using SU.Properties;

namespace SU.Forms;

public class MainMenu : Form
{
	private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private IContainer components;

	private GroupBox MenuPanel;

	private StatusStrip MenuStatus;

	private ToolStripStatusLabel StatusVersionLabel;

	private Button APButton;

	private Button SButton;

	private Button MBButton;

	private Button RUButton;

	private Button ARButton;

	private Button TMButton;

	private Button URButton;

	private Label ProgName;

	private BackgroundWorker checkUpdateWorker;

	private BackgroundWorker updaterUnpacker;

	public MainMenu()
	{
		InitializeComponent();
	}

	private void BtnState(bool enabled)
	{
		foreach (Button item in MenuPanel.Controls.OfType<Button>())
		{
			item.Enabled = enabled;
		}
	}

	private void RunForm(Form form, string formName, string fullName, bool hideMainForm = true)
	{
		if (SU.Properties.Settings.Default.RandomWindowName)
		{
			string text = Utils.RandomString(10, lowerCase: true);
			form.Text = text;
		}
		else
		{
			form.Text = fullName;
		}
		form.TopMost = SU.Properties.Settings.Default.AlwaysOnTop;
		if (SU.Properties.Settings.Default.CloseMainMenuOnAction)
		{
			if (hideMainForm)
			{
				Hide();
			}
			form.ShowDialog();
		}
		else if (Application.OpenForms[formName] == null)
		{
			form.Show();
		}
	}

	private void MainMenu_Load(object sender, EventArgs e)
	{
		logger.Debug((object)"Инициализация главной формы.");
		Process.EnterDebugMode();
		if (SU.Properties.Settings.Default.RandomWindowName)
		{
			string text = Utils.RandomString(Utils.GetRandomNumber(8, 18), lowerCase: true);
			Text = text;
		}
		else
		{
			Text = "SimpleUnlocker [Main]";
		}
		base.TopMost = SU.Properties.Settings.Default.AlwaysOnTop;
		StatusVersionLabel.Text = "Версия: " + Application.ProductVersion;
		checkUpdateWorker.WorkerReportsProgress = true;
		if (SU.Properties.Settings.Default.AutoCheckUpdate && !Program.AutoUpdateClickNo)
		{
			checkUpdateWorker.RunWorkerAsync();
		}
	}

	private void URButton_Click(object sender, EventArgs e)
	{
		RunForm(new UnblockRestriction(), "UnblockRestriction", "SimpleUnlocker [Разблокировка ограничений]");
	}

	private void ARButton_Click(object sender, EventArgs e)
	{
		RunForm(new AutorunView(), "AutorunView", "SimpleUnlocker [Автозапуск приложений]");
	}

	private void TMButton_Click(object sender, EventArgs e)
	{
		BtnState(enabled: false);
		RunForm(new TaskManager(), "TaskManager", "SimpleUnlocker [Диспетчер задач]");
		BtnState(enabled: true);
	}

	private void SButton_Click(object sender, EventArgs e)
	{
		RunForm(new Settings(), "Settings", "Настройки", hideMainForm: false);
	}

	private void MBButton_Click(object sender, EventArgs e)
	{
		RunForm(new MBRBackup(), "MBRBackup", "SimpleUnlocker [Восстановление MBR]");
	}

	private void RUButton_Click(object sender, EventArgs e)
	{
		RunForm(new OtherSoftware(), "OtherSoftware", "SimpleUnlocker [Сторонние утилиты]");
	}

	private void checkUpdateWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		Utils.CheckUpdate(this, Client_DownloadFileCompleted, InformateUser: false);
	}

	private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
	{
		if (!Directory.Exists("temp"))
		{
			Directory.CreateDirectory("temp");
		}
		File.Move("temp\\updater.temp", "temp\\updater.zip");
		updaterUnpacker.RunWorkerAsync();
	}

	private void unpackerWorker_DoWork(object sender, DoWorkEventArgs e)
	{
		logger.Info((object)"Распаковка апдейтера");
		try
		{
			ZipFile val = ZipFile.Read("temp\\updater.zip");
			try
			{
				val.ExtractAll(Environment.CurrentDirectory + "\\bin", (ExtractExistingFileAction)1);
			}
			finally
			{
				((IDisposable)val)?.Dispose();
			}
		}
		catch (Exception ex)
		{
			logger.Error((object)ex.Message);
			MessageBox.Show(ex.ToString());
		}
	}

	private void updaterUnpacker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		logger.Info((object)"Запуск процесса обновления");
		Version arg = new Version(Application.ProductVersion);
		File.Delete("temp\\updater.zip");
		Utils.RunFile("bin\\su_updater.exe", $"{arg} {Path.GetFileName(Application.ExecutablePath)}", UAC: true, Hidden: false, WaitForExit: false);
		Environment.Exit(0);
	}

	private void APButton_Click(object sender, EventArgs e)
	{
		About about = new About();
		if (Application.OpenForms["About"] == null)
		{
			about.Show();
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
		this.MenuPanel = new System.Windows.Forms.GroupBox();
		this.APButton = new System.Windows.Forms.Button();
		this.SButton = new System.Windows.Forms.Button();
		this.MBButton = new System.Windows.Forms.Button();
		this.RUButton = new System.Windows.Forms.Button();
		this.ARButton = new System.Windows.Forms.Button();
		this.TMButton = new System.Windows.Forms.Button();
		this.URButton = new System.Windows.Forms.Button();
		this.MenuStatus = new System.Windows.Forms.StatusStrip();
		this.StatusVersionLabel = new System.Windows.Forms.ToolStripStatusLabel();
		this.ProgName = new System.Windows.Forms.Label();
		this.checkUpdateWorker = new System.ComponentModel.BackgroundWorker();
		this.updaterUnpacker = new System.ComponentModel.BackgroundWorker();
		this.MenuPanel.SuspendLayout();
		this.MenuStatus.SuspendLayout();
		base.SuspendLayout();
		this.MenuPanel.Controls.Add(this.APButton);
		this.MenuPanel.Controls.Add(this.SButton);
		this.MenuPanel.Controls.Add(this.MBButton);
		this.MenuPanel.Controls.Add(this.RUButton);
		this.MenuPanel.Controls.Add(this.ARButton);
		this.MenuPanel.Controls.Add(this.TMButton);
		this.MenuPanel.Controls.Add(this.URButton);
		this.MenuPanel.Location = new System.Drawing.Point(12, 42);
		this.MenuPanel.Name = "MenuPanel";
		this.MenuPanel.Size = new System.Drawing.Size(452, 164);
		this.MenuPanel.TabIndex = 1;
		this.MenuPanel.TabStop = false;
		this.MenuPanel.Text = "Меню";
		this.APButton.Location = new System.Drawing.Point(230, 132);
		this.APButton.Name = "APButton";
		this.APButton.Size = new System.Drawing.Size(216, 23);
		this.APButton.TabIndex = 6;
		this.APButton.Text = "О программе";
		this.APButton.UseVisualStyleBackColor = true;
		this.APButton.Click += new System.EventHandler(APButton_Click);
		this.SButton.Location = new System.Drawing.Point(6, 132);
		this.SButton.Name = "SButton";
		this.SButton.Size = new System.Drawing.Size(216, 23);
		this.SButton.TabIndex = 5;
		this.SButton.Text = "Настройки";
		this.SButton.UseVisualStyleBackColor = true;
		this.SButton.Click += new System.EventHandler(SButton_Click);
		this.MBButton.Location = new System.Drawing.Point(6, 103);
		this.MBButton.Name = "MBButton";
		this.MBButton.Size = new System.Drawing.Size(440, 23);
		this.MBButton.TabIndex = 4;
		this.MBButton.Text = "Восстановление MBR";
		this.MBButton.UseVisualStyleBackColor = true;
		this.MBButton.Click += new System.EventHandler(MBButton_Click);
		this.RUButton.Location = new System.Drawing.Point(230, 61);
		this.RUButton.Name = "RUButton";
		this.RUButton.Size = new System.Drawing.Size(216, 36);
		this.RUButton.TabIndex = 3;
		this.RUButton.Text = "Запуск сторонних утилит";
		this.RUButton.UseVisualStyleBackColor = true;
		this.RUButton.Click += new System.EventHandler(RUButton_Click);
		this.ARButton.Location = new System.Drawing.Point(6, 61);
		this.ARButton.Name = "ARButton";
		this.ARButton.Size = new System.Drawing.Size(216, 36);
		this.ARButton.TabIndex = 2;
		this.ARButton.Text = "Автозапуск програм";
		this.ARButton.UseVisualStyleBackColor = true;
		this.ARButton.Click += new System.EventHandler(ARButton_Click);
		this.TMButton.Location = new System.Drawing.Point(230, 19);
		this.TMButton.Name = "TMButton";
		this.TMButton.Size = new System.Drawing.Size(216, 36);
		this.TMButton.TabIndex = 1;
		this.TMButton.Text = "Диспетчер задач";
		this.TMButton.UseVisualStyleBackColor = true;
		this.TMButton.Click += new System.EventHandler(TMButton_Click);
		this.URButton.Location = new System.Drawing.Point(6, 19);
		this.URButton.Name = "URButton";
		this.URButton.Size = new System.Drawing.Size(216, 36);
		this.URButton.TabIndex = 0;
		this.URButton.Text = "Разблокировка ограничений";
		this.URButton.UseVisualStyleBackColor = true;
		this.URButton.Click += new System.EventHandler(URButton_Click);
		this.MenuStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.StatusVersionLabel });
		this.MenuStatus.Location = new System.Drawing.Point(0, 213);
		this.MenuStatus.Name = "MenuStatus";
		this.MenuStatus.Size = new System.Drawing.Size(476, 24);
		this.MenuStatus.SizingGrip = false;
		this.MenuStatus.TabIndex = 2;
		this.StatusVersionLabel.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
		this.StatusVersionLabel.Name = "StatusVersionLabel";
		this.StatusVersionLabel.Size = new System.Drawing.Size(89, 19);
		this.StatusVersionLabel.Text = "Версия: 0.0.0.0";
		this.ProgName.AutoSize = true;
		this.ProgName.Font = new System.Drawing.Font("Segoe UI", 16f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.ProgName.Location = new System.Drawing.Point(153, 9);
		this.ProgName.Name = "ProgName";
		this.ProgName.Size = new System.Drawing.Size(165, 30);
		this.ProgName.TabIndex = 3;
		this.ProgName.Text = "SimpleUnlocker";
		this.checkUpdateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(checkUpdateWorker_DoWork);
		this.updaterUnpacker.DoWork += new System.ComponentModel.DoWorkEventHandler(unpackerWorker_DoWork);
		this.updaterUnpacker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(updaterUnpacker_RunWorkerCompleted);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(476, 237);
		base.Controls.Add(this.ProgName);
		base.Controls.Add(this.MenuStatus);
		base.Controls.Add(this.MenuPanel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "MainMenu";
		base.ShowIcon = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		base.Load += new System.EventHandler(MainMenu_Load);
		this.MenuPanel.ResumeLayout(false);
		this.MenuStatus.ResumeLayout(false);
		this.MenuStatus.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
