namespace FritzBoxDial
{
    partial class MyDndForm
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
            this.DnDPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.DnDPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // DnDPanel
            // 
            this.DnDPanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.DnDPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DnDPanel.Controls.Add(this.label1);
            this.DnDPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DnDPanel.Location = new System.Drawing.Point(0, 0);
            this.DnDPanel.Name = "DnDPanel";
            this.DnDPanel.Size = new System.Drawing.Size(284, 262);
            this.DnDPanel.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(282, 260);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ziehen Sie die Kontakte hier hinein...";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MyDndForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.DnDPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "MyDndForm";
            this.Text = "Form1";
            this.Shown += new System.EventHandler(this.MyDnDForm_Shown);
            this.DnDPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel DnDPanel;
        private System.Windows.Forms.Label label1;
    }
}

