using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SU.Classes.UR;

namespace SU.Forms.UR;

public class AssociationEdit : Form
{
	private IContainer components;

	private Label AssocLabel;

	private TextBox AssocBox;

	private Label label2;

	private Label label1;

	private ComboBox FileTypeBox;

	private Button AcceptBtn;

	private Button CancelBtn;

	public Association _assoc { get; set; }

	public List<FileType> _avaliableTypes { get; }

	public AssociationEdit(Association assoc, List<FileType> avaliableTypes)
	{
		InitializeComponent();
		base.DialogResult = DialogResult.Cancel;
		_assoc = assoc;
		_avaliableTypes = avaliableTypes;
	}

	private void AssociationEdit_Load(object sender, EventArgs e)
	{
		foreach (FileType avaliableType in _avaliableTypes)
		{
			FileTypeBox.Items.Add(avaliableType.Name);
		}
		Console.WriteLine(_assoc.Type?.ShellOpenCommand?.DefaultCommand);
		FileTypeBox.SelectedItem = _assoc.Type.Name;
		AssocBox.Text = _assoc.Name;
	}

	private void AcceptBtn_Click(object sender, EventArgs e)
	{
		FileType fileType = _avaliableTypes.Find((FileType t) => t.Name == FileTypeBox.SelectedItem.ToString());
		Associations.UpdateAssocFileType(_assoc, fileType);
		_assoc.Type = fileType;
		base.DialogResult = DialogResult.OK;
		Close();
	}

	private void CancelBtn_Click(object sender, EventArgs e)
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
		this.AssocLabel = new System.Windows.Forms.Label();
		this.AssocBox = new System.Windows.Forms.TextBox();
		this.label2 = new System.Windows.Forms.Label();
		this.label1 = new System.Windows.Forms.Label();
		this.FileTypeBox = new System.Windows.Forms.ComboBox();
		this.AcceptBtn = new System.Windows.Forms.Button();
		this.CancelBtn = new System.Windows.Forms.Button();
		base.SuspendLayout();
		this.AssocLabel.AutoSize = true;
		this.AssocLabel.Location = new System.Drawing.Point(12, 28);
		this.AssocLabel.Name = "AssocLabel";
		this.AssocLabel.Size = new System.Drawing.Size(71, 13);
		this.AssocLabel.TabIndex = 0;
		this.AssocLabel.Text = "Ассоциация:";
		this.AssocBox.BackColor = System.Drawing.SystemColors.Control;
		this.AssocBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
		this.AssocBox.Location = new System.Drawing.Point(89, 28);
		this.AssocBox.Name = "AssocBox";
		this.AssocBox.ReadOnly = true;
		this.AssocBox.Size = new System.Drawing.Size(309, 13);
		this.AssocBox.TabIndex = 4;
		this.AssocBox.TabStop = false;
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(28, 83);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(0, 13);
		this.label2.TabIndex = 2;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(19, 52);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(64, 13);
		this.label1.TabIndex = 3;
		this.label1.Text = "Тип файла:";
		this.FileTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.FileTypeBox.FormattingEnabled = true;
		this.FileTypeBox.Location = new System.Drawing.Point(89, 49);
		this.FileTypeBox.Name = "FileTypeBox";
		this.FileTypeBox.Size = new System.Drawing.Size(309, 21);
		this.FileTypeBox.TabIndex = 4;
		this.AcceptBtn.Location = new System.Drawing.Point(242, 83);
		this.AcceptBtn.Name = "AcceptBtn";
		this.AcceptBtn.Size = new System.Drawing.Size(75, 23);
		this.AcceptBtn.TabIndex = 5;
		this.AcceptBtn.Text = "ОК";
		this.AcceptBtn.UseVisualStyleBackColor = true;
		this.AcceptBtn.Click += new System.EventHandler(AcceptBtn_Click);
		this.CancelBtn.Location = new System.Drawing.Point(323, 83);
		this.CancelBtn.Name = "CancelBtn";
		this.CancelBtn.Size = new System.Drawing.Size(75, 23);
		this.CancelBtn.TabIndex = 6;
		this.CancelBtn.Text = "Отмена";
		this.CancelBtn.UseVisualStyleBackColor = true;
		this.CancelBtn.Click += new System.EventHandler(CancelBtn_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(410, 121);
		base.Controls.Add(this.CancelBtn);
		base.Controls.Add(this.AcceptBtn);
		base.Controls.Add(this.FileTypeBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.AssocBox);
		base.Controls.Add(this.AssocLabel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AssociationEdit";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.Load += new System.EventHandler(AssociationEdit_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
