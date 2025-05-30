using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Interface.Classes;

namespace Interface
{
    public partial class PageItineraire : Form
    {
        private Accueil formAccueil;
        public PageItineraire(Accueil accueil)
        {
            InitializeComponent();
            formAccueil = accueil;
            Utils.CentrerControle(pnlRecherche, false, true);
            Utils.CentrerControle(pnlItineraire1, false, true);
            Utils.CentrerControle(pnlItineraire2, false, true);
        }

        private void PageItineraire_Load(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }
    }
}