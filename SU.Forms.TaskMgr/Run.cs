using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SU.Properties;

namespace SU.Forms.TaskMgr;

public class Run : Form
{
	private IContainer components;

	private Label label1;

	private Label label2;

	private Label label3;

	private TextBox ExecBox;

	private Button viewBtn;

	private Button cancelBtn;

	private Button doneBtn;

	private OpenFileDialog ofdRun;

	private PictureBox runBox;

	public Run()
	{
		InitializeComponent();
	}

	private void StartFile()
	{
		Hide();
		try
		{
			Process.Start(ExecBox.Text);
		}
		catch
		{
			MessageBox.Show("Не удалось найти \"" + ExecBox.Text + "\". Проверьте правильность введенного имени и попробуйте еще раз.", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			Show();
			return;
		}
		Close();
	}

	private void cancelBtn_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void viewBtn_Click(object sender, EventArgs e)
	{
		if (ofdRun.ShowDialog() == DialogResult.OK)
		{
			ExecBox.Text = ofdRun.FileName;
		}
	}

	private void doneBtn_Click(object sender, EventArgs e)
	{
		StartFile();
	}

	private void ExecBox_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == Convert.ToChar(Keys.Return))
		{
			StartFile();
		}
	}

	private void ExecBox_TextChanged(object sender, EventArgs e)
	{
		doneBtn.Enabled = !string.IsNullOrWhiteSpace(ExecBox.Text);
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
		this.label2 = new System.Windows.Forms.Label();
		this.label3 = new System.Windows.Forms.Label();
		this.ExecBox = new System.Windows.Forms.TextBox();
		this.viewBtn = new System.Windows.Forms.Button();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.doneBtn = new System.Windows.Forms.Button();
		this.ofdRun = new System.Windows.Forms.OpenFileDialog();
		this.runBox = new System.Windows.Forms.PictureBox();
		((System.ComponentModel.ISupportInitialize)this.runBox).BeginInit();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label1.Location = new System.Drawing.Point(90, 28);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(333, 15);
		this.label1.TabIndex = 0;
		this.label1.Text = "Введите имя программы, папки, документа или ресурса";
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label2.Location = new System.Drawing.Point(90, 43);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(247, 15);
		this.label2.TabIndex = 1;
		this.label2.Text = "Интернета, которые требуется открыть.";
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 204);
		this.label3.Location = new System.Drawing.Point(12, 76);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(62, 15);
		this.label3.TabIndex = 2;
		this.label3.Text = "Открыть:";
		this.ExecBox.Location = new System.Drawing.Point(93, 75);
		this.ExecBox.Name = "ExecBox";
		this.ExecBox.Size = new System.Drawing.Size(330, 20);
		this.ExecBox.TabIndex = 3;
		this.ExecBox.TextChanged += new System.EventHandler(ExecBox_TextChanged);
		this.ExecBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(ExecBox_KeyPress);
		this.viewBtn.Location = new System.Drawing.Point(348, 121);
		this.viewBtn.Name = "viewBtn";
		this.viewBtn.Size = new System.Drawing.Size(75, 23);
		this.viewBtn.TabIndex = 4;
		this.viewBtn.Text = "Обзор...";
		this.viewBtn.UseVisualStyleBackColor = true;
		this.viewBtn.Click += new System.EventHandler(viewBtn_Click);
		this.cancelBtn.Location = new System.Drawing.Point(267, 121);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(75, 23);
		this.cancelBtn.TabIndex = 5;
		this.cancelBtn.Text = "Отмена";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.doneBtn.Enabled = false;
		this.doneBtn.Location = new System.Drawing.Point(186, 121);
		this.doneBtn.Name = "doneBtn";
		this.doneBtn.Size = new System.Drawing.Size(75, 23);
		this.doneBtn.TabIndex = 6;
		this.doneBtn.Text = "Выполнить";
		this.doneBtn.UseVisualStyleBackColor = true;
		this.doneBtn.Click += new System.EventHandler(doneBtn_Click);
		this.ofdRun.Filter = "Приложения| *.exe *.bat *.cmd *.vbs|Все файлы|*.*";
		this.ofdRun.RestoreDirectory = true;
		this.runBox.Image = SU.Properties.Resources.runIcon;
		this.runBox.Location = new System.Drawing.Point(15, 22);
		this.runBox.Name = "runBox";
		this.runBox.Size = new System.Drawing.Size(59, 45);
		this.runBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
		this.runBox.TabIndex = 7;
		this.runBox.TabStop = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(435, 156);
		base.Controls.Add(this.runBox);
		base.Controls.Add(this.doneBtn);
		base.Controls.Add(this.cancelBtn);
		base.Controls.Add(this.viewBtn);
		base.Controls.Add(this.ExecBox);
		base.Controls.Add(this.label3);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "Run";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		((System.ComponentModel.ISupportInitialize)this.runBox).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
