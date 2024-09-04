using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WFConFin.Dados;
using WFConFin.Servicos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var conectionString = builder.Configuration.GetConnectionString("ConnectionPostgress");
builder.Services.AddDbContext<WFConfinDBContext>(x => x.UseNpgsql(conectionString));



builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var chave = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("Chave").Get<string>()); //Pego a informação do appSetting com a chave e transformo em bytes

builder.Services.AddAuthentication(
    
    
    x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }
    
    ).AddJwtBearer(
    
    x =>
    {
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(chave),
            ValidateIssuer = false,
            //ValidateActor = false
            ValidateAudience = false
        };

    });

builder.Services.AddSingleton<TokenService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();  //Incluir isso para permitir a autenticação com o token
app.UseAuthorization();


app.MapControllers();

app.Run();
