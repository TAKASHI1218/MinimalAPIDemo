using API_Practice;
using API_Practice.Data;
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

// ==============================
//  Coupon Endpoints
// ==============================

// -- クーポン一覧取得 -- //　
app.MapGet("api/coupon",async (ICouponRepository _couponRepo, ILogger<Program> _logger) =>
{
    APIResponse response = new();
    response.Result = await _couponRepo.GetAllCouponAsync();
    response.IsSuucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCouponList").Produces<APIResponse>(200);

// -- Idを指定してクーポンを取得 -- //
app.MapGet("api/coupon/{id:int}",async (int id, ICouponRepository _couponRepo, ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Idからクーポン取得");
    APIResponse response = new();
    response.Result = await _couponRepo.GetCouponByIdAsync(id);
    response.IsSuucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCouponById").Produces<APIResponse>(200);

// -- クーポン作成 -- //　
app.MapPost("api/coupon", async ([FromBody] CouponCreateDTO coupon_C_DTO, 
                                 ICouponRepository _couponRepo, 
                                 IValidator<CouponCreateDTO> _validator ,
                                 IMapper _mapper, 
                                 ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "クーポン作成");

    APIResponse response = new() {IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

    // FluentValidationによるバリデーション処理
    var validationResult = await _validator.ValidateAsync(coupon_C_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessage.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    // 独自実装によるバリデーション処理
    if (await _couponRepo.GetCouponByNameAsync(coupon_C_DTO.Name) != null)
    {
        response.ErrorMessage.Add("すでに使用されているクーポン名です");
        return Results.BadRequest(response);
    }

    // 1. CouponにCouponCreateDTOをマップ、作成日登録
    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
    coupon.Created = DateTime.Now;

    // 2. Couponに追加
    await _couponRepo.CreateCouponAsync(coupon);
    await _couponRepo.SaveAsync();

    // 3. 外部公開用のデータ表示のためCoponDTOにCouponをマップ
    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    // 201を返しGetCouponByIdルートよりデータ取得可能
    response.Result = couponDTO;
    response.IsSuucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);

    // 参考：過去の処理
    // return Results.CreatedAtRoute("GetCouponById", new { coupon.Id }, couponDTO);
    // return Results.Created($"/api/coupon/{coupon.Id}",coupon);

    // 作成に成功した場合は"201"作成できなかった場合は"400"を返す
    // CouponCreateDTOを受け取りCouponDTOを返す
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<APIResponse>(201).Produces(400);

// -- クーポン更新 -- //　
app.MapPut("api/coupon", async ([FromBody] CouponUpdateDTO coupon_U_DTO,
                                ICouponRepository _couponRepo, 
                                IValidator<CouponUpdateDTO> _validator, 
                                IMapper _mapper, 
                                ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "クーポン更新");

    APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

    // FluentValidationによるバリデーション処理
    var validationResult = await _validator.ValidateAsync(coupon_U_DTO);
    if (!validationResult.IsValid)
    {
        response.ErrorMessage.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    Coupon coupon = _mapper.Map<Coupon>(coupon_U_DTO);
    coupon.LastUpdated = DateTime.Now;
    _couponRepo.UpdateCoupon(coupon);
    await _couponRepo.SaveAsync();

    response.Result = _mapper.Map<CouponDTO>(await _couponRepo.GetCouponByIdAsync(coupon_U_DTO.Id));
    response.IsSuucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

// -- クーポンの削除 -- //　
app.MapDelete("api/coupon/{id:int}", async (int id, ICouponRepository _couponRepo, ILogger<Program> _logger) =>
{ 
    _logger.Log(LogLevel.Information, "クーポン削除");

    APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

    Coupon counponFromStore = await _couponRepo.GetCouponByIdAsync(id);
    if (counponFromStore != null)
    {
        _couponRepo.RemoveCoupon(counponFromStore);
        await _couponRepo.SaveAsync();
        response.IsSuucess = true;
        response.StatusCode = HttpStatusCode.NoContent;
        return Results.Ok(response);
    }
    else
    {
        response.ErrorMessage.Add("存在しないクーポンです");
        return Results.BadRequest(response);
    }
}).WithName("DeleteCoupon").Produces<APIResponse>(204).Produces(400);

app.UseHttpsRedirection();

# region Weatherforecast
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");
# endregion

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

