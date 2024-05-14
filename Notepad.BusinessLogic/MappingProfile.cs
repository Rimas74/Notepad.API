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
        }
    }
}
