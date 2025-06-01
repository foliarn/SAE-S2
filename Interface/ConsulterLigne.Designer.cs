namespace Interface
{
    partial class ConsulterLigne
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
            components = new System.ComponentModel.Container();
            pnlChoixLigne = new Panel();
            btnValiderChoix = new Button();
            cmbChoixLigne = new ComboBox();
            lblChoixLigne = new Label();
            panel3 = new Panel();
            lblChoixLigneHead = new Label();
            btnMenu = new Button();
            class1BindingSource = new BindingSource(components);
            dgvLigne = new DataGridView();
            lblTitreDgv = new Label();
            pnlChoixLigne.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)class1BindingSource).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLigne).BeginInit();
            SuspendLayout();
            // 
            // pnlChoixLigne
            // 
            pnlChoixLigne.BackColor = Color.White;
            pnlChoixLigne.BorderStyle = BorderStyle.FixedSingle;
            pnlChoixLigne.Controls.Add(btnValiderChoix);
            pnlChoixLigne.Controls.Add(cmbChoixLigne);
            pnlChoixLigne.Controls.Add(lblChoixLigne);
            pnlChoixLigne.Controls.Add(panel3);
            pnlChoixLigne.Controls.Add(lblChoixLigneHead);
            pnlChoixLigne.Location = new Point(34, 12);
            pnlChoixLigne.Name = "pnlChoixLigne";
            pnlChoixLigne.Size = new Size(240, 150);
            pnlChoixLigne.TabIndex = 16;
            // 
            // btnValiderChoix
            // 
            btnValiderChoix.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnValiderChoix.Location = new Point(65, 110);
            btnValiderChoix.Name = "btnValiderChoix";
            btnValiderChoix.Size = new Size(110, 25);
            btnValiderChoix.TabIndex = 15;
            btnValiderChoix.Text = "Valider";
            btnValiderChoix.UseVisualStyleBackColor = true;
            btnValiderChoix.Click += btnValiderChoix_Click;
            // 
            // cmbChoixLigne
            // 
            cmbChoixLigne.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbChoixLigne.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbChoixLigne.FormattingEnabled = true;
            cmbChoixLigne.Items.AddRange(new object[] { "A1", "A2", "A3", "A4", "A5", "test" });
            cmbChoixLigne.Location = new Point(20, 62);
            cmbChoixLigne.Name = "cmbChoixLigne";
            cmbChoixLigne.Size = new Size(200, 23);
            cmbChoixLigne.TabIndex = 15;
            // 
            // lblChoixLigne
            // 
            lblChoixLigne.AutoSize = true;
            lblChoixLigne.Location = new Point(20, 40);
            lblChoixLigne.Name = "lblChoixLigne";
            lblChoixLigne.Size = new Size(146, 15);
            lblChoixLigne.TabIndex = 12;
            lblChoixLigne.Text = "Choisir la ligne à consulter";
            // 
            // panel3
            // 
            panel3.BackColor = Color.DimGray;
            panel3.Location = new Point(21, 27);
            panel3.Name = "panel3";
            panel3.Size = new Size(165, 1);
            panel3.TabIndex = 12;
            // 
            // lblChoixLigneHead
            // 
            lblChoixLigneHead.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblChoixLigneHead.AutoSize = true;
            lblChoixLigneHead.BackColor = Color.Transparent;
            lblChoixLigneHead.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblChoixLigneHead.Location = new Point(21, 7);
            lblChoixLigneHead.Name = "lblChoixLigneHead";
            lblChoixLigneHead.Size = new Size(109, 17);
            lblChoixLigneHead.TabIndex = 2;
            lblChoixLigneHead.Text = "Choisir une ligne";
            // 
            // btnMenu
            // 
            btnMenu.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnMenu.AutoSize = true;
            btnMenu.Location = new Point(12, 644);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(155, 25);
            btnMenu.TabIndex = 17;
            btnMenu.Text = "Revenir au menu principal";
            btnMenu.UseVisualStyleBackColor = true;
            btnMenu.Click += btnMenu_Click;
            // 
            // dgvLigne
            // 
            dgvLigne.AllowUserToAddRows = false;
            dgvLigne.AllowUserToDeleteRows = false;
            dgvLigne.AllowUserToResizeColumns = false;
            dgvLigne.AllowUserToResizeRows = false;
            dgvLigne.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dgvLigne.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLigne.Location = new Point(325, 75);
            dgvLigne.Name = "dgvLigne";
            dgvLigne.ReadOnly = true;
            dgvLigne.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLigne.Size = new Size(820, 542);
            dgvLigne.TabIndex = 18;
            // 
            // lblTitreDgv
            // 
            lblTitreDgv.AutoSize = true;
            lblTitreDgv.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitreDgv.Location = new Point(531, 23);
            lblTitreDgv.Name = "lblTitreDgv";
            lblTitreDgv.Size = new Size(74, 45);
            lblTitreDgv.TabIndex = 19;
            lblTitreDgv.Text = "xxx";
            // 
            // ConsulterLigne
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(lblTitreDgv);
            Controls.Add(dgvLigne);
            Controls.Add(btnMenu);
            Controls.Add(pnlChoixLigne);
            Name = "ConsulterLigne";
            Text = "Corolis";
            pnlChoixLigne.ResumeLayout(false);
            pnlChoixLigne.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)class1BindingSource).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLigne).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel pnlChoixLigne;
        private Button btnValiderChoix;
        private ComboBox cmbChoixLigne;
        private Label lblChoixLigne;
        private Panel panel3;
        private Label lblChoixLigneHead;
        private Button btnMenu;
        private BindingSource class1BindingSource;
        private DataGridView dgvLigne;
        private Label lblTitreDgv;
    }
}