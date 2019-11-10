using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    class ODiferente : OComparacao, IOperador
    {
        #region IOperator Members

        private string _cadeia = "!=";
        public override Cadeia Cadeia
        {
            get
            {
                return new Cadeia(_cadeia);
            }
        }

        #endregion

        public ODiferente()
        {

        }

        public ODiferente(int NumeroLinha)
        {
            this.Linha = NumeroLinha;
        }

    }
}
