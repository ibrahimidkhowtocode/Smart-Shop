namespace Smart_Shop
{
    partial class AddEditProductForm
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

        private void InitializeComponent()
        {
            this.txtBarcode = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.numPurchasePrice = new System.Windows.Forms.NumericUpDown();
            this.numSalePrice = new System.Windows.Forms.NumericUpDown();
            this.numQuantity = new System.Windows.Forms.NumericUpDown();
            this.numMinStock = new System.Windows.Forms.NumericUpDown();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numPurchasePrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSalePrice)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinStock)).BeginInit();
            this.SuspendLayout();

            // txtBarcode
            this.txtBarcode.Location = new System.Drawing.Point(120, 20);
            this.txtBarcode.Name = "txtBarcode";
            this.txtBarcode.Size = new System.Drawing.Size(200, 20);
            this.txtBarcode.TabIndex = 0;

            // txtName
            this.txtName.Location = new System.Drawing.Point(120, 50);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 20);
            this.txtName.TabIndex = 1;

            // numPurchasePrice
            this.numPurchasePrice.DecimalPlaces = 2;
            this.numPurchasePrice.Location = new System.Drawing.Point(120, 80);
            this.numPurchasePrice.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numPurchasePrice.Name = "numPurchasePrice";
            this.numPurchasePrice.Size = new System.Drawing.Size(120, 20);
            this.numPurchasePrice.TabIndex = 2;

            // numSalePrice
            this.numSalePrice.DecimalPlaces = 2;
            this.numSalePrice.Location = new System.Drawing.Point(120, 110);
            this.numSalePrice.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numSalePrice.Name = "numSalePrice";
            this.numSalePrice.Size = new System.Drawing.Size(120, 20);
            this.numSalePrice.TabIndex = 3;

            // numQuantity
            this.numQuantity.Location = new System.Drawing.Point(120, 140);
            this.numQuantity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.numQuantity.Name = "numQuantity";
            this.numQuantity.Size = new System.Drawing.Size(120, 20);
            this.numQuantity.TabIndex = 4;

            // numMinStock
            this.numMinStock.Location = new System.Drawing.Point(120, 170);
            this.numMinStock.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            this.numMinStock.Name = "numMinStock";
            this.numMinStock.Size = new System.Drawing.Size(120, 20);
            this.numMinStock.TabIndex = 5;

            // btnSave
            this.btnSave.Location = new System.Drawing.Point(80, 210);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // btnCancel
            this.btnCancel.Location = new System.Drawing.Point(180, 210);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            // label1
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Barcode";

            // label2
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Name";

            // label3
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Purchase Price";

            // label4
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Sale Price";

            // label5
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(20, 143);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(46, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Quantity";

            // label6
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(20, 173);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(86, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Minimum Stock";

            // AddEditProductForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 250);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.numMinStock);
            this.Controls.Add(this.numQuantity);
            this.Controls.Add(this.numSalePrice);
            this.Controls.Add(this.numPurchasePrice);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtBarcode);
            this.Name = "AddEditProductForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add/Edit Product";
            ((System.ComponentModel.ISupportInitialize)(this.numPurchasePrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numSalePrice)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinStock)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox txtBarcode;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.NumericUpDown numPurchasePrice;
        private System.Windows.Forms.NumericUpDown numSalePrice;
        private System.Windows.Forms.NumericUpDown numQuantity;
        private System.Windows.Forms.NumericUpDown numMinStock;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
    }
}