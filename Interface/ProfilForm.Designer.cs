// ProfilForm.Designer.cs
namespace Interface
{
    partial class ProfilForm
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
            pnlProfil = new Panel();
            rdoModeNoAttente = new RadioButton();
            rdoModeNoCorres = new RadioButton();
            rdoModeVitesse = new RadioButton();
            btnDeconnexion = new Button();
            btnSauvegarder = new Button();
            lblPreferences = new Label();
            txtPassword = new TextBox();
            lblPassword = new Label();
            txtUsername = new TextBox();
            lblUsername = new Label();
            lblTitre = new Label();
            btnFermer = new Button();
            btnMenuAdmin = new Button();
            pnlProfil.SuspendLayout();
            SuspendLayout();
            // 
            // pnlProfil
            // 
            pnlProfil.BackColor = Color.White;
            pnlProfil.BorderStyle = BorderStyle.FixedSingle;
            pnlProfil.Controls.Add(btnMenuAdmin);
            pnlProfil.Controls.Add(rdoModeNoAttente);
            pnlProfil.Controls.Add(rdoModeNoCorres);
            pnlProfil.Controls.Add(rdoModeVitesse);
            pnlProfil.Controls.Add(btnDeconnexion);
            pnlProfil.Controls.Add(btnSauvegarder);
            pnlProfil.Controls.Add(lblPreferences);
            pnlProfil.Controls.Add(txtPassword);
            pnlProfil.Controls.Add(lblPassword);
            pnlProfil.Controls.Add(txtUsername);
            pnlProfil.Controls.Add(lblUsername);
            pnlProfil.Controls.Add(lblTitre);
            pnlProfil.Controls.Add(btnFermer);
            pnlProfil.Location = new Point(0, 0);
            pnlProfil.Name = "pnlProfil";
            pnlProfil.Size = new Size(350, 400);
            pnlProfil.TabIndex = 0;
            // 
            // rdoModeNoAttente
            // 
            rdoModeNoAttente.AutoSize = true;
            rdoModeNoAttente.Location = new Point(20, 301);
            rdoModeNoAttente.Name = "rdoModeNoAttente";
            rdoModeNoAttente.Size = new Size(189, 19);
            rdoModeNoAttente.TabIndex = 13;
            rdoModeNoAttente.TabStop = true;
            rdoModeNoAttente.Text = "Privilégier l'évitement d'attente";
            rdoModeNoAttente.UseVisualStyleBackColor = true;
            // 
            // rdoModeNoCorres
            // 
            rdoModeNoCorres.AutoSize = true;
            rdoModeNoCorres.Location = new Point(20, 266);
            rdoModeNoCorres.Name = "rdoModeNoCorres";
            rdoModeNoCorres.Size = new Size(248, 19);
            rdoModeNoCorres.TabIndex = 12;
            rdoModeNoCorres.TabStop = true;
            rdoModeNoCorres.Text = "Privilégier l'évitement de correspondances";
            rdoModeNoCorres.UseVisualStyleBackColor = true;
            // 
            // rdoModeVitesse
            // 
            rdoModeVitesse.AutoSize = true;
            rdoModeVitesse.Location = new Point(20, 231);
            rdoModeVitesse.Name = "rdoModeVitesse";
            rdoModeVitesse.Size = new Size(192, 19);
            rdoModeVitesse.TabIndex = 11;
            rdoModeVitesse.TabStop = true;
            rdoModeVitesse.Text = "Privilégier la vitesse de parcours";
            rdoModeVitesse.UseVisualStyleBackColor = true;
            // 
            // btnDeconnexion
            // 
            btnDeconnexion.BackColor = Color.LightCoral;
            btnDeconnexion.Location = new Point(6, 335);
            btnDeconnexion.Name = "btnDeconnexion";
            btnDeconnexion.Size = new Size(110, 35);
            btnDeconnexion.TabIndex = 10;
            btnDeconnexion.Text = "Se déconnecter";
            btnDeconnexion.UseVisualStyleBackColor = false;
            btnDeconnexion.Click += btnDeconnexion_Click;
            // 
            // btnSauvegarder
            // 
            btnSauvegarder.BackColor = Color.DarkSeaGreen;
            btnSauvegarder.Location = new Point(122, 335);
            btnSauvegarder.Name = "btnSauvegarder";
            btnSauvegarder.Size = new Size(100, 35);
            btnSauvegarder.TabIndex = 9;
            btnSauvegarder.Text = "Sauvegarder";
            btnSauvegarder.UseVisualStyleBackColor = false;
            btnSauvegarder.Click += btnSauvegarder_Click;
            // 
            // lblPreferences
            // 
            lblPreferences.AutoSize = true;
            lblPreferences.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPreferences.Location = new Point(20, 200);
            lblPreferences.Name = "lblPreferences";
            lblPreferences.Size = new Size(96, 19);
            lblPreferences.TabIndex = 6;
            lblPreferences.Text = "Préférences :";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(20, 155);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(200, 23);
            txtPassword.TabIndex = 5;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(20, 130);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(83, 15);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Mot de passe :";
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(20, 95);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(200, 23);
            txtUsername.TabIndex = 3;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(20, 70);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(105, 15);
            lblUsername.TabIndex = 2;
            lblUsername.Text = "Nom d'utilisateur :";
            // 
            // lblTitre
            // 
            lblTitre.AutoSize = true;
            lblTitre.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitre.Location = new Point(20, 20);
            lblTitre.Name = "lblTitre";
            lblTitre.Size = new Size(157, 25);
            lblTitre.TabIndex = 1;
            lblTitre.Text = "Profil Utilisateur";
            // 
            // btnFermer
            // 
            btnFermer.BackColor = Color.LightGray;
            btnFermer.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnFermer.Location = new Point(310, 10);
            btnFermer.Name = "btnFermer";
            btnFermer.Size = new Size(25, 25);
            btnFermer.TabIndex = 0;
            btnFermer.Text = "✕";
            btnFermer.UseVisualStyleBackColor = false;
            btnFermer.Click += btnFermer_Click;
            // 
            // btnMenuAdmin
            // 
            btnMenuAdmin.BackColor = SystemColors.ControlDark;
            btnMenuAdmin.Location = new Point(228, 335);
            btnMenuAdmin.Name = "btnMenuAdmin";
            btnMenuAdmin.Size = new Size(110, 35);
            btnMenuAdmin.TabIndex = 14;
            btnMenuAdmin.Text = "Menu Admin";
            btnMenuAdmin.UseVisualStyleBackColor = false;
            btnMenuAdmin.Click += btnMenuAdmin_Click;
            // 
            // ProfilForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            ClientSize = new Size(350, 400);
            Controls.Add(pnlProfil);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ProfilForm";
            StartPosition = FormStartPosition.Manual;
            TopMost = true;
            pnlProfil.ResumeLayout(false);
            pnlProfil.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlProfil;
        private System.Windows.Forms.Button btnFermer;
        private System.Windows.Forms.Label lblTitre;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label lblPreferences;
        private System.Windows.Forms.Button btnSauvegarder;
        private System.Windows.Forms.Button btnDeconnexion;
        private RadioButton rdoModeNoCorres;
        private RadioButton rdoModeVitesse;
        private RadioButton rdoModeNoAttente;
        private Button btnMenuAdmin;
    }
}