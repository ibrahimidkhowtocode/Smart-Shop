namespace Smart_Shop
{
    partial class AdminForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSummary = new System.Windows.Forms.TabPage();
            this.tabBusiness = new System.Windows.Forms.TabPage();
            this.tabExpenses = new System.Windows.Forms.TabPage();
            this.tabNotes = new System.Windows.Forms.TabPage();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabSummary);
            this.tabControl1.Controls.Add(this.tabBusiness);
            this.tabControl1.Controls.Add(this.tabExpenses);
            this.tabControl1.Controls.Add(this.tabNotes);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(800, 450);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSummary
            // 
            this.tabSummary.Location = new System.Drawing.Point(4, 22);
            this.tabSummary.Name = "tabSummary";
            this.tabSummary.Padding = new System.Windows.Forms.Padding(3);
            this.tabSummary.Size = new System.Drawing.Size(792, 424);
            this.tabSummary.TabIndex = 0;
            this.tabSummary.Text = "Summary";
            this.tabSummary.UseVisualStyleBackColor = true;
            // 
            // tabBusiness
            // 
            this.tabBusiness.Location = new System.Drawing.Point(4, 22);
            this.tabBusiness.Name = "tabBusiness";
            this.tabBusiness.Padding = new System.Windows.Forms.Padding(3);
            this.tabBusiness.Size = new System.Drawing.Size(792, 424);
            this.tabBusiness.TabIndex = 1;
            this.tabBusiness.Text = "Business";
            this.tabBusiness.UseVisualStyleBackColor = true;
            // 
            // tabExpenses
            // 
            this.tabExpenses.Location = new System.Drawing.Point(4, 22);
            this.tabExpenses.Name = "tabExpenses";
            this.tabExpenses.Size = new System.Drawing.Size(792, 424);
            this.tabExpenses.TabIndex = 2;
            this.tabExpenses.Text = "Expenses";
            this.tabExpenses.UseVisualStyleBackColor = true;
            // 
            // tabNotes
            // 
            this.tabNotes.Location = new System.Drawing.Point(4, 22);
            this.tabNotes.Name = "tabNotes";
            this.tabNotes.Size = new System.Drawing.Size(792, 424);
            this.tabNotes.TabIndex = 3;
            this.tabNotes.Text = "Cashier Notes";
            this.tabNotes.UseVisualStyleBackColor = true;
            // 
            // btnLogout
            // 
            this.btnLogout.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnLogout.Location = new System.Drawing.Point(50, 470);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(150, 40);
            this.btnLogout.TabIndex = 1;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Location = new System.Drawing.Point(220, 470);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(150, 40);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // AdminForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 530);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "AdminForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Smart Shop - Admin Dashboard";
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSummary;
        private System.Windows.Forms.TabPage tabBusiness;
        private System.Windows.Forms.TabPage tabExpenses;
        private System.Windows.Forms.TabPage tabNotes;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnExit;
    }
}