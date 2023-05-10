using AutoMapper;
using SysJudo.Application.Dto.GruposDeAcesso;
using SysJudo.Domain.Entities;

namespace SysJudo.Application.Configuration.Resolvers;

public class GrupoAcessoAlterarResolver : IValueResolver<AlterarGrupoAcessoDto, GrupoAcessoDto , List<GrupoAcessoPermissao>>
{
    private readonly IMapper _mapper;

    public GrupoAcessoAlterarResolver(IMapper mapper)
    {
        _mapper = mapper;
    }

    public List<GrupoAcessoPermissao> Resolve(AlterarGrupoAcessoDto source, GrupoAcessoDto destination,
        List<GrupoAcessoPermissao> destMember, ResolutionContext context)
    {
        var permissoes = destMember.ToList();
        foreach (var permissao in source.Permissoes)
        {
            var perm = source.Permissoes
                .FirstOrDefault(c => c.PermissaoId == permissao.PermissaoId);
            if (perm == null)
            {
                permissoes.Add(_mapper.Map<GrupoAcessoPermissao>(permissao));
            }
        }
        
        foreach (var permissao in source.Permissoes)
        {
            var perm = source.Permissoes
                .FirstOrDefault(c => c.PermissaoId == permissao.PermissaoId);
            if (perm != null) continue;

            permissoes.Remove(_mapper.Map<GrupoAcessoPermissao>(permissao));
        }

        return permissoes;
    }
}