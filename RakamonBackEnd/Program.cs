using BusinessLogic.Services;
using DataAccess.DB;
using DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RakamonBackEnd.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı Bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Session ekleme
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum süresi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// CORS politikası ekleme
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:5173") // Frontend'in URL'si
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials(); // Session için gerekli
        });
});




// Bağımlılıkları ekleme
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Controller servisi
builder.Services.AddControllers();

// Swagger/OpenAPI yapılandırması
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware zinciri
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();


// Session kullanımı
app.UseSession(); // Session burada etkinleştirilmeli

// Authorization Middleware


// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Authorization işlemleri
app.UseAuthentication(); // JWT doğrulama
app.UseAuthorization();  // Yetkilendirme

// Authorization Middleware
app.UseMiddleware<RoleBasedAuthorizationMiddleware>();

// Controller haritalama
app.MapControllers();

app.Run();
