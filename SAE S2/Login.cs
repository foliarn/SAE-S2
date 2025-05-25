using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SAE_S2
{
    public partial class Login : Form
    {

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
    }
}
