﻿using Minishop.DAL.Base;
using Minishop.Domain.Entity;

namespace Minishop.DAL.Repositories
{
    public interface ISuppliersRepository : IBaseRepository<Supplier>
    {
        Task<List<Supplier>> Pesquisar(int paginaAtual, int qtdPagina);
        Task<Supplier> PesquisaPorId(int id);
        Task<bool> VerificarCnpjExistente(string cnpj);
        Task<bool> VerificarEmailExistente(string email);
        Task<bool> VerificarEmailExistente2(string email, int id);


    }
}
