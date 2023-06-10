using System.Text;
using AutoMapper;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SysJudo.Application.Contracts;
using SysJudo.Application.Dto.Agremiacao;
using SysJudo.Application.Dto.Base;
using SysJudo.Application.Notifications;
using SysJudo.Core.Enums;
using SysJudo.Core.Extension;
using SysJudo.Domain.Contracts.Repositories;
using SysJudo.Domain.Contracts.Repositories.RepositoriesFiltros;
using SysJudo.Domain.Entities;
using SysJudo.Domain.Entities.EntitiesFiltros;

namespace SysJudo.Application.Services;

public partial class AgremiacaoService : BaseService, IAgremiacaoService
{
    private readonly IAgremiacaoRepository _agremiacaoRepository;
    private readonly IAgremiacaoFiltroRepository _filtroRepository;
    private readonly IFileService _fileService;
    private readonly HttpContextAccessor _httpContextAccessor;
    private readonly IRegiaoRepository _regiaoRepository;

    public AgremiacaoService(IMapper mapper, INotificator notificator, IAgremiacaoRepository agremiacaoRepository,
        IFileService fileService, IAgremiacaoFiltroRepository filtroRepository,
        IRegistroDeEventoRepository registroDeEventoRepository, IOptions<HttpContextAccessor> httpContextAccessor,
        IRegiaoRepository regiaoRepository) :
        base(mapper, notificator, registroDeEventoRepository)
    {
        _regiaoRepository = regiaoRepository;
        _httpContextAccessor = httpContextAccessor.Value;
        _agremiacaoRepository = agremiacaoRepository;
        _fileService = fileService;
        _filtroRepository = filtroRepository;
    }

