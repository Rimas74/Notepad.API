using AutoMapper;
using Notepad.Repositories.Entities;
using Notepad.Common.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<NoteUpdateDTO, Note>().ReverseMap();
            CreateMap<Note, NoteDTO>().ReverseMap();
            CreateMap<CreateNoteDTO, Note>()
                .ForMember(dest => dest.NoteId, opt => opt.Ignore())
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
                .ReverseMap();


            CreateMap<CategoryDTO, Category>()
                .ReverseMap();

            CreateMap<CreateCategoryDTO, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UpdateCategoryDTO, Category>()
                .ForMember(dest => dest.CategoryId, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<NoteUpdateImageDTO, Note>()
               .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
               .ReverseMap();
        }
    }
}
