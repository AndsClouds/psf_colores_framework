using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//para la conexión a base de datos
using System.Data.SqlClient;
//para la conversión de colores y su comparación
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Conversions;
using ColorMine.ColorSpaces.Comparisons;
using System.Drawing.Text;
using System.Configuration;

namespace psf_colores_framework
{
    public partial class Form1 : Form
    {
        string cadena_conexion_bd = ConfigurationManager.ConnectionStrings["conexion_principal"].ConnectionString;

        List<string> lista_nros_serie = new List<string>();



        //generación de valores rgb aleatorios
        private Random generador_num_aleatorio = new Random();

        private int valor_aleatorio()
        {
            return generador_num_aleatorio.Next(0, 256);
        }

        //lista para los colores aceptados
        List<Rgb> lista_aceptados = new List<Rgb>();


        // función de transformación a cielab y comparación
        private void rgb_a_cielab(int r, int g, int b)
        {
            


            var color_rgb = new Rgb { R = r, G = g, B = b };
            var color_cielab = color_rgb.To<Lab>();

            foreach (var color_aceptado in lista_aceptados)
            {
                var color_aceptado_cielab = color_aceptado.To<Lab>();
                var diferencia = color_cielab.Compare(color_aceptado_cielab, new CieDe2000Comparison());
                //umbral de comparación
                if (diferencia > 22) // valor de umbral ajustable
                {
                    lista_aceptados.Add(color_rgb);
                    //MessageBox.Show("diferencial: " + diferencia + "Color aceptado: R=" + r + " G=" + g + " B=" + b);
                    
                    return;
                }
            }


        }

        private void vercolores()
        {
            foreach (var color in lista_aceptados)
            {
                Console.WriteLine("R=" + color.R + " G=" + color.G + " B=" + color.B);
            }
        }


        // recuperador del id para mostrar en la primera caja de texto
        private int recuperador_id()
        {
            int id_devuelto;

            using (SqlConnection conn = new SqlConnection(cadena_conexion_bd))
            {
                conn.Open();
                SqlCommand consulta = new SqlCommand("select ident_current ( 'dispositivos' )", conn);
                id_devuelto = Convert.ToInt32(consulta.ExecuteScalar());
            }
            return id_devuelto;
        }


        private void recuperador_nro_serie()
        {
            using (SqlConnection conn = new SqlConnection(cadena_conexion_bd))
            {
                conn.Open();
                SqlCommand consulta_nro_serie = new SqlCommand("select nro_serie from dispositivos", conn);

                using (SqlDataReader lector = consulta_nro_serie.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        string nro_serie = lector.GetString(0);
                        lista_nros_serie.Add(nro_serie);
                        label_nro_disp label_nro = new label_nro_disp();
                        label_nro.colocar_nro_disp(nro_serie);
                        flowLayoutPanel1.Controls.Add(label_nro);
                    }
                }
                conn.Close();
            }
        }


        public Form1()
        {
            InitializeComponent();
            lista_aceptados.Add(new Rgb { R = 0, G = 0, B = 0 }); // negro_prueba
            recuperador_nro_serie();
            textBox1.Text = (recuperador_id()+1).ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool hay_vacios = string.IsNullOrWhiteSpace(textBox2.Text) ||
                                     string.IsNullOrWhiteSpace(textBox3.Text) ||
                                     string.IsNullOrWhiteSpace(textBox4.Text);

            bool num_en_puertos = int.TryParse(textBox3.Text, out _);
            if (hay_vacios || !num_en_puertos)
            {
                MessageBox.Show("Introduzca correctamente los datos por favor"); return;
            }
            else
            {
                foreach (string nro_registrado in lista_nros_serie)
                {
                    if (nro_registrado.Equals(textBox4.Text))
                    {
                        MessageBox.Show("Ya existe un dispositivo con ese número de serie. Introduzca otro por favor.");
                        textBox4.Clear();
                        return;
                    }
                }

                using (SqlConnection conn = new SqlConnection(cadena_conexion_bd))
                {
                    conn.Open();
                    SqlCommand consulta = new SqlCommand("INSERT INTO dispositivos (nombre_disp, cant_puertos_disp, nro_serie) VALUES (@nombre_disp, @cant_puertos_disp, @nro_serie)", conn);

                    consulta.Parameters.AddWithValue("@nombre_disp", textBox2.Text);
                    consulta.Parameters.AddWithValue("@cant_puertos_disp", textBox3.Text);
                    consulta.Parameters.AddWithValue("@nro_serie", textBox4.Text);
                    consulta.ExecuteNonQuery();


                    while (lista_aceptados.Count <= int.Parse(textBox3.Text))
                    {
                        rgb_a_cielab(valor_aleatorio(), valor_aleatorio(), valor_aleatorio());
                    }


                    vercolores();
                    for (int i = 1; i <= int.Parse(textBox3.Text); i++)
                    {

                        SqlCommand consulta_puertos = new SqlCommand("INSERT INTO puertos (id_disp, nro_puerto, r_puerto, g_puerto, b_puerto) values (@id_disp, @nro_puerto, @r_puerto, @g_puerto, @b_puerto)", conn);
                        consulta_puertos.Parameters.AddWithValue("@id_disp", Convert.ToInt32(textBox1.Text));
                        consulta_puertos.Parameters.AddWithValue("@nro_puerto", i);
                        consulta_puertos.Parameters.AddWithValue("@r_puerto", (int)lista_aceptados[i].R);
                        consulta_puertos.Parameters.AddWithValue("@g_puerto", (int)lista_aceptados[i].G);
                        consulta_puertos.Parameters.AddWithValue("@b_puerto", (int)lista_aceptados[i].B);
                        consulta_puertos.ExecuteNonQuery();

                    }
                    conn.Close();
                }
                this.Hide();
                MessageBox.Show("¡Dispositivo registrado correctamente!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
