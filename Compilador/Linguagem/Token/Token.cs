﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public abstract class Token
    {
        private int _linha;
        public int Linha
        {
            get { return _linha; }
            set { _linha = value; }
        }

        private string _texto;
        public string Texto
        {
            get
            {
                if (this is Valor)
                {
                    _texto = ((Valor)this).NomeVariavel != null ? ((Valor)this).NomeVariavel : ((Valor)this).ValorVariavel.ToString();
                }
                else
                {
                    _texto = ((Operador)this).Cadeia.Valor;
                }

                return _texto;
            }
        }

    }
}
