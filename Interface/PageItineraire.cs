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
    public partial class PageItineraire : Form
    {
        private Accueil formAccueil;
        public PageItineraire(Accueil accueil)
        {
            InitializeComponent();
            formAccueil = accueil;
        }

        private void PageItineraire_Load(object sender, EventArgs e)
        {

        }
    }
}
