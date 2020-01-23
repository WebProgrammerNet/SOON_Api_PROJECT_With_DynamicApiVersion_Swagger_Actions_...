using ApiProjectModul.DataTransferObjects;
using ApiProjectModul.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiProjectModul.MappingProfiles
{
    public class CompositionMappings : Profile
    {
        public CompositionMappings()
        {
            CreateMap<Composition, CompositionDto>().ReverseMap();
            CreateMap<Composition, CreateCompositionDto>().ReverseMap();
            CreateMap<Composition, UpdateCompositionDto>().ReverseMap();
        }
    }
}
