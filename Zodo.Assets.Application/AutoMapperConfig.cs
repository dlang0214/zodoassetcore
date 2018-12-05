using AutoMapper;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Account, AccountListDto>()
                    .ForMember(dto => dto.DeptName, opts => opts.MapFrom(src => src.Dept.Name))
                    .ReverseMap();
            });
        }
    }
}
