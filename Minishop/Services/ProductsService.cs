﻿using Minishop.DAL.Repositories;
using Minishop.Domain.DTO;
using Minishop.Services;
using Minishop.Services.Base;

namespace Minishop.Services
{
    public class ProductsService
    {
        //usando o CustomersRepository via injeção de dependência:
        private readonly ProductsRepository _productsRepository;

        public ProductsService(ProductsRepository productsRepository)
        {
            _productsRepository = productsRepository;
        }

        public async Task<ProductCountResponse> Contar()
        {
            int quantidadeTotal = await _productsRepository.Contagem();
            int quantidadeProdutosAtivos = await _productsRepository.ContagemProdutosAtivos();
            int quantidadeProdutosInativos = await _productsRepository.ContagemProdutosInativos();

            var response = new ProductCountResponse
            {
                ItemCount = quantidadeTotal,
                ActiveCount = quantidadeProdutosAtivos,
                InactiveCount = quantidadeProdutosInativos
            };

            return response;
        }

        public async Task<ServicePagedResponse<ProductsResponse>> Pesquisar(PageQueryRequest queryResquest)
        {
            //Lista Customers com paginação
            {
                // Consulta itens no banco
                var listaPesquisa = await _productsRepository.Pesquisar(
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
                // Conta itens do banco
                var contagem = await _productsRepository.Contagem();
                // Transforma Product em ProductResponse
                var listaConvertida = listaPesquisa
                    .Select(product => new ProductsResponse(product));

                // Cria resultado com paginação
                return new ServicePagedResponse<ProductsResponse>(
                    listaConvertida,
                    contagem,
                    queryResquest.PaginaAtual,
                    queryResquest.Quantidade
                );
            }
            //No método de listagem de todos os products, os usos do método Select da biblioteca Linq
            //funcionam como um transformador para cada objeto da lista;

        }
    }
}