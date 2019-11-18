using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Compilador.Linguagem.Lexico;
using Compilador.Linguagem.Sintatico;
using Compilador.Linguagem.Variaveis;
using Linguagem;

namespace Compilador.Interface
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Variaveis variaveis = new Variaveis(new List<Linguagem.Valor>());
        CodigoIntermediario codigo = null;

        private void btnLexica_Click(object sender, EventArgs e)
        {
            AnalisadorLexico Al = new AnalisadorLexico();

            if (Al.Validar(txtCodigo.Text, variaveis.ListaVariaveis))
            {
                AnalisadorSintatico Asin = new AnalisadorSintatico();
                if (Asin.Validar(Al))
                {
                    AnalisadorSemantico Asem = new AnalisadorSemantico();
                    if (Asem.Validar(Asin))
                    {
                        dgvCodigo.DataSource = Asem.getCodigoIntermediario();
                        codigo = Asem.Codigo;
                    }
                    else
                    {
                        MessageBox.Show(Asem.MensagemErro);
                    }
                }
                else
                {
                    MessageBox.Show(Asin.MensagemErro);
                }
            }
            else
            {
                MessageBox.Show(Al.MensagemErro);
            }
        }

        private void btnAdicionar_Click(objetc sender, EvetArgs e)
        {
            Linguagem.Valor vl = new Linguagem.Valor(txtNome.Text, txtValor.Text, cmbTipo.Text);
            variaveis.AdicionarVariavel(vl);

            BindingSource bSource = new BindingSource();
            bSource.DataSource = variaveis;
            dgvVariaveis.DataSource = bSource;
            // ((BindingSource) dgvVariaveis.DataSource). Add(vl);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            variaveis.AdicionarVariavel(new Linguagem.Valor("RETORNO", "10", Linguagem.Tipos.Dec));
            variaveis.AdicionarVariavel(new Linguagem.Valor("IGNICAO", "5", Linguagem.Tipos.Dec));
            variaveis.AdicionarVariavel(new Linguagem.Valor("GERAMENSAGEM", "20", Linguagem.Tipos.Dec));

            BindingSource bSource = new BindingSource();
            bSource.DataSource = variaveis.ListaVariaveis;
            dgvVariaveis.DataSource = bSource;
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            Linguagem.Compilador cmp = new Linguagem.Compilador();
            cmp.Executar(codigo, variaveis);

            txtErro.Text = "";

            if (cmp.MensagemErro.Count > 0)
            {
                foreach (string err in cmp.MensagemErro)
                {
                    txtErro.Text += err + "\r\n";
                }
            }
            else
            {
                MessageBox.Show("Validação com sucesso!");
            }
        }
    }
}
