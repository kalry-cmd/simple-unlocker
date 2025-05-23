using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SU.Classes;
using SU.Classes.UR;

namespace SU.Forms.UR;

public class FileTypeBackup : Form
{
	private IContainer components;

	private Label label1;

	private TextBox nameBox;

	private TabControl runControl;

	private TabPage openTab;

	private TextBox delegateOpenBox;

	private Label label4;

	private TextBox isolateOpenBox;

	private Label label3;

	private TextBox openDefaultBox;

	private Label label2;

	private TabPage runAsTab;

	private TabPage runAsUserTab;

	private TextBox delegateRunAsBox;

	private Label label5;

	private TextBox isolateRunAsBox;

	private Label label6;

	private TextBox defaultRunAsBox;

	private Label label7;

	private TextBox delegateRunAsUserBox;

	private Label label8;

	private TextBox isolateRunAsUserBox;

	private Label label9;

	private TextBox defaultRunAsUserBox;

	private Label label10;

	private Button cancelBtn;

	private Button acceptBtn;

	private Label label11;

	private ComboBox copyFileTypeBox;

	private TextBox descriptionBox;

	private Label label12;

	private Label label13;

	private TextBox defaultIconBox;

	public List<FileType> _fileTypes { get; }

	public FileType _createdFileType { get; set; }

	public FileTypeBackup(List<FileType> fileTypes)
	{
		InitializeComponent();
		List<TextBox> obj = new List<TextBox> { openDefaultBox, defaultRunAsBox, isolateOpenBox, isolateRunAsBox, isolateRunAsUserBox, defaultRunAsUserBox };
		List<TextBox> list = new List<TextBox> { delegateOpenBox, delegateRunAsUserBox, delegateRunAsBox };
		Utils.SendMessage(nameBox.Handle, 5377, 1, "Пример: CoolFileType");
		Utils.SendMessage(descriptionBox.Handle, 5377, 1, "Пример: Cool FileType Really");
		Utils.SendMessage(defaultIconBox.Handle, 5377, 1, "Пример: C:\\Windows\\system32\\imageres.dll,-67");
		foreach (TextBox item in obj)
		{
			Utils.SendMessage(item.Handle, 5377, 1, "Пример: %1 %");
		}
		foreach (TextBox item2 in list)
		{
			Utils.SendMessage(item2.Handle, 5377, 1, "Пример: {ea72d00e-4960-42fa-ba92-7792a7944c1d}");
		}
		base.DialogResult = DialogResult.Cancel;
		_fileTypes = fileTypes;
	}

