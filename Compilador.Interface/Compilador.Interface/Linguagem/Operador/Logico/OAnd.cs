using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public class OAnd : OLogico, IOperador
    {
        #region IOperador Members

        private string _cadeia = "&&";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }

        }

        #endregion

        public OAnd()
        {

        }

        public OAnd(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }
    }
}
