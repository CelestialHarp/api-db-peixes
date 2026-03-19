//ALERTA: A SENHA ESTÀ NO DbPescesCONTEXT.cs, deverá ser colocada em appsettings.json futuramente.
using API_DB_PESCES_em_C__bonitona.Models;
using API_DB_PESCES_em_C__bonitona.Services;
using API_DB_PESCES_em_C__bonitona.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbPescesContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuração da Chave Secreta
var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"] ?? "chave_super_secreta_padrao_muito_longa_123");

// Configuração da Autenticação
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();

// ==========================================
//    CONFIGURAÇÃO DO SWAGGER PARA .NET 10
//  (tive que arranajar um jeito de ajeitar)
// ==========================================
builder.Services.AddSwaggerGen(c =>
{
    
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o seu token JWT abaixo.\nExemplo: eyJhbGciOiJIUzI1NiIs..."
    });

    //aqui o jeito que arrumei:
    c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

// Injeções de dependência
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PesceService>();
builder.Services.AddScoped<PrecoService>();
builder.Services.AddScoped<LoteService>();
builder.Services.AddScoped<CatalogoService>();
builder.Services.AddScoped<CarrinhoService>();
builder.Services.AddScoped<PedidoService>();

// ==========================================
// CONFIGURAÇÃO DO CORS (Liberar o Front-end)
// (Outro "make it work" que tive que fazer)
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()   // Permite qualquer porta/URL (ex: 127.0.0.1:5500)
              .AllowAnyMethod()   // Permite GET, POST, PUT, DELETE
              .AllowAnyHeader();  // Permite o envio do Token e outros cabeçalhos
    });
});

var app = builder.Build();

DbSeeder.Seed(app);

app.UseCors("PermitirTudo");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication(); // Quem é você?
app.UseAuthorization();  // O que você pode fazer?

app.MapControllers();

app.Run();

