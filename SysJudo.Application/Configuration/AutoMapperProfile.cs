using AutoMapper;
using SysJudo.Domain.Entities;
using SysJudo.Application.Dto.Base;
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

        #region Pais

        CreateMap<Pais, SysJudo.Application.Dto.Pais.PaisDto>().ReverseMap();
        CreateMap<Pais, SysJudo.Application.Dto.Pais.CreatePaisDto>().ReverseMap();
        CreateMap<Pais, SysJudo.Application.Dto.Pais.UpdatePaisDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Pais>, PagedDto<SysJudo.Application.Dto.Pais.PaisDto>>().ReverseMap();

        #endregion

        #region Estado

        CreateMap<Estado, SysJudo.Application.Dto.Estado.EstadoDto>().ReverseMap();
        CreateMap<Estado, SysJudo.Application.Dto.Estado.CreateEstadoDto>().ReverseMap();
        CreateMap<Estado, SysJudo.Application.Dto.Estado.UpdadeEstadoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Estado>, PagedDto<SysJudo.Application.Dto.Estado.EstadoDto>>().ReverseMap();

        #endregion

        #region Cidade

        CreateMap<Cidade, SysJudo.Application.Dto.Cidade.CidadeDto>().ReverseMap();
        CreateMap<Cidade, SysJudo.Application.Dto.Cidade.CreateCidadeDto>().ReverseMap();
        CreateMap<Cidade, SysJudo.Application.Dto.Cidade.UpdateCidadeDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Cidade>, PagedDto<SysJudo.Application.Dto.Cidade.CidadeDto>>().ReverseMap();

        #endregion

        #region Regiao

        CreateMap<Regiao, SysJudo.Application.Dto.Regiao.RegiaoDto>().ReverseMap();
        CreateMap<Regiao, SysJudo.Application.Dto.Regiao.CreateRegiaoDto>().ReverseMap();
        CreateMap<Regiao, SysJudo.Application.Dto.Regiao.UpdateRegiaoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Regiao>, PagedDto<SysJudo.Application.Dto.Regiao.RegiaoDto>>().ReverseMap();

        #endregion

        #region Agremiacao

        CreateMap<Agremiacao, SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>().ReverseMap();
        CreateMap<Agremiacao, SysJudo.Application.Dto.Agremiacao.CadastrarAgremiacaoDto>().ReverseMap();
        CreateMap<Agremiacao, SysJudo.Application.Dto.Agremiacao.AlterarAgremiacaoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Agremiacao>, PagedDto<SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>>().ReverseMap();

        #endregion

        #region Atleta

        CreateMap<Atleta, SysJudo.Application.Dto.Atleta.AtletaDto>().ReverseMap();
        CreateMap<Atleta, SysJudo.Application.Dto.Atleta.CreateAtletaDto>().ReverseMap();
        CreateMap<Atleta, SysJudo.Application.Dto.Atleta.UpdateAtletaDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Atleta>, PagedDto<SysJudo.Application.Dto.Atleta.AtletaDto>>().ReverseMap();

        #endregion
        
        #region EmissoresIdentidade

        CreateMap<EmissoresIdentidade, SysJudo.Application.Dto.EmissoresIdentidade.EmissoresIdentidadeDto>().ReverseMap();
        CreateMap<EmissoresIdentidade, SysJudo.Application.Dto.EmissoresIdentidade.CreateEmissoresIdentidadeDto>().ReverseMap();
        CreateMap<EmissoresIdentidade, SysJudo.Application.Dto.EmissoresIdentidade.UpdateEmissoresIdentidadeDto>().ReverseMap();
        CreateMap<ResultadoPaginado<EmissoresIdentidade>, PagedDto<SysJudo.Application.Dto.EmissoresIdentidade.EmissoresIdentidadeDto>>().ReverseMap();

        #endregion
        
        #region Nacionalidade

        CreateMap<Nacionalidade, SysJudo.Application.Dto.Nacionalidade.NacionalidadeDto>().ReverseMap();
        CreateMap<Nacionalidade, SysJudo.Application.Dto.Nacionalidade.CreateNacionalidadeDto>().ReverseMap();
        CreateMap<Nacionalidade, SysJudo.Application.Dto.Nacionalidade.UpdateNacionalidadeDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Nacionalidade>, PagedDto<SysJudo.Application.Dto.Nacionalidade.NacionalidadeDto>>().ReverseMap();

        #endregion
        
        #region Profissao

        CreateMap<Profissao, SysJudo.Application.Dto.Profissao.ProfissaoDto>().ReverseMap();
        CreateMap<Profissao, SysJudo.Application.Dto.Profissao.CreateProfissaoDto>().ReverseMap();
        CreateMap<Profissao, SysJudo.Application.Dto.Profissao.UpdateProfissaoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<Profissao>, PagedDto<SysJudo.Application.Dto.Profissao.ProfissaoDto>>().ReverseMap();

        #endregion
        
        /* **** Filtros **** */

        #region Agremiacao

        CreateMap<Agremiacao, AgremiacaoFiltro>().ReverseMap();
        CreateMap<AgremiacaoFiltro, SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>().ReverseMap();
        CreateMap<ResultadoPaginado<AgremiacaoFiltro>, PagedDto<SysJudo.Application.Dto.Agremiacao.AgremiacaoDto>>().ReverseMap();


        #endregion
    }
}