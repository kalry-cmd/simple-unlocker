using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SU.Classes;
using SU.Classes.UR;

namespace SU.Forms.UR;

public class AssociationBackup : Form
{
	private string _assocName;

	private IContainer components;

	private Button CancelBtn;

	private Button AcceptBtn;

	private ComboBox FileTypeBox;

	private Label label1;

	private Label label2;

	private Label AssocLabel;

	private TextBox AssocBox;

	public List<FileType> _avaliableTypes { get; }

	public Association _assoc { get; set; }

	public AssociationBackup(List<FileType> aTypes)
	{
		InitializeComponent();
		base.DialogResult = DialogResult.Cancel;
		_avaliableTypes = aTypes;
	}

	private void AssociationBackup_Load(object sender, EventArgs e)
	{
		Utils.SendMessage(AssocBox.Handle, 5377, 1, "Пример: su или .su");
		foreach (FileType avaliableType in _avaliableTypes)
		{
			FileTypeBox.Items.Add(avaliableType.Name);
		}
	}

	private void CancelBtn_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void AcceptBtn_Click(object sender, EventArgs e)
	{
		_assocName = AssocBox.Text;
		if (string.IsNullOrWhiteSpace(_assocName))
		{
			MessageBox.Show("Вы не указали название ассоциации", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (_assocName.Count((char x) => x == '.') > 1)
		{
			MessageBox.Show("Вы превысили лимит точек (Максимальное количество: 1)", "", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			return;
		}
		if (!_assocName.StartsWith("."))
		{
			_assocName = "." + _assocName;
		}
		_assoc = Associations.CreateAssociation(_assocName, _avaliableTypes.Find((FileType t) => t.Name == FileTypeBox.SelectedItem.ToString()), createInRegistry: true);
		base.DialogResult = DialogResult.OK;
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
		this.CancelBtn = new System.Windows.Forms.Button();
		this.AcceptBtn = new System.Windows.Forms.Button();
		this.FileTypeBox = new System.Windows.Forms.ComboBox();
		this.label1 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.AssocLabel = new System.Windows.Forms.Label();
		this.AssocBox = new System.Windows.Forms.TextBox();
		base.SuspendLayout();
		this.CancelBtn.Location = new System.Drawing.Point(325, 75);
		this.CancelBtn.Name = "CancelBtn";
		this.CancelBtn.Size = new System.Drawing.Size(75, 23);
		this.CancelBtn.TabIndex = 13;
		this.CancelBtn.Text = "Отмена";
		this.CancelBtn.UseVisualStyleBackColor = true;
		this.CancelBtn.Click += new System.EventHandler(CancelBtn_Click);
		this.AcceptBtn.Location = new System.Drawing.Point(244, 75);
		this.AcceptBtn.Name = "AcceptBtn";
		this.AcceptBtn.Size = new System.Drawing.Size(75, 23);
		this.AcceptBtn.TabIndex = 12;
		this.AcceptBtn.Text = "ОК";
		this.AcceptBtn.UseVisualStyleBackColor = true;
		this.AcceptBtn.Click += new System.EventHandler(AcceptBtn_Click);
		this.FileTypeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.FileTypeBox.FormattingEnabled = true;
		this.FileTypeBox.Location = new System.Drawing.Point(91, 46);
		this.FileTypeBox.Name = "FileTypeBox";
		this.FileTypeBox.Size = new System.Drawing.Size(309, 21);
		this.FileTypeBox.TabIndex = 11;
		this.label1.AutoSize = true;
		this.label1.Location = new System.Drawing.Point(19, 49);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(64, 13);
		this.label1.TabIndex = 10;
		this.label1.Text = "Тип файла:";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(30, 75);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(0, 13);
		this.label2.TabIndex = 9;
		this.AssocLabel.AutoSize = true;
		this.AssocLabel.Location = new System.Drawing.Point(12, 22);
		this.AssocLabel.Name = "AssocLabel";
		this.AssocLabel.Size = new System.Drawing.Size(71, 13);
		this.AssocLabel.TabIndex = 7;
		this.AssocLabel.Text = "Ассоциация:";
		this.AssocBox.Location = new System.Drawing.Point(91, 20);
		this.AssocBox.Name = "AssocBox";
		this.AssocBox.Size = new System.Drawing.Size(309, 20);
		this.AssocBox.TabIndex = 14;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(413, 112);
		base.Controls.Add(this.AssocBox);
		base.Controls.Add(this.CancelBtn);
		base.Controls.Add(this.AcceptBtn);
		base.Controls.Add(this.FileTypeBox);
		base.Controls.Add(this.label1);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.AssocLabel);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "AssociationBackup";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		base.Load += new System.EventHandler(AssociationBackup_Load);
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
