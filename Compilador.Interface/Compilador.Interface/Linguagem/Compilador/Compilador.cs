
using Compilador.Linguagem;
using Linguagem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Compilador.Linguagem
{
    public class Compilador
    {

        private List<string> _mensagemerro = new List<String>();
        public List<string> MensagemErro
        {

            get
            {
                return _mensagemerro;
            }

            set
            {
                _mensagemerro = value;
            }
        }

        public void Executar(CodigoIntermediario Codigo, Variaveis.Variaveis ListaVariaveis)
        {
            foreach (ExpressaoCodigoIntermediario expressao in Codigo.Codigo)
            {

                //*** EXECUTA CADA EXPRESSÃO DO CODIGO INTERMEDIÁRIO
                ExecutarExpressao(expressao);
            }
        }

        private void ExecutarExpressao(ExpressaoCodigoIntermediario expressao)
        {

            //*** SE A EXPRESSÃO ESTIVER SOB CONDIÇÃO (IF), PRECISA VER QUAL INSTRUÇÃO SER EXECUTADA
            if (expressao.Condicao.Count > 0)
            {

                //** SE A CONDIÇÃO FOR VÁLIDA, ENTÃO EXECUTA A INSTRUÇÃO NA PROPRIEDADE EXPRESSÃO SENÃO A INSTRUÇÃO
                // DA PROPRIEDADE EXPRESSACONDICAONAOATENDIDA
                if (CondicaoExpressaoValida(expressao.Condicao))
                {
                    if (expressao.Expressao.Count > 0)
                    {
                        ExecutarInstrucao(expressao.Expressao);
                    }
                }
                else
                {
                    if (expressao.ExpressaoCondicaoNaoAtendida.Count > 0)
                    {
                        ExecutarInstrucao(expressao.ExpressaoCondicaoNaoAtendida);
                    }
                }
            }
            else
            {
                //*** CASO NÃO ESTEJA SOB CONDICAO EXECUTA A INSTRUCAO DENTRO DA EXPRESSAO
                ExecutarInstrucao(expressao.Expressao);
            }
        }

        private bool CondicaoExpressaoValida(List<Token> Condicao)
        {
            bool retorno = true;

            //*** PARA CADA TOKEN, AGRUPA EM INSTRUÇÃO E COM A CONDIÇÃO FORMADA VERIFICA SUA
            // VALIDADE (TRUE OU FALSE) COM SEUS OPERADORES LÓGICOS (AND OU OR)

            StringBuilder sb = new StringBuilder();
            foreach (Token tk in Condicao)
            {

                if (tk is Valor)
                {
                    sb.Append(((Valor)tk).ValorVariavel);
                }
                if (tk is OMatematico)
                {
                    sb.Append(tk.Texto);
                }
                if (tk is OLogico)
                {
                    sb.Append(tk.Texto);
                }
                if (tk is OComparacao)
                {
                    sb.Append(tk.Texto);
                }
                sb.Append(" ");
                retorno = ValidarBooleano(sb.ToString());
                return retorno;
            }

            return retorno;
        }

        private bool ExecutarInstrucao(List<Token> Instrucao)
        {
            bool retorno = true;

            ///*** PARA CADA TOKEN, AGRUPA EM INSTRUÇÃO E EXECUTA A INSTRUÇÃO PERMITINDO A REGRA OU GERANDO ERRO DE REGRA VIOLADA.

            StringBuilder sb = new StringBuilder();
            StringBuilder sbTexto = new StringBuilder();

            foreach (Token tk in Instrucao)
            {

                if (tk is Valor)
                {
                    sb.Append(((Valor)tk).ValorVariavel);
                    sbTexto.Append(tk.Texto);
                }
                if (tk is OMatematico)
                {
                    sb.Append(tk.Texto);
                    sbTexto.Append(tk.Texto);
                }

                if (tk is OComparacao)
                {
                    sb.Append(tk.Texto);
                    sbTexto.Append(tk.Texto);
                }
                sb.Append(" ");
                sbTexto.Append(" ");

            }
            retorno = ValidarBooleano(sb.ToString());

            if (!retorno)
            {
                _mensagemerro.Add("Regra Violada: " + sbTexto.ToString() + " (" + sb.ToString() + ")");
            }
            return retorno;

        }

        private bool ValidarBooleano(string instrucao)
        {

            ///*** SUBSTITUI ASPAS DUPLAS POR ASPAS SIMPLES PORQUE O CLR DO DATABASE (SQL) UTILIZA ASPAS SIMPLES PARA
            // SUAS STRINGS E NOSSA LINGUAGEM UTILIZA ASPAS DUPLAS 
            instrucao = instrucao.Replace('"'.ToString(), "'");

            DataTable table = new DataTable();
            table.Columns.Add("expression", string.Empty.GetType(), instrucao);
            System.Data.DataRow row = table.NewRow();
            table.Rows.Add(row);
            return bool.Parse((string)row["expression"]);
        }
    }

}