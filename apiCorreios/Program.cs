using System.Text;
using apiCorreios.Data;
using apiCorreios.Models;
using apiCorreios.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

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

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettingsService.JwtSettings.Key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

/*builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});
*/

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

//Configurar CORS
app.UseCors(MyAllowSpecificOrigins);


//Logar Usuario de Servico
app.MapPost("/login", (Usuario usuarioServico) =>
{
    if (usuarioServico.Email == "servico@dieboldnixdorf.com" && usuarioServico.Senha == "prodiebold11")
    {
        var usuarioLogado = new Usuario { Email = usuarioServico.Email, Role = "User" };
        usuarioLogado.Senha = string.Empty;

        var token = JwtBearerService.GenerateToken(usuarioLogado);
        return Results.Ok(new { Usuario = usuarioServico, token = token });
    }
    else
        return Results.NotFound(new { message = "Usuário e senha inválidos!" });
}).AllowAnonymous();

//Metodo de Rastreio
app.MapGet("/Rastro/{listaObjetos}", ([FromRoute]string listaObjetos, [FromServices] CorreiosService srv) =>
{
    return srv.ObterRastreio(listaObjetos);
}).RequireAuthorization();

//Metodo Preco Prazo
app.MapPost("/CalculoPrecoPrazo", ([FromBody] CalculoPrecoPrazoRequest corpo, [FromServices] CorreiosService srv) =>
{
    return srv.CalcularPrecoPrazo(corpo.cepOrigem, corpo.cepDestino, corpo.itens);
}).RequireAuthorization();

app.Run();

