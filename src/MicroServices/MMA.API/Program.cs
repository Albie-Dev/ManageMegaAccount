using MMA.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMMAService();

builder.Services.AddCoreService();

builder.Services.AddCoreAuthentication<Program>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicyBuilder =>
{
    corsPolicyBuilder.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod();
});

app.UseHttpsRedirection();
app.MapAccesTokenFromQuery();
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
app.MapRuntimeContext();
app.MapCoreMiddleware();


app.Run();
