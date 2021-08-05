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
    public partial class Form1 : Form
    {
        public string modelosTireoide;
        public string nomeDaPasta;

       









        public Form1()
        {
            InitializeComponent();
            
            txtLaudo.KeyDown += txtLaudo_KeyDown;



        }

        private void Form1_Load(object sender, EventArgs e)
        {



        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            adicionarFrases frases = new adicionarFrases();
            frases.Show();

        }

        private void menuFrases_Click(object sender, EventArgs e)
        {
            adicionarFrases frases = new adicionarFrases();
            frases.Show();

        }



        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string caminhoModelos = txtModelosTireoide.Text;
            string modelo = this.cboModelosTireoide.GetItemText(this.cboModelosTireoide.SelectedItem);
            string[] caminhos = { caminhoModelos, modelo };
            string modeloTexto = File.ReadAllText(Path.Combine(caminhos));
            txtLaudo.Text = modeloTexto;

        }



        public string Volume(string medidas)
        {
            
            string[] medida = medidas.Trim().Split(" ");

            if (medida.Length == 3)
            {
                String resultado = "";
                double med1 = Double.Parse(medida[0]);
                double med2 = Double.Parse(medida[1]);
                double med3 = Double.Parse(medida[2]);
                double VolMed = Math.Round((med1 * med2 * med3 * 0.523));
                if (VolMed < 1)
                {
                    
                    resultado = String.Format("{0} x {1} x {2} cm, com volume menor que 1 cm³", med1.ToString(), med2.ToString(), med3.ToString());
                }
                else
                {
                    resultado = String.Format("{0} x {1} x {2} cm, com volume de {3} cm³", med1.ToString(), med2.ToString(), med3.ToString(), VolMed.ToString());
                }
                
                return resultado;
                

            }

            if (medida.Length == 2)
            {
                string med1 = medida[0];
                string med2 = medida[1];
                String resultado = String.Format("{0} x {1} cm", med1.ToString(), med2.ToString());
                return resultado;

            }
            if (medida.Length == 1)
            {
                string med1 = medida[0];                
                String resultado = String.Format("{0} cm", med1.ToString());
                return resultado;

            }
            if (medida.Length == 0)
            {
                //string message = e.Message;
                //Console.WriteLine(e.Message);
                string message = "Algum dos campos ou não foi(ram) preenchido(s) ou foi(ram) preenchido(s) de forma incorreta. Verifique novamente.";
                string caption = "Erro detectado";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                //if (result == System.Windows.Forms.DialogResult.Yes)
                //{
                //    // Closes the parent form.
                //    this.Close();
                //}

            }

            return null;




        }

        public string VolumeTotal(string medidas)
        {
            string[] medida = medidas.Split(" ");

            if(medida.Length < 3)
            {
                double semMedida = 0;
                return semMedida.ToString();
            }

            if (medida.Length == 3)
            {
                double med1 = Double.Parse(medida[0]);
                double med2 = Double.Parse(medida[1]);
                double med3 = Double.Parse(medida[2]);
                double VolMed = Math.Round((med1 * med2 * med3 * 0.523));
                
                return VolMed.ToString();


            }
            return null;

        }

        public string CriarLaudo(string laudo, string exameNome, string arquivoFrases, int alteracoesIndex, string local, string medidas, string horas, string distMamilo, string distPele, string paredeMioma)
        {
            laudo = txtLaudo.Text;
            var modeloExame = cboModelosTireoide.GetItemText(this.cboModelosTireoide.SelectedItem);
            var mme = cboTireoideAlteracoes.GetItemText(this.cboTireoideAlteracoes.SelectedItem);
            var localX = cboTireoideLocalizacao.GetItemText(this.cboTireoideLocalizacao.SelectedItem);

            using (var conn = new System.Data.SQLite.SQLiteConnection("Data Source=D:\\Rafael\\Projetos programacao e eletronica\\Projetos C#\\Laudos - Litedb\\frasesSqlite.db;Version=3;"))
            {

                conn.Open();
                using var comandoSql = new SQLiteCommand(conn);
                comandoSql.CommandText = "SELECT procurarPor, substituirPor, descricaoAchado, fraseRetirarConclusao, conclusao, conclusaoPlural FROM frases  WHERE mme = '" + mme + "'";
                //comandoSql.CommandText = "SELECT procurarPor, substituirPor, descricaoAchado, fraseRetirarConclusao, conclusao, conclusaoPlural FROM frases  WHERE tipo_exame = '" + modeloExame + "'";
                comandoSql.Prepare();
                comandoSql.ExecuteNonQuery();
                SQLiteDataReader entradas = comandoSql.ExecuteReader();



                while (entradas.Read())
                {

                    var xxx = entradas["procurarPor"].ToString();
                    var zzz = entradas["substituirPor"].ToString();
                    string[] xxx2 = xxx.Split(";");
                    string[] zzz2 = zzz.Split(";");
                    int xxx_tamanho = xxx2.Length;

                    var fraseRetirarConclusao = entradas["fraseRetirarConclusao"].ToString();
                    var conclusao = entradas["conclusao"].ToString();
                    var conclusaoPlural = entradas["conclusaoPlural"].ToString();
                    for (int i = 0; i < xxx_tamanho; i++)
                    {
                        if (laudo.Contains(xxx2[i]))
                        {
                            laudo = laudo.Replace(xxx2[i], zzz2[i]).Replace("{local}", localX);
                            //laudo = laudo.Replace("{local}", localX + System.Environment.NewLine);


                        }




                    }
                    if (laudo.Contains(fraseRetirarConclusao))
                    {
                        laudo = laudo.Replace(fraseRetirarConclusao, conclusao + System.Environment.NewLine + "{conclusão}");

                    }
                    else
                    {

                        if (laudo.Contains(conclusao) ^ laudo.Contains(conclusaoPlural))
                        {
                            laudo = laudo.Replace(conclusao, conclusaoPlural);
                        }
                        else
                        {
                            laudo = laudo.Replace("{conclusão}", conclusao + System.Environment.NewLine + "{conclusão}");

                        }


                    }




                }



                txtLaudo.Text = laudo;
                conn.Close();
            }


            return null;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string laudo = txtLaudo.Text;
            string exame = "tireóide";
            string arquivosFrases = txtFrases.Text;
            int alteracoesIndex = cboTireoideAlteracoes.SelectedIndex;
            string local = cboTireoideLocalizacao.Text;
            string medidas = Volume(txtTireoideAlteracoesMedidas.Text);
            

            string mamaLoc = "";
            string distMamilo = "";
            string distPele = "";
            string paredeMioma = "";

            CriarLaudo(laudo, exame, arquivosFrases, alteracoesIndex, local, medidas, mamaLoc, distMamilo, distPele, paredeMioma);

            

            if (cboTireoideDoppler.SelectedIndex > -1)
            {
                string laudo2 = txtLaudo.Text;
                if (cboTireoideDoppler.SelectedIndex == 0)
                {
                    

                }
                if (cboTireoideDoppler.SelectedIndex == 1)
                {
                    laudo2 = laudo2.Replace("{doppler}", "Sem fluxo ao Doppler.");

                }
                if (cboTireoideDoppler.SelectedIndex == 2)
                {
                    laudo2 = laudo2.Replace("{doppler}", "Fluxo periférico ao Doppler.");

                }
                if (cboTireoideDoppler.SelectedIndex == 3)
                {
                    laudo2 = laudo2.Replace("{doppler}", "Fluxo ao Doppler periférico maior ou igual ao fluxo central.");

                }
                if (cboTireoideDoppler.SelectedIndex == 4)
                {
                    laudo2 = laudo2.Replace("{doppler}", "Fluxo ao Doppler central maior que o fluxo periférico.");

                }
                if (cboTireoideDoppler.SelectedIndex == 5)
                {
                    laudo2 = laudo2.Replace("{doppler}", "Fluxo central ao Doppler.");

                }
                
                txtLaudo.Text = laudo2;


            }

            //txtLaudo.Text = laudo;





            //string laudo = txtLaudo.Text;            

            //string arquivoJson = File.ReadAllText(txtFrases.Text);
            //dynamic obj = JsonConvert.DeserializeObject(arquivoJson);

            //int indexJson = cboTireoideAlteracoes.SelectedIndex;
            //string local = cboTireoideLocalizacao.Text;
            //string medidas = txtTireoideAlteracoesMedidas.Text;


            ////*******************************************************

            //JArray items = (JArray)obj.tireóide[indexJson]["frasesCorpoLaudo"];
            //int indexfrasesCorpoLaudo = 0;

            ////string fraseFinal = obj.tireóide[indexJson]["frase"].ToString();
            ////fraseFinal = fraseFinal.Replace("{local}", local).Replace("{medidas}", medidas);
            ////fraseFinal += obj.tireóide[indexJson]["doppler"].ToString();

            //var conclusao = obj.tireóide[indexJson]["conclusão"].ToString();
            //conclusao = conclusao.Replace("{local}", local);

            //var doppler = obj.tireóide[indexJson]["doppler"].ToString();

            //////var fraseRetirarCorpo = obj.tireóide[indexJson]["fraseRetirarCorpo"].ToString();
            //var fraseRetirarConclusao = obj.tireóide[indexJson]["fraseRetirarConclusão"].ToString();

            //var placeholderAlteracoesCorpo = obj.tireóide[indexJson]["placeholderAlteraçõesCorpo"].ToString();
            //var placeholderAlteracoesConclusao = obj.tireóide[indexJson]["placeholderConclusão"].ToString();


            //// doppler
            ////try
            ////{
            ////if (cboTireoideDoppler.SelectedIndex > -1)
            ////{
            ////    //fraseFinal += obj.tireóide[indexJson]["doppler"].ToString();
            ////    ////fraseFinal += fraseFinal.Replace("{doppler}", "").Trim();
            ////    //fraseFinal = fraseFinal.Replace("{doppler}", "").Trim();
            ////    if (cboTireoideDoppler.SelectedIndex == 0)
            ////    {
            ////        fraseFinal = fraseFinal.Replace("{doppler}", "");
            ////        //laudo = laudo.Replace("{doppler}", "").Trim();

            ////    }

            ////    if (cboTireoideDoppler.SelectedIndex == 1)
            ////    {
            ////        fraseFinal = fraseFinal.Replace("{doppler}", "Sem fluxo ao Doppler.");

            ////    }
            ////    if (cboTireoideDoppler.SelectedIndex == 2)
            ////    {
            ////        fraseFinal = fraseFinal.Replace("{doppler}", "Fluxo periférico ao Doppler.");

            ////    }
            ////    if (cboTireoideDoppler.SelectedIndex == 3)
            ////    {
            ////        fraseFinal = fraseFinal.Replace("{doppler}", "Fluxo ao Doppler periférico maior ou igual ao fluxo central.");

            ////    }
            ////    if (cboTireoideDoppler.SelectedIndex == 4)
            ////    {
            ////        fraseFinal = fraseFinal.Replace("{doppler}", "Fluxo ao Doppler central maior que o fluxo periférico.");

            ////    }
            ////    if (cboTireoideDoppler.SelectedIndex == 5)
            ////    {
            ////        fraseFinal = fraseFinal.Replace("{doppler}", "Fluxo central ao Doppler.");

            ////    }

            ////}

            ////obj.tireóide[indexJson]["frasesCorpoLaudo"];


            //foreach (var result in items)
            //    {

            //        string procurarPor = (string)obj.tireóide[indexJson]["frasesCorpoLaudo"][indexfrasesCorpoLaudo]["procurarPor"];
            //        string substituirPor = (string)obj.tireóide[indexJson]["frasesCorpoLaudo"][indexfrasesCorpoLaudo]["substituirPor"];
            //        string colocaPlaceholder = (string)obj.tireóide[indexJson]["frasesCorpoLaudo"][indexfrasesCorpoLaudo]["colocaPlaceholder"];
            //        string podeTerDoppler = (string)obj.tireóide[indexJson]["frasesCorpoLaudo"][indexfrasesCorpoLaudo]["podeTerDoppler"];

            //        if (podeTerDoppler.Equals("sim"))
            //        {
            //            substituirPor += " "+doppler;
            //        }

            //        if (colocaPlaceholder.Equals("sim"))
            //                {
            //                    laudo = laudo.Replace(procurarPor, substituirPor + System.Environment.NewLine + placeholderAlteracoesCorpo);
            //                    laudo = laudo.Replace("{local}", local).Replace("{medidas}", medidas);


            //                }
            //        if (colocaPlaceholder.Equals("não"))
            //            {
            //                    laudo = laudo.Replace(procurarPor, substituirPor);
            //                    laudo = laudo.Replace("{local}", local).Replace("{medidas}", medidas);

            //            }








            //    indexfrasesCorpoLaudo++;

            //    }


            //if (cboTireoideDoppler.SelectedIndex == 0)
            //{

            //    laudo = laudo.Replace(doppler, "").Trim();


            //}
            //if (cboTireoideDoppler.SelectedIndex == 1)
            //{

            //    laudo = laudo.Replace(doppler, "Sem fluxo ao Doppler.");                

            //}
            //if (cboTireoideDoppler.SelectedIndex == 2)
            //{

            //    laudo = laudo.Replace(doppler, "Presença de fluxo periférico ao Doppler.");

            //}
            //if (cboTireoideDoppler.SelectedIndex == 3)
            //{

            //    laudo = laudo.Replace(doppler, "Presença de fluxo periférico maior que central ao Doppler.");

            //}
            //if (cboTireoideDoppler.SelectedIndex == 4)
            //{

            //    laudo = laudo.Replace(doppler, "Presença de fluxo central maior que periférico ao Doppler.");

            //}
            //if (cboTireoideDoppler.SelectedIndex == 5)
            //{

            //    laudo = laudo.Replace(doppler, "Presença de fluxo apenas central ao Doppler.");

            //}

            //txtLaudo.Text = laudo;

            //// como colocar o plural das frases na conclusão

            //var frasePlural = obj.tireóide[indexJson]["fraseConclusãoPlural"].ToString();
            //frasePlural = frasePlural.Replace("{local}", local);

            //if (laudo.Contains(fraseRetirarConclusao))
            //{
            //    laudo = laudo.Replace(fraseRetirarConclusao, conclusao + System.Environment.NewLine + placeholderAlteracoesConclusao);

            //}
            //else
            //{

            //    if (laudo.Contains(conclusao) ^ laudo.Contains(frasePlural))
            //    {
            //        laudo = laudo.Replace(conclusao, frasePlural);
            //    }
            //    else
            //    {
            //        laudo = laudo.Replace(placeholderAlteracoesConclusao, conclusao + System.Environment.NewLine + placeholderAlteracoesConclusao);

            //    }


            //}

            //txtLaudo.Text = laudo;

            ////}
            ////catch(Exception ex)
            ////{
            ////    string message = "Algum dos campos está incompleto ou escrito errado. Verifique e tente novamente.";
            ////    string caption = "Erro detectado";
            ////    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            ////    DialogResult result;

            ////    // Displays the MessageBox.
            ////    result = MessageBox.Show(message, caption, buttons);
            ////}
            ////finally
            ////{
            ////    Console.WriteLine("Fim");

            ////}




        }

        private void cboTireoideMedidasLobos_Click(object sender, EventArgs e)
        {
            string laudo = txtLaudo.Text;

            string loboDireito = Volume(txtTireoideLD.Text);
            string istmo = Volume(txtTireoideIST.Text);
            string loboEsquerdo = Volume(txtTireoideLE.Text);

            //double LD;
            //double LE;

            //if (string.IsNullOrWhiteSpace(txtTireoideLD.Text))
            //{

            //}
            //else
            //{

            //   LD = Double.Parse(VolumeTotal(txtTireoideLD.Text));

            //}
            //if (string.IsNullOrWhiteSpace(txtTireoideLE.Text))
            //{

            //}
            //else
            //{
            //    LE = Double.Parse(VolumeTotal(txtTireoideLD.Text));

            //}



            double LD = Double.Parse(VolumeTotal(txtTireoideLD.Text));
            double LE = Double.Parse(VolumeTotal(txtTireoideLE.Text));
            double volTotalTireoide = LD + LE;
            laudo = laudo.Replace("{lobo direito}", loboDireito).Replace("{istmo}", istmo).Replace("{lobo esquerdo}", loboEsquerdo).Replace("{volume}", volTotalTireoide.ToString() + " cm³.");

            bool LDVazio = string.IsNullOrEmpty(loboDireito);
            bool LEVazio = string.IsNullOrEmpty(loboEsquerdo);
            bool ISTVazio = string.IsNullOrEmpty(istmo);







            txtLaudo.Text = laudo;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {

                modelosTireoide = folderBrowserDialog1.SelectedPath;
                txtModelosTireoide.Text = modelosTireoide;



                //txtModelosTireoide.Text = folderBrowserDialog1.SelectedPath;
                //como colocar os títulos dos laudos como entradas na combobox
                //string[] files = Directory.GetFiles("D:\\Rafael\\Projetos programacao e eletronica\\Projetos C#\\Laudos\\Modelos\\Tireóide");
                string[] files = Directory.GetFiles(modelosTireoide);
                foreach (string filePath in files) cboModelosTireoide.Items.Add(Path.GetFileName(filePath));

                //string[] alteracoes = File.ReadAllLines("D:\\Rafael\\Projetos programacao e eletronica\\Projetos C#\\Laudos\\Modelos\\alterações.txt");
                ////string[] alteracoesLinhas = alteracoes
                //foreach (string line in alteracoes)
                //{
                //    string[] alteracoesLinhas = line.Split(";");
                //    if (alteracoesLinhas[0] == "tireóide")
                //    {
                //        cboTireoideAlteracoes.Items.Add(alteracoesLinhas[1]);

                //    }


                //}
            }
        }

        private void btnEscolherFrases_Click(object sender, EventArgs e)
        {
            cboMamaAlteracoes.Items.Clear();
            cboTireoideAlteracoes.Items.Clear();
            cboTVAlteracoes.Items.Clear();
            cboAbdAlteracoes.Items.Clear();
            cboGenAlteracoes.Items.Clear();

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtFrases.Text = openFileDialog1.FileName;


                // ler o Json
                dynamic frases = JsonConvert.DeserializeObject(File.ReadAllText(txtFrases.Text));

                //funcionou para loop no Json e carregar no cbo

                string zzz = File.ReadAllText(txtFrases.Text);
                dynamic obj = JsonConvert.DeserializeObject(zzz);

                string nomePasta = Path.GetFileName(folderBrowserDialog1.SelectedPath).ToLower();

                //tireóide
                //foreach (var bbb in obj.tireóide)
                foreach (var bbb in obj.exame["tireóide"])
                {
                    //txtLaudo.Text += bbb.mme + Environment.NewLine;
                    cboTireoideAlteracoes.Items.Add(bbb.mme);

                }

                //mama

                foreach (var bbb in obj.exame["mama"])
                {
                    //txtLaudo.Text += bbb.mme + Environment.NewLine;
                    cboMamaAlteracoes.Items.Add(bbb.mme);

                }

                //// pélvico

                foreach (var bbb in obj.exame["pélvico"])
                {
                    //txtLaudo.Text += bbb.mme + Environment.NewLine;
                    cboTVAlteracoes.Items.Add(bbb.mme);

                }

                /// abdome
                foreach (var bbb in obj.exame["abdome"])
                {
                    //txtLaudo.Text += bbb.mme + Environment.NewLine;
                    cboAbdAlteracoes.Items.Add(bbb.mme);

                }

                /// generico
                bool nomePastaVazio = string.IsNullOrEmpty(nomeDaPasta);

                if (nomePastaVazio == true)
                {

                }
                else
                {
                    foreach (var bbb in obj.exame[nomePasta])
                    {
                        //txtLaudo.Text += bbb.mme + Environment.NewLine;
                        cboGenAlteracoes.Items.Add(bbb.mme);

                    }

                }

               
                





            }





            //int qtasAlteracoes = cboTireoideAlteracoes.Items.Count;
            //for (int i = 0; i < qtasAlteracoes; i++)
            //{
            //    string alt = "alt_" + i;
            //    alteracoes.Add(alt);
            //}








        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (folderBrowserDialog2.ShowDialog() == DialogResult.OK)
            {

                modelosTireoide = folderBrowserDialog2.SelectedPath;
                txtModelosMama.Text = modelosTireoide;

                string[] files = Directory.GetFiles(modelosTireoide);
                foreach (string filePath in files) cboModelosMama.Items.Add(Path.GetFileName(filePath));


            }

        }

        private void btnCopiarLaudo_Click(object sender, EventArgs e)
        {



            // retirar tudo que esteja entre parênteses
            string placeholders = @"\{.*?\}";
            var procurarEdeletar = new Regex(placeholders);
            string laudo = txtLaudo.Text;
            Clipboard.SetText(laudo);

            foreach (Match match in procurarEdeletar.Matches(laudo))
            {
                string laudoFinal = procurarEdeletar.Replace(laudo, "");
                txtLaudo.Text = laudoFinal;
                Clipboard.SetText(laudoFinal);
            }



            






            //string laudoFinal = procurarEdeletar.Replace(laudo, "");




            //MatchCollection placeholdersEncontrados = procurarEdeletar.Matches(laudo);

            //Regex.Replace(laudo)

            //for (int contar = 0; contar < placeholdersEncontrados.Count; contar++)
            //{
            //    laudo = laudo.Replace(procurarEdeletar, "");

            //}

        }

        private void brnAdicionarModelosMama_Click(object sender, EventArgs e)
        {
            string caminhoModelos = txtModelosMama.Text;
            string modelo = this.cboModelosMama.GetItemText(this.cboModelosMama.SelectedItem);
            string[] caminhos = { caminhoModelos, modelo };
            string modeloTexto = File.ReadAllText(Path.Combine(caminhos));
            txtLaudo.Text = modeloTexto;
        }

        private void button3_Click_2(object sender, EventArgs e)
        {



            string laudo = txtLaudo.Text;
            string exame = "mama";
            string local = cboMamaLado.Text;            
            string medidas = Volume(txtMamaMedidas.Text);            
            string arquivosFrases = txtFrases.Text;
            int alteracoesIndex = cboMamaAlteracoes.SelectedIndex;

            string mamaLoc = cboMamaLocalizacao.Text;
            string distMamilo = txtMamaMamilo.Text;
            string distPele = txtMamaPele.Text;
            string paredeMioma = "";



            CriarLaudo(laudo, exame, arquivosFrases, alteracoesIndex, local, medidas, mamaLoc,distMamilo,distPele, paredeMioma);



        }

        private void button5_Click(object sender, EventArgs e)
        {

            // retirar tudo que esteja entre parênteses
            string retirarBirads = @"BI-RADS:\s\d";
            string recomedacoes = @"Recomendações:(\s[A-Za-zá-ú-.]*)+";
            var procurarEdeletar = new Regex(retirarBirads);
            var procurarEdeletarRecomedacoes = new Regex(recomedacoes);
            string laudo = txtLaudo.Text;
            

            foreach (Match match in procurarEdeletar.Matches(laudo))
            {
                foreach(Match rec in procurarEdeletarRecomedacoes.Matches(laudo))
                {
                    if (cboMamaBirads.SelectedIndex == 0)
                    {
                        laudo = procurarEdeletar.Replace(laudo, "BI-RADS: 0");

                        laudo = procurarEdeletarRecomedacoes.Replace(laudo, "Recomendações: Sugere-se correlacionar com estudo mamográfico.");
                        //laudoFinal = laudo.Replace("BI-RADS: 1", "BI-RADS: 0").Replace("Recomendações: Prosseguir seguimento.", "Recomendações: Sugere-se correlacionar com estudo mamográfico.");

                    }
                    if (cboMamaBirads.SelectedIndex == 1)
                    {
                        laudo = procurarEdeletar.Replace(laudo, "BI-RADS: 1");
                        
                        laudo = procurarEdeletarRecomedacoes.Replace(laudo, "Recomendações: Sugere-se prosseguir seguimento.");

                    }

                    if (cboMamaBirads.SelectedIndex == 2)
                    {
                        laudo = procurarEdeletar.Replace(laudo, "BI-RADS: 2");
                        laudo = procurarEdeletarRecomedacoes.Replace(laudo, "Recomendações: Achados benignos. Sugere-se prosseguir seguimento.");


                    }
                    if (cboMamaBirads.SelectedIndex == 3)
                    {
                        laudo = procurarEdeletar.Replace(laudo, "BI-RADS: 3");
                        laudo = procurarEdeletarRecomedacoes.Replace(laudo, "Recomendações: Achados provavelmente benignos. Sugere-se correlacionar com novo estudo em 6 meses.");
                        //laudo = laudo.Replace("Recomendações: Prosseguir seguimento.", "Recomendações: Achados provavelmente benignos. Sugere-se correlacionar com novo estudo em 6 meses.");
                    }
                    if (cboMamaBirads.SelectedIndex == 4)
                    {
                        laudo = procurarEdeletar.Replace(laudo, "BI-RADS: 4");
                        laudo = procurarEdeletarRecomedacoes.Replace(laudo, "Recomendações: Achados suspeitos. Sugere-se correlacionar com biópsia da lesão.");
                    }
                    if (cboMamaBirads.SelectedIndex == 5)
                    {
                        laudo = procurarEdeletar.Replace(laudo, "BI-RADS: 5");
                        laudo = procurarEdeletarRecomedacoes.Replace(laudo, "Recomendações: Achados suspeitos. Sugere-se correlacionar com biópsia da lesão.");
                    }
                    if (cboMamaBirads.SelectedIndex == 6)
                    {
                        laudo = procurarEdeletar.Replace(laudo, "BI-RADS: 6");
                        laudo = procurarEdeletarRecomedacoes.Replace(laudo, "Recomendações: Prosseguir com seguimento clínico.");
                    }


                }


            }

            //}
            txtLaudo.Text = laudo;
        }

        private void btnTVModelos_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {

               string modelosTV = folderBrowserDialog1.SelectedPath;
                txtTVModelos.Text = modelosTV;

                string[] files = Directory.GetFiles(modelosTV);
                foreach (string filePath in files) cboTVModelos.Items.Add(Path.GetFileName(filePath));


            }

        }

        private void btnTVAdicionarModelos_Click(object sender, EventArgs e)
        {

            string caminhoModelos = txtTVModelos.Text;
            string modelo = this.cboTVModelos.GetItemText(this.cboTVModelos.SelectedItem);
            string[] caminhos = { caminhoModelos, modelo };
            string modeloTexto = File.ReadAllText(Path.Combine(caminhos));
            txtLaudo.Text = modeloTexto;
        }

        private void btnTVAdicionarAlteracoes_Click(object sender, EventArgs e)
        {


            string laudo = txtLaudo.Text;
            string exameNome = "pélvico";
            string arquivoFrases = txtFrases.Text;
            int alteracoesIndex = cboTVAlteracoes.SelectedIndex;
            string medidas = Volume(txtTVMedidas.Text);
            string paredeMioma = cboTVLocalizacaoMioma.Text;


            string local = cboTVLocalizacao.Text;
            
            string mamaLoc = "";
            string distMamilo = "";
            string distPele = "";

            
            CriarLaudo(laudo, exameNome, arquivoFrases, alteracoesIndex, local, medidas, mamaLoc, distMamilo,distPele,paredeMioma);



     

        }

        private void btnTVAdicionarMedidas_Click(object sender, EventArgs e)
        {
            string laudo = txtLaudo.Text;

            string utero = Volume(txtTVUteroMedidas.Text);
            string endometrio = Volume(txtTVEndometrioMedidas.Text);
            string ovarioDireito = Volume(txtTVOvdMedidas.Text);
            string ovarioEsquerdo = Volume(txtTVOveMedidas.Text);


            laudo = laudo.Replace("{útero}", utero).Replace("{endometrio}", endometrio).Replace("{ovario direito}", ovarioDireito).Replace("{ovario esquerdo}", ovarioEsquerdo);

            txtLaudo.Text = laudo;
            



       

        }

        private void txtTVUteroMedidas_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void btnAbdAdicionarAlteracoes_Click(object sender, EventArgs e)
        {
            string laudo = txtLaudo.Text;
            string exameNome = "abdome";
            string arquivoFrases = txtFrases.Text;
            int alteracoesIndex = cboAbdAlteracoes.SelectedIndex;
            string medidas = Volume(txtAbdMedidas.Text);            
            string local = cboAbdLocalizacao.Text;

            string mamaLoc = "";
            string distMamilo = "";
            string distPele = "";
            string paredeMioma = "";


            CriarLaudo(laudo, exameNome, arquivoFrases, alteracoesIndex, local, medidas, mamaLoc, distMamilo, distPele, paredeMioma);

        }

        private void btnAbdModelos_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {

                string modelosAbd = folderBrowserDialog1.SelectedPath;
                txtAbdModelos.Text = modelosAbd;

                string[] files = Directory.GetFiles(modelosAbd);
                foreach (string filePath in files) cboAbdModelos.Items.Add(Path.GetFileName(filePath));
                cboAbdModelos.Items.Remove("localizações.txt");

                string arquivoLocalizacao = Path.Combine(modelosAbd, "localizações.txt");
                string[] localizacao = File.ReadAllLines(arquivoLocalizacao);
                foreach (string linha in localizacao) cboAbdLocalizacao.Items.Add(linha);


            }
        }

        private void bntAbdAdicionarModelos_Click(object sender, EventArgs e)
        {
            string caminhoModelos = txtAbdModelos.Text;
            string modelo = this.cboAbdModelos.GetItemText(this.cboAbdModelos.SelectedItem);
            string[] caminhos = { caminhoModelos, modelo };
            string modeloTexto = File.ReadAllText(Path.Combine(caminhos));
            txtLaudo.Text = modeloTexto;
        }

        private void cboAbdLocalizacao_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnGenEscolherModelos_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                cboGenAlteracoes.Items.Clear();
                cboGenLocalizacoes.Items.Clear();
                cboGenModelos.Items.Clear();

                string modelosGen = folderBrowserDialog1.SelectedPath;
                txtGenModelos.Text = modelosGen;

                string[] files = Directory.GetFiles(modelosGen);
                foreach (string filePath in files) cboGenModelos.Items.Add(Path.GetFileName(filePath));
                cboGenModelos.Items.Remove("localizações.txt");

                string arquivoLocalizacao = Path.Combine(modelosGen, "localizações.txt");
                string[] localizacao = File.ReadAllLines(arquivoLocalizacao);
                foreach (string linha in localizacao) cboGenLocalizacoes.Items.Add(linha);

                string nomePasta = Path.GetFileName(folderBrowserDialog1.SelectedPath).ToLower();

                // ler o Json
                dynamic frases = JsonConvert.DeserializeObject(File.ReadAllText(txtFrases.Text));

                //funcionou para loop no Json e carregar no cbo

                string zzz = File.ReadAllText(txtFrases.Text);
                dynamic obj = JsonConvert.DeserializeObject(zzz);

               

                nomeDaPasta = nomePasta;

                foreach (var bbb in obj.exame[nomePasta])
                {
                    //txtLaudo.Text += bbb.mme + Environment.NewLine;
                    cboGenAlteracoes.Items.Add(bbb.mme);

                }

                //lblSaida.Text = nomeDaPasta;



            }

        }

        private void btnGenCategoria_Click(object sender, EventArgs e)
        {


        }

        private void btnGenAddModelo_Click(object sender, EventArgs e)
        {
            string caminhoModelos = txtGenModelos.Text;
            string modelo = this.cboGenModelos.GetItemText(this.cboGenModelos.SelectedItem);
            string[] caminhos = { caminhoModelos, modelo };
            string modeloTexto = File.ReadAllText(Path.Combine(caminhos));
            txtLaudo.Text = modeloTexto;

        }

        private void bntGenAdicionar_Click(object sender, EventArgs e)
        {

            string nomePasta = Path.GetFileName(folderBrowserDialog1.SelectedPath);

            string laudo = txtLaudo.Text;
            string exameNome = nomeDaPasta;
            string arquivoFrases = txtFrases.Text;
            int alteracoesIndex = cboGenAlteracoes.SelectedIndex;
            string medidas = Volume(txtGenMedidas.Text);
            string local = cboGenLocalizacoes.Text;

            string mamaLoc = "";
            string distMamilo = "";
            string distPele = "";
            string paredeMioma = "";


            CriarLaudo(laudo, exameNome, arquivoFrases, alteracoesIndex, local, medidas, mamaLoc, distMamilo, distPele, paredeMioma);

        }

        private void btnApagarLaudo_Click(object sender, EventArgs e)
        {
            //string medidasTexto = @"([\d+[,\d]*]*\.\d+)|[\d+[,\d]*]*";

            var resultado = MessageBox.Show("Deseja apagar o laudo?", "Apagar Laudo", MessageBoxButtons.YesNo);
            //var result = MessageBox.Show('Deseja apagar o Laudo?','Apagar Laudo',MessageBoxButtons.YesNo);
            if (resultado == DialogResult.Yes)
            {
                txtLaudo.Clear();
            }
        }

        private void txtLaudo_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {



        }

        private void txtLaudo_KeyPress(object sender, KeyPressEventArgs e)
        {
            //lblSaida.Text = "tecla enter pressionada";
            //string medidasTexto = @"[0-9]+\,?[0-9,]*\s[0-9]+\,?[0-9,]*\s[0-9]+\,?[0-9,]*";
            string medidasTexto = @"\d+(?:,\d{1,5})?\s\d+(?:,\d{1,5})?\s\d+(?:,\d{1,5})?\s\s";
            var procurarEcalcular = new Regex(medidasTexto);
            string laudo = txtLaudo.Text;

            //laudo = Regex.Replace(laudo, medidasTexto, Volume(medidasTexto));
            //txtLaudo.Text = laudo;



            foreach (Match match in procurarEcalcular.Matches(laudo))
            {

                //lblSaida.Text = Volume(match.ToString());
                laudo = laudo.Replace(match.ToString(), Volume(match.ToString()));
                txtLaudo.Text = laudo;




            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (var conn = new System.Data.SQLite.SQLiteConnection("Data Source=D:\\Rafael\\Projetos programacao e eletronica\\Projetos C#\\Laudos - Litedb\\frasesSqlite.db;Version=3;"))
            {
                var modeloExame = cboModelosTireoide.GetItemText(this.cboModelosTireoide.SelectedItem);
                conn.Open();
                using var comandoSql = new SQLiteCommand(conn);
                comandoSql.CommandText = "SELECT * FROM frases WHERE tipo_exame = '"+modeloExame+"'";
                comandoSql.Prepare();
                comandoSql.ExecuteNonQuery();
                SQLiteDataReader entradas = comandoSql.ExecuteReader();
                while (entradas.Read())
                {
                    cboTireoideAlteracoes.Items.Add(entradas["mme"].ToString());
                }                
                conn.Close();
                


            }




        }

        private void txtLaudo_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyData == Keys.F1)
            {
                var laudo = txtLaudo.Text;

                string variavel = "$";

                //int prim = laudo.IndexOf(variavel) + variavel.Length;
                int prim = laudo.IndexOf(variavel);
                //int ult = laudo.LastIndexOf(variavel);

                txtLaudo.SelectionStart = prim;
                txtLaudo.SelectionLength = 1;






                //MessageBox.Show("Tab");
                e.IsInputKey = true;
            }
            //if (e.KeyData == (Keys.Tab | Keys.Shift))
            //{
            //    MessageBox.Show("Shift + Tab");
            //    e.IsInputKey = true;
            //}

        }
    }
}
