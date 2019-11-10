﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public class OFimSe : Operador, IOperador
    {
        #region IOperador Members

        private string _cadeia = "endif";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }

        }

        #endregion

        public OFimSe()
        {

        }

        public OFimSe(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }
    }
}
