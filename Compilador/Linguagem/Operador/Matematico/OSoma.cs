using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public class OSoma: OMatematico, IOperador
    {
        #region IOperador Members

        private string _cadeia = "+";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }
        }

        #endregion

        public OSoma()
        {

        }

        public OSoma(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }
    }
}
