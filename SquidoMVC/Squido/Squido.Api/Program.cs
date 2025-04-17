using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Squido.DAOs.Interfaces;
using Squido.DAOs.Repositories;
using Squido.Models;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.DAOs.Repositories;
using WebApplication1.Mapper;
using WebApplication1.Services.Interfaces;
using WebApplication1.Services.Services;

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddAuthorization();
        builder.Services.AddDbContext<SquidoDbContext>(
            options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IBookService, BookService>();
        builder.Services.AddAutoMapper(typeof(MappingProfile));
    
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddControllers().AddJsonOptions(option =>
        {
            option.JsonSerializerOptions.ReferenceHandler = null;
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }

        app.UseHttpsRedirection();
        
        
        app.UseAuthorization();
        app.MapControllers();
                
        app.Run();
    }
}