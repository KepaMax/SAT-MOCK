using AutoMapper;
using EXAM_SYSTEM.Application.Common.Models.Schools;
using EXAM_SYSTEM.Application.Common.Models.Students;
using EXAM_SYSTEM.Domain.Entities;

namespace EXAM_SYSTEM.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<School, SchoolDto>();
        CreateMap<Student, StudentProfileDto>();
    }
}
