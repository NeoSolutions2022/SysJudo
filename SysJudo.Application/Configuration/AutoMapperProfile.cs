using AutoMapper;
using SysJudo.Application.Dto.Base;
using SysJudo.Domain.Entities;
using SysJudo.Core.Extension;
using SysJudo.Domain.Entities.EntitiesFiltros;
using SysJudo.Domain.Paginacao;

namespace SysJudo.Application.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Sistema

        CreateMap<Sistema, SysJudo.Application.Dto.Sistema.SistemaDto>().ReverseMap();
        CreateMap<Sistema, SysJudo.Application.Dto.Sistema.CreateSistemaDto>().ReverseMap();
        CreateMap<Sistema, SysJudo.Application.Dto.Sistema.UpdateSistemaDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Sistema>, PagedDto<SysJudo.Application.Dto.Sistema.SistemaDto>>().ReverseMap();

        #endregion

        #region Cliente

        CreateMap<Cliente, SysJudo.Application.Dto.Cliente.ClienteDto>().ReverseMap();
        CreateMap<Cliente, SysJudo.Application.Dto.Cliente.CreateClienteDto>().ReverseMap();
        CreateMap<Cliente, SysJudo.Application.Dto.Cliente.UpdateClienteDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Cliente>, PagedDto<SysJudo.Application.Dto.Cliente.ClienteDto>>().ReverseMap();

        #endregion

        #region Faixa

        CreateMap<Faixa, SysJudo.Application.Dto.Faixa.FaixaDto>().ReverseMap();
        CreateMap<Faixa, SysJudo.Application.Dto.Faixa.CreateFaixaDto>().ReverseMap();
        CreateMap<Faixa, SysJudo.Application.Dto.Faixa.UpdateFaixaDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Faixa>, PagedDto<SysJudo.Application.Dto.Faixa.FaixaDto>>().ReverseMap();

        #endregion

        #region Usuario

        CreateMap<Usuario, SysJudo.Application.Dto.Usuario.UsuarioDto>().ReverseMap();
        CreateMap<Usuario, SysJudo.Application.Dto.Usuario.CreateUsuarioDto>().ReverseMap();
        CreateMap<Usuario, SysJudo.Application.Dto.Usuario.UpdateUsuarioDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Usuario>, PagedDto<SysJudo.Application.Dto.Usuario.UsuarioDto>>().ReverseMap();

        #endregion

        #region Adiministrador

        CreateMap<Administrador, SysJudo.Application.Dto.Administrador.AdministradorDto>().ReverseMap();
        CreateMap<Administrador, SysJudo.Application.Dto.Administrador.CreateAdministradorDto>().ReverseMap();
        CreateMap<Administrador, SysJudo.Application.Dto.Administrador.UpdateAdministradorDto>().ReverseMap();

        #endregion

        #region Regiao

        CreateMap<Regiao, SysJudo.Application.Dto.Regiao.RegiaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros())
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<Regiao, SysJudo.Application.Dto.Regiao.CreateRegiaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros())
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<Regiao, SysJudo.Application.Dto.Regiao.UpdateRegiaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros())
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<ResultadoPaginado<Regiao>, PagedDto<SysJudo.Application.Dto.Regiao.RegiaoDto>>().ReverseMap();

        #endregion

        #region Agremiacao

        CreateMap<Agremiacao, SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros())
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<Agremiacao, SysJudo.Application.Dto.Agremiacao.CadastrarAgremiacaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros())
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<Agremiacao, SysJudo.Application.Dto.Agremiacao.AlterarAgremiacaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros())
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<ResultadoPaginado<Agremiacao>, PagedDto<SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>>()
            .ReverseMap();

        #endregion

        #region Atleta

        CreateMap<Atleta, SysJudo.Application.Dto.Atleta.AtletaDto>().ReverseMap();
        CreateMap<Atleta, SysJudo.Application.Dto.Atleta.CreateAtletaDto>().ReverseMap();
        CreateMap<Atleta, SysJudo.Application.Dto.Atleta.UpdateAtletaDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Atleta>, PagedDto<SysJudo.Application.Dto.Atleta.AtletaDto>>().ReverseMap();

        #endregion

        #region EmissoresIdentidade

        CreateMap<EmissoresIdentidade, SysJudo.Application.Dto.EmissoresIdentidade.EmissoresIdentidadeDto>()
            .ReverseMap();
        CreateMap<EmissoresIdentidade, SysJudo.Application.Dto.EmissoresIdentidade.CreateEmissoresIdentidadeDto>()
            .ReverseMap();
        CreateMap<EmissoresIdentidade, SysJudo.Application.Dto.EmissoresIdentidade.UpdateEmissoresIdentidadeDto>()
            .ReverseMap();
        CreateMap<ResultadoPaginado<EmissoresIdentidade>,
            PagedDto<SysJudo.Application.Dto.EmissoresIdentidade.EmissoresIdentidadeDto>>().ReverseMap();

        #endregion

        #region Nacionalidade

        CreateMap<Nacionalidade, SysJudo.Application.Dto.Nacionalidade.NacionalidadeDto>().ReverseMap();
        CreateMap<Nacionalidade, SysJudo.Application.Dto.Nacionalidade.CreateNacionalidadeDto>().ReverseMap();
        CreateMap<Nacionalidade, SysJudo.Application.Dto.Nacionalidade.UpdateNacionalidadeDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Nacionalidade>, PagedDto<SysJudo.Application.Dto.Nacionalidade.NacionalidadeDto>>()
            .ReverseMap();

        #endregion

        #region Profissao

        CreateMap<Profissao, SysJudo.Application.Dto.Profissao.ProfissaoDto>().ReverseMap();
        CreateMap<Profissao, SysJudo.Application.Dto.Profissao.CreateProfissaoDto>().ReverseMap();
        CreateMap<Profissao, SysJudo.Application.Dto.Profissao.UpdateProfissaoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Profissao>, PagedDto<SysJudo.Application.Dto.Profissao.ProfissaoDto>>()
            .ReverseMap();

        #endregion

        #region RegistroDeEventos

        CreateMap<RegistroDeEvento, SysJudo.Application.Dto.RegistroDeEvento.RegistroDeEventoDto>().ReverseMap();
        CreateMap<RegistroDeEvento, SysJudo.Application.Dto.RegistroDeEvento.AdicionarRegistroDeEvento>().ReverseMap();
        CreateMap<ResultadoPaginado<RegistroDeEvento>,
            PagedDto<SysJudo.Application.Dto.RegistroDeEvento.RegistroDeEventoDto>>().ReverseMap();

        #endregion

        #region GrupoAcesso

        CreateMap<GrupoAcesso, Dto.GruposDeAcesso.GrupoAcessoDto>().ReverseMap();
        CreateMap<GrupoAcesso, Dto.GruposDeAcesso.CadastrarGrupoAcessoDto>().ReverseMap();
        CreateMap<GrupoAcessoUsuario, Dto.Usuario.GrupoAcessoUsuarioDto>().ReverseMap();
        CreateMap<GrupoAcesso, Dto.GruposDeAcesso.AlterarGrupoAcessoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<GrupoAcesso>, PagedDto<Dto.GruposDeAcesso.GrupoAcessoDto>>().ReverseMap();
        
        CreateMap<GrupoAcessoPermissao, Dto.GruposDeAcesso.GrupoAcessoPermissaoDto>().ReverseMap();
        CreateMap<GrupoAcessoPermissao, Dto.GruposDeAcesso.ManterGrupoAcessoPermissaoDto>().ReverseMap();
        CreateMap<PagedDto<Dto.GruposDeAcesso.PermissaoDto>, ResultadoPaginado<Permissao>>()
            .ReverseMap();
        CreateMap<Dto.GruposDeAcesso.PermissaoDto, Permissao>()
            .ReverseMap();
        CreateMap<Dto.Permissoes.CadastrarPermissaoDto, Permissao>()
            .ReverseMap();
        CreateMap<Dto.Permissoes.AlterarPermissaoDto, Permissao>()
            .ReverseMap();

        #endregion

        /* **** Filtros **** */

        #region Agremiacao

        CreateMap<Agremiacao, AgremiacaoFiltro>().ReverseMap();
        CreateMap<AgremiacaoFiltro, SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>().ReverseMap();
        CreateMap<AgremiacaoFiltro, SysJudo.Application.Dto.Agremiacao.AgremiacaoFiltroDto>().ReverseMap();
        CreateMap<ResultadoPaginado<AgremiacaoFiltro>, PagedDto<SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>>()
            .ReverseMap();
        CreateMap<ResultadoPaginado<AgremiacaoFiltro>,
            PagedDto<SysJudo.Application.Dto.Agremiacao.AgremiacaoFiltroDto>>().ReverseMap();

        #endregion

        #region GrupoAcesso

        CreateMap<GrupoAcesso, GrupoDeAcessoFiltro>().ReverseMap();
        CreateMap<GrupoDeAcessoFiltro, SysJudo.Application.Dto.GruposDeAcesso.GrupoAcessoDto>().ReverseMap();
        CreateMap<GrupoDeAcessoFiltro, SysJudo.Application.Dto.GruposDeAcesso.GrupoDeAcessoFiltroDto>().ReverseMap();
        CreateMap<ResultadoPaginado<GrupoDeAcessoFiltro>, PagedDto<SysJudo.Application.Dto.GruposDeAcesso.GrupoAcessoDto>>().ReverseMap();
        CreateMap<ResultadoPaginado<GrupoDeAcessoFiltro>, PagedDto<SysJudo.Application.Dto.GruposDeAcesso.GrupoDeAcessoFiltroDto>>().ReverseMap();

        #endregion
    }
}