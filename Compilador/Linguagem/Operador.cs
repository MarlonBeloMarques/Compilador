using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem
{
    public abstract class Operador : Token, IOperador
    {
        #region IOperator Members

        public abstract Cadeia Cadeia
        {
            get;
        }

        #endregion
        
    }
}
