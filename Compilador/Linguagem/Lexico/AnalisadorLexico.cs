using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem.Lexico
{
    public class AnalisadorLexico
    {
        private string Espaco = "°";

        private string _mensagemErro;
        public string MensagemErro
        {
            get
            {
                return _mensagemErro;
            }
            set
            {
                _mensagemErro = value;
            }
        }

        private Variaveis.Variaveis var = null;
        public Variaveis.Variaveis Variaveis
        {
            get
            {
                return var;
            }
        }

        private List<Token> _codigoFonte;
        public List<Token> CodigoFonte
        {
            get
            {
                if (_codigoFonte == null)
                {
                    _codigoFonte = new List<Token>();
                }
                return _codigoFonte;
            }
        }

        public bool Validar(string Codigo, List<Valor> ListaVariaveis)
        {
            bool retorno = true;

            //TODO: SUBSTITUI + de 1 ESPAÇOS POR ESPAÇO SOMENTE FORA DE STRINGS IDENTIFICADO POR COMEÇR COM (") E TERMINAR COM (")
            //TODO: SUBSTITUI ESPAÇO PELA VARIAVEL Espaço PARA NÃO DAR PROBLEMA NA INDENTIFICAÇÃO DE TOKENS STRINGS COM ESPAÇO QUANDO FIZER SPLIT
            string codigoRemontado = "";
            bool dentroDeString = false;
            for (int pos = 0; pos < Codigo.Length; pos++)
            {
                char letra = Codigo[pos];
                char letraAnterior = new char();
                char proximaLetra = new char();

                if (pos > 0)
                {
                    letraAnterior = Codigo[pos - 1];
                }

                if (pos < Codigo.Length - 1)
                {
                    proximaLetra = Codigo[pos + 1];
                }

                if (letra == '"')
                {
                    if (!dentroDeString)
                    {
                        dentroDeString = true;
                    }
                    else
                    {
                        dentroDeString = false;
                    }
                }

                if (letra == '\n' && dentroDeString)
                {
                    break;
                }
                else if (dentroDeString)
                {
                    codigoRemontado += letra.ToString();
                }
                else if (letra == ' ')
                {
                    if (letraAnterior != ' ' &&
                        letraAnterior != '\n' &&
                        proximaLetra != '\n')
                    {
                        codigoRemontado += Espaco;
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    codigoRemontado += letra.ToString();
                }
            }

            if (dentroDeString)
            {
                this._mensagemErro = "Sequencia String de valores não fechada.";
                return false;
            }

            Codigo = codigoRemontado;

            //TODO: SEPARA ENTER DOS OUTROS CARACTERES COM ESPAÇO
            Codigo = Codigo.Replace("\n", Espaco + "\n" + Espaco);

            string[] Tokens = Codigo.Split(Convert.ToChar(Espaco));
            int Linha = 1;

            for (int pos = 0; pos < Tokens.Length; pos++)
            {
                string valor = Tokens[pos] != "\n" ? Tokens[pos].Trim() : Tokens[pos];

                //TODO: IDENTIFICA AS LINHAS POR ENTER
                if (valor == "\n")
                {
                    Linha++;
                    continue;
                }
                else if (valor == "")
                {
                    continue;
                }

                var = new Variaveis.Variaveis(ListaVariaveis);
                Int64 numeroConvertido = 0;

                //TODO: SE FOR UMA STRING
                if (valor[0] == '"')
                {
                    CodigoFonte.Add(new Valor(numeroConvertido.ToString(), Tipos.Dec, Linha));
                }

                //TODO: SE É UM IF
                else if (new OSe().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OSe(Linha));
                }

                //TODO: SE É UM THEN
                else if (new OEntao().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OEntao(Linha));
                }

                //TODO: SE É UM ELSE
                else if (new OSenao().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OSenao(Linha));
                }

                //TODO: SE É UM ENDIF
                else if (new OFimSe().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OFimSe(Linha));
                }

                //TODO: SE É UM IGUAL
                else if (new OIgual().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OIgual(Linha));
                }

                //TODO: SE É UM DIFERENTE
                else if (new ODiferente().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new ODiferente(Linha));
                }

                //TODO: SE É UM MAIOR
                else if (new OMaior().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OMaior(Linha));
                }

                //TODO: SE É UM MENOR
                else if (new OMenor().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OMenor(Linha));
                }

                //TODO: SE É UM MAIOR IGUAL
                else if (new OMaiorIgual().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OMaiorIgual(Linha));
                }

                //TODO: SE É UM MENOR IGUAL
                else if (new OMenorIgual().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OMenorIgual(Linha));
                }

                //TODO: SE É UM SOMA
                else if (new OSoma().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OSoma(Linha));
                }

                //TODO: SE É UM SUBTRAÇÃO
                else if (new OSubtracao().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OSubtracao(Linha));
                }

                //TODO: SE É UM MULTIPLICACAO
                else if (new OMultiplicacao().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OMultiplicacao(Linha));
                }

                //TODO: SE É UM DIVISAO
                else if (new ODivisao().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new ODivisao(Linha));
                }

                //TODO: SE É UM OR
                else if (new OOr().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OOr(Linha));
                }

                //TODO: SE É UM AND
                else if (new OAnd().Cadeia.Valor == valor)
                {
                    CodigoFonte.Add(new OAnd(Linha));
                }

                //TODO: SE É UM VAZIO, PULA
                else if (valor == "")
                {
                    continue;
                }

                //TODO: SE NÃO ENTROU EM ENHUM DOS CASOS ANTERIORES, O SIMBOLO NÃO É RECONHECIDO
                else
                {
                    this._mensagemErro = "Simbolo " + valor + " não reconhecido na linha " + Linha + ".";
                    retorno = false;
                    break;
                }

            }

            return retorno;
        }
    }
}