	private void acceptBtn_Click(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(nameBox.Text))
		{
			MessageBox.Show("Вы не указали название типа файла", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (nameBox.Text.StartsWith("."))
		{
			MessageBox.Show("Тип файла не может начинатся с точки", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		FileCommand open = Associations.CreateFileCommand("open", openDefaultBox.Text, isolateOpenBox.Text, delegateOpenBox.Text);
		FileCommand runas = Associations.CreateFileCommand("runas", defaultRunAsBox.Text, isolateRunAsBox.Text, delegateRunAsBox.Text);
		FileCommand runasuser = Associations.CreateFileCommand("runasuser", defaultRunAsUserBox.Text, isolateRunAsUserBox.Text, delegateRunAsUserBox.Text);
		_createdFileType = Associations.CreateFileType(nameBox.Text, descriptionBox.Text, defaultIconBox.Text, open, runas, runasuser);
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void FileTypeBackup_Load(object sender, EventArgs e)
	{
		foreach (FileType fileType in _fileTypes)
		{
			copyFileTypeBox.Items.Add(fileType.Name);
		}
	}

	private void copyFileTypeBox_SelectedIndexChanged(object sender, EventArgs e)
	{
		FileType fileType = _fileTypes.Find((FileType t) => t.Name == copyFileTypeBox.SelectedItem.ToString());
		descriptionBox.Text = fileType?.Description;
		openDefaultBox.Text = fileType?.ShellOpenCommand?.DefaultCommand;
		isolateOpenBox.Text = fileType?.ShellOpenCommand?.IsolatedCommand;
		delegateOpenBox.Text = fileType?.ShellOpenCommand?.DelegateExecute;
		defaultRunAsBox.Text = fileType?.ShellRunAsCommand?.DefaultCommand;
		isolateRunAsBox.Text = fileType?.ShellRunAsCommand?.IsolatedCommand;
		delegateRunAsBox.Text = fileType?.ShellRunAsCommand?.DelegateExecute;
		defaultRunAsUserBox.Text = fileType?.ShellRunAsUserCommand?.DefaultCommand;
		isolateRunAsUserBox.Text = fileType?.ShellRunAsUserCommand?.IsolatedCommand;
		delegateRunAsUserBox.Text = fileType?.ShellRunAsUserCommand?.DelegateExecute;
		defaultIconBox.Text = fileType?.DefaultIcon;
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
		this.label1 = new System.Windows.Forms.Label();
		this.nameBox = new System.Windows.Forms.TextBox();
		this.runControl = new System.Windows.Forms.TabControl();
		this.openTab = new System.Windows.Forms.TabPage();
		this.delegateOpenBox = new System.Windows.Forms.TextBox();
		this.label4 = new System.Windows.Forms.Label();
		this.isolateOpenBox = new System.Windows.Forms.TextBox();
		this.label3 = new System.Windows.Forms.Label();
		this.openDefaultBox = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.runAsTab = new System.Windows.Forms.TabPage();
		this.delegateRunAsBox = new System.Windows.Forms.TextBox();
		this.label5 = new System.Windows.Forms.Label();
		this.isolateRunAsBox = new System.Windows.Forms.TextBox();
		this.label6 = new System.Windows.Forms.Label();
		this.defaultRunAsBox = new System.Windows.Forms.TextBox();
		this.label7 = new System.Windows.Forms.Label();
		this.runAsUserTab = new System.Windows.Forms.TabPage();
		this.delegateRunAsUserBox = new System.Windows.Forms.TextBox();
		this.label8 = new System.Windows.Forms.Label();
		this.isolateRunAsUserBox = new System.Windows.Forms.TextBox();
		this.label9 = new System.Windows.Forms.Label();
		this.defaultRunAsUserBox = new System.Windows.Forms.TextBox();
		this.label10 = new System.Windows.Forms.Label();
		this.cancelBtn = new System.Windows.Forms.Button();
		this.acceptBtn = new System.Windows.Forms.Button();
		this.label11 = new System.Windows.Forms.Label();
		this.copyFileTypeBox = new System.Windows.Forms.ComboBox();
		this.descriptionBox = new System.Windows.Forms.TextBox();
		this.label12 = new System.Windows.Forms.Label();
		this.label13 = new System.Windows.Forms.Label();
		this.defaultIconBox = new System.Windows.Forms.TextBox();
		this.runControl.SuspendLayout();
		this.openTab.SuspendLayout();
		this.runAsTab.SuspendLayout();
		this.runAsUserTab.SuspendLayout();
		base.SuspendLayout();
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(40, 18);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(60, 13);
		this.label1.TabIndex = 0;
		this.label1.Text = "Название:";
		this.nameBox.Location = new System.Drawing.Point(106, 15);
		this.nameBox.Name = "nameBox";
		this.nameBox.Size = new System.Drawing.Size(468, 20);
		this.nameBox.TabIndex = 1;
		this.runControl.Controls.Add(this.openTab);
		this.runControl.Controls.Add(this.runAsTab);
		this.runControl.Controls.Add(this.runAsUserTab);
		this.runControl.Location = new System.Drawing.Point(16, 94);
		this.runControl.Name = "runControl";
		this.runControl.SelectedIndex = 0;
		this.runControl.Size = new System.Drawing.Size(562, 166);
		this.runControl.TabIndex = 2;
		this.openTab.Controls.Add(this.delegateOpenBox);
		this.openTab.Controls.Add(this.label4);
		this.openTab.Controls.Add(this.isolateOpenBox);
		this.openTab.Controls.Add(this.label3);
		this.openTab.Controls.Add(this.openDefaultBox);
		this.openTab.Controls.Add(this.label2);
		this.openTab.Location = new System.Drawing.Point(4, 22);
		this.openTab.Name = "openTab";
		this.openTab.Padding = new System.Windows.Forms.Padding(3);
		this.openTab.Size = new System.Drawing.Size(554, 140);
		this.openTab.TabIndex = 0;
		this.openTab.Text = "Open";
		this.openTab.UseVisualStyleBackColor = true;
		this.delegateOpenBox.Location = new System.Drawing.Point(11, 106);
		this.delegateOpenBox.Name = "delegateOpenBox";
		this.delegateOpenBox.Size = new System.Drawing.Size(528, 20);
		this.delegateOpenBox.TabIndex = 5;
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(8, 90);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(158, 13);
		this.label4.TabIndex = 4;
		this.label4.Text = "Делегированное выполнение";
		this.isolateOpenBox.Location = new System.Drawing.Point(11, 67);
		this.isolateOpenBox.Name = "isolateOpenBox";
		this.isolateOpenBox.Size = new System.Drawing.Size(528, 20);
		this.isolateOpenBox.TabIndex = 3;
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(8, 51);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(134, 13);
		this.label3.TabIndex = 2;
		this.label3.Text = "Изолированная команда";
		this.openDefaultBox.Location = new System.Drawing.Point(11, 28);
		this.openDefaultBox.Name = "openDefaultBox";
		this.openDefaultBox.Size = new System.Drawing.Size(528, 20);
		this.openDefaultBox.TabIndex = 1;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(8, 12);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(80, 13);
		this.label2.TabIndex = 0;
		this.label2.Text = "По умолчанию";
		this.runAsTab.Controls.Add(this.delegateRunAsBox);
		this.runAsTab.Controls.Add(this.label5);
		this.runAsTab.Controls.Add(this.isolateRunAsBox);
		this.runAsTab.Controls.Add(this.label6);
		this.runAsTab.Controls.Add(this.defaultRunAsBox);
		this.runAsTab.Controls.Add(this.label7);
		this.runAsTab.Location = new System.Drawing.Point(4, 22);
		this.runAsTab.Name = "runAsTab";
		this.runAsTab.Padding = new System.Windows.Forms.Padding(3);
		this.runAsTab.Size = new System.Drawing.Size(554, 140);
		this.runAsTab.TabIndex = 1;
		this.runAsTab.Text = "Runas";
		this.runAsTab.UseVisualStyleBackColor = true;
		this.delegateRunAsBox.Location = new System.Drawing.Point(11, 106);
		this.delegateRunAsBox.Name = "delegateRunAsBox";
		this.delegateRunAsBox.Size = new System.Drawing.Size(528, 20);
		this.delegateRunAsBox.TabIndex = 11;
		this.label5.AutoSize = true;
		this.label5.Location = new System.Drawing.Point(8, 90);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(158, 13);
		this.label5.TabIndex = 10;
		this.label5.Text = "Делегированное выполнение";
		this.isolateRunAsBox.Location = new System.Drawing.Point(11, 67);
		this.isolateRunAsBox.Name = "isolateRunAsBox";
		this.isolateRunAsBox.Size = new System.Drawing.Size(528, 20);
		this.isolateRunAsBox.TabIndex = 9;
		this.label6.AutoSize = true;
		this.label6.Location = new System.Drawing.Point(8, 51);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(134, 13);
		this.label6.TabIndex = 8;
		this.label6.Text = "Изолированная команда";
		this.defaultRunAsBox.Location = new System.Drawing.Point(11, 28);
		this.defaultRunAsBox.Name = "defaultRunAsBox";
		this.defaultRunAsBox.Size = new System.Drawing.Size(528, 20);
		this.defaultRunAsBox.TabIndex = 7;
		this.label7.AutoSize = true;
		this.label7.Location = new System.Drawing.Point(8, 12);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(80, 13);
		this.label7.TabIndex = 6;
		this.label7.Text = "По умолчанию";
		this.runAsUserTab.Controls.Add(this.delegateRunAsUserBox);
		this.runAsUserTab.Controls.Add(this.label8);
		this.runAsUserTab.Controls.Add(this.isolateRunAsUserBox);
		this.runAsUserTab.Controls.Add(this.label9);
		this.runAsUserTab.Controls.Add(this.defaultRunAsUserBox);
		this.runAsUserTab.Controls.Add(this.label10);
		this.runAsUserTab.Location = new System.Drawing.Point(4, 22);
		this.runAsUserTab.Name = "runAsUserTab";
		this.runAsUserTab.Size = new System.Drawing.Size(554, 140);
		this.runAsUserTab.TabIndex = 2;
		this.runAsUserTab.Text = "RunasUser";
		this.runAsUserTab.UseVisualStyleBackColor = true;
		this.delegateRunAsUserBox.Location = new System.Drawing.Point(11, 106);
		this.delegateRunAsUserBox.Name = "delegateRunAsUserBox";
		this.delegateRunAsUserBox.Size = new System.Drawing.Size(528, 20);
		this.delegateRunAsUserBox.TabIndex = 11;
		this.label8.AutoSize = true;
		this.label8.Location = new System.Drawing.Point(8, 90);
		this.label8.Name = "label8";
		this.label8.Size = new System.Drawing.Size(158, 13);
		this.label8.TabIndex = 10;
		this.label8.Text = "Делегированное выполнение";
		this.isolateRunAsUserBox.Location = new System.Drawing.Point(11, 67);
		this.isolateRunAsUserBox.Name = "isolateRunAsUserBox";
		this.isolateRunAsUserBox.Size = new System.Drawing.Size(528, 20);
		this.isolateRunAsUserBox.TabIndex = 9;
		this.label9.AutoSize = true;
		this.label9.Location = new System.Drawing.Point(8, 51);
		this.label9.Name = "label9";
		this.label9.Size = new System.Drawing.Size(134, 13);
		this.label9.TabIndex = 8;
		this.label9.Text = "Изолированная команда";
		this.defaultRunAsUserBox.Location = new System.Drawing.Point(11, 28);
		this.defaultRunAsUserBox.Name = "defaultRunAsUserBox";
		this.defaultRunAsUserBox.Size = new System.Drawing.Size(528, 20);
		this.defaultRunAsUserBox.TabIndex = 7;
		this.label10.AutoSize = true;
		this.label10.Location = new System.Drawing.Point(8, 12);
		this.label10.Name = "label10";
		this.label10.Size = new System.Drawing.Size(80, 13);
		this.label10.TabIndex = 6;
		this.label10.Text = "По умолчанию";
		this.cancelBtn.Location = new System.Drawing.Point(499, 262);
		this.cancelBtn.Name = "cancelBtn";
		this.cancelBtn.Size = new System.Drawing.Size(75, 23);
		this.cancelBtn.TabIndex = 3;
		this.cancelBtn.Text = "Отмена";
		this.cancelBtn.UseVisualStyleBackColor = true;
		this.cancelBtn.Click += new System.EventHandler(cancelBtn_Click);
		this.acceptBtn.Location = new System.Drawing.Point(418, 262);
		this.acceptBtn.Name = "acceptBtn";
		this.acceptBtn.Size = new System.Drawing.Size(75, 23);
		this.acceptBtn.TabIndex = 4;
		this.acceptBtn.Text = "ОК";
		this.acceptBtn.UseVisualStyleBackColor = true;
		this.acceptBtn.Click += new System.EventHandler(acceptBtn_Click);
		this.label11.AutoSize = true;
		this.label11.Location = new System.Drawing.Point(13, 267);
		this.label11.Name = "label11";
		this.label11.Size = new System.Drawing.Size(91, 13);
		this.label11.TabIndex = 5;
		this.label11.Text = "Скопировать из:";
		this.copyFileTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.copyFileTypeBox.FormattingEnabled = true;
		this.copyFileTypeBox.Location = new System.Drawing.Point(110, 264);
		this.copyFileTypeBox.Name = "copyFileTypeBox";
		this.copyFileTypeBox.Size = new System.Drawing.Size(277, 21);
		this.copyFileTypeBox.TabIndex = 6;
		this.copyFileTypeBox.SelectedIndexChanged += new System.EventHandler(copyFileTypeBox_SelectedIndexChanged);
		this.descriptionBox.Location = new System.Drawing.Point(106, 41);
		this.descriptionBox.Name = "descriptionBox";
		this.descriptionBox.Size = new System.Drawing.Size(468, 20);
		this.descriptionBox.TabIndex = 8;
		this.label12.AutoSize = true;
		this.label12.Location = new System.Drawing.Point(40, 44);
		this.label12.Name = "label12";
		this.label12.Size = new System.Drawing.Size(60, 13);
		this.label12.TabIndex = 7;
		this.label12.Text = "Описание:";
		this.label13.AutoSize = true;
		this.label13.Location = new System.Drawing.Point(52, 70);
		this.label13.Name = "label13";
		this.label13.Size = new System.Drawing.Size(48, 13);
		this.label13.TabIndex = 9;
		this.label13.Text = "Иконка:";
		this.defaultIconBox.Location = new System.Drawing.Point(106, 67);
		this.defaultIconBox.Name = "defaultIconBox";
		this.defaultIconBox.Size = new System.Drawing.Size(468, 20);
		this.defaultIconBox.TabIndex = 10;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(586, 295);
		base.Controls.Add(this.defaultIconBox);
		base.Controls.Add(this.label13);
		base.Controls.Add(this.descriptionBox);
		base.Controls.Add(this.label12);
		base.Controls.Add(this.copyFileTypeBox);
		base.Controls.Add(this.label11);
		base.Controls.Add(this.acceptBtn);
		base.Controls.Add(this.cancelBtn);
		base.Controls.Add(this.runControl);
		base.Controls.Add(this.nameBox);
		base.Controls.Add(this.label1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "FileTypeBackup";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.Load += new System.EventHandler(FileTypeBackup_Load);
		this.runControl.ResumeLayout(false);
		this.openTab.ResumeLayout(false);
		this.openTab.PerformLayout();
		this.runAsTab.ResumeLayout(false);
		this.runAsTab.PerformLayout();
		this.runAsUserTab.ResumeLayout(false);
		this.runAsUserTab.PerformLayout();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
