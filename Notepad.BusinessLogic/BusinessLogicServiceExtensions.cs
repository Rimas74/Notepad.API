using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notepad.BusinessLogic
{
    public static class BusinessLogicServiceExtensions
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<INoteService, NoteService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddSingleton<FileManager>(provider =>
                new FileManager(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")));
            return services;
        }
    }
}
