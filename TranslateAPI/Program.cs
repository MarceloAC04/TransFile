using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TranslateAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();  
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependence Injection class MongoDbService
builder.Services.AddSingleton<MongoDbService>();

//Adiciona servi�o de Jwt Bearer (forma de autentica��o)
builder.Services.AddAuthentication(options =>
{
    options.DefaultChallengeScheme = "JwtBearer";
    options.DefaultAuthenticateScheme = "JwtBearer";
})

.AddJwtBearer("JwtBearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //valida quem est� solicitando
        ValidateIssuer = true,

        //valida quem est� recebendo
        ValidateAudience = true,

        //define se o tempo de expira��o ser� validado
        ValidateLifetime = true,

        //forma de criptografia e valida a chave de autentica��o
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("transfile-webapi-chave-symmetricsecuritykey")),

        //valida o tempo de expira��o do token
        ClockSkew = TimeSpan.FromMinutes(30),

        //nome do issuer (de onde est� vindo)
        ValidIssuer = "TransFile-WebAPI",

        //nome do audience (para onde est� indo)
        ValidAudience = "TransFile-WebAPI"
    };
});


//Swagger
builder.Services.AddSwaggerGen(options =>
{

    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API TransFile",
        Description = "Backend API",
        Contact = new OpenApiContact
        {
            Name = "Grupo 5 LabWare"
        }
    });

    //Usando a autentica�ao no Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Value: Bearer TokenJWT ",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder =>
        {
            builder.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

var app = builder.Build();

// Swagger
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    options.RoutePrefix = string.Empty;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use Services
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
