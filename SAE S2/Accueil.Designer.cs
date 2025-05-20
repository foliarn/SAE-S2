namespace SAE_S2
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
            pnlRecherche.SuspendLayout();
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
            pnlRecherche.Location = new Point(50, 150);
            pnlRecherche.Margin = new Padding(10);
            pnlRecherche.Name = "pnlRecherche";
            pnlRecherche.Padding = new Padding(3);
            pnlRecherche.Size = new Size(350, 335);
            pnlRecherche.TabIndex = 0;
            // 
            // rdoArrive
            // 
            rdoArrive.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            rdoArrive.AutoSize = true;
            rdoArrive.Location = new Point(200, 230);
            rdoArrive.Name = "rdoArrive";
            rdoArrive.Size = new Size(156, 29);
            rdoArrive.TabIndex = 2;
            rdoArrive.TabStop = true;
            rdoArrive.Text = "Heure d'arrivée";
            rdoArrive.UseVisualStyleBackColor = true;
            rdoArrive.Visible = false;
            // 
            // btnTrouver
            // 
            btnTrouver.Anchor = AnchorStyles.Bottom;
            btnTrouver.AutoSize = true;
            btnTrouver.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnTrouver.Location = new Point(79, 292);
            btnTrouver.Name = "btnTrouver";
            btnTrouver.Size = new Size(194, 35);
            btnTrouver.TabIndex = 8;
            btnTrouver.Text = "Trouver un itinéraire";
            btnTrouver.UseVisualStyleBackColor = true;
            // 
            // rdoDepart
            // 
            rdoDepart.Anchor = AnchorStyles.None;
            rdoDepart.AutoSize = true;
            rdoDepart.Location = new Point(17, 230);
            rdoDepart.Name = "rdoDepart";
            rdoDepart.Size = new Size(166, 29);
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
            dtpHeure.Location = new Point(15, 265);
            dtpHeure.Name = "dtpHeure";
            dtpHeure.ShowUpDown = true;
            dtpHeure.Size = new Size(102, 31);
            dtpHeure.TabIndex = 5;
            dtpHeure.Visible = false;
            // 
            // chkHeure
            // 
            chkHeure.AutoSize = true;
            chkHeure.Checked = true;
            chkHeure.CheckState = CheckState.Checked;
            chkHeure.Location = new Point(17, 219);
            chkHeure.Name = "chkHeure";
            chkHeure.Size = new Size(185, 29);
            chkHeure.TabIndex = 7;
            chkHeure.Text = "Partir maintenant ?";
            chkHeure.UseVisualStyleBackColor = true;
            chkHeure.CheckedChanged += chkHeure_CheckedChanged;
            // 
            // lblDest
            // 
            lblDest.AutoSize = true;
            lblDest.Location = new Point(17, 143);
            lblDest.Name = "lblDest";
            lblDest.Size = new Size(102, 25);
            lblDest.TabIndex = 3;
            lblDest.Text = "Destination";
            // 
            // cmbDest
            // 
            cmbDest.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbDest.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbDest.FormattingEnabled = true;
            cmbDest.Items.AddRange(new object[] { "Ar1", "A2", "A3", "A4", "A5", "A6", "A7" });
            cmbDest.Location = new Point(15, 171);
            cmbDest.Name = "cmbDest";
            cmbDest.Size = new Size(244, 33);
            cmbDest.TabIndex = 2;
            // 
            // lblDepart
            // 
            lblDepart.AutoSize = true;
            lblDepart.Location = new Point(17, 64);
            lblDepart.Name = "lblDepart";
            lblDepart.Size = new Size(66, 25);
            lblDepart.TabIndex = 2;
            lblDepart.Text = "Départ";
            // 
            // cmbDepart
            // 
            cmbDepart.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbDepart.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbDepart.FormattingEnabled = true;
            cmbDepart.Items.AddRange(new object[] { "Ar1", "A2", "A3", "A4", "A5", "A6", "A7" });
            cmbDepart.Location = new Point(17, 92);
            cmbDepart.Name = "cmbDepart";
            cmbDepart.Size = new Size(244, 33);
            cmbDepart.TabIndex = 1;
            // 
            // lblTitre
            // 
            lblTitre.Anchor = AnchorStyles.Top;
            lblTitre.AutoSize = true;
            lblTitre.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitre.Location = new Point(12, 3);
            lblTitre.Name = "lblTitre";
            lblTitre.Size = new Size(330, 38);
            lblTitre.TabIndex = 0;
            lblTitre.Text = "Rechercher un itinéraire";
            lblTitre.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Accueil
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1258, 664);
            Controls.Add(pnlRecherche);
            Name = "Accueil";
            Text = "Form1";
            Load += Accueil_Load;
            pnlRecherche.ResumeLayout(false);
            pnlRecherche.PerformLayout();
            ResumeLayout(false);
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
    }
}