
public partial class Form1: Form
{
     public Form1()
     {
          InitializeComponent ();
     }

     Linguagem.Variaveis variaveis = new Linguagem.Variaveis (new List<Linguagem.Valor>());
     Linguagem.CodigoIntermediario codigo = null;

     private void btnLexica_Click(object sender, EventArgs e)
     {
          Linguagem.AnalisadorLexico Al = new Linguagem.AnalisadorLexico();

          if (Al.Validar(txtCodigo.Text, variaveis.ListaVariaveis))
          {
               Linguagem.AnalisadorSintatico Asin = new Linguagem.AnalisadorSintatico();
               if (Asin.Validar(Al))
               {
                  Linguagem.AnalisadorSemantico Asem = new Linguagem.AnalisadorSemantico();
                  if (Asem.Validar(Asin))
                  {
                      dgvCodigo.DataSource = Asem.getCodigoIntermediario();
                      codigo = Asem.Codigo;
                  }
                  else 
                  {
                       MessageBox.Show (Asem.MensagemErro);
                  }
               }
               else 
               {
                   MessageBox.Show (Asin.MensagemErro);
               }
            }
            else 
            {
                MessageBox.Show (Al.MensagemErro);
            }
       }

        private void btnAdicionar_Click (objetc sender, EvetArgs e)
        {
             Linguagem.Valor vl = new Linguagem.Valor (txtNome.Text, txtValor.Text, cmbTipo.Text);
             variaveis.AdicionarVariavel (vl);

             BindingSource bSource = new  BindingSource();
             bSource.DataSource = variaveis;
             dgvVariaveis.DataSource = bSource;
             // ((BindingSource) dgvVariaveis.DataSource). Add(vl);
        } 

        private void Form1_Load (object sender, EventArgs e)
        {
            variaveisAdicionarVariavel(new Linguagem.Valor ("RETORNO", "10", Linguagem.Tipos.Dec));
            variaveisAdicionarVariavel(new Linguagem.Valor ("IGNICAO", "5", Linguagem.Tipos.Dec));
            variaveisAdicionarVariavel(new Linguagem.Valor ("GERAMENSAGEM", "20", Linguagem.Tipos.Dec));

            BindingSource bSource = new BindingSource();
            bSource.DataSource = variaveis.ListaVariaveis;
            dgvVariaveis.DataSource = bSource;
         }

         private void btnCompilar_Click(object sender, EventArgs e)
         {
               Linguagem.Compilador cmp = new Linguagem.Compilador();
               cmp.Executar (codigo, variaveis);

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