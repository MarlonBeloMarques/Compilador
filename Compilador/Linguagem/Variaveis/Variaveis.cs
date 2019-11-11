using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador.Linguagem.Variaveis
{
    public class Variaveis
    {
        public Variaveis(List<Valor> Variaveis)
        {
            //TODO: CARREGA A LISTA DE VARIAVEIS
            this._listaVariaveis = Variaveis;
        }

        public Valor getVariavel(string Nome)
        {
            Valor retorno = null;
            foreach (Valor item in _listaVariaveis)
            {
                if (item.NomeVariavel == Nome)
                {
                    retorno = item;
                    break;
                }
            }
            return retorno;
        }

        private List<Valor> _listaVariaveis;
        public List<Valor> ListaVariaveis
        {
            get
            {
                return _listaVariaveis;
            }
        }

        public void AdicionarVariavel(Valor Variavel)
        {
            _listaVariaveis.Add(Variavel);
        }

        public bool ExisteVariavel(Valor Variavel)
        {
            bool retorno = false;

            foreach (Valor var in _listaVariaveis)
            {
                if (var.NomeVariavel == Variavel.NomeVariavel)
                {
                    retorno = true;
                    break;
                }
            }

            return retorno;
        }

        public bool ExisteVariavel(string Variavel)
        {
            bool retorno = false;

            Valor vl = new Valor(Variavel, "0", null);
            retorno = ExisteVariavel(vl);

            return retorno;
        }
    }
}
