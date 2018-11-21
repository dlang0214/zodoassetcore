using System;
using System.Collections.Generic;
using System.Text;
using Zodo.Assets.Core;
using AutoMapper;

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
