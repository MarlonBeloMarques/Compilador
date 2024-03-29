﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public class OMultiplicacao : OMatematico, IOperador
    {
        #region IOperador Members

        private string _cadeia = "*";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }
        }

        #endregion

        public OMultiplicacao()
        {

        }

        public OMultiplicacao(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }
    }
}
