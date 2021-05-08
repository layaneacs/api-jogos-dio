using ApiCatalogoJogos.Exceptions;
using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.Services;
using ApiCatalogoJogos.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JogosController : ControllerBase
    {

        private readonly IJogoService _service;

        public JogosController(IJogoService service)
        {
            _service = service;
        }

        /// <summary>
        /// Buscar todos os jogos de forma paginada
        /// </summary>
        /// <remarks>
        /// Não é possivel retornar os jogos sem paginação
        /// </remarks>
        /// <param name="pagina">Indica qual página esta sendo consultada. Minímo 1</param>
        /// <param name="quatidade">Indica a quantidade de itens por página. Minímo 1 e máximo 50.</param>
        /// <response code="200">Retorna a lista de jogos</response>
        /// <response code="204">Caso não haja jogos</response>
        [HttpGet]        
        public async Task<ActionResult<IEnumerable<JogoViewModel>>> Obter([FromQuery, Range(1 , int.MaxValue)] int pagina = 1, [FromQuery, Range(1, 50)] int quatidade = 5)
        {
            //-- forçando um erro para verificar o funcionamento do middleware
            // throw new Exception();

            var jogos = await _service.Obter(pagina, quatidade);
            if(jogos.Count() == 0)
            {
                return NoContent();
            }
            return Ok(jogos);
        }

        [HttpGet("{idJogo:guid}")]
        public async Task<ActionResult<JogoViewModel>> Obter([FromRoute] Guid idJogo)
        {
            var jogo = await _service.Obter(idJogo);
            if(jogo == null)
            {
                return NoContent();
            }
            return Ok(jogo);
        }

        [HttpPost]
        public async Task<ActionResult<JogoViewModel>> Inserir([FromBody] JogoInputModel jogoInput)
        {
            try
            {
                var jogo = await _service.Inserir(jogoInput);
                return Ok(jogo);
            }
            catch (JogoJaExisteException ex)            
            {
                return UnprocessableEntity("Ja existe um jogo com esse nome para essa produtora");
            }
        }
        
        [HttpPut("{idJogo:guid}")] //--Atualizo o objeto inteiro
        public async Task<ActionResult> Atualizar([FromRoute] Guid idJogo, [FromBody] JogoInputModel jogoInput)
        {
            try
            {
                await _service.Atualizar(idJogo, jogoInput);
                return Ok(jogoInput);
            }
            catch (JogoNaoCadastradoException ex)            
            {
                return UnprocessableEntity("não existe esse jogo");
            }
        }

        [HttpPatch("{idJogo:guid}/preco/{preco:double}")] //--Atualizo apenas uma parte
        public async Task<ActionResult> Atualizar([FromRoute] Guid idJogo, [FromRoute] double preco)
        {
            try
            {
                await _service.Atualizar(idJogo, preco);
                return Ok();
            }
            catch (JogoNaoCadastradoException ex)            
            {
                return UnprocessableEntity("não existe esse jogo");
            }
            
        }

        [HttpDelete("{idJogo:guid}")]
        public async Task<ActionResult> Deletar([FromRoute] Guid idJogo)
        {
            try
            {
                await _service.Remover(idJogo);
                return Ok();
                   
            }
            catch (JogoNaoCadastradoException ex)            
            {
                return UnprocessableEntity("não existe esse jogo");
            }
        }
    }
}
