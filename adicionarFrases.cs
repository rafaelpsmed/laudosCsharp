using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Data.SQLite;



namespace Laudos
{
    public partial class adicionarFrases : Form
    {

        public adicionarFrases()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void btnAddFrase_Click(object sender, EventArgs e)
        {
            var modelo = cboModelos.GetItemText(this.cboModelos.SelectedItem);
            var lembrete = txtLembrete.Text;
            var procurarPor = txtProcurar.Text;
            var substituirPor = txtSubstituir.Text;
            var descricaoAchado = txtDescAchado.Text;
            var fraseRetirarConclusao = txtRetirarConclusao.Text;
            var conclusao = txtConclusao.Text;
            var conclusaoPlural = txtConclusaoPlural.Text;


            //SQLiteConnection conn = new SQLiteConnection("Data Source = D:\\Rafael\\Projetos programacao e eletronica\\Projetos C#\\Laudos - Litedb\\frasesSqlite.db;Version=3;");





            using (var conn = new System.Data.SQLite.SQLiteConnection("Data Source=D:\\Rafael\\Projetos programacao e eletronica\\Projetos C#\\Laudos - Litedb\\frasesSqlite.db;Version=3;"))
            {
                conn.Open();
                using var comandoSql = new SQLiteCommand(conn);
                comandoSql.CommandText = "INSERT INTO frases (tipo_exame, mme, procurarPor, substituirPor, descricaoAchado, fraseRetirarConclusao, conclusao, conclusaoPlural, placeholderAlteracoesCorpoLaudo) VALUES ('" + modelo + "','" + lembrete + "','" + procurarPor + "','" + substituirPor + "','" + descricaoAchado + "','" + fraseRetirarConclusao + "','" + conclusao + "','" + conclusaoPlural + "', 'teste')";
                comandoSql.Prepare();
                comandoSql.ExecuteNonQuery();
                conn.Close();


            }





            //using (var bd = new LiteDatabase(@"D:\Rafael\Projetos programacao e eletronica\Projetos C#\Laudos - Litedb\frases.db"))
            //{
            //    var col = bd.GetCollection<Frases>("frases");

            //    var frases = new Frases
            //    {
            //        modelo = txtModelo.Text,
            //        lembrete = txtLembrete.Text,
            //        procurarPor = new string[] { txtProcurar.Text },
            //        substituirPor = new string[] { txtSubstituir.Text },
            //        fraseRetirarConclusao = txtRetirarConclusao.Text,
            //        conclusao = txtConclusao.Text,
            //        conclusaoPlural = txtConclusaoPlural.Text


            //    };

            //    col.Insert(frases);
            //}
        }

        private void btnEscolherModelos_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {

                string modelos = folderBrowserDialog1.SelectedPath;
                txtModelos2.Text = modelos;
                string[] files = Directory.GetFiles(modelos);
                foreach (string filePath in files) cboModelos.Items.Add(Path.GetFileName(filePath));


            }
        }

        private void btnAddModelo_Click(object sender, EventArgs e)
        {
            string caminhoModelos = txtModelos2.Text;
            string modelo = this.cboModelos.GetItemText(this.cboModelos.SelectedItem);
            string[] caminhos = { caminhoModelos, modelo };
            string modeloTexto = File.ReadAllText(Path.Combine(caminhos));
            txtLaudo.Text = modeloTexto;
        }
    }
}
