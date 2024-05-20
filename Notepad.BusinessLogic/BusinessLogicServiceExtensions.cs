using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            services.AddScoped<IFileManager>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<FileManager>>();
                var basePath = Path.Combine(Directory.GetCurrentDirectory(), "images");
                return new FileManager(basePath, logger);
            });
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
