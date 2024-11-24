using BusinessLogic.Services;
using DataAccess.DB;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RakamonBackEnd.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Veritaban� Ba�lant�s�
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session ekleme
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// CORS politikas� ekleme
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Frontend'in URL'si
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Session i�in gerekli
        });
});




// Ba��ml�l�klar� ekleme
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Controller servisi
builder.Services.AddControllers();

// Swagger/OpenAPI yap�land�rmas�
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware zinciri
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();


// Session kullan�m�
app.UseSession(); // Session burada etkinle�tirilmeli

// Authorization Middleware


// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authorization i�lemleri
app.UseAuthentication(); // JWT do�rulama
app.UseAuthorization();  // Yetkilendirme

// Authorization Middleware
app.UseMiddleware<RoleBasedAuthorizationMiddleware>();

// Controller haritalama
app.MapControllers();

app.Run();
