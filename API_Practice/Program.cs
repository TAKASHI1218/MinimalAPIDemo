using API_Practice;
using API_Practice.Data;
using API_Practice.Endpoints;
using API_Practice.Model;
using API_Practice.Model.DTO;
using API_Practice.Repository;
using API_Practice.Repository.IRepository;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
// Swagger
builder.Services.AddSwaggerGen();
// DB
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ICouponRepository, CouponRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
// パッケージ
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

# region Sample Endpoints
// ==============================
//  Sample Endpoints
// ==============================
//app.MapGet("/hellowGET", () => "Hello GET");
//app.MapPost("/helloPOST", () => "Hello POST");
//app.MapGet("/hellowBadRequest", () =>
//{
//    return Results.BadRequest("Exception!!");
//});
//app.MapGet("/helloInteger/{id:int}", (int id) =>
//{
//    return Results.Ok("Id!!" + id);
//});
# endregion

// CouponEndpoints　
app.ConfigureCouponEndpoints();

app.UseHttpsRedirection();

app.Run();
