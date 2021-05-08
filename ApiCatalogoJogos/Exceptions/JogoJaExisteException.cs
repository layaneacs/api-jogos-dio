using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Exceptions
{
    public class JogoJaExisteException: Exception
    {
        public JogoJaExisteException()
            : base ("Este jogo já esta cadastrado.")
        {

        }
    }
}
