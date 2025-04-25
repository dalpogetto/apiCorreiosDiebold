using apiCorreios.Data;
using apiCorreios.Models;
using apiCorreios.Services;
using Microsoft.AspNetCore.Mvc;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:4200",
                                              "http://localhost:4200/");

                      policy.AllowAnyMethod();
                      policy.AllowAnyHeader();
                      policy.SetIsOriginAllowed(origin => true);
                      policy.AllowCredentials();});
});

// Add services to the container.

builder.Services.AddDbContext<ApiDbContext>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<CorreiosService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

//Configurar CORS
app.UseCors(MyAllowSpecificOrigins);



//Metodo de Rastreio
app.MapGet("/Rastro/{listaObjetos}", ([FromRoute]string listaObjetos, [FromServices] CorreiosService srv) =>
{
    return srv.ObterRastreio(listaObjetos);
});

//Metodo Preco Prazo
app.MapPost("/CalculoPrecoPrazo", ([FromBody] CalculoPrecoPrazoRequest corpo, [FromServices] CorreiosService srv) =>
{
    return srv.CalcularPrecoPrazo(corpo.cepOrigem, corpo.cepDestino, corpo.itens);
});

app.Run();
