using API_Practice.Data;

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
app.MapGet("api/coupon", () =>
{
    return Results.Ok(CouponStore.couponList);
});

// Idを指定してクーポンを取得
app.MapGet("api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponList.FirstOrDefault(u => u.Id == id));
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