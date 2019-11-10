using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    class OMaior : OComparacao, IOperador
    {
        #region IOperator Members

        private string _cadeia = ">";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }
        }

        #endregion

        public OMaior()
        {

        }

        public OMaior(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }

    }
}
