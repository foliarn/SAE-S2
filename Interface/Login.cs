using Interface.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interface
{
    public partial class Login : Form
    {
        public static bool estConnecte = false; // Indique si l'utilisateur est connecté ou non (admin)
        string username = "admin";
        string password = "admin";
        private void Login_Load(object sender, EventArgs e)
        {
            Utils.CentrerControle(pnlLogin);
        }

        private void txtMdp_TextChanged(object sender, EventArgs e)
        {
            if (txtMdp.Text.Length > 0)
            {
                chkAfficherMdp.Visible = true;
            }
            else
            {
                chkAfficherMdp.Visible = false;
            }
        }
        private void chkAfficherMdp_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAfficherMdp.Checked)
            {
                txtMdp.UseSystemPasswordChar = false;
            }
            else
            {
                txtMdp.UseSystemPasswordChar = true;
            }
        }

        private void lblUsername_Click(object sender, EventArgs e)
        {
            txtUsername.Focus();
        }

        private void lblMdp_Click(object sender, EventArgs e)
        {
            txtMdp.Focus();
        }

        private void btnMenu_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public Login()
        {
            InitializeComponent();
        }

        private void btnConnexion_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == username && txtMdp.Text == password)
            {
                estConnecte = true;
                MessageBox.Show("Connexion réussie !", "Succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK; // Indique que la connexion a réussi
                this.Close(); // Ferme la fenêtre de connexion
            }
            else
            {
                MessageBox.Show("Identifiant ou mot de passe incorrect.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
