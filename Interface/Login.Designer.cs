namespace SAE_S2
{
    partial class Login
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
            pnlLogin = new Panel();
            chkAfficherMdp = new CheckBox();
            btnConnexion = new Button();
            txtMdp = new TextBox();
            txtUsername = new TextBox();
            lblMdp = new Label();
            lblUsername = new Label();
            btnMenu = new Button();
            pnlLogin.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLogin
            // 
            pnlLogin.AutoSize = true;
            pnlLogin.BorderStyle = BorderStyle.FixedSingle;
            pnlLogin.Controls.Add(chkAfficherMdp);
            pnlLogin.Controls.Add(btnConnexion);
            pnlLogin.Controls.Add(txtMdp);
            pnlLogin.Controls.Add(txtUsername);
            pnlLogin.Controls.Add(lblMdp);
            pnlLogin.Controls.Add(lblUsername);
            pnlLogin.Location = new Point(475, 217);
            pnlLogin.Name = "pnlLogin";
            pnlLogin.Size = new Size(250, 200);
            pnlLogin.TabIndex = 0;
            // 
            // chkAfficherMdp
            // 
            chkAfficherMdp.AutoSize = true;
            chkAfficherMdp.Location = new Point(23, 138);
            chkAfficherMdp.Name = "chkAfficherMdp";
            chkAfficherMdp.Size = new Size(153, 19);
            chkAfficherMdp.TabIndex = 5;
            chkAfficherMdp.Text = "Afficher le mot de passe";
            chkAfficherMdp.UseVisualStyleBackColor = true;
            chkAfficherMdp.CheckedChanged += chkAfficherMdp_CheckedChanged;
            // 
            // btnConnexion
            // 
            btnConnexion.Anchor = AnchorStyles.Bottom;
            btnConnexion.AutoSize = true;
            btnConnexion.Location = new Point(80, 170);
            btnConnexion.Name = "btnConnexion";
            btnConnexion.Size = new Size(85, 25);
            btnConnexion.TabIndex = 4;
            btnConnexion.Text = "Se connecter";
            btnConnexion.UseVisualStyleBackColor = true;
            btnConnexion.Click += btnConnexion_Click;
            // 
            // txtMdp
            // 
            txtMdp.Location = new Point(23, 95);
            txtMdp.Name = "txtMdp";
            txtMdp.Size = new Size(195, 23);
            txtMdp.TabIndex = 3;
            txtMdp.UseSystemPasswordChar = true;
            txtMdp.TextChanged += txtMdp_TextChanged;
            // 
            // txtUsername
            // 
            txtUsername.Location = new Point(21, 36);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(195, 23);
            txtUsername.TabIndex = 2;
            // 
            // lblMdp
            // 
            lblMdp.AutoSize = true;
            lblMdp.Location = new Point(23, 77);
            lblMdp.Name = "lblMdp";
            lblMdp.Size = new Size(77, 15);
            lblMdp.TabIndex = 1;
            lblMdp.Text = "Mot de passe";
            lblMdp.Click += lblMdp_Click;
            // 
            // lblUsername
            // 
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(21, 18);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(61, 15);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "Identifiant";
            // 
            // btnMenu
            // 
            btnMenu.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnMenu.AutoSize = true;
            btnMenu.Location = new Point(12, 644);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(155, 25);
            btnMenu.TabIndex = 1;
            btnMenu.Text = "Revenir au menu principal";
            btnMenu.UseVisualStyleBackColor = true;
            btnMenu.Click += btnMenu_Click;
            // 
            // Login
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(btnMenu);
            Controls.Add(pnlLogin);
            Name = "Login";
            Text = "Corolis";
            Load += Login_Load;
            pnlLogin.ResumeLayout(false);
            pnlLogin.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlLogin;
        private Button btnConnexion;
        private TextBox txtMdp;
        private TextBox txtUsername;
        private Label lblMdp;
        private Label lblUsername;
        private CheckBox chkAfficherMdp;
        private Button btnMenu;
    }
}