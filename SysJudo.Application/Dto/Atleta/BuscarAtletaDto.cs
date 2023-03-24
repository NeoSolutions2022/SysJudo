using SysJudo.Application.Dto.Base;
using SysJudo.Core.Extension;

namespace SysJudo.Application.Dto.Atleta;

public class BuscarAtletaDto : BuscaPaginadaDto<Domain.Entities.Atleta>
{
    public string? RegistroFederacao { get; set; } = null!;
    public string? RegistroConfederacao { get; set; } = null!;
    public string? Nome { get; set; } = null!;
    public DateTime? DataNascimento { get; set; }
    public DateTime? DataFiliacao { get; set; }
    public string? Cep { get; set; } = null!;
    public string? Endereco { get; set; } = null!;
    public string? Bairro { get; set; }
    public string? Telefone { get; set; } = null!;
    public string? Email { get; set; } = null!;
    public string? Cpf { get; set; } = null!;
    public string? Identidade { get; set; } = null!;
    public int? IdFaixa { get; set; }
    public int? IdSexo { get; set; }
    public int? IdEstadoCivil { get; set; }
    public int? IdNacionalidade { get; set; }
    public string? Cidade { get; set; }
    public string? Estado { get; set; }
    public string? Pais { get; set; }
    public int? IdAgremiacao { get; set; }

    public override void AplicarFiltro(ref IQueryable<Domain.Entities.Atleta> query)
    {
        var expression = MontarExpressao();

        if (!string.IsNullOrWhiteSpace(RegistroFederacao))
        {
            query = query.Where(c => c.RegistroFederacao.Contains(RegistroFederacao));
        }

        if (!string.IsNullOrWhiteSpace(RegistroConfederacao))
        {
            query = query.Where(c => c.RegistroConfederacao!.Contains(RegistroConfederacao));
        }

        if (!string.IsNullOrWhiteSpace(Nome))
        {
            query = query.Where(c => c.Nome.Contains(Nome));
        }

        if (DataNascimento.HasValue)
        {
            query = query.Where(c => c.DataNascimento == DataNascimento);
        }

        if (DataFiliacao.HasValue)
        {
            query = query.Where(c => c.DataFiliacao == DataFiliacao);
        }

        if (!string.IsNullOrWhiteSpace(Cep))
        {
            query = query.Where(c => c.Cep.Contains(Cep));
        }

        if (!string.IsNullOrWhiteSpace(Endereco))
        {
            query = query.Where(c => c.Endereco.Contains(Endereco));
        }

        if (!string.IsNullOrWhiteSpace(Bairro))
        {
            query = query.Where(c => c.Bairro != null && c.Bairro.Contains(Bairro));
        }

        if (!string.IsNullOrWhiteSpace(Telefone))
        {
            query = query.Where(c => c.Telefone.Contains(Telefone));
        }

        if (!string.IsNullOrWhiteSpace(Email))
        {
            query = query.Where(c => c.Email.Contains(Email));
        }

        if (!string.IsNullOrWhiteSpace(Cpf))
        {
            query = query.Where(c => c.Cpf.Contains(Cpf));
        }

        if (!string.IsNullOrWhiteSpace(Identidade))
        {
            query = query.Where(c => c.Identidade.Contains(Identidade));
        }

        if (IdFaixa.HasValue)
        {
            query = query.Where(c => c.IdFaixa == IdFaixa);
        }

        if (IdSexo.HasValue)
        {
            query = query.Where(c => c.IdSexo == IdSexo);
        }

        if (IdEstadoCivil.HasValue)
        {
            query = query.Where(c => c.IdEstadoCivil == IdEstadoCivil);
        }

        if (IdNacionalidade.HasValue)
        {
            query = query.Where(c => c.IdNacionalidade == IdNacionalidade);
        }

        if (!string.IsNullOrWhiteSpace(Cidade))
        {
            query = query.Where(c => c.Identidade.Contains(Cidade));
        }

        if (!string.IsNullOrWhiteSpace(Estado))
        {
            query = query.Where(c => c.Identidade.Contains(Estado));
        }

        if (!string.IsNullOrWhiteSpace(Pais))
        {
            query = query.Where(c => c.Identidade.Contains(Pais));
        }

        if (IdAgremiacao.HasValue)
        {
            query = query.Where(c => c.IdAgremiacao == IdAgremiacao);
        }

        query = query.Where(expression);
    }

    public override void AplicarOrdenacao(ref IQueryable<Domain.Entities.Atleta> query)
    {
        if (DirecaoOrdenacao.EqualsIgnoreCase("asc"))
        {
            query = OrdenarPor.ToLower() switch
            {
                "registrofederação" => query.OrderBy(c => c.RegistroFederacao),
                "registroconfereração" => query.OrderBy(c => c.RegistroConfederacao),
                "nome" => query.OrderBy(c => c.Nome),
                "datanascimento" => query.OrderBy(c => c.DataNascimento),
                "datafiliação" => query.OrderBy(c => c.DataFiliacao),
                "cep" => query.OrderBy(c => c.Cep),
                "endereço" => query.OrderBy(c => c.Endereco),
                "bairro" => query.OrderBy(c => c.Bairro),
                "telefone" => query.OrderBy(c => c.Telefone),
                "email" => query.OrderBy(c => c.Email),
                "cpf" => query.OrderBy(c => c.Cpf),
                "identidade" => query.OrderBy(c => c.Identidade),
                "idfaixa" => query.OrderBy(c => c.IdFaixa),
                "idsexo" => query.OrderBy(c => c.IdSexo),
                "idestadocivil" => query.OrderBy(c => c.IdEstadoCivil),
                "idnacionalidade" => query.OrderBy(c => c.IdNacionalidade),
                "cidade" => query.OrderBy(c => c.Cidade),
                "estado" => query.OrderBy(c => c.Estado),
                "pais" => query.OrderBy(c => c.Pais),
                "idagremiacao" => query.OrderBy(c => c.IdAgremiacao),
                "id" or _ => query.OrderBy(c => c.Id)
            };
            return;
        }

        query = OrdenarPor.ToLower() switch
        {
            "registrofederação" => query.OrderByDescending(c => c.RegistroFederacao),
            "registroconfereração" => query.OrderByDescending(c => c.RegistroConfederacao),
            "nome" => query.OrderByDescending(c => c.Nome),
            "datanascimento" => query.OrderByDescending(c => c.DataNascimento),
            "datafiliação" => query.OrderByDescending(c => c.DataFiliacao),
            "cep" => query.OrderByDescending(c => c.Cep),
            "endereço" => query.OrderByDescending(c => c.Endereco),
            "bairro" => query.OrderByDescending(c => c.Bairro),
            "telefone" => query.OrderByDescending(c => c.Telefone),
            "email" => query.OrderByDescending(c => c.Email),
            "cpf" => query.OrderByDescending(c => c.Cpf),
            "identidade" => query.OrderByDescending(c => c.Identidade),
            "idfaixa" => query.OrderByDescending(c => c.IdFaixa),
            "idsexo" => query.OrderByDescending(c => c.IdSexo),
            "idestadocivil" => query.OrderByDescending(c => c.IdEstadoCivil),
            "idnacionalidade" => query.OrderByDescending(c => c.IdNacionalidade),
            "cidade" => query.OrderByDescending(c => c.Cidade),
            "estado" => query.OrderByDescending(c => c.Estado),
            "pais" => query.OrderByDescending(c => c.Pais),
            "idagremiacao" => query.OrderByDescending(c => c.IdAgremiacao),
            "id" or _ => query.OrderByDescending(c => c.Id)
        };
    }
}