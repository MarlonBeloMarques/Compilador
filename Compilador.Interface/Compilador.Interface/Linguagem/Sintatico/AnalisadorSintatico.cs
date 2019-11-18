using Compilador.Linguagem.Lexico;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Compilador.Linguagem.Sintatico
{
    public class AnalisadorSintatico
    {
        private Mensagens _mensagens;

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

        AnalisadorLexico _analise;
        public AnalisadorLexico AnaliseLexica
        {
            get
            {
                return _analise;
            }
        }

        private string ExpressaoRegularCadeia()
        {
            //(\"(\w|.|\,|\+|\*|\/|\&|\(|\)|\%|\$|\#||!|\?|\<|\>|\;|\ ) *\") | \w+
            return @"(\" + '"'.ToString() + @"(\w|\.|\:|\,|\-|\+|\*|\/|\&|\(|\)|\%|\$|\#|\@|\!|\?|\<|\>|\;|\ )*\" + '"'.ToString() + @")|\w+";
        }

        private string ExpressaoRegularPermitePontoEmVariavel()
        {
            return @"(\.\w+)*";
        }

        private string ExpressaoRegularOperadoresComparacao()
        {
            StringBuilder sb = new StringBuilder();

            //Operador igual
            sb.Append(@"(\");
            sb.Append(new OIgual().Cadeia.Valor);

            sb.Append(@"|");

            //Operador diferente
            sb.Append(@"\");
            sb.Append(new ODiferente().Cadeia.Valor);

            sb.Append(@"|");

            //Operador maior
            sb.Append(@"\");
            sb.Append(new OMaior().Cadeia.Valor);

            sb.Append(@"|");

            //Operador menor
            sb.Append(@"\");
            sb.Append(new OMenor().Cadeia.Valor);

            sb.Append(@"|");

            //Operador maiorIgual
            sb.Append(@"\");
            sb.Append(new OMaiorIgual().Cadeia.Valor);

            sb.Append(@"|");

            //Operador menorIgual
            sb.Append(@"\");
            sb.Append(new OMenorIgual().Cadeia.Valor);

            sb.Append(@")");

            return sb.ToString();
        }

        private string ExpressaoRegularOperadoresLogicos()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(@"(");

            //Operador and
            sb.Append(new OAnd().Cadeia.Valor);

            sb.Append(@"|");

            //Operador or
            sb.Append(new OOr().Cadeia.Valor);

            sb.Append(@"|");

            return sb.ToString();
        }

        private string ExpressaoRegularOperadoresMatematicos(bool ComEspacoFinal)
        {
            //((\+|\-|\*|\/)|s|w+\s)*
            StringBuilder sb = new StringBuilder();

            if (ComEspacoFinal)
            {
                sb.Append(@"((");
            }
            else
            {
                sb.Append(@"(\s(");
            }

            //Operador Soma
            sb.Append(@"\");
            sb.Append(new OSoma().Cadeia.Valor);

            sb.Append(@"|");

            //Operador Subtracao
            sb.Append(@"\");
            sb.Append(new OSubtracao().Cadeia.Valor);

            sb.Append(@"|");

            //Operador Multiplicacao
            sb.Append(@"\");
            sb.Append(new OMultiplicacao().Cadeia.Valor);

            sb.Append(@"|");

            //Operador Divisao
            sb.Append(@"\");
            sb.Append(new ODivisao().Cadeia.Valor);

            if (ComEspacoFinal)
            {
                sb.Append(@")\s\w+" + ExpressaoRegularPermitePontoEmVariavel() + @"\s)*");
            }
            else
            {
                sb.Append(@")\s\w+" + ExpressaoRegularPermitePontoEmVariavel() + @")*");
            }

            return sb.ToString();
        }

        private string ExpressaoRegularExpressoes()
        {
            //^\w+\s MATEMÁTICO: ((\+|\*|\/)\s\w+\s)* (\=|\<>|\>)+\s\w+ MATEMÁTICO: ((\+|\-|\*|\/)\s\w+\s)* $
            StringBuilder sb = new StringBuilder();
            string OperadoresMatematicos = ExpressaoRegularOperadoresMatematicos(true);
            string OperadoresMatematicosFinais = ExpressaoRegularOperadoresMatematicos(false);
            string OperadoresComparacao = ExpressaoRegularOperadoresComparacao();

            sb.Append(@"^" + ExpressaoRegularCadeia() + ExpressaoRegularPermitePontoEmVariavel() + @"\s");
            sb.Append(OperadoresMatematicos);
            sb.Append(OperadoresComparacao);
            sb.Append(@"+\s" + ExpressaoRegularCadeia() + ExpressaoRegularPermitePontoEmVariavel() + @"");
            sb.Append(OperadoresMatematicosFinais);
            sb.Append(@"$");

            return sb.ToString();
        }

        private string ExpressaoRegularSeEntao()
        {
            StringBuilder sb = new StringBuilder();
            string OperadoresComparacao = ExpressaoRegularOperadoresComparacao();
            string OperadoresLogicos = ExpressaoRegularOperadoresLogicos();
            string OperadoresMatematicos = ExpressaoRegularOperadoresMatematicos(true);

            sb.Append(@"if\s" + ExpressaoRegularCadeia() + ExpressaoRegularPermitePontoEmVariavel() + @"\s");
            sb.Append(OperadoresMatematicos);
            sb.Append(OperadoresComparacao);
            sb.Append(@"\s" + ExpressaoRegularCadeia() + ExpressaoRegularPermitePontoEmVariavel() + @"\s(");
            sb.Append(OperadoresMatematicos);
            sb.Append(OperadoresLogicos);
            sb.Append(@"\s" + ExpressaoRegularCadeia() + ExpressaoRegularPermitePontoEmVariavel() + @"\s");
            sb.Append(OperadoresMatematicos);
            sb.Append(OperadoresComparacao);
            sb.Append(@"\s" + ExpressaoRegularCadeia() + ExpressaoRegularPermitePontoEmVariavel() + @"\s");
            sb.Append(OperadoresMatematicos);
            sb.Append(@") *then");

            return sb.ToString();
        }

        public bool Validar(AnalisadorLexico Analise)
        {
            bool retorno = true;

            _analise = Analise;

            //*** ANÁLISE DAS POSIÇÕES DE CADA TOKEN SE ESTÁ CORRETO
            string lido = "";
            int linha = 0;

            bool dentrodeIF = false;
            bool dentrodeTHEN = false;
            bool dentrodeELSE = false;
            bool ocorreuMudancaEstruturaIf = false;
            bool ocorreuOperadorMatematico = false;
            bool ocorreuOperadorComparacao = false;
            bool ocorreuOperadorLogico = false;

            string ConteudoIf = "";
            string ConteudoThen_PorLinha = "";
            string ConteudoElse_PorLinha = "";

            for (int Pos = 0; Pos < Analise.CodigoFonte.Count; Pos++)
            {
                Token tk = Analise.CodigoFonte[Pos];
                //*** VALIDA SE AS LINHAS NÃO COMEÇAM COM OPERADORES DE COMPARAÇÃO, MATEMATICOS OU LOGICOS (EXCETO IF/ELSE, ELSE/SENAO, ENDIF/FIMSE)

                //*** SE OCORREU MUDANÇA DE LINHA
                if (linha != tk.Linha)
                {
                    lido = "";
                    //** SE O PRIMEIRO TOKEN DA LINHA É UM OPERADOR E NÃO O SE/IF, ENTÃO O SEU USO É INCORRETO
                    if (tk is Operador && !(tk is OSe) && !(tk is OSenao) && !(tk is OFimSe))
                    {
                        this._mensagemErro = "Erro de sintaxe: Uso incorreto do operador no inicio da expressão. Linha " + tk.Linha + ".";
                        retorno = false;
                        break;
                    }
                }
                linha = tk.Linha;

                //*** VALIDA SE AS LINHAS NÃO TERMINAM COM OPERADORES DE COMPARAÇÃO, MATEMATICOS OU IF/ELSE
                //** SE NAO É O ULTIMO TOKEN
                if (Pos < Analise.CodigoFonte.Count - 1)
                {
                    //*** RECUPERA QUAL SERÁ O PROXIMO TOKEN
                    Token ProximoToken = Analise.CodigoFonte[Pos + 1];

                    //*** SE O PROXIMO TOKEN ESTA EM OUTRA LINHA ENTAO ELE É O PRIMEIRO TOKEN DA PROXIMA LINHA E
                    // CONSEQUENTEMENTE ESTAMOS NO ULTIMO TOKEN
                    if (ProximoToken.Linha != linha)
                    {
                        //*** SE FOR OPERADOR DE COMPARAÇÃO OU MATEMÁTICO OU IF/SE
                        if (tk is OComparacao || tk is OMatematico || tk is OSe)
                        {
                            this._mensagemErro = "Erro de sintaxe: Uso incorreto do operador no final da expressão. Linha " + tk.Linha + ".";
                            retorno = false;
                            break;
                        }
                    }
                }

                //*** VALIDA SE SEGUE A SEQUENCIA VALOR -> OPERADOR -> VALOR
                if (tk is Valor)
                {
                    if (lido == "" || lido == "O")
                    {
                        lido = "V";
                    }
                    else
                    {
                        this._mensagemErro = "Erro de sintaxe: Simbolo " + ((Valor)tk).NomeVariavel + " Utilizado de forma incorreta na linha " + linha + ".";
                        retorno = false;
                        break;
                    }
                }
                else if (tk is Operador)
                {
                    if (lido == "" || lido == "V")
                    {
                        lido = "O";
                    }
                    else
                    {
                        this._mensagemErro = "Erro de sintaxe: Operador utilizado de forma incorreta na linha " + linha + ".";
                        retorno = false;
                        break;
                    }
                }

                //***VALIDA SE O IF/SE POSSUI UMA ESTRUTURA CORRETA
                ocorreuMudancaEstruturaIf = false;
                if (tk is OSe)
                {
                    dentrodeIF = true;
                    dentrodeTHEN = false;
                    dentrodeELSE = false;

                    ocorreuMudancaEstruturaIf = true;
                }
                if (tk is OEntao)
                {
                    dentrodeIF = false;
                    dentrodeTHEN = true;
                    dentrodeELSE = false;

                    ocorreuMudancaEstruturaIf = true;
                }
                if (tk is OSenao)
                {
                    dentrodeIF = false;
                    dentrodeTHEN = false;
                    dentrodeELSE = true;

                    ocorreuMudancaEstruturaIf = true;
                }
                if (tk is OFimSe)
                {
                    dentrodeIF = false;
                    dentrodeTHEN = false;
                    dentrodeELSE = false;

                    ocorreuMudancaEstruturaIf = true;
                }

                Token tkAnterior = null;
                Token tkProximo = null;

                if (Pos > 0)
                {
                    tkAnterior = Analise.CodigoFonte[Pos - 1];
                }

                if (Pos < Analise.CodigoFonte.Count - 1)
                {
                    tkProximo = Analise.CodigoFonte[Pos + 1];
                }

                //** REGRAS DE EXPRESSOES DENTRO DE IF, THEN E ELSE

                // 1. OPERADORES LÓGICOS SOMENTE DDEVEM OCORRER DENTRO DE IF
                if (tk is OLogico && !dentrodeIF)
                {
                    this._mensagemErro = "Erro de sintaxe: Operador Lógico " + ((OLogico)tk).Cadeia.Valor + " fora de operador condicional na linha " + linha + ".";
                    retorno = false;
                    break;
                }

                // 2. VALIDA SE FORMATO DO IF ESTÁ CORRETO
                if (dentrodeIF)
                {
                    //SE NESTA ÁREA HOUVER OPERADOR IF, COMPARACAO, MATEMATICO, LOGICO OU VALOR
                    if (tk is OSe)
                    {
                        ConteudoIf += ((OSe)tk).Cadeia.Valor;
                    }
                    else if (tk is OComparacao)
                    {
                        ConteudoIf += ((OComparacao)tk).Cadeia.Valor;
                    }
                    else if (tk is OMatematico)
                    {
                        ConteudoIf += ((OMatematico)tk).Cadeia.Valor;
                    }
                    else if (tk is OLogico)
                    {
                        ConteudoIf += ((OLogico)tk).Cadeia.Valor;
                    }
                    else if (tk is Valor)
                    {
                        if (((Valor)tk).NomeVariavel != null)
                        {
                            ConteudoIf += ((Valor)tk).NomeVariavel;
                        }
                        else
                        {
                            ConteudoIf += ((Valor)tk).ValorVariavel;
                        }
                    }
                    else
                    {
                        this._mensagemErro = "Erro de sintaxe: Operador Condicional " + new OSe().Cadeia.Valor +
                            " com estrutura errada identificado na linha " + linha + ".";
                        retorno = false;
                        break;
                    }

                    //SEPARA AS PALAVRAS POR ESPAÇO
                    ConteudoIf += " ";
                }
                //*** VALIDA O IF FEITO E TAMBEM SE O FORMATO DO THEN ESTA CORRETO
                else if (dentrodeTHEN)
                {
                    if (ConteudoIf != "")
                    {
                        ConteudoIf += new OEntao().Cadeia.Valor;
                        string ER = ExpressaoRegularSeEntao();
                        Match match = Regex.Match(ConteudoIf, ER);

                        if (match.Success)
                        {
                            ConteudoIf = "";
                        }
                        else
                        {
                            this._mensagemErro = "Erro de sintaxe: Operador Condicional " + new OSe().Cadeia.Valor +
                                " com estrutura errada identificado na linha " + linha + ".";
                            retorno = false;
                            break;
                        }
                    }
                    else
                    {
                        // SE NESTA ÁREA HOUVER OPERADOR COMPARACAO, MATEMATICO, LOGICO OU VALOR GUARDA PARA VALIDACAO NA QUEBRA DE LINHA,
                        //SE NAO ESTA ERRADO
                        if (tk is OComparacao)
                        {
                            ConteudoThen_PorLinha += ((OComparacao)tk).Cadeia.Valor;
                        }
                        else if (tk is OMatematico)
                        {
                            ConteudoThen_PorLinha += ((OMatematico)tk).Cadeia.Valor;
                        }
                        else if (tk is OLogico)
                        {
                            ConteudoThen_PorLinha += ((OLogico)tk).Cadeia.Valor;
                        }
                        else if (tk is Valor)
                        {
                            if (((Valor)tk).NomeVariavel != null)
                            {
                                ConteudoThen_PorLinha += ((Valor)tk).NomeVariavel;
                            }
                            else
                            {
                                ConteudoThen_PorLinha += ((Valor)tk).ValorVariavel;
                            }
                        }
                        else
                        {
                            this._mensagemErro = "Erro de sintaxe: Operador Condicional " + new OEntao().Cadeia.Valor +
                                " com estrutura errada identificado na linha " + linha + ".";
                            retorno = false;
                            break;
                        }

                        //*** A CADA MUDANÇA DE LINHA VALIDA SE AS LINHAS DO THEN POSSUI UMA ESTRUTURA DE EXPRESSAO CORRETA
                        if (tkAnterior != null)
                        {
                            if ((!(tk is OEntao) && tk.Linha != tkProximo.Linha) || tkProximo is OSenao || tkProximo is OFimSe)
                            {
                                string ER = ExpressaoRegularExpressoes();
                                Match match = Regex.Match(ConteudoThen_PorLinha, ER);

                                if (match.Success)
                                {
                                    ConteudoElse_PorLinha = "";
                                }
                                else
                                {
                                    this._mensagemErro = "Erro de sintaxe: Operador Condicional " + new OEntao().Cadeia.Valor +
                                        " com expressao errada, identificada na linha " + linha + ".";
                                    retorno = false;
                                    break;
                                }
                            }
                        }
                        //SEPARA AS PLAVRAS POR ESPAÇO
                        if (ConteudoElse_PorLinha != "")
                        {
                            ConteudoElse_PorLinha += " ";
                        }
                    }
                }
                else if (dentrodeELSE)
                {
                    // SE NESTA ÁREA HOUVER OPERADOR COMPARACAO, MATEMATICO, LOGICO OU VALOR GUARDA PARA VALIDACAO NA QUEBRA DE LINHA,
                    //SE NAO ESTA ERRADO
                    if (tk is OComparacao)
                    {
                        ConteudoElse_PorLinha += ((OComparacao)tk).Cadeia.Valor;
                    }
                    else if (tk is OMatematico)
                    {
                        ConteudoElse_PorLinha += ((OMatematico)tk).Cadeia.Valor;
                    }
                    else if (tk is OLogico)
                    {
                        ConteudoElse_PorLinha += ((OLogico)tk).Cadeia.Valor;
                    }
                    else if (tk is Valor)
                    {
                        if (((Valor)tk).NomeVariavel != null)
                        {
                            ConteudoElse_PorLinha += ((Valor)tk).NomeVariavel;
                        }
                        else
                        {
                            ConteudoElse_PorLinha += ((Valor)tk).ValorVariavel;
                        }
                    }
                    else
                    {
                        this._mensagemErro = "Erro de sintaxe: Operador Condicional " + new OSenao().Cadeia.Valor +
                            " com simbolo nao reconhecido, identificado na linha " + linha + ".";
                        retorno = false;
                        break;
                    }

                    if (tkAnterior != null)
                    {
                        if ((!(tk is OSenao) && tk.Linha != tkProximo.Linha) || tkProximo is OFimSe)
                        {
                            if (ConteudoElse_PorLinha.Trim() != "")
                            {
                                string ER = ExpressaoRegularExpressoes();
                                Match match = Regex.Match(ConteudoElse_PorLinha, ER);

                                if (match.Success)
                                {
                                    ConteudoElse_PorLinha = "";
                                }
                                else
                                {
                                    this._mensagemErro = "Erro de sintaxe: Operador Condicional " + new OSenao().Cadeia.Valor +
                                        " com expressao errada identificado na linha " + linha + ".";
                                    retorno = false;
                                    break;
                                }
                            }
                        }
                    }

                    //SEPARA AS PALAVRAS POR ESPAÇO
                    if(ConteudoElse_PorLinha != "")
                    {
                        ConteudoElse_PorLinha += " ";
                    }
                }
                else
                {
                    if(ConteudoIf != "")
                    {
                        this._mensagemErro = "Erro de sintaxe: Operador Condicional " + new OSe().Cadeia.Valor +
                            " sem fechamento do operador " + new OSenao().Cadeia.Valor + " ou " + new OFimSe().Cadeia.Valor + " identidicado na linha " + linha + ".";
                        retorno = false; 
                        break;
                    }
                }
            }

            return retorno;
        }

    }
}
