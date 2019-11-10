﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public class OSe : Operador, IOperador
    {
        #region IOperador Members

        private string _cadeia = "if";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }

        }

        #endregion

        public OSe()
        {

        }

        public OSe(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }
    }
}
