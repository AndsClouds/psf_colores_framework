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

using System.Configuration;


namespace psf_colores_framework
{
    public partial class interfaz_principal : Form
    {
        // Campo para la cadena de conexión. No ejecutar lógica fuera de métodos.
        private readonly string cadena_conexion_bd;

        public interfaz_principal()
        {
            // Inicializar la cadena de conexión en el constructor y validar.
            cadena_conexion_bd = ConfigurationManager.ConnectionStrings["conexion_principal"]?.ConnectionString;
            if (string.IsNullOrWhiteSpace(cadena_conexion_bd))
            {
                throw new InvalidOperationException("La cadena de conexión 'conexion_principal' no se encontró en el archivo de configuración.");
            }
            else
            {
                               Console.WriteLine("Cadena de conexión cargada correctamente.");
            }

                InitializeComponent();
        }

        //para el combobox

        private class dispositivo_recuperado
        {
            public int id_disp { get; set; }
            public string nombre_disp { get; set; }
            public int cant_puertos_disp { get; set; }
        }

        List<dispositivo_recuperado> lista_dispositivos = new List<dispositivo_recuperado>();

        public void llenar_combo()
        {                       

            using (SqlConnection conn = new SqlConnection(cadena_conexion_bd))
            {
                conn.Open();

                SqlCommand consulta_combobox = new SqlCommand("select * from dispositivos", conn);
                SqlDataReader lector = consulta_combobox.ExecuteReader();
                while (lector.Read())
                {
                    dispositivo_recuperado disp = new dispositivo_recuperado();
                    disp.id_disp = lector.GetInt32(0);
                    disp.nombre_disp = lector.GetString(1);
                    disp.cant_puertos_disp = lector.GetInt32(2);
                    lista_dispositivos.Add(disp);
                    
                }

                conn.Close();

                comboBox1.DataSource = lista_dispositivos;
                comboBox1.DisplayMember = "nombre_disp";
                comboBox1.ValueMember = "id_disp";
                
            }
        }

        //para los rectángulos de colores
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (comboBox1.SelectedValue == null) return;

            
            flowLayoutPanel1.Controls.Clear();

            
            int id_seleccionado;

            
            if (comboBox1.SelectedValue is int)
            {
                id_seleccionado = (int)comboBox1.SelectedValue;
            }
            else
            {
                // Intentar parsear la representación en cadena
                if (!int.TryParse(comboBox1.SelectedValue.ToString(), out id_seleccionado))
                {
                    // Manejo simple de error: escribir en consola y salir del handler
                    Console.WriteLine("No se pudo convertir SelectedValue a int: " + comboBox1.SelectedValue);
                    return;
                }
            }

            //consultar la base de datos y añadir los rectángulos
            using (SqlConnection conn = new SqlConnection(cadena_conexion_bd))
            {
                conn.Open();

                SqlCommand consulta_puertos = new SqlCommand("select nro_puerto, r_puerto, g_puerto, b_puerto from puertos where id_disp = @id_disp", conn);
                consulta_puertos.Parameters.AddWithValue("@id_disp", id_seleccionado);

                using (SqlDataReader lector_puertos = consulta_puertos.ExecuteReader())
                {
                    while (lector_puertos.Read())
                    {
                        int nro_puerto = lector_puertos.GetInt32(0);
                        int r_puerto = lector_puertos.GetInt32(1);
                        int g_puerto = lector_puertos.GetInt32(2);
                        int b_puerto = lector_puertos.GetInt32(3);
                        Console.WriteLine("id del dispo: " + id_seleccionado + " nro_puerto: " + nro_puerto + " r_puerto " + r_puerto + " g_puerto " + g_puerto + " b_puerto " + b_puerto);
                        rectangulo_label_label rectangulo = new rectangulo_label_label();
                        rectangulo.colocar_datos(nro_puerto, r_puerto, g_puerto, b_puerto);
                        flowLayoutPanel1.Controls.Add(rectangulo);
                    }
                    Console.WriteLine("id del dispo: " + id_seleccionado);
                }

                
            }
        }

        //para el botón
        private void button2_Click(object sender, EventArgs e)
        {
            Form1 formulario_ingreso = new Form1();
            formulario_ingreso.ShowDialog();
            comboBox1.DataSource = null;
            comboBox1.Items.Clear();
            llenar_combo();
        }

        private void interfaz_principal_Load(object sender, EventArgs e)
        {
            llenar_combo();
        }

    }
}
