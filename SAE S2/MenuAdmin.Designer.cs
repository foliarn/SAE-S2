namespace SAE_S2
{
    partial class MenuAdmin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuAdmin));
            pnlTitre = new Panel();
            lblTitre = new Label();
            pnlMenuCreation = new Panel();
            pnlLigneDeco = new Panel();
            pnlCreerArret = new Panel();
            picIconPlus1 = new PictureBox();
            lblCreerArret = new Label();
            lblCreerArret1 = new Label();
            pnlCreerLigne = new Panel();
            picIconPlus = new PictureBox();
            lblCreerLigne = new Label();
            lblCreerLigne1 = new Label();
            lblTitreMenuCreer = new Label();
            lblSSTitreMenuCreer = new Label();
            pnlMenuModif = new Panel();
            pnlLigne2 = new Panel();
            pnlModifArret = new Panel();
            picIconEdit1 = new PictureBox();
            lblModifArret = new Label();
            lblModifArret1 = new Label();
            pnlModifLigne = new Panel();
            picIconEdit = new PictureBox();
            lblModifLigne = new Label();
            lblModifLigne1 = new Label();
            lblTitreMenuModif = new Label();
            lblSSTitreMenuModif = new Label();
            pnlCreation = new Panel();
            btnCreationValider = new Button();
            txtSaisirNom = new TextBox();
            lblSaisirNom = new Label();
            panel1 = new Panel();
            lblSaisirNomHead = new Label();
            btnMenu = new Button();
            pnlTitre.SuspendLayout();
            pnlMenuCreation.SuspendLayout();
            pnlCreerArret.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIconPlus1).BeginInit();
            pnlCreerLigne.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIconPlus).BeginInit();
            pnlMenuModif.SuspendLayout();
            pnlModifArret.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIconEdit1).BeginInit();
            pnlModifLigne.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picIconEdit).BeginInit();
            pnlCreation.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTitre
            // 
            pnlTitre.BackColor = Color.White;
            pnlTitre.Controls.Add(lblTitre);
            pnlTitre.Location = new Point(384, 57);
            pnlTitre.Name = "pnlTitre";
            pnlTitre.Size = new Size(508, 100);
            pnlTitre.TabIndex = 0;
            // 
            // lblTitre
            // 
            lblTitre.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
            lblTitre.AutoSize = true;
            lblTitre.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitre.Location = new Point(97, 33);
            lblTitre.Name = "lblTitre";
            lblTitre.Size = new Size(317, 32);
            lblTitre.TabIndex = 0;
            lblTitre.Text = "Que souhaitez-vous faire ?";
            // 
            // pnlMenuCreation
            // 
            pnlMenuCreation.BackColor = Color.White;
            pnlMenuCreation.BorderStyle = BorderStyle.FixedSingle;
            pnlMenuCreation.Controls.Add(pnlLigneDeco);
            pnlMenuCreation.Controls.Add(pnlCreerArret);
            pnlMenuCreation.Controls.Add(pnlCreerLigne);
            pnlMenuCreation.Controls.Add(lblTitreMenuCreer);
            pnlMenuCreation.Controls.Add(lblSSTitreMenuCreer);
            pnlMenuCreation.Location = new Point(222, 233);
            pnlMenuCreation.Name = "pnlMenuCreation";
            pnlMenuCreation.Size = new Size(302, 220);
            pnlMenuCreation.TabIndex = 1;
            // 
            // pnlLigneDeco
            // 
            pnlLigneDeco.BackColor = Color.DimGray;
            pnlLigneDeco.Location = new Point(28, 49);
            pnlLigneDeco.Name = "pnlLigneDeco";
            pnlLigneDeco.Size = new Size(235, 1);
            pnlLigneDeco.TabIndex = 11;
            // 
            // pnlCreerArret
            // 
            pnlCreerArret.BackColor = Color.Transparent;
            pnlCreerArret.BorderStyle = BorderStyle.FixedSingle;
            pnlCreerArret.Controls.Add(picIconPlus1);
            pnlCreerArret.Controls.Add(lblCreerArret);
            pnlCreerArret.Controls.Add(lblCreerArret1);
            pnlCreerArret.Cursor = Cursors.Hand;
            pnlCreerArret.Location = new Point(25, 137);
            pnlCreerArret.Name = "pnlCreerArret";
            pnlCreerArret.Size = new Size(206, 59);
            pnlCreerArret.TabIndex = 10;
            pnlCreerArret.Click += pnlCreerArret_Click;
            // 
            // picIconPlus1
            // 
            picIconPlus1.Image = Properties.Resources.icon_plus;
            picIconPlus1.Location = new Point(3, 9);
            picIconPlus1.Name = "picIconPlus1";
            picIconPlus1.Size = new Size(20, 20);
            picIconPlus1.SizeMode = PictureBoxSizeMode.StretchImage;
            picIconPlus1.TabIndex = 9;
            picIconPlus1.TabStop = false;
            picIconPlus1.Click += pnlCreerArret_Click;
            // 
            // lblCreerArret
            // 
            lblCreerArret.AutoSize = true;
            lblCreerArret.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCreerArret.Location = new Point(26, 9);
            lblCreerArret.Name = "lblCreerArret";
            lblCreerArret.Size = new Size(93, 17);
            lblCreerArret.TabIndex = 7;
            lblCreerArret.Text = "Créer un arrêt";
            lblCreerArret.Click += pnlCreerArret_Click;
            // 
            // lblCreerArret1
            // 
            lblCreerArret1.AutoSize = true;
            lblCreerArret1.Location = new Point(25, 35);
            lblCreerArret1.Name = "lblCreerArret1";
            lblCreerArret1.Size = new Size(151, 15);
            lblCreerArret1.TabIndex = 8;
            lblCreerArret1.Text = "Création d'une nouvel arrêt";
            lblCreerArret1.Click += pnlCreerArret_Click;
            // 
            // pnlCreerLigne
            // 
            pnlCreerLigne.BackColor = Color.Transparent;
            pnlCreerLigne.BorderStyle = BorderStyle.FixedSingle;
            pnlCreerLigne.Controls.Add(picIconPlus);
            pnlCreerLigne.Controls.Add(lblCreerLigne);
            pnlCreerLigne.Controls.Add(lblCreerLigne1);
            pnlCreerLigne.Cursor = Cursors.Hand;
            pnlCreerLigne.Location = new Point(25, 63);
            pnlCreerLigne.Name = "pnlCreerLigne";
            pnlCreerLigne.Size = new Size(206, 59);
            pnlCreerLigne.TabIndex = 8;
            pnlCreerLigne.Click += pnlCreerLigne_Click;
            // 
            // picIconPlus
            // 
            picIconPlus.Image = Properties.Resources.icon_plus;
            picIconPlus.Location = new Point(3, 9);
            picIconPlus.Name = "picIconPlus";
            picIconPlus.Size = new Size(20, 20);
            picIconPlus.SizeMode = PictureBoxSizeMode.StretchImage;
            picIconPlus.TabIndex = 9;
            picIconPlus.TabStop = false;
            picIconPlus.Click += pnlCreerLigne_Click;
            // 
            // lblCreerLigne
            // 
            lblCreerLigne.AutoSize = true;
            lblCreerLigne.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCreerLigne.Location = new Point(26, 9);
            lblCreerLigne.Name = "lblCreerLigne";
            lblCreerLigne.Size = new Size(100, 17);
            lblCreerLigne.TabIndex = 7;
            lblCreerLigne.Text = "Créer une ligne";
            lblCreerLigne.Click += pnlCreerLigne_Click;
            // 
            // lblCreerLigne1
            // 
            lblCreerLigne1.AutoSize = true;
            lblCreerLigne1.Location = new Point(25, 35);
            lblCreerLigne1.Name = "lblCreerLigne1";
            lblCreerLigne1.Size = new Size(162, 15);
            lblCreerLigne1.TabIndex = 8;
            lblCreerLigne1.Text = "Création d'une nouvelle ligne";
            lblCreerLigne1.Click += pnlCreerLigne_Click;
            // 
            // lblTitreMenuCreer
            // 
            lblTitreMenuCreer.AutoSize = true;
            lblTitreMenuCreer.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitreMenuCreer.Location = new Point(240, 29);
            lblTitreMenuCreer.Name = "lblTitreMenuCreer";
            lblTitreMenuCreer.Size = new Size(59, 17);
            lblTitreMenuCreer.TabIndex = 1;
            lblTitreMenuCreer.Text = "Création";
            // 
            // lblSSTitreMenuCreer
            // 
            lblSSTitreMenuCreer.AutoSize = true;
            lblSSTitreMenuCreer.Location = new Point(261, 14);
            lblSSTitreMenuCreer.Name = "lblSSTitreMenuCreer";
            lblSSTitreMenuCreer.Size = new Size(38, 15);
            lblSSTitreMenuCreer.TabIndex = 0;
            lblSSTitreMenuCreer.Text = "Menu";
            // 
            // pnlMenuModif
            // 
            pnlMenuModif.BackColor = Color.White;
            pnlMenuModif.BorderStyle = BorderStyle.FixedSingle;
            pnlMenuModif.Controls.Add(pnlLigne2);
            pnlMenuModif.Controls.Add(pnlModifArret);
            pnlMenuModif.Controls.Add(pnlModifLigne);
            pnlMenuModif.Controls.Add(lblTitreMenuModif);
            pnlMenuModif.Controls.Add(lblSSTitreMenuModif);
            pnlMenuModif.Location = new Point(742, 233);
            pnlMenuModif.Name = "pnlMenuModif";
            pnlMenuModif.Size = new Size(302, 220);
            pnlMenuModif.TabIndex = 12;
            // 
            // pnlLigne2
            // 
            pnlLigne2.BackColor = Color.DimGray;
            pnlLigne2.Location = new Point(29, 49);
            pnlLigne2.Name = "pnlLigne2";
            pnlLigne2.Size = new Size(235, 1);
            pnlLigne2.TabIndex = 11;
            // 
            // pnlModifArret
            // 
            pnlModifArret.BackColor = Color.Transparent;
            pnlModifArret.BorderStyle = BorderStyle.FixedSingle;
            pnlModifArret.Controls.Add(picIconEdit1);
            pnlModifArret.Controls.Add(lblModifArret);
            pnlModifArret.Controls.Add(lblModifArret1);
            pnlModifArret.Cursor = Cursors.Hand;
            pnlModifArret.Location = new Point(25, 137);
            pnlModifArret.Name = "pnlModifArret";
            pnlModifArret.Size = new Size(206, 59);
            pnlModifArret.TabIndex = 10;
            pnlModifArret.Click += pnlModifArret_Click;
            // 
            // picIconEdit1
            // 
            picIconEdit1.Image = (Image)resources.GetObject("picIconEdit1.Image");
            picIconEdit1.Location = new Point(3, 9);
            picIconEdit1.Name = "picIconEdit1";
            picIconEdit1.Size = new Size(18, 18);
            picIconEdit1.SizeMode = PictureBoxSizeMode.Zoom;
            picIconEdit1.TabIndex = 10;
            picIconEdit1.TabStop = false;
            picIconEdit1.Click += pnlModifArret_Click;
            // 
            // lblModifArret
            // 
            lblModifArret.AutoSize = true;
            lblModifArret.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblModifArret.Location = new Point(26, 9);
            lblModifArret.Name = "lblModifArret";
            lblModifArret.Size = new Size(111, 17);
            lblModifArret.TabIndex = 7;
            lblModifArret.Text = "Modifier un arrêt";
            lblModifArret.Click += pnlModifArret_Click;
            // 
            // lblModifArret1
            // 
            lblModifArret1.AutoSize = true;
            lblModifArret1.Location = new Point(25, 35);
            lblModifArret1.Name = "lblModifArret1";
            lblModifArret1.Size = new Size(129, 15);
            lblModifArret1.TabIndex = 8;
            lblModifArret1.Text = "Modification d'un arrêt";
            lblModifArret1.Click += pnlModifArret_Click;
            // 
            // pnlModifLigne
            // 
            pnlModifLigne.BackColor = Color.Transparent;
            pnlModifLigne.BorderStyle = BorderStyle.FixedSingle;
            pnlModifLigne.Controls.Add(picIconEdit);
            pnlModifLigne.Controls.Add(lblModifLigne);
            pnlModifLigne.Controls.Add(lblModifLigne1);
            pnlModifLigne.Cursor = Cursors.Hand;
            pnlModifLigne.Location = new Point(25, 63);
            pnlModifLigne.Name = "pnlModifLigne";
            pnlModifLigne.Size = new Size(206, 59);
            pnlModifLigne.TabIndex = 8;
            pnlModifLigne.Click += pnlModifLigne_Click;
            // 
            // picIconEdit
            // 
            picIconEdit.Image = (Image)resources.GetObject("picIconEdit.Image");
            picIconEdit.Location = new Point(3, 9);
            picIconEdit.Name = "picIconEdit";
            picIconEdit.Size = new Size(18, 18);
            picIconEdit.SizeMode = PictureBoxSizeMode.Zoom;
            picIconEdit.TabIndex = 9;
            picIconEdit.TabStop = false;
            picIconEdit.Click += pnlModifLigne_Click;
            // 
            // lblModifLigne
            // 
            lblModifLigne.AutoSize = true;
            lblModifLigne.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblModifLigne.Location = new Point(26, 9);
            lblModifLigne.Name = "lblModifLigne";
            lblModifLigne.Size = new Size(118, 17);
            lblModifLigne.TabIndex = 7;
            lblModifLigne.Text = "Modifier une ligne";
            lblModifLigne.Click += pnlModifLigne_Click;
            // 
            // lblModifLigne1
            // 
            lblModifLigne1.AutoSize = true;
            lblModifLigne1.Location = new Point(25, 35);
            lblModifLigne1.Name = "lblModifLigne1";
            lblModifLigne1.Size = new Size(137, 15);
            lblModifLigne1.TabIndex = 8;
            lblModifLigne1.Text = "Modification d'une ligne";
            lblModifLigne1.Click += pnlModifLigne_Click;
            // 
            // lblTitreMenuModif
            // 
            lblTitreMenuModif.AutoSize = true;
            lblTitreMenuModif.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitreMenuModif.Location = new Point(3, 29);
            lblTitreMenuModif.Name = "lblTitreMenuModif";
            lblTitreMenuModif.Size = new Size(83, 17);
            lblTitreMenuModif.TabIndex = 1;
            lblTitreMenuModif.Text = "Modification";
            // 
            // lblSSTitreMenuModif
            // 
            lblSSTitreMenuModif.AutoSize = true;
            lblSSTitreMenuModif.Location = new Point(3, 14);
            lblSSTitreMenuModif.Name = "lblSSTitreMenuModif";
            lblSSTitreMenuModif.Size = new Size(38, 15);
            lblSSTitreMenuModif.TabIndex = 0;
            lblSSTitreMenuModif.Text = "Menu";
            // 
            // pnlCreation
            // 
            pnlCreation.BackColor = Color.White;
            pnlCreation.BorderStyle = BorderStyle.FixedSingle;
            pnlCreation.Controls.Add(btnCreationValider);
            pnlCreation.Controls.Add(txtSaisirNom);
            pnlCreation.Controls.Add(lblSaisirNom);
            pnlCreation.Controls.Add(panel1);
            pnlCreation.Controls.Add(lblSaisirNomHead);
            pnlCreation.Location = new Point(530, 226);
            pnlCreation.Name = "pnlCreation";
            pnlCreation.Size = new Size(200, 129);
            pnlCreation.TabIndex = 13;
            pnlCreation.Visible = false;
            // 
            // btnCreationValider
            // 
            btnCreationValider.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnCreationValider.Location = new Point(62, 96);
            btnCreationValider.Name = "btnCreationValider";
            btnCreationValider.Size = new Size(73, 23);
            btnCreationValider.TabIndex = 14;
            btnCreationValider.Text = "Valider";
            btnCreationValider.UseVisualStyleBackColor = true;
            // 
            // txtSaisirNom
            // 
            txtSaisirNom.Location = new Point(21, 54);
            txtSaisirNom.Name = "txtSaisirNom";
            txtSaisirNom.Size = new Size(165, 23);
            txtSaisirNom.TabIndex = 13;
            // 
            // lblSaisirNom
            // 
            lblSaisirNom.AutoSize = true;
            lblSaisirNom.Location = new Point(21, 36);
            lblSaisirNom.Name = "lblSaisirNom";
            lblSaisirNom.Size = new Size(74, 15);
            lblSaisirNom.TabIndex = 12;
            lblSaisirNom.Text = "Saisir le nom";
            // 
            // panel1
            // 
            panel1.BackColor = Color.DimGray;
            panel1.Location = new Point(21, 27);
            panel1.Name = "panel1";
            panel1.Size = new Size(165, 1);
            panel1.TabIndex = 12;
            // 
            // lblSaisirNomHead
            // 
            lblSaisirNomHead.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblSaisirNomHead.AutoSize = true;
            lblSaisirNomHead.BackColor = Color.Transparent;
            lblSaisirNomHead.Font = new Font("Segoe UI Semibold", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSaisirNomHead.Location = new Point(3, 7);
            lblSaisirNomHead.Name = "lblSaisirNomHead";
            lblSaisirNomHead.Size = new Size(183, 17);
            lblSaisirNomHead.TabIndex = 2;
            lblSaisirNomHead.Text = "Saisir le nom du nouvel arrêt";
            // 
            // btnMenu
            // 
            btnMenu.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnMenu.AutoSize = true;
            btnMenu.Location = new Point(12, 644);
            btnMenu.Name = "btnMenu";
            btnMenu.Size = new Size(155, 25);
            btnMenu.TabIndex = 14;
            btnMenu.Text = "Revenir au menu principal";
            btnMenu.UseVisualStyleBackColor = true;
            btnMenu.Click += btnMenu_Click;
            // 
            // MenuAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1264, 681);
            Controls.Add(btnMenu);
            Controls.Add(pnlCreation);
            Controls.Add(pnlMenuModif);
            Controls.Add(pnlMenuCreation);
            Controls.Add(pnlTitre);
            Name = "MenuAdmin";
            Text = "Form1";
            pnlTitre.ResumeLayout(false);
            pnlTitre.PerformLayout();
            pnlMenuCreation.ResumeLayout(false);
            pnlMenuCreation.PerformLayout();
            pnlCreerArret.ResumeLayout(false);
            pnlCreerArret.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIconPlus1).EndInit();
            pnlCreerLigne.ResumeLayout(false);
            pnlCreerLigne.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIconPlus).EndInit();
            pnlMenuModif.ResumeLayout(false);
            pnlMenuModif.PerformLayout();
            pnlModifArret.ResumeLayout(false);
            pnlModifArret.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIconEdit1).EndInit();
            pnlModifLigne.ResumeLayout(false);
            pnlModifLigne.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picIconEdit).EndInit();
            pnlCreation.ResumeLayout(false);
            pnlCreation.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel pnlTitre;
        private Label lblTitre;
        private Panel pnlMenuCreation;
        private Label lblTitreMenuCreer;
        private Label lblSSTitreMenuCreer;
        private Panel pnlCreerLigne;
        private Label lblCreerLigne;
        private Label lblCreerLigne1;
        private Panel pnlCreerArret;
        private PictureBox picIconPlus1;
        private Label lblCreerArret;
        private Label lblCreerArret1;
        private PictureBox picIconPlus;
        private Panel pnlLigneDeco;
        private Panel pnlMenuModif;
        private Panel pnlLigne2;
        private Panel pnlModifArret;
        private Label lblModifArret;
        private Label lblModifArret1;
        private Panel pnlModifLigne;
        private PictureBox picIconEdit;
        private Label lblModifLigne;
        private Label lblModifLigne1;
        private Label lblTitreMenuModif;
        private Label lblSSTitreMenuModif;
        private PictureBox picIconEdit1;
        private Panel pnlCreation;
        private Label lblSaisirNomHead;
        private Label lblSaisirNom;
        private Panel panel1;
        private Button btnCreationValider;
        private TextBox txtSaisirNom;
        private Button btnMenu;
    }
}