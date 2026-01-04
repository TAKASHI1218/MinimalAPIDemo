using API_Practice.Data;
using API_Practice.Model;
using API_Practice.Model.DTO;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    _logger.Log(LogLevel.Information, "クーポン一覧取得");
    return Results.Ok(CouponStore.couponList);
}).WithName("GetCouponList").Produces<IEnumerable<Coupon>>(200);

// Idを指定してクーポンを取得
app.MapGet("api/coupon/{id:int}", (int id, ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "Idからクーポン取得");
    return Results.Ok(CouponStore.couponList.FirstOrDefault(u => u.Id == id));
}).WithName("GetCouponById").Produces<Coupon>(200);

// クーポン作成
app.MapPost("api/coupon", ([FromBody] CouponCreateDTO coupon_C_DTO, ILogger<Program> _logger) =>
{
    _logger.Log(LogLevel.Information, "クーポン作成");
    if (string.IsNullOrEmpty(coupon_C_DTO.Name))
    {
        return Results.BadRequest("クーポン名を入力してください");
    }

    if (CouponStore.couponList.FirstOrDefault(x => x.Name.ToLower() == coupon_C_DTO.Name.ToLower()) != null)
    {
        return Results.BadRequest("すでに使用されているクーポン名です");
    }
    
    // 1. CouponCreateDTOの値をCouponに設定
    Coupon coupon = new()
    {
        IsActive = coupon_C_DTO.IsActive,
        Name = coupon_C_DTO.Name,
        Percent = coupon_C_DTO.Percent
    };

    // 2. Couponに追加
    coupon.Id = CouponStore.couponList.Max(x => x.Id) + 1;
    CouponStore.couponList.Add(coupon);

    // 3. 外部公開用のデータ表示のためCouponをCoponDTOをに設定
    CouponDTO couponDTO = new()
    {
        Id = coupon.Id,
        IsActive = coupon.IsActive,
        Name = coupon.Name,
        Percent = coupon.Percent,
        Created = coupon.Created
    };

    // 201を返しGetCouponByIdルートよりデータ取得可能
    return Results.CreatedAtRoute("GetCouponById", new { coupon.Id }, couponDTO);
    //※右記と同義「return Results.Created($"/api/coupon/{coupon.Id}",coupon);」

    // 作成に成功した場合は"201"作成できなかった場合は"400"を返す
    // CouponCreateDTOを受け取りCouponDTOを返す
}).WithName("CreateCoupon").Accepts<CouponCreateDTO>("application/json").Produces<CouponDTO>(201).Produces(400); 

app.MapPut("api/coupon", () => {

});

app.MapDelete("api/coupon", () => {

});
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