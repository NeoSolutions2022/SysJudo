using Microsoft.EntityFrameworkCore;
using SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;
using SysJudo.Domain.Entities.EntitiesFiltros;
using SysJudo.Infra.Abstractions;
using SysJudo.Infra.Context;

namespace SysJudo.Infra.Repositories.RepositoriesFiltros;

public class AgremiacaoFiltroRepository : RepositoryFiltro<AgremiacaoFiltro>, IAgremiacaoFiltroRepository
{
    public AgremiacaoFiltroRepository(BaseApplicationDbContext context) : base(context)
    {
    }

    public void Cadastrar(AgremiacaoFiltro agremiacao)
    {
        Context.AgremiacoesFiltro.Add(agremiacao);
    }

    public void CadastrarTodos(List<AgremiacaoFiltro> agremiacoesFiltros)
    {
        Context.AgremiacoesFiltro.AddRange(agremiacoesFiltros);
    }

    public async Task<List<AgremiacaoFiltro>> Listar()
    {
        return await Context.AgremiacoesFiltro
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task RemoverTodos()
    {
        Context.AgremiacoesFiltro.RemoveRange(await Context.AgremiacoesFiltro.AsNoTracking().ToListAsync());
    }

    public async Task<List<AgremiacaoFiltro>> Pesquisar(string valor)
    {
        return await Context.AgremiacoesFiltro.Where(c =>
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
                (c.Complemento != null && c.Complemento.Contains(valor)) ||
                c.Telefone.Contains(valor) ||
                c.Email.Contains(valor) ||
                c.Cnpj.Contains(valor) ||
                (c.InscricaoMunicipal != null && c.InscricaoMunicipal.Contains(valor)) ||
                (c.InscricaoEstadual != null && c.InscricaoEstadual.Contains(valor)) ||
                c.DataCnpj == ConvertToDateTime(valor) ||
                c.DataAta == ConvertToDateTime(valor) ||
                c.Pais.Contains(valor) || c.Cidade.Contains(valor) ||
                c.Estado.Contains(valor) || c.RegiaoNome.Contains(valor))
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