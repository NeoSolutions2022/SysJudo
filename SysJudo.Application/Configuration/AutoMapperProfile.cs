using AutoMapper;
using SysJudo.Application.Dto.Administrador;
using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Application.Dto.Atleta;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Dto.Cliente;
using SysJudo.Application.Dto.EmissoresIdentidade;
using SysJudo.Application.Dto.Faixa;
using SysJudo.Application.Dto.GruposDeAcesso;
using SysJudo.Application.Dto.Nacionalidade;
using SysJudo.Application.Dto.Permissoes;
using SysJudo.Application.Dto.Profissao;
using SysJudo.Application.Dto.Regiao;
using SysJudo.Application.Dto.RegistroDeEvento;
using SysJudo.Application.Dto.Sistema;
using SysJudo.Application.Dto.Usuario;
using SysJudo.Core.Extension;
using SysJudo.Domain.Entities;
using SysJudo.Domain.Entities.EntitiesFiltros;
using SysJudo.Domain.Paginacao;

namespace SysJudo.Application.Configuration;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region Sistema

        CreateMap<Sistema, SistemaDto>().ReverseMap();
        CreateMap<Sistema, CreateSistemaDto>().ReverseMap();
        CreateMap<Sistema, UpdateSistemaDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Sistema>, PagedDto<SistemaDto>>().ReverseMap();

        #endregion

        #region Cliente

        CreateMap<Cliente, ClienteDto>().ReverseMap();
        CreateMap<Cliente, CreateClienteDto>().ReverseMap();
        CreateMap<Cliente, UpdateClienteDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Cliente>, PagedDto<ClienteDto>>().ReverseMap();

        #endregion

        #region Faixa

        CreateMap<Faixa, FaixaDto>().ReverseMap();
        CreateMap<Faixa, CreateFaixaDto>().ReverseMap();
        CreateMap<Faixa, UpdateFaixaDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Faixa>, PagedDto<FaixaDto>>().ReverseMap();

        #endregion

        #region Usuario

        CreateMap<Usuario, UsuarioDto>().ReverseMap();
        CreateMap<Usuario, CreateUsuarioDto>().ReverseMap();
        CreateMap<Usuario, UpdateUsuarioDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Usuario>, PagedDto<UsuarioDto>>().ReverseMap();

        #endregion

        #region Adiministrador

        CreateMap<Administrador, AdministradorDto>().ReverseMap();
        CreateMap<Administrador, CreateAdministradorDto>().ReverseMap();
        CreateMap<Administrador, UpdateAdministradorDto>().ReverseMap();

        #endregion

        #region Regiao

        CreateMap<Regiao, RegiaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<Regiao, CreateRegiaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<Regiao, UpdateRegiaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros())
            .ReverseMap();
        CreateMap<ResultadoPaginado<Regiao>, PagedDto<RegiaoDto>>().ReverseMap();

        #endregion

        #region Agremiacao

        CreateMap<Agremiacao, AgremiacaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Cnpj = dest.Cnpj.SomenteNumeros()!)
            .ReverseMap();
        CreateMap<Agremiacao, CadastrarAgremiacaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Cnpj = dest.Cnpj.SomenteNumeros()!)
            .ReverseMap();
        CreateMap<Agremiacao, AlterarAgremiacaoDto>()
            .AfterMap((_, dest) => dest.Cep = dest.Cep.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Telefone = dest.Telefone.SomenteNumeros()!)
            .AfterMap((_, dest) => dest.Cnpj = dest.Cnpj.SomenteNumeros()!)
            .ReverseMap();
        CreateMap<ResultadoPaginado<Agremiacao>, PagedDto<AgremiacaoDto>>()
            .ReverseMap();

        #endregion

        #region Atleta

        CreateMap<Atleta, AtletaDto>().ReverseMap();
        CreateMap<Atleta, CreateAtletaDto>().ReverseMap();
        CreateMap<Atleta, UpdateAtletaDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Atleta>, PagedDto<AtletaDto>>().ReverseMap();

        #endregion

        #region EmissoresIdentidade

        CreateMap<EmissoresIdentidade, EmissoresIdentidadeDto>()
            .ReverseMap();
        CreateMap<EmissoresIdentidade, CreateEmissoresIdentidadeDto>()
            .ReverseMap();
        CreateMap<EmissoresIdentidade, UpdateEmissoresIdentidadeDto>()
            .ReverseMap();
        CreateMap<ResultadoPaginado<EmissoresIdentidade>,
            PagedDto<EmissoresIdentidadeDto>>().ReverseMap();

        #endregion

        #region Nacionalidade

        CreateMap<Nacionalidade, NacionalidadeDto>().ReverseMap();
        CreateMap<Nacionalidade, CreateNacionalidadeDto>().ReverseMap();
        CreateMap<Nacionalidade, UpdateNacionalidadeDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Nacionalidade>, PagedDto<NacionalidadeDto>>()
            .ReverseMap();

        #endregion

        #region Profissao

        CreateMap<Profissao, ProfissaoDto>().ReverseMap();
        CreateMap<Profissao, CreateProfissaoDto>().ReverseMap();
        CreateMap<Profissao, UpdateProfissaoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Profissao>, PagedDto<ProfissaoDto>>()
            .ReverseMap();

        #endregion

        #region RegistroDeEventos

        CreateMap<RegistroDeEvento, RegistroDeEventoDto>().ReverseMap();
        CreateMap<RegistroDeEvento, AdicionarRegistroDeEvento>().ReverseMap();
        CreateMap<ResultadoPaginado<RegistroDeEvento>,
            PagedDto<RegistroDeEventoDto>>().ReverseMap();

        #endregion

        #region GrupoAcesso

        CreateMap<GrupoAcesso, GrupoAcessoDto>().ReverseMap();
        CreateMap<GrupoAcesso, CadastrarGrupoAcessoDto>().ReverseMap();
        CreateMap<GrupoAcessoUsuario, GrupoAcessoUsuarioDto>().ReverseMap();
        CreateMap<GrupoAcesso, AlterarGrupoAcessoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<GrupoAcesso>, PagedDto<GrupoAcessoDto>>().ReverseMap();
        
        CreateMap<GrupoAcessoPermissao, GrupoAcessoPermissaoDto>().ReverseMap();
        CreateMap<GrupoAcessoPermissao, ManterGrupoAcessoPermissaoDto>().ReverseMap();
        CreateMap<PagedDto<PermissaoDto>, ResultadoPaginado<Permissao>>()
            .ReverseMap();
        CreateMap<PermissaoDto, Permissao>()
            .ReverseMap();
        CreateMap<CadastrarPermissaoDto, Permissao>()
            .ReverseMap();
        CreateMap<AlterarPermissaoDto, Permissao>()
            .ReverseMap();

        #endregion

        /* **** Filtros **** */

        #region Agremiacao

        CreateMap<Agremiacao, AgremiacaoFiltro>().ReverseMap();
        CreateMap<AgremiacaoFiltro, AgremiacaoDto>().ReverseMap();
        CreateMap<AgremiacaoFiltro, AgremiacaoFiltroDto>().ReverseMap();
        CreateMap<ResultadoPaginado<AgremiacaoFiltro>, PagedDto<AgremiacaoDto>>()
            .ReverseMap();
        CreateMap<ResultadoPaginado<AgremiacaoFiltro>,
            PagedDto<AgremiacaoFiltroDto>>().ReverseMap();

        #endregion

        #region GrupoAcesso

        CreateMap<GrupoAcesso, GrupoDeAcessoFiltro>().ReverseMap();
        CreateMap<GrupoDeAcessoFiltro, GrupoAcessoDto>().ReverseMap();
        CreateMap<GrupoDeAcessoFiltro, GrupoDeAcessoFiltroDto>().ReverseMap();
        CreateMap<ResultadoPaginado<GrupoDeAcessoFiltro>, PagedDto<GrupoAcessoDto>>().ReverseMap();
        CreateMap<ResultadoPaginado<GrupoDeAcessoFiltro>, PagedDto<GrupoDeAcessoFiltroDto>>().ReverseMap();

        #endregion
    }
}