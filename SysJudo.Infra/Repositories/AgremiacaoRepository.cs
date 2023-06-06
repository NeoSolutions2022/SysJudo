using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Paginacao;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Entities;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories;

public class AgremiacaoRepository : Repository<Agremiacao>, IAgremiacaoRepository
{
    public AgremiacaoRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Cadastrar(Agremiacao agremiacao)
    {
        Context.Agremiacoes.Add(agremiacao);
    }

    public void Alterar(Agremiacao agremiacao)
    {
        Context.Agremiacoes.Update(agremiacao);
    }

    public async Task<Agremiacao?> Obter(int id)
    {
        return await Context.Agremiacoes
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public void Deletar(Agremiacao agremiacao)
    {
        Context.Agremiacoes.Remove(agremiacao);
    }

    public async Task<IResultadoPaginado<Agremiacao>> Buscar(IBuscaPaginada<Agremiacao> filtro)
    {
        var query = Context.Agremiacoes
            .Include(c => c.Regiao).AsQueryable();
        return await base.Buscar(query, filtro);
    }

    public async Task<List<Agremiacao>> ObterTodos()
    {
        return await Context.Agremiacoes
            .Include(c => c.Regiao)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Agremiacao>> Pesquisar(string valor)
    {
        return await Context.Agremiacoes.Include(c => c.Regiao).Where(c =>
                c.Sigla.Contains(valor) ||
                c.Nome.Contains(valor) ||
                (c.Fantasia != null && c.Fantasia.Contains(valor)) ||
                c.Responsavel.Contains(valor) ||
                c.Representante.Contains(valor) ||
                c.DataFiliacao == ConvertToDateTime(valor) ||
                c.DataNascimento == ConvertToDateTime(valor) ||
                c.Cep.Contains(valor) ||
                c.Endereco.Contains(valor) ||
                c.Bairro.Contains(valor) ||
                c.Complemento.Contains(valor) ||
                c.Telefone.Contains(valor) ||
                c.Email.Contains(valor) ||
                c.Cnpj.Contains(valor) ||
                (c.InscricaoMunicipal != null && c.InscricaoMunicipal.Contains(valor)) ||
                (c.InscricaoEstadual != null && c.InscricaoEstadual.Contains(valor)) ||
                c.DataCnpj == ConvertToDateTime(valor) ||
                c.DataAta == ConvertToDateTime(valor) ||
                c.Pais.Contains(valor) || 
                c.Cidade.Contains(valor) ||
                c.Estado.Contains(valor) || 
                c.Regiao.Descricao.Contains(valor))
            .AsNoTracking()
            .ToListAsync();
    }

    private DateOnly ConvertToDateTime(string data)
    {
        if (DateOnly.TryParse(data, out var result))
        {
            return result;
        }
        else
        {
            return new DateOnly(9999, 01, 01);
        }
    }
}