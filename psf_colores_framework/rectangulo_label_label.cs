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
    public partial class rectangulo_label_label : UserControl
    {
        public rectangulo_label_label()
        {
            InitializeComponent();
        }

        private void rectangulo_label_label_Load(object sender, EventArgs e)
        {

        }

        public void colocar_datos(int puerto_num, int componente_r, int componente_g, int componente_b)
        {
            label1.Text = "Puerto " + puerto_num.ToString();
            label2.Text = "R=" + componente_r.ToString() + " G=" + componente_g.ToString() + " B=" + componente_b.ToString();
            panel1.BackColor = Color.FromArgb(componente_r, componente_g, componente_b);
        }

    }
}
