using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Ionic.Zip;
using SU.Classes;
using SU.Properties;

namespace SU.Forms;

public class About : Form
{
	private IContainer components;

	private Label label1;

	private Button authorBtn;

	private Button chkUpdateBtn;

	private Label verLabel;

	private PictureBox pictureBox1;

	public About()
	{
		InitializeComponent();
		verLabel.Text = "Версия: " + Application.ProductVersion;
		base.TopMost = SU.Properties.Settings.Default.AlwaysOnTop;
	}

	private void authorBtn_Click(object sender, EventArgs e)
	{
		Process.Start("https://ds1nc.ru");
	}

	private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
	{
		if (!Directory.Exists("temp"))
		{
			Directory.CreateDirectory("temp");
		}
		File.Move("temp\\updater.temp", "temp\\updater.zip");
		Console.WriteLine("[INFO] Распаковка апдейтера");
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
			MessageBox.Show(ex.ToString());
		}
		Console.WriteLine("[INFO] Запуск процесса обновления");
		Version arg = new Version(Application.ProductVersion);
		File.Delete("temp\\updater.zip");
		Utils.RunFile("bin\\su_updater.exe", $"{arg} {Path.GetFileName(Application.ExecutablePath)}", UAC: true, Hidden: false, WaitForExit: false);
		Environment.Exit(1);
	}

	private void chkUpdateBtn_Click(object sender, EventArgs e)
	{
		Utils.CheckUpdate(this, DownloadFileCompleted, InformateUser: true);
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
            this.label1 = new System.Windows.Forms.Label();
            this.authorBtn = new System.Windows.Forms.Button();
            this.chkUpdateBtn = new System.Windows.Forms.Button();
            this.verLabel = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(95, 15);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(254, 46);
            this.label1.TabIndex = 6;
            this.label1.Text = "SimpleUnlocker";
            // 
            // authorBtn
            // 
            this.authorBtn.ForeColor = System.Drawing.Color.Black;
            this.authorBtn.Location = new System.Drawing.Point(16, 87);
            this.authorBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.authorBtn.Name = "authorBtn";
            this.authorBtn.Size = new System.Drawing.Size(113, 28);
            this.authorBtn.TabIndex = 7;
            this.authorBtn.Text = "Сайт автора";
            this.authorBtn.UseVisualStyleBackColor = true;
            this.authorBtn.Click += new System.EventHandler(this.authorBtn_Click);
            // 
            // chkUpdateBtn
            // 
            this.chkUpdateBtn.ForeColor = System.Drawing.Color.Black;
            this.chkUpdateBtn.Location = new System.Drawing.Point(137, 87);
            this.chkUpdateBtn.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkUpdateBtn.Name = "chkUpdateBtn";
            this.chkUpdateBtn.Size = new System.Drawing.Size(247, 28);
            this.chkUpdateBtn.TabIndex = 8;
            this.chkUpdateBtn.Text = "Проверка обновления";
            this.chkUpdateBtn.UseVisualStyleBackColor = true;
            this.chkUpdateBtn.Click += new System.EventHandler(this.chkUpdateBtn_Click);
            // 
            // verLabel
            // 
            this.verLabel.AutoSize = true;
            this.verLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.verLabel.ForeColor = System.Drawing.Color.Black;
            this.verLabel.Location = new System.Drawing.Point(99, 54);
            this.verLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.verLabel.Name = "verLabel";
            this.verLabel.Size = new System.Drawing.Size(168, 28);
            this.verLabel.TabIndex = 9;
            this.verLabel.Text = "Версия: Unknown";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SU_Properties_Resources.SULogo;
            this.pictureBox1.Location = new System.Drawing.Point(16, 15);
            this.pictureBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(71, 65);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(395, 123);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.verLabel);
            this.Controls.Add(this.chkUpdateBtn);
            this.Controls.Add(this.authorBtn);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "About";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

	}
}
