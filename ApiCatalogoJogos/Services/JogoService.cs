using ApiCatalogoJogos.Entities;
using ApiCatalogoJogos.Exceptions;
using ApiCatalogoJogos.InputModel;
using ApiCatalogoJogos.Repositories;
using ApiCatalogoJogos.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiCatalogoJogos.Services
{
    public class JogoService : IJogoService
    {
        private readonly IJogoRepository _repository;

        public JogoService(IJogoRepository repository)
        {
            _repository = repository;
        }
        public async Task Atualizar(Guid id, JogoInputModel jogo)
        {
            var entidadeJogo = await _repository.Obter(id);

            if (entidadeJogo == null)
                throw new JogoNaoCadastradoException();

            entidadeJogo.Nome = jogo.Nome;
            entidadeJogo.Produtora = jogo.Produtora;
            entidadeJogo.Preco = jogo.Preco;

            await _repository.Atualizar(entidadeJogo);
        }

        public async Task Atualizar(Guid id, double preco)
        {
            var entidadeJogo = await _repository.Obter(id);

            if (entidadeJogo == null)
                throw new JogoNaoCadastradoException();

            entidadeJogo.Preco = preco;

            await _repository.Atualizar(entidadeJogo);
        }        

        public async Task<JogoViewModel> Inserir(JogoInputModel jogo)
        {
            var entidade = await _repository.Obter(jogo.Nome, jogo.Produtora);
            if (entidade.Count >  0)
            {
                throw new JogoJaExisteException();
            }

            var jogoInsert = new Jogo
            {
                Id = Guid.NewGuid(),
                Nome = jogo.Nome,
                Produtora = jogo.Produtora,
                Preco = jogo.Preco
            };

            await _repository.Inserir(jogoInsert);

            return new JogoViewModel
            {
                Id = jogoInsert.Id,
                Nome = jogoInsert.Nome,
                Produtora = jogoInsert.Produtora,
                Preco = jogoInsert.Preco
            };


        }

        public async Task<List<JogoViewModel>> Obter(int pagina, int quantidade)
        {
            var jogos = await _repository.Obter(pagina, quantidade);
            return jogos
                .Select(jogo => new JogoViewModel {
                    Id = jogo.Id,
                    Nome = jogo.Nome,
                    Produtora = jogo.Produtora,
                    Preco = jogo.Preco
                })
                .ToList();
            
        }

        public async Task<JogoViewModel> Obter(Guid idJogo)
        {
            var jogo = await _repository.Obter(idJogo);
            if (jogo == null)
            {
                return null;
            }

            return new JogoViewModel
            {
                Id = jogo.Id,
                Nome = jogo.Nome,
                Produtora = jogo.Produtora,
                Preco = jogo.Preco
            };
            
        }

        public async Task Remover(Guid id)
        {
            var jogo = _repository.Obter(id);
            if(jogo == null)
            {
                throw new JogoNaoCadastradoException();
            }
            await _repository.Remover(id);
        }

        public void Dispose()
        {
            _repository?.Dispose();
        }
    }
}
