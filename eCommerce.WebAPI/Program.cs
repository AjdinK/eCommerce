using eCommerce.Model.Requests;
using eCommerce.Model.Responses;
using eCommerce.Services;
using eCommerce.Services.Database;
using eCommerce.Services.Validators;
using eCommerce.WebAPI.Filters;
using FluentValidation;
using Mapster;
using Microsoft.AspNetCore.Mvc.Filters;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(
   options => options.Filters.Add<ExceptionFilter>()
);

// register Mapster for object mapping
builder.Services.AddMapster();

// configure a few mappings explicitly if needed (optional)
// Mapster will automatically map same-named properties, but configuration
// ensures any custom rules or future needs can be added here.
TypeAdapterConfig<Product, ProductResponse>.NewConfig();
TypeAdapterConfig<Category, CategoryResponse>.NewConfig();

// register application services
builder.Services.AddScoped<IProductService, ProductService>();
// category service
builder.Services.AddScoped<ICategoryService, CategoryService>();

builder.Services.AddScoped<IValidator<CategoriesInsertRequest>, CategoryInsertValidator>();
builder.Services.AddScoped<IValidator<CategoriesUpdateRequest>, CategoryUpdateValidator>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "v1",
            Title = "eCommerce API",
            Description = "API for managing products and categories in the eCommerce application"
        });

        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();


    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
