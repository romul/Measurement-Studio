namespace MeasurementStudio
{
    partial class ChoiceOfConfiguration
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.bNext = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbModes = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.bEdit = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbProfiles = new System.Windows.Forms.ComboBox();
            this.bOk = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.bNext);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cbModes);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(292, 146);
            this.panel1.TabIndex = 0;
            // 
            // bNext
            // 
            this.bNext.Location = new System.Drawing.Point(178, 97);
            this.bNext.Name = "bNext";
            this.bNext.Size = new System.Drawing.Size(75, 23);
            this.bNext.TabIndex = 2;
            this.bNext.Text = "Далее >>";
            this.bNext.UseVisualStyleBackColor = true;
            this.bNext.Click += new System.EventHandler(this.bNext_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(135, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Веберите конфигурацию:";
            // 
            // cbModes
            // 
            this.cbModes.FormattingEnabled = true;
            this.cbModes.Location = new System.Drawing.Point(31, 47);
            this.cbModes.Name = "cbModes";
            this.cbModes.Size = new System.Drawing.Size(222, 21);
            this.cbModes.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.bEdit);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cbProfiles);
            this.panel2.Controls.Add(this.bOk);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(292, 146);
            this.panel2.TabIndex = 3;
            // 
            // bEdit
            // 
            this.bEdit.Image = global::MeasurementStudio.Properties.Resources.pencil1;
            this.bEdit.Location = new System.Drawing.Point(253, 47);
            this.bEdit.Name = "bEdit";
            this.bEdit.Size = new System.Drawing.Size(25, 23);
            this.bEdit.TabIndex = 3;
            this.bEdit.UseVisualStyleBackColor = true;
            this.bEdit.Click += new System.EventHandler(this.bEdit_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(107, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Выберите профиль:";
            // 
            // cbProfiles
            // 
            this.cbProfiles.FormattingEnabled = true;
            this.cbProfiles.Location = new System.Drawing.Point(31, 47);
            this.cbProfiles.Name = "cbProfiles";
            this.cbProfiles.Size = new System.Drawing.Size(216, 21);
            this.cbProfiles.TabIndex = 1;
            this.cbProfiles.SelectedIndexChanged += new System.EventHandler(this.cbProfiles_SelectedIndexChanged);
            // 
            // bOk
            // 
            this.bOk.Location = new System.Drawing.Point(203, 97);
            this.bOk.Name = "bOk";
            this.bOk.Size = new System.Drawing.Size(75, 23);
            this.bOk.TabIndex = 0;
            this.bOk.Text = "OK";
            this.bOk.UseVisualStyleBackColor = true;
            this.bOk.Click += new System.EventHandler(this.bOk_Click);
            // 
            // ChoiceOfConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 146);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ChoiceOfConfiguration";
            this.Text = "ChoiceOfConfiguration";
            this.Load += new System.EventHandler(this.ChoiceOfConfiguration_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChoiceOfConfiguration_FormClosing);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bNext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbModes;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button bEdit;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbProfiles;
        private System.Windows.Forms.Button bOk;
    }
}