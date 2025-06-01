namespace Interface
{
    partial class Accueil
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
            pnlRecherche = new Panel();
            rdoArrive = new RadioButton();
            btnTrouver = new Button();
            rdoDepart = new RadioButton();
            dtpHeure = new DateTimePicker();
            chkHeure = new CheckBox();
            lblDest = new Label();
            cmbDest = new ComboBox();
            lblDepart = new Label();
            cmbDepart = new ComboBox();
            lblTitre = new Label();
            picPlan = new PictureBox();
            picRetrecir = new PictureBox();
            pnlPlan = new Panel();
            picLogin = new PictureBox();
            btnLigne = new Button();
            lblPlan = new Label();
            picLogo = new PictureBox();
            button1 = new Button();
            pnlRecherche.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picPlan).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picRetrecir).BeginInit();
            pnlPlan.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogin).BeginInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // pnlRecherche
            // 
            pnlRecherche.Anchor = AnchorStyles.None;
            pnlRecherche.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            pnlRecherche.BorderStyle = BorderStyle.FixedSingle;
            pnlRecherche.Controls.Add(rdoArrive);
            pnlRecherche.Controls.Add(btnTrouver);
            pnlRecherche.Controls.Add(rdoDepart);
            pnlRecherche.Controls.Add(dtpHeure);
            pnlRecherche.Controls.Add(chkHeure);
            pnlRecherche.Controls.Add(lblDest);
            pnlRecherche.Controls.Add(cmbDest);
            pnlRecherche.Controls.Add(lblDepart);
            pnlRecherche.Controls.Add(cmbDepart);
            pnlRecherche.Controls.Add(lblTitre);
            pnlRecherche.Location = new Point(30, 261);
            pnlRecherche.Margin = new Padding(7, 6, 7, 6);
            pnlRecherche.Name = "pnlRecherche";
            pnlRecherche.Padding = new Padding(2);
            pnlRecherche.Size = new Size(250, 200);
            pnlRecherche.TabIndex = 0;
            // 
            // rdoArrive
            // 
            rdoArrive.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rdoArrive.AutoSize = true;
            rdoArrive.Location = new Point(150, 138);
            rdoArrive.Margin = new Padding(2);
            rdoArrive.Name = "rdoArrive";
            rdoArrive.Size = new Size(105, 19);
            rdoArrive.TabIndex = 2;
            rdoArrive.Text = "Heure d'arrivée";
            rdoArrive.UseVisualStyleBackColor = true;
            rdoArrive.Visible = false;
            // 
            // btnTrouver
            // 
            btnTrouver.Anchor = AnchorStyles.Bottom;
            btnTrouver.AutoSize = true;
            btnTrouver.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTrouver.Location = new Point(57, 159);
            btnTrouver.Margin = new Padding(2);
            btnTrouver.Name = "btnTrouver";
            btnTrouver.Size = new Size(194, 35);
            btnTrouver.TabIndex = 8;
            btnTrouver.Text = "Trouver un itinéraire";
            btnTrouver.UseVisualStyleBackColor = true;
            btnTrouver.Click += btnTrouver_Click;
            // 
            // rdoDepart
            // 
            rdoDepart.Anchor = AnchorStyles.None;
            rdoDepart.AutoSize = true;
            rdoDepart.Checked = true;
            rdoDepart.Location = new Point(10, 137);
            rdoDepart.Margin = new Padding(2);
            rdoDepart.Name = "rdoDepart";
            rdoDepart.Size = new Size(110, 19);
            rdoDepart.TabIndex = 1;
            rdoDepart.TabStop = true;
            rdoDepart.Text = "Heure de départ";
            rdoDepart.UseVisualStyleBackColor = true;
            rdoDepart.Visible = false;
            // 
            // dtpHeure
            // 
            dtpHeure.CustomFormat = "HH:mm";
            dtpHeure.Format = DateTimePickerFormat.Custom;
            dtpHeure.Location = new Point(10, 159);
            dtpHeure.Margin = new Padding(2);
            dtpHeure.Name = "dtpHeure";
            dtpHeure.ShowUpDown = true;
            dtpHeure.Size = new Size(73, 23);
            dtpHeure.TabIndex = 5;
            dtpHeure.Value = new DateTime(2025, 6, 1, 12, 0, 0, 0);
            dtpHeure.Visible = false;
            // 
            // chkHeure
            // 
            chkHeure.AutoSize = true;
            chkHeure.Checked = true;
            chkHeure.CheckState = CheckState.Checked;
            chkHeure.Location = new Point(10, 131);
            chkHeure.Margin = new Padding(2);
            chkHeure.Name = "chkHeure";
            chkHeure.Size = new Size(126, 19);
            chkHeure.TabIndex = 7;
            chkHeure.Text = "Partir maintenant ?";
            chkHeure.UseVisualStyleBackColor = true;
            chkHeure.CheckedChanged += chkHeure_CheckedChanged;
            // 
            // lblDest
            // 
            lblDest.AutoSize = true;
            lblDest.Location = new Point(12, 86);
            lblDest.Margin = new Padding(2, 0, 2, 0);
            lblDest.Name = "lblDest";
            lblDest.Size = new Size(67, 15);
            lblDest.TabIndex = 3;
            lblDest.Text = "Destination";
            // 
            // cmbDest
            // 
            cmbDest.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbDest.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbDest.FormattingEnabled = true;
            cmbDest.Items.AddRange(new object[] { "Ar1", "A2", "A3", "A4", "A5", "A6", "A7" });
            cmbDest.Location = new Point(10, 103);
            cmbDest.Margin = new Padding(2);
            cmbDest.Name = "cmbDest";
            cmbDest.Size = new Size(172, 23);
            cmbDest.TabIndex = 2;
            // 
            // lblDepart
            // 
            lblDepart.AutoSize = true;
            lblDepart.Location = new Point(12, 38);
            lblDepart.Margin = new Padding(2, 0, 2, 0);
            lblDepart.Name = "lblDepart";
            lblDepart.Size = new Size(42, 15);
            lblDepart.TabIndex = 2;
            lblDepart.Text = "Départ";
            // 
            // cmbDepart
            // 
            cmbDepart.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbDepart.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbDepart.FormattingEnabled = true;
            cmbDepart.Items.AddRange(new object[] { "Ar1", "A2", "A3", "A4", "A5", "A6", "A7" });
            cmbDepart.Location = new Point(10, 55);
            cmbDepart.Margin = new Padding(2);
            cmbDepart.Name = "cmbDepart";
            cmbDepart.Size = new Size(172, 23);
            cmbDepart.TabIndex = 1;
            // 
            // lblTitre
            // 
            lblTitre.Anchor = AnchorStyles.Top;
            lblTitre.AutoSize = true;
            lblTitre.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitre.Location = new Point(10, 2);
            lblTitre.Margin = new Padding(2, 0, 2, 0);
            lblTitre.Name = "lblTitre";
            lblTitre.Size = new Size(224, 25);
            lblTitre.TabIndex = 0;
            lblTitre.Text = "Rechercher un itinéraire";
            lblTitre.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // picPlan
            // 
            picPlan.BorderStyle = BorderStyle.FixedSingle;
            picPlan.Cursor = Cursors.Hand;
            picPlan.Dock = DockStyle.Fill;
            picPlan.Image = Properties.Resources.plan_beauvais;
            picPlan.Location = new Point(0, 0);
            picPlan.Name = "picPlan";
            picPlan.Size = new Size(816, 470);
            picPlan.SizeMode = PictureBoxSizeMode.CenterImage;
            picPlan.TabIndex = 1;
            picPlan.TabStop = false;
            picPlan.Click += pnlPlan_Click;
            // 
            // picRetrecir
            // 
            picRetrecir.BackColor = Color.Transparent;
            picRetrecir.Cursor = Cursors.Hand;
            picRetrecir.Image = Properties.Resources.icon_retrecir;
            picRetrecir.Location = new Point(0, 0);
            picRetrecir.Name = "picRetrecir";
            picRetrecir.Size = new Size(32, 32);
            picRetrecir.SizeMode = PictureBoxSizeMode.StretchImage;
            picRetrecir.TabIndex = 2;
            picRetrecir.TabStop = false;
            picRetrecir.Visible = false;
            picRetrecir.Click += picRetrecir_Click;
            // 
            // pnlPlan
            // 
            pnlPlan.BackColor = Color.Transparent;
            pnlPlan.Controls.Add(picRetrecir);
            pnlPlan.Controls.Add(picPlan);
            pnlPlan.Location = new Point(301, 103);
            pnlPlan.Name = "pnlPlan";
            pnlPlan.Size = new Size(816, 470);
            pnlPlan.TabIndex = 3;
            pnlPlan.Click += pnlPlan_Click;
            // 
            // picLogin
            // 
            picLogin.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            picLogin.Cursor = Cursors.Hand;
            picLogin.Image = Properties.Resources.icon_login;
            picLogin.Location = new Point(1248, 0);
            picLogin.Name = "picLogin";
            picLogin.Size = new Size(48, 48);
            picLogin.SizeMode = PictureBoxSizeMode.StretchImage;
            picLogin.TabIndex = 4;
            picLogin.TabStop = false;
            picLogin.Click += picLogin_Click;
            // 
            // btnLigne
            // 
            btnLigne.AutoSize = true;
            btnLigne.Font = new Font("Segoe UI", 12F);
            btnLigne.Location = new Point(645, 589);
            btnLigne.Name = "btnLigne";
            btnLigne.Size = new Size(266, 35);
            btnLigne.TabIndex = 5;
            btnLigne.Text = "Accéder au détail des lignes";
            btnLigne.UseVisualStyleBackColor = true;
            btnLigne.Click += btnLigne_Click;
            // 
            // lblPlan
            // 
            lblPlan.AutoSize = true;
            lblPlan.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPlan.Location = new Point(572, 55);
            lblPlan.Name = "lblPlan";
            lblPlan.Size = new Size(434, 45);
            lblPlan.TabIndex = 6;
            lblPlan.Text = "Plan 2025 du réseau Corolis";
            // 
            // picLogo
            // 
            picLogo.Image = Properties.Resources.Logo_corolis_mini;
            picLogo.Location = new Point(2, 103);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(288, 87);
            picLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            picLogo.TabIndex = 7;
            picLogo.TabStop = false;
            // 
            // button1
            // 
            button1.Location = new Point(138, 550);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 8;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Accueil
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 637);
            Controls.Add(button1);
            Controls.Add(picLogo);
            Controls.Add(lblPlan);
            Controls.Add(btnLigne);
            Controls.Add(picLogin);
            Controls.Add(pnlPlan);
            Controls.Add(pnlRecherche);
            Margin = new Padding(2);
            Name = "Accueil";
            Text = "Corolis";
            pnlRecherche.ResumeLayout(false);
            pnlRecherche.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picPlan).EndInit();
            ((System.ComponentModel.ISupportInitialize)picRetrecir).EndInit();
            pnlPlan.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)picLogin).EndInit();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlRecherche;
        private Label lblTitre;
        private ComboBox cmbDepart;
        private Label lblDepart;
        private Label lblDest;
        private ComboBox cmbDest;
        private DateTimePicker dtpHeure;
        private CheckBox chkHeure;
        private Button btnTrouver;
        private RadioButton rdoDepart;
        private RadioButton rdoArrive;
        private PictureBox picPlan;
        private PictureBox picRetrecir;
        private Panel pnlPlan;
        private PictureBox picLogin;
        private Button btnLigne;
        private Label lblPlan;
        private PictureBox picLogo;
        private Button button1;
    }
}