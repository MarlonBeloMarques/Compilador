﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public class OEntao : Operador, IOperador
    {
        #region IOperador Members

        private string _cadeia = "then";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }

        }

        #endregion

        public OEntao()
        {

        }

        public OEntao(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }
    }
}
