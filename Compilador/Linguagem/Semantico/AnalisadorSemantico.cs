// Etapa 1° - Propriedades e métodos de apoio

using Compilador.Linguagem;
using Compilador.Linguagem.Sintatico;
using Linguagem;
using System.Collections.Generic;
using System.Data;
using System.Text;

public class AnalisadorSemantico
{
    private string _mensagemerro;
    public string MensagemErro
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

    AnalisadorSintatico _analise;
    public AnalisadorSintatico AnaliseSintatica
    {
        get
        {
            return _analise;
        }
    }

    CodigoIntermediario _codigoIntermediario = new CodigoIntermediario();
    public CodigoIntermediario Codigo
    {
        get
        {
            return _codigoIntermediario;
        }
    }

    public DataTable getCodigoIntermediario()
    {
        DataTable retorno = new DataTable();
        retorno.Columns.Add("Condicao");
        retorno.Columns.Add("Expressao");
        retorno.Columns.Add("ExpCondicaoNaoAtendida");

        foreach (ExpressaoCodigoIntermediario expressao in _codigoIntermediario.Codigo)
        {
            DataRow DR = retorno.NewRow();
            StringBuilder exp = new StringBuilder();

            // CONDIÇÃO
            foreach (Token tk in expressao.Condicao)
            {
                exp.Append(tk.Texto);
                exp.Append(" ");
            }

            DR["Condicao"] = exp.ToString();

            exp = new StringBuilder();

            // EXPRESSÃO
            foreach (Token tk in expressao.Expressao)
            {
                exp.Append(tk.Texto);
                exp.Append(" ");
            }

            DR["Expressao"] = exp.ToString();

            exp = new StringBuilder();

            // EXPRESSÃO CONDIÇÃO NÃO ATENDIDA
            foreach (Token tk in expressao.ExpressaoCondicaoNaoAtendida)
            {
                exp.Append(tk.Texto);
                exp.Append(" ");
            }

            DR["ExpCondicaoNaoAtendida"] = exp.ToString().Trim();
            retorno.Rows.Add(DR);
        }

        return retorno;
    }

    // Etapa 2 - Validação

