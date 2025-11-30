using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace psf_colores_framework
{
    public partial class label_nro_disp : UserControl
    {
        public label_nro_disp()
        {
            InitializeComponent();
        }

        private void label_nro_disp_Load(object sender, EventArgs e)
        {

        }

        public void colocar_nro_disp(string nro_disp)
        {
            label1.Text = nro_disp;
        }
    }
}
