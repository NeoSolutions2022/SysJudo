using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Agremiacao;

public class BuscarAgremiacaoDto : BuscaPaginadaDto<Domain.Entities.Agremiacao>
{
    public string? Sigla { get; set; }
    public string? Nome { get; set; }
    public string? Responsavel { get; set; }
    public string? Cep { get; set; } = null!;
    public string? Endereco { get; set; } = null!;
    public string? Bairro { get; set; } = null!;
    public string? Complemento { get; set; } = null!;
    public string? Telefone { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string? Cnpj { get; set; } = null!;
    public string? Representante { get; set; } = null!;
    public string? Anotacoes { get; set; }
    public string? Pais { get; set; } = null!;
    public string? Cidade { get; set; } = null!;
    public string? Estado { get; set; } = null!;
    public int? IdRegiao { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Agremiacao> query)
    {
        var expression = MontarExpressao();

        if (!string.IsNullOrWhiteSpace(Sigla))
        {
            query = query.Where(a => a.Sigla.Contains(Sigla));
        }

        if (!string.IsNullOrWhiteSpace(Cidade))
        {
            query = query.Where(a => a.Cidade.Contains(Cidade));
        }

        if (!string.IsNullOrWhiteSpace(Estado))
        {
            query = query.Where(a => a.Estado.Contains(Estado));
        }

        if (!string.IsNullOrWhiteSpace(Pais))
        {
            query = query.Where(a => a.Pais.Contains(Pais));
        }

        if (IdRegiao.HasValue)
        {
            query = query.Where(s => s.IdRegiao == IdRegiao);
        }

        if (!string.IsNullOrWhiteSpace(Representante))
        {
            query = query.Where(a => a.Representante.Contains(Representante));
        }

        if (!string.IsNullOrWhiteSpace(Anotacoes))
        {
            query = query.Where(a => a.Anotacoes.Contains(Anotacoes));
        }

        if (!string.IsNullOrWhiteSpace(Nome))
        {
            query = query.Where(a => a.Nome.Contains(Nome));
        }

        if (!string.IsNullOrWhiteSpace(Responsavel))
        {
            query = query.Where(a => a.Responsavel.Contains(Responsavel));
        }

        if (!string.IsNullOrWhiteSpace(Cep))
        {
            query = query.Where(a => a.Cep.Contains(Cep));
        }

        if (!string.IsNullOrWhiteSpace(Endereco))
        {
            query = query.Where(a => a.Endereco.Contains(Endereco));
        }

        if (!string.IsNullOrWhiteSpace(Bairro))
        {
            query = query.Where(a => a.Bairro.Contains(Bairro));
        }

        if (!string.IsNullOrWhiteSpace(Complemento))
        {
            query = query.Where(a => a.Complemento.Contains(Complemento));
        }

        if (!string.IsNullOrWhiteSpace(Telefone))
        {
            query = query.Where(a => a.Telefone.Contains(Telefone));
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            query = query.Where(a => a.Email.Contains(Email));
        }

        if (!string.IsNullOrWhiteSpace(Cnpj))
        {
            query = query.Where(a => a.Cnpj.Contains(Cnpj));
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Agremiacao> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "id" => query.OrderBy(c => c.Id),
                "reponsavel" => query.OrderBy(c => c.Responsavel),
                "nome" => query.OrderBy(c => c.Nome),
                "complemento" => query.OrderBy(c => c.Complemento),
                "telefone" => query.OrderBy(c => c.Telefone),
                "cnpj" => query.OrderBy(c => c.Cnpj),
                "email" => query.OrderBy(c => c.Telefone),
                "cep" => query.OrderBy(c => c.Cep),
                "endereco" => query.OrderBy(c => c.Endereco),
                "bairro" => query.OrderBy(c => c.Nome),
                "representante" => query.OrderBy(c => c.Representante),
                "anotacoes" => query.OrderBy(c => c.Anotacoes),
                "cidade" => query.OrderBy(c => c.Cidade),
                "estado" => query.OrderBy(c => c.Estado),
                "pais" => query.OrderBy(c => c.Pais),
                "idregiao" => query.OrderBy(c => c.IdRegiao),
                "sigla" or _ => query.OrderBy(c => c.Sigla)
            };
            return;
        }

        query = OrdenarPor.ToLower() switch
        {
            "id" => query.OrderByDescending(c => c.Id),
            "reponsavel" => query.OrderByDescending(c => c.Responsavel),
            "nome" => query.OrderByDescending(c => c.Nome),
            "complemento" => query.OrderByDescending(c => c.Complemento),
            "telefone" => query.OrderByDescending(c => c.Telefone),
            "cnpj" => query.OrderByDescending(c => c.Cnpj),
            "email" => query.OrderByDescending(c => c.Telefone),
            "cep" => query.OrderByDescending(c => c.Cep),
            "endereco" => query.OrderByDescending(c => c.Endereco),
            "bairro" => query.OrderByDescending(c => c.Nome),
            "representante" => query.OrderByDescending(c => c.Representante),
            "anotacoes" => query.OrderByDescending(c => c.Anotacoes),
            "cidade" => query.OrderByDescending(c => c.Cidade),
            "estado" => query.OrderByDescending(c => c.Estado),
            "pais" => query.OrderByDescending(c => c.Pais),
            "idregiao" => query.OrderByDescending(c => c.IdRegiao),
            "sigla" or _ => query.OrderByDescending(c => c.Sigla)
        };
    }
}