    public bool Validar(AnalisadorSintatico Analise)
    {
        bool retorno = true;

        _analise = Analise;



        /* 
            AÇÃO 1: VALIDAR AS SEGUINTES REGRAS
            1. OPERADORES MATEMÁTICOS DEVEM OPERAR EM VALORES DE TIPO DECIMAL, HEXA OU BINARIO
            2. OPERADORES MATEMÁTICOS DEVEM OPERAR EM VALORES DE TIPOS IGUAIS NOS 2 LADOS
            3. OPERADORES DE COMPARAÇÃO MAIOR, MENOR, MAIOR-IGUAL E MENOR-IGUAL DEVEM OPERAR  EM VALORES DE TIPO DECIMAL, HEXA OU BINARIO
            4. OPERADORES DE COMPARAÇÃO DEVEM OPERAR ENTRE VALORES DE TIPOS IGUAIS NOS 2 LADOS
        */

        bool dentrodeIF = false;
        bool dentrodeTHEN = false;
        bool dentrodeELSE = false;
        ExpressaoCodigoIntermediario expressao = new ExpressaoCodigoIntermediario();

        for (int pos = 0; pos < Analise.AnaliseLexica.CodigoFonte.Count; pos++)
        {
            Token tk = Analise.AnaliseLexica.CodigoFonte[pos];
            Token tkAnterior = null;
            Token tkProximo = null;

            int linha = 0;
            linha = tk.Linha;

            if (pos > 0)
            {
                tkAnterior = Analise.AnaliseLexica.CodigoFonte[pos - 1];
            }

            if (pos < Analise.AnaliseLexica.CodigoFonte.Count - 1)
            {
                tkProximo = Analise.AnaliseLexica.CodigoFonte[pos + 1];
            }

            if (tk is OMatematico && tk is OComparacao)
            {
                /* 
                    REGRAS 1 E 2   
                    O ANALISADOR SINTÁTICO GARANTE QUE IRÁ TER 2 NÚMEROS NAS OPERAÇÕES
                    ENTÃO PODEMOS INFERIR QUE O TOKEN ANTERIOR E PROXIMO SÃO VALORES E ASSIM SABER APENAS O TIPO
                */
                if (tk is OMaior || tk is OMenor || tk is OMaiorIgual || tk is OMenorIgual)
                {
                    if (((Valor)tkAnterior).Tipo != Tipos.Dec && ((Valor)tkAnterior).Tipo != Tipos.Hex
                        && ((Valor)tkAnterior).Tipo != Tipos.Bin)
                    {
                        string NomeValor = ((Valor)tkAnterior).NomeVariavel == "" ?
                                            ((Valor)tkAnterior).ValorVariavel.ToString() :
                                            ((Valor)tkAnterior).NomeVariavel;

                        this._mensagemerro = "Os tipo de valor " + NomeValor + " não é de possível comparação na linha: " + linha + ".";
                        retorno = false;
                        break;
                    }
                }
            }
            else
            {
                this._mensagemerro = "Não é possivel efetuar operação aritimética com valores do tipo " + ((Valor)tk).Tipo + ". Erro na linha: " + linha + ".";
                retorno = false;
                break;
            }
            /* 
               REGRAS 3 E 4    
               O ANALISADOR SINTÁTICO GARANTE QUE IRÁ TER 2 NÚMEROS NAS OPERAÇÕES
               ENTÃO PODEMOS INFERIR QUE O TOKEN ANTERIOR E PROXIMO SÃO VALORES E ASSIM SABER APENAS O TIPO
           */
            if (tk is OComparacao)
            {
                if (((Valor)tkAnterior).Tipo == Tipos.Dec || ((Valor)tkAnterior).Tipo == Tipos.Hex
                     || ((Valor)tkAnterior).Tipo == Tipos.Bin)
                {
                    this._mensagemerro = "Os tipos de valores devem ser iguais na comparação da linha: " + linha + ".";
                    retorno = false;
                    break;
                }
                else
                {
                    if (!(tk is OIgual))
                    {
                        this._mensagemerro = "Não é possivel efetuar comparação numérica com valores do tipo " + ((Valor)tkAnterior).Tipo + " e " + ((Valor)tkProximo).Tipo + ". Erro na linha: " + linha + ".";
                        retorno = false;
                        break;
                    }
                }
            }

            // AÇÃO 2: GERAR CÓDIGO INTERMEDIARIO
            if (tkAnterior != null)
            {
                // A CADA QUEBRA DE LINHA SE NÃO UMA NOVA EXPRESSÃO É INSTANCIADA
                if (tkAnterior.Linha != tk.Linha)
                {
                    if (expressao.Expressao.Count > 0 || expressao.ExpressaoCondicaoNaoAtendida.Count > 0)
                    {
                        // ADICIONA A EXPRESSÃO GERADA AO CÓDIGO INTERMEDIÁRIO
                        _codigoIntermediario.AdicionarExpressao(expressao);

                        // SE ESTIVER DENTRO DE UM THEN OU ELSE, A PRÓXIMA EXPRESSÃO DEVE SABER CONDIÇÃO DO IF
                        ExpressaoCodigoIntermediario expressaoTemp = new ExpressaoCodigoIntermediario();
                        if ((dentrodeTHEN || dentrodeELSE) && !(tk is OFimSe))
                        {
                            // COPIE A EXPRESSÃO IF PARA A NOVA EXPRESSOA
                            expressaoTemp.Condicao = expressao.getCopiaCondicao();
                        }

                        // A EXPRESSAO PASSA A SER NOVA EXPRESSAO
                        expressao = expressaoTemp;
                    }

                    // SE NÃO ESTÁ EM CONDIÇÃO E A EXPRESSÃO MANTÉM CONDIÇÃO EM SUA PROPRIEDADE, LIMPA
                    if (tk is OFimSe && expressao.Condicao.Count > 0)
                    {
                        expressao.Condicao = new List<Token>();
                    }
                }
            }

            if (tk is OSe)
            {
                dentrodeIF = true;
                dentrodeTHEN = false;
                dentrodeELSE = false;

                // NESTE PONTO DEVE IR AO PRÓXIMO TOKEN
                continue;
            }

            if (tk is OEntao)
            {
                dentrodeIF = false;
                dentrodeTHEN = true;
                dentrodeELSE = false;

                // NESTE PONTO DEVE IR AO PRÓXIMO TOKEN
                continue;
            }

            if (tk is OSenao)
            {
                dentrodeIF = false;
                dentrodeTHEN = false;
                dentrodeELSE = true;

                // NESTE PONTO DEVE IR AO PRÓXIMO TOKEN
                continue;
            }

            if (tk is OFimSe)
            {
                dentrodeIF = false;
                dentrodeTHEN = false;
                dentrodeELSE = false;

                // NESTE PONTO DEVE IR AO PRÓXIMO TOKEN
                continue;
            }

            if (dentrodeIF)
            {
                expressao.AdicionarTokenEmCondicao(tk);
            }
            else if (dentrodeTHEN)
            {
                expressao.AdicionarTokenEmExpressao(tk);
            }
            else if (dentrodeELSE)
            {
                expressao.AdicionarTokenEmExpressaoCondicaoNaoAtendida(tk);
            }
            else
            {
                expressao.AdicionarTokenEmExpressao(tk);
            }
        }

        // AO FIM DO GIRO SE AINDA HOUVER EXPRESSÃO A SER ADICIONADA, ENTÃO A ADICIONA AO CÓDIGO INTERMEDIARIO
        if (expressao.Expressao.Count > 0 || expressao.ExpressaoCondicaoNaoAtendida.Count > 0)
        {
            // ADICIONA A EXPRESSÃO GERADA AO CÓDIGO INTERMÉDIARIO
            _codigoIntermediario.AdicionarExpressao(expressao);
        }
        return retorno;
    }
    
}