    public async Task<AgremiacaoDto?> Cadastrar(CadastrarAgremiacaoDto dto)
    {
        if (!ValidarFoto(dto))
        {
            return null;
        }

        var agremiacao = Mapper.Map<Agremiacao>(dto);
        if (!await Validar(agremiacao))
        {
            return null;
        }

        var nomeDoc = new StringBuilder();
        nomeDoc.Append("Documentos anexados=");
        var links = new StringBuilder();
        if (dto.Documentos != null)
        {
            foreach (var documento in dto.Documentos.Where(documento => documento is { Length: > 0 }))
            {
                links.Append(agremiacao.DocumentosUri + "&" +
                             await _fileService.Upload(documento, EUploadPath.FotosAgremiacao));
                nomeDoc.Append($"{documento.FileName};");
            }
        }

        agremiacao.DocumentosUri = links.ToString();

        if (dto.Foto is { Length: > 0 })
        {
            agremiacao.Foto = await _fileService.Upload(dto.Foto, EUploadPath.FotosAgremiacao);
        }

        _agremiacaoRepository.Cadastrar(agremiacao);
        var regiao = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            var dataCnpj = "NULL";
            if (agremiacao.DataCnpj != null)
            {
                dataCnpj = new DateTime(agremiacao.DataCnpj.Value.Year, agremiacao.DataCnpj.Value.Month,
                    agremiacao.DataCnpj.Value.Day).ToString("dd/MM/yyyy");
            }

            var dataAta = "NULL";
            if (agremiacao.DataAta != null)
            {
                dataAta = new DateTime(agremiacao.DataAta.Value.Year, agremiacao.DataAta.Value.Month,
                    agremiacao.DataAta.Value.Day).ToString("dd/MM/yyyy");
            }

            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao =
                    $"Sigla={agremiacao.Sigla};Nome={agremiacao.Nome};Fantasia={agremiacao.Fantasia};Responsavel={agremiacao.Responsavel};Representante={agremiacao.Representante};Data de filiacao={new DateTime(agremiacao.DataFiliacao.Year, agremiacao.DataFiliacao.Month, agremiacao.DataFiliacao.Day):dd/MM/yyyy};Data de nascimento={new DateTime(agremiacao.DataNascimento.Year, agremiacao.DataNascimento.Month, agremiacao.DataNascimento.Day):dd/MM/yyyy};Cep={agremiacao.Cep};Endereco={agremiacao.Endereco};Bairro={agremiacao.Bairro};Complemento={agremiacao.Complemento};Cidade={agremiacao.Cidade};Estado={agremiacao.Estado};Pais={agremiacao.Pais};Telefone={agremiacao.Telefone};Email={agremiacao.Email};Cnpj={agremiacao.Cnpj};Inscricao municipal={agremiacao.InscricaoMunicipal};Inscricao estadual={agremiacao.InscricaoEstadual};Data Cnpj={dataCnpj};Data Ata={dataAta};Foto={agremiacao.Foto};Alvara de locacao={agremiacao.AlvaraLocacao};Estatuto={agremiacao.Estatuto};Contrato social={agremiacao.ContratoSocial};Documentacao atualizada={agremiacao.DocumentacaoAtualizada};Regiao={regiao?.Descricao};Anotacoes={agremiacao.Anotacoes};{nomeDoc};",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 4,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 2
            });
            await RegistroDeEventos.UnitOfWork.Commit();
            return Mapper.Map<AgremiacaoDto>(agremiacao);
        }

        Notificator.Handle("Não foi possível cadastrar a agremiação");
        return null;
    }

    public async Task<AgremiacaoDto?> Alterar(int id, AlterarAgremiacaoDto dto)
    {
        var caminhoFoto = "";
        if (id != dto.Id)
        {
            Notificator.Handle("Os ids não conferem");
            return null;
        }

        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var agremiacaoInicial = await _agremiacaoRepository.Obter(id);
        var regiaoInicial = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);
        var regiao = await _regiaoRepository.ObterPorId(dto.IdRegiao);

        if (dto.Foto is { Length: > 0 })
        {
            caminhoFoto = await ManterFoto(dto.Foto);
        }

        var dataCnpj = "NULL";
        if (agremiacao.DataCnpj != null)
        {
            dataCnpj = new DateTime(agremiacao.DataCnpj.Value.Year, agremiacao.DataCnpj.Value.Month,
                agremiacao.DataCnpj.Value.Day).ToString("dd/MM/yyyy");
        }

        var dataAta = "NULL";
        if (agremiacao.DataAta != null)
        {
            dataAta = new DateTime(agremiacao.DataAta.Value.Year, agremiacao.DataAta.Value.Month,
                agremiacao.DataAta.Value.Day).ToString("dd/MM/yyyy");
        }

        var dataCnpjInicial = "NULL";
        if (agremiacaoInicial!.DataCnpj != null)
        {
            dataCnpj = new DateTime(agremiacaoInicial.DataCnpj.Value.Year, agremiacaoInicial.DataCnpj.Value.Month,
                agremiacao.DataCnpj!.Value.Day).ToString("dd/MM/yyyy");
        }

        var dataAtaInicial = "NULL";
        if (agremiacaoInicial.DataAta != null)
        {
            dataAta = new DateTime(agremiacaoInicial.DataAta.Value.Year, agremiacaoInicial.DataAta.Value.Month,
                agremiacaoInicial.DataAta.Value.Day).ToString("dd/MM/yyyy");
        }

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao =
                $"Sigla={agremiacaoInicial.Sigla};Nome={agremiacaoInicial.Nome};Fantasia={agremiacaoInicial.Fantasia};Responsavel={agremiacaoInicial.Responsavel};Representante={agremiacaoInicial.Representante};DataFiliacao={new DateTime(agremiacao.DataFiliacao.Year, agremiacao.DataFiliacao.Month, agremiacao.DataFiliacao.Day).ToString("dd/MM/yyyy")};DataNascimento={new DateTime(agremiacao.DataNascimento.Year, agremiacao.DataNascimento.Month, agremiacao.DataNascimento.Day).ToString("dd/MM/yyyy")};Cep={agremiacaoInicial.Cep};Endereco={agremiacaoInicial.Endereco};Bairro={agremiacaoInicial.Bairro};Complemento={agremiacaoInicial.Complemento};Cidade={agremiacaoInicial.Cidade};Estado={agremiacaoInicial.Estado};Pais={agremiacaoInicial.Pais};Telefone={agremiacaoInicial.Telefone};Email={agremiacaoInicial.Email};Cnpj={agremiacaoInicial.Cnpj};InscricaoMunicipal={agremiacaoInicial.InscricaoMunicipal};InscricaoEstadual={agremiacaoInicial.InscricaoEstadual};DataCnpj={dataCnpjInicial};DataAta={dataAtaInicial};Foto={agremiacaoInicial.Foto};AlvaraLocacao={agremiacaoInicial.AlvaraLocacao};Estatuto={agremiacaoInicial.Estatuto};ContratoSocial={agremiacaoInicial.ContratoSocial};DocumentacaoAtualizada={agremiacaoInicial.DocumentacaoAtualizada};Regiao={regiaoInicial?.Descricao};Anotacoes={agremiacaoInicial.Anotacoes};<br>" +
                $"Sigla={dto.Sigla};Nome={dto.Nome};Fantasia={dto.Fantasia};Responsavel={dto.Responsavel};Representante={dto.Representante};DataFiliacao={new DateTime(agremiacao.DataFiliacao.Year, agremiacao.DataFiliacao.Month, agremiacao.DataFiliacao.Day).ToString("dd/MM/yyyy")};DataNascimento={new DateTime(agremiacao.DataNascimento.Year, agremiacao.DataNascimento.Month, agremiacao.DataNascimento.Day).ToString("dd/MM/yyyy")};Cep={dto.Cep};Endereco={dto.Endereco};Bairro={dto.Bairro};Complemento={dto.Complemento};Cidade={dto.Cidade};Estado={dto.Estado};Pais={dto.Pais};Telefone={dto.Telefone};Email={dto.Email};Cnpj={dto.Cnpj};InscricaoMunicipal={dto.InscricaoMunicipal};InscricaoEstadual={dto.InscricaoEstadual};DataCnpj={dataCnpj};DataAta={dataAta};Foto={caminhoFoto};AlvaraLocacao={dto.AlvaraLocacao};Estatuto={dto.Estatuto};ContratoSocial={dto.ContratoSocial};DocumentacaoAtualizada={dto.DocumentacaoAtualizada};Regiao={regiao?.Descricao};Anotacoes={dto.Anotacoes}",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 5,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        Mapper.Map(dto, agremiacao);
        if (!await Validar(agremiacao))
        {
            return null;
        }

        agremiacao.Foto = caminhoFoto;
        _agremiacaoRepository.Alterar(agremiacao);

        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<AgremiacaoDto>(agremiacao);
        }

        Notificator.Handle("Não foi possível alterar a agremiação");
        return null;
    }

    #region Exportar

    public async Task<string> Exportar(ExportarAgremiacaoDto dto)
    {
        var descricao = new StringBuilder();
        descricao.Append("Colunas= ");
        var linha = 2;
        var agremiacoes = await _filtroRepository.Listar();
        var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("CADAGRE" + DateTime.Now.ToString("yyyyMMDDHHmmss"));

        #region Ordenacao

        if (dto.Ordenacao != null)
        {
            if (dto.Ordenacao.Propriedade == "Nome")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Nome.ToLower(), obj2.Nome.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Nome.ToLower(), obj1.Nome.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Sigla")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Sigla.ToLower(), obj2.Sigla.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Sigla.ToLower(), obj1.Sigla.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Fantasia")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes = agremiacoes.OrderBy(c => c.Fantasia).ToList();
                }
                else
                {
                    agremiacoes = agremiacoes.OrderByDescending(c => c.Fantasia).ToList();
                }
            }

            if (dto.Ordenacao.Propriedade == "Responsavel")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Responsavel.ToLower(), obj2.Responsavel.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Responsavel.ToLower(), obj1.Responsavel.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Representante")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Representante.ToLower(), obj2.Representante.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Representante.ToLower(), obj1.Representante.ToLower(), StringComparison.Ordinal));
                }
            }
            
            if (dto.Ordenacao.Propriedade == "Foto")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes = agremiacoes.OrderBy(c => c.Foto).ToList();
                    
                }
                else
                {
                    agremiacoes = agremiacoes.OrderByDescending(c => c.Foto).ToList();
                    
                }
            }

            if (dto.Ordenacao.Propriedade == "DataFiliacao")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes = agremiacoes.OrderBy(c => c.DataFiliacao).ToList();
                }
                else
                {
                    agremiacoes = agremiacoes.OrderByDescending(c => c.DataFiliacao).ToList();
                }
            }
            
            if (dto.Ordenacao.Propriedade == "DataAta")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes = agremiacoes.OrderBy(c => c.DataAta).ToList();
                }
                else
                {
                    agremiacoes = agremiacoes.OrderByDescending(c => c.DataAta).ToList();
                }
            }
            
            if (dto.Ordenacao.Propriedade == "DataCnpj")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes = agremiacoes.OrderBy(c => c.DataCnpj).ToList();
                }
                else
                {
                    agremiacoes = agremiacoes.OrderByDescending(c => c.DataCnpj).ToList();
                }
            }

            if (dto.Ordenacao.Propriedade == "DataNascimento")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes = agremiacoes.OrderBy(c => c.DataNascimento).ToList();
                }
                else
                {
                    agremiacoes = agremiacoes.OrderByDescending(c => c.DataNascimento).ToList();
                }
            }

            if (dto.Ordenacao.Propriedade == "Cep")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Cep.ToString(), obj2.Cep.ToString(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Cep.ToString(), obj1.Cep.ToString(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Endereco")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Endereco.ToLower(), obj2.Endereco.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Endereco.ToLower(), obj1.Endereco.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Bairro")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Bairro.ToLower(), obj2.Bairro.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Bairro.ToLower(), obj1.Bairro.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Complemento")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes = agremiacoes.OrderBy(c => c.Complemento).ToList();
                    
                }
                else
                {
                    agremiacoes = agremiacoes.OrderByDescending(c => c.Complemento).ToList();
                    
                }
            }

            if (dto.Ordenacao.Propriedade == "Cidade")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Cidade.ToLower(), obj2.Cidade.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Cidade.ToLower(), obj1.Cidade.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Estado")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Estado.ToLower(), obj2.Estado.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Estado.ToLower(), obj1.Estado.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Regiao")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.RegiaoNome.ToLower(), obj2.RegiaoNome.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.RegiaoNome.ToLower(), obj1.RegiaoNome.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Pais")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Pais.ToLower(), obj2.Pais.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Pais.ToLower(), obj1.Pais.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Telefone")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Telefone.ToString(), obj2.Telefone.ToString(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Telefone.ToString(), obj1.Telefone.ToString(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Email")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Email.ToLower(), obj2.Email.ToLower(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Email.ToLower(), obj1.Email.ToLower(), StringComparison.Ordinal));
                }
            }

            if (dto.Ordenacao.Propriedade == "Cnpj")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.Cnpj.ToString(), obj2.Cnpj.ToString(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.Cnpj.ToString(), obj1.Cnpj.ToString(), StringComparison.Ordinal));
                }
            }
            
            if (dto.Ordenacao.Propriedade == "InscricaoMunicipal")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.InscricaoMunicipal?.ToString(), obj2.InscricaoMunicipal?.ToString(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.InscricaoMunicipal?.ToString(), obj1.InscricaoMunicipal?.ToString(), StringComparison.Ordinal));
                }
            }
            
            if (dto.Ordenacao.Propriedade == "InscricaoEstadual")
            {
                if (dto.Ordenacao.Ascendente)
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj1.InscricaoEstadual?.ToString(), obj2.InscricaoEstadual?.ToString(), StringComparison.Ordinal));
                }
                else
                {
                    agremiacoes.Sort((obj1, obj2) =>
                        String.Compare(obj2.InscricaoEstadual?.ToString(), obj1.InscricaoEstadual?.ToString(), StringComparison.Ordinal));
                }
            }
        }

        #endregion

        foreach (var agremiacao in agremiacoes)
        {
            var contador = 1;
            if (dto.Sigla)
            {
                descricao.Append("Sigla;");
                ws.Cell(1, contador).Value = "Sigla";
                ws.Cell(linha, contador).Value = agremiacao.Sigla;
                contador++;
            }
            
            if (dto.Nome)
            {
                descricao.Append("Nome;");
                ws.Cell(1, contador).Value = "Nome";
                ws.Cell(linha, contador).Value = agremiacao.Nome;
                contador++;
            }

            if (dto.Fantasia)
            {
                descricao.Append("Fantasia;");
                ws.Cell(1, contador).Value = "Fantasia";
                ws.Cell(linha, contador).Value = agremiacao.Fantasia == null ? "NULL" : agremiacao.Fantasia;
                contador++;
            }

            if (dto.Responsavel)
            {
                descricao.Append("Responsavel;");
                ws.Cell(1, contador).Value = "Responsavel";
                ws.Cell(linha, contador).Value = agremiacao.Responsavel;
                contador++;
            }

            if (dto.Representante)
            {
                descricao.Append("Representante;");
                ws.Cell(1, contador).Value = "Representante";
                ws.Cell(linha, contador).Value = agremiacao.Representante;
                contador++;
            }

            if (dto.DataFiliacao)
            {
                descricao.Append("Data Filiacao;");
                ws.Cell(1, contador).Value = "Data Filiacao";
                ws.Cell(linha, contador).Value = new DateTime(agremiacao.DataFiliacao.Year,
                    agremiacao.DataFiliacao.Month,
                    agremiacao.DataFiliacao.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.DataNascimento)
            {
                descricao.Append("Data Nascimento;");
                ws.Cell(1, contador).Value = "Data Nascimento";
                ws.Cell(linha, contador).Value = new DateTime(agremiacao.DataNascimento.Year,
                    agremiacao.DataNascimento.Month,
                    agremiacao.DataNascimento.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.Cep)
            {
                descricao.Append("Cep;");
                ws.Cell(1, contador).Value = "Cep";
                ws.Cell(linha, contador).Value = agremiacao.Cep;
                contador++;
            }

            if (dto.Endereco)
            {
                descricao.Append("Endereco;");
                ws.Cell(1, contador).Value = "Endereco";
                ws.Cell(linha, contador).Value = agremiacao.Endereco;
                contador++;
            }

            if (dto.Bairro)
            {
                descricao.Append("Bairro;");
                ws.Cell(1, contador).Value = "Bairro";
                ws.Cell(linha, contador).Value = agremiacao.Endereco;
                contador++;
            }

            if (dto.Complemento)
            {
                descricao.Append("Complemento;");
                ws.Cell(1, contador).Value = "Complemento";
                ws.Cell(linha, contador).Value = agremiacao.Complemento;
                contador++;
            }

            if (dto.Cidade)
            {
                descricao.Append("Cidade;");
                ws.Cell(1, contador).Value = "Cidade";
                ws.Cell(linha, contador).Value = agremiacao.Cidade;
                contador++;
            }

            if (dto.Estado)
            {
                descricao.Append("Estado;");
                ws.Cell(1, contador).Value = "Estado";
                ws.Cell(linha, contador).Value = agremiacao.Estado;
                contador++;
            }

            if (dto.Pais)
            {
                descricao.Append("Pais;");
                ws.Cell(1, contador).Value = "Pais";
                ws.Cell(linha, contador).Value = agremiacao.Pais;
                contador++;
            }

            if (dto.Telefone)
            {
                descricao.Append("Telefone;");
                ws.Cell(1, contador).Value = "Telefone";
                ws.Cell(linha, contador).Value = agremiacao.Telefone;
                contador++;
            }

            if (dto.Email)
            {
                descricao.Append("Email;");
                ws.Cell(1, contador).Value = "Email";
                ws.Cell(linha, contador).Value = agremiacao.Email;
                contador++;
            }

            if (dto.Cnpj)
            {
                descricao.Append("Cnpj;");
                ws.Cell(1, contador).Value = "Cnpj";
                ws.Cell(linha, contador).Value = agremiacao.Cnpj;
            }
            
            if (dto.InscricaoMunicipal)
            {
                descricao.Append("Inscricao Municipal;");
                ws.Cell(1, contador).Value = "Inscricao Municipal";
                ws.Cell(linha, contador).Value =
                    agremiacao.InscricaoMunicipal == null ? "NULL" : agremiacao.InscricaoMunicipal;
                contador++;
            }

            if (dto.InscricaoEstadual)
            {
                descricao.Append("Inscricao Estadual;");
                ws.Cell(1, contador).Value = "Inscricao Estadual";
                ws.Cell(linha, contador).Value =
                    agremiacao.InscricaoEstadual == null ? "NULL" : agremiacao.InscricaoEstadual;
                contador++;
            }

            if (dto.DataCnpj)
            {
                descricao.Append("Data Cnpj;");
                ws.Cell(1, contador).Value = "Data Cnpj";
                ws.Cell(linha, contador).Value = agremiacao.DataCnpj == null
                    ? "NULL"
                    : new DateTime(agremiacao.DataCnpj.Value.Year,
                        agremiacao.DataCnpj.Value.Month,
                        agremiacao.DataCnpj.Value.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.DataAta)
            {
                descricao.Append("Data Ata;");
                ws.Cell(1, contador).Value = "Data Ata";
                ws.Cell(linha, contador).Value = agremiacao.DataAta == null
                    ? "NULL"
                    : new DateTime(agremiacao.DataAta.Value.Year,
                        agremiacao.DataAta.Value.Month,
                        agremiacao.DataAta.Value.Day).ToString("dd/MM/yyyy");
                contador++;
            }

            if (dto.Foto)
            {
                descricao.Append("Foto;");
                ws.Cell(1, contador).Value = "Foto";
                ws.Cell(linha, contador).Value = agremiacao.Foto == null ? "NULL" : agremiacao.Foto;
                contador++;
            }

            if (dto.AlvaraLocacao)
            {
                descricao.Append("Alvara Locacao;");
                ws.Cell(1, contador).Value = "Alvara Locacao";
                ws.Cell(linha, contador).Value = agremiacao.AlvaraLocacao;
                contador++;
            }

            if (dto.Estatuto)
            {
                descricao.Append("Estatuto;");
                ws.Cell(1, contador).Value = "Estatuto";
                ws.Cell(linha, contador).Value = agremiacao.Estatuto;
                contador++;
            }

            if (dto.ContratoSocial)
            {
                descricao.Append("Contrato Social;");
                ws.Cell(1, contador).Value = "Contrato Social";
                ws.Cell(linha, contador).Value = agremiacao.ContratoSocial;
                contador++;
            }

            if (dto.DocumentacaoAtualizada)
            {
                descricao.Append("Documentacao Atualizada;");
                ws.Cell(1, contador).Value = "Documentacao Atualizada";
                ws.Cell(linha, contador).Value = agremiacao.DocumentacaoAtualizada;
                contador++;
            }
            
            if (dto.IdRegiao)
            {
                descricao.Append("Regiao;");
                ws.Cell(1, contador).Value = "Regiao";
                ws.Cell(linha, contador).Value = agremiacao.RegiaoNome;
                contador++;
            }
            
            if (dto.Anotacoes)
            {
                descricao.Append("Anotacoes;");
                ws.Cell(1, contador).Value = "Anotacoes";
                ws.Cell(linha, contador).Value = agremiacao.Anotacoes;
                contador++;
            }

            linha++;
        }

        ws.Columns().AdjustToContents();

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = $"{descricao}",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 10,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        await RegistroDeEventos.UnitOfWork.Commit();
        return await _fileService.UploadExcel(workbook, EUploadPath.FotosAgremiacao);
    }

    #endregion

    public async Task<PagedDto<AgremiacaoDto>> Buscar(BuscarAgremiacaoDto dto)
    {
        var agremiacao = await _agremiacaoRepository.Buscar(dto);
        await RegistroDeEventos.UnitOfWork.Commit();
        return Mapper.Map<PagedDto<AgremiacaoDto>>(agremiacao);
    }

    public async Task<List<AgremiacaoFiltroDto>> Pesquisar(string valor, List<AgremiacaoFiltro>? agremiacoes = null)
    {
        var agremiacoesFiltro = await _filtroRepository.Listar();
        if (agremiacoesFiltro.Any())
        {
            agremiacoes = await _filtroRepository.Pesquisar(valor);
        }
        else
        {
            var agremiacoesN = await _agremiacaoRepository.Pesquisar(valor);
            if (agremiacoesN.Any())
            {
                agremiacoes = Mapper.Map<List<AgremiacaoFiltro>>(agremiacoesN);
                foreach (var agremiacaoN in agremiacoesN)
                {
                    foreach (var agremiacaoF in
                             agremiacoes.Where(agremiacaoF => agremiacaoF.Sigla == agremiacaoN.Sigla))
                    {
                        agremiacaoF.RegiaoNome = agremiacaoN.Regiao.Descricao;
                        agremiacaoF.Pais = agremiacaoN.Pais;
                        agremiacaoF.Cidade = agremiacaoN.Cidade;
                        agremiacaoF.Estado = agremiacaoN.Estado;
                    }
                }
            }
        }

        if (agremiacoes == null || agremiacoes.Count == 0)
        {
            return new List<AgremiacaoFiltroDto>();
        }

        await _filtroRepository.RemoverTodos();
        foreach (var agremiacao in agremiacoes.DistinctBy(c => c.Id))
        {
            _filtroRepository.Cadastrar(agremiacao);
        }

        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = $"Valor={valor}",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 14,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        if (await _filtroRepository.UnitOfWork.Commit())
        {
            return Mapper.Map<List<AgremiacaoFiltroDto>>(
                await _filtroRepository.Listar());
        }

        return Mapper.Map<List<AgremiacaoFiltroDto>>(agremiacoes);
    }

    public async Task<AgremiacaoDto?> ObterPorId(int id)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);

        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return null;
        }

        var agremiacaoDto = Mapper.Map<AgremiacaoDto>(agremiacao);
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao = "Visualizar agremiacao",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 7,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        await RegistroDeEventos.UnitOfWork.Commit();

        return agremiacaoDto;
    }

    public async Task LimparFiltro()
    {
        await _filtroRepository.RemoverTodos();

        var agremiacoesN = await _agremiacaoRepository.ObterTodos();
        
        var agremiacoes = Mapper.Map<List<AgremiacaoFiltro>>(agremiacoesN);
        foreach (var agremiacaoN in agremiacoesN)
        {
            foreach (var agremiacaoF in
                     agremiacoes.Where(agremiacaoF => agremiacaoF.Sigla == agremiacaoN.Sigla))
            {
                agremiacaoF.RegiaoNome = agremiacaoN.Regiao.Descricao;
                agremiacaoF.Pais = agremiacaoN.Pais;
                agremiacaoF.Cidade = agremiacaoN.Cidade;
                agremiacaoF.Estado = agremiacaoN.Estado;
            }
        }
        
        foreach (var agremiacao in agremiacoes.DistinctBy(c => c.Id))
        {
            _filtroRepository.Cadastrar(agremiacao);
        }
        
        if (!await _agremiacaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível limpar o filtro");
        }
    }

    public async Task Deletar(int id)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        var regiao = await _regiaoRepository.ObterPorId(agremiacao.IdRegiao);
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            Descricao =
                $"Sigla={agremiacao.Sigla};Nome={agremiacao.Nome};Fantasia={agremiacao.Fantasia};Responsavel={agremiacao.Responsavel};Representante={agremiacao.Representante};DataFiliacao={agremiacao.DataFiliacao};DataNascimento={agremiacao.DataNascimento};Cep={agremiacao.Cep};Endereco={agremiacao.Endereco};Bairro={agremiacao.Bairro};Complemento={agremiacao.Complemento};Cidade={agremiacao.Cidade};Estado={agremiacao.Estado};Pais={agremiacao.Pais};Telefone={agremiacao.Telefone};Email={agremiacao.Email};Cnpj={agremiacao.Cnpj};InscricaoMunicipal={agremiacao.InscricaoMunicipal};InscricaoEstadual={agremiacao.InscricaoEstadual};DataCnpj={agremiacao.DataCnpj};DataAta={agremiacao.DataAta};Foto={agremiacao.Foto};AlvaraLocacao={agremiacao.AlvaraLocacao};Estatuto={agremiacao.Estatuto};ContratoSocial={agremiacao.ContratoSocial};DocumentacaoAtualizada={agremiacao.DocumentacaoAtualizada};Regiao={regiao?.Descricao};Anotacoes={agremiacao.Anotacoes};",
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 6,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });
        await RegistroDeEventos.UnitOfWork.Commit();
        _agremiacaoRepository.Deletar(agremiacao);
        if (!await _agremiacaoRepository.UnitOfWork.Commit())
        {
            Notificator.Handle("Não foi possível remover a agremiação");
        }
    }

    public async Task Anotar(int id, AnotarAgremiacaoDto anotacao)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        agremiacao.Anotacoes = anotacao.Anotacoes;
        _agremiacaoRepository.Alterar(agremiacao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = $"Adicionar anotacoes em agremiacao: {agremiacao.Anotacoes}",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 11,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 2
            });
            await RegistroDeEventos.UnitOfWork.Commit();
            return;
        }

        Notificator.Handle("Não foi possível alterar anotação");
    }

    public async Task EnviarDocumentos(int id, EnviarDocumentosDto dto)
    {
        if (!dto.Documentos.Any())
        {
            Notificator.Handle("Nenhum arquivo foi informado");
            return;
        }

        if (!ValidarAnexo(dto))
        {
            Notificator.Handle("Erro ao validar os documentos");
            return;
        }

        var agremiacao = await _agremiacaoRepository.Obter(id);
        if (agremiacao == null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        var nome = new StringBuilder();
        nome.Append("Documento anexado = ");
        StringBuilder links = new StringBuilder();
        foreach (var documento in dto.Documentos.Where(documento => documento is { Length: > 0 }))
        {
            links.Append("&" +
                         await _fileService.Upload(documento, EUploadPath.FotosAgremiacao));
            nome.Append($"{documento.FileName}; ");
        }

        agremiacao.DocumentosUri += links.ToString();
        _agremiacaoRepository.Alterar(agremiacao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                Descricao = nome.ToString(),
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 8,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                FuncaoMenuId = 2
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return;
        }

        Notificator.Handle("Não foi possível enviar documentos.");
    }

    public async Task DeletarDocumento(int id, int documentoId)
    {
        var agremiacao = await _agremiacaoRepository.Obter(id);

        if (agremiacao is null)
        {
            Notificator.HandleNotFoundResource();
            return;
        }

        if (agremiacao.DocumentosUri == "&" || agremiacao.DocumentosUri == string.Empty ||
            agremiacao.DocumentosUri == null)
        {
            Notificator.Handle("Não há anexos ou documento não existe.");
            return;
        }

        var nome = new StringBuilder();
        nome.Append("Documento desanexado=");
        var documentos = agremiacao.DocumentosUri.Split('&').ToList();
        var remover = documentos[documentoId];
        nome.Append($"{remover};");
        documentos.Remove(remover);
        remover = documentos[0];
        documentos.Remove(remover);
        StringBuilder links = new StringBuilder();
        for (int i = 0; i <= documentos.Count - 1; i++)
        {
            links.Append("&" + documentos[i]);
        }

        agremiacao.DocumentosUri = links.ToString();
        _agremiacaoRepository.Alterar(agremiacao);
        if (await _agremiacaoRepository.UnitOfWork.Commit())
        {
            RegistroDeEventos.Adicionar(new RegistroDeEvento
            {
                DataHoraEvento = DateTime.Now,
                ComputadorId = ObterIp(),
                Descricao = $"{nome}",
                ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
                TipoOperacaoId = 6,
                UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
                AdministradorId = null,
                UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
                AdministradorNome = null,
                FuncaoMenuId = 2
            });

            await RegistroDeEventos.UnitOfWork.Commit();
            return;
        }

        Notificator.Handle("Não foi possível remover documentos.");
    }

    public async Task DownloadDocumento(DownloadDocumentoDto documento)
    {
        RegistroDeEventos.Adicionar(new RegistroDeEvento
        {
            Descricao = $"Documento baixado: {documento.Nome};",
            DataHoraEvento = DateTime.Now,
            ComputadorId = ObterIp(),
            ClienteId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterClienteId()),
            TipoOperacaoId = 8,
            UsuarioNome = _httpContextAccessor.HttpContext?.User.ObterNome(),
            AdministradorNome = null,
            UsuarioId = Convert.ToInt32(_httpContextAccessor.HttpContext?.User.ObterUsuarioId()),
            AdministradorId = null,
            FuncaoMenuId = 2
        });

        await RegistroDeEventos.UnitOfWork.Commit();
    }
    private async Task<bool> Validar(Agremiacao agremiacao)
    {
        if (!agremiacao.Validar(out var validationResult))
        {
            Notificator.Handle(validationResult.Errors);
        }

        var existente = await _agremiacaoRepository.FirstOrDefault(s =>
            (s.Sigla == agremiacao.Sigla || s.Nome == agremiacao.Nome) && s.Id != agremiacao.Id);
        if (existente != null)
        {
            Notificator.Handle("Já existe uma agremiação cadastrada com essa sigla e/ou nome");
        }

        return !Notificator.HasNotification;
    }

    #region ManterAnexos

    private async Task<string> ManterFoto(IFormFile foto)
    {
        var caminho = await _fileService.Upload(foto, EUploadPath.FotosAgremiacao);
        return caminho;
    }

    #endregion

    private bool ValidarFoto(CadastrarAgremiacaoDto dto)
    {
        if (dto.Foto?.Length > 10000000)
        {
            Notificator.Handle("Foto deve ter no máximo 10Mb");
        }

        if (dto.Foto != null && dto.Foto.FileName.Split(".").Last() != "jfif" &&
            dto.Foto.FileName.Split(".").Last() != "png" && dto.Foto.FileName.Split(".").Last() != "jpg"
            && dto.Foto.FileName.Split(".").Last() != "jpeg")
        {
            Notificator.Handle("Foto deve do tipo png, jfif ou jpg");
        }

        return !Notificator.HasNotification;
    }

    private bool ValidarAnexo(EnviarDocumentosDto dto)
    {
        foreach (var documento in dto.Documentos)
        {
            if (documento.Length > 10000000)
            {
                Notificator.Handle("Os documentos devem ter no máximo 10Mb");
            }

            if (documento.FileName.Split(".").Last() != "pdf")
            {
                Notificator.Handle("Aceito apenas documentos do tipo PDF");
            }
        }

        return !Notificator.HasNotification;
    }
}