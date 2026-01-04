using API_Practice;
using API_Practice.Data;
using API_Practice.Model;
using API_Practice.Model.DTO;
using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

// クーポン一覧取得　
app.MapGet("api/coupon", (ILogger<Program> _logger) =>
{
    APIResponse response = new();
    response.Result = CouponStore.couponList;
    response.IsSuucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCouponList").Produces<APIResponse>(200);

// Idを指定してクーポンを取得
app.MapGet("api/coupon/{id:int}", (int id, ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Idからクーポン取得");
    APIResponse response = new();
    response.Result = CouponStore.couponList.FirstOrDefault(u => u.Id == id);
    response.IsSuucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("GetCouponById").Produces<APIResponse>(200);

// クーポン作成
app.MapPost("api/coupon", async ([FromBody] CouponCreateDTO coupon_C_DTO,IValidator<CouponCreateDTO> _validator ,IMapper _mapper,  ILogger<Program> _logger) =>
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
    if (CouponStore.couponList.FirstOrDefault(x => x.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        response.ErrorMessage.Add("すでに使用されているクーポン名です");
        return Results.BadRequest(response);
    }

    // 1. CouponにCouponCreateDTOをマップ
    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);

    // 2. Couponに追加
    coupon.Id = CouponStore.couponList.Max(x => x.Id) + 1;
    CouponStore.couponList.Add(coupon);

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

// クーポン更新
app.MapPut("api/coupon", async ([FromBody] CouponUpdateDTO coupon_U_DTO, IValidator<CouponUpdateDTO> _validator, IMapper _mapper, ILogger<Program> _logger) =>
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

    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(x => x.Id == coupon_U_DTO.Id);
    couponFromStore.IsActive = coupon_U_DTO.IsActive;
    couponFromStore.Name = coupon_U_DTO.Name;
    couponFromStore.Percent = coupon_U_DTO.Percent;
    couponFromStore.LastUpdated = DateTime.Now;

    response.Result = _mapper.Map<CouponDTO>(couponFromStore);
    response.IsSuucess = true;
    response.StatusCode = HttpStatusCode.OK;
    return Results.Ok(response);
}).WithName("UpdateCoupon").Accepts<CouponUpdateDTO>("application/json").Produces<APIResponse>(200).Produces(400);

// クーポンの削除
app.MapDelete("$api/coupon/{id:int}", (int id, ILogger<Program> _logger) =>
{

    _logger.Log(LogLevel.Information, "クーポン削除");

    APIResponse response = new() { IsSuucess = false, StatusCode = HttpStatusCode.BadRequest };

    Coupon counponFromStore = CouponStore.couponList.FirstOrDefault(x => x.Id == id);
    if (counponFromStore != null)
    {
        CouponStore.couponList.Remove(counponFromStore);
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

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}