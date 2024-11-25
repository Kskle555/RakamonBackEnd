Görev Yönetim Uygulaması (Backend)
Bu proje, Görev Yönetim Uygulaması için geliştirilmiş bir .NET Core tabanlı backend uygulamasıdır. Uygulama, kullanıcı yönetimi, görev yönetimi ve rol bazlı yetkilendirme gibi temel işlevleri sağlar. Ayrıca, MSSQL Server tabanlı bir veritabanı kullanılarak veriler güvenli bir şekilde saklanır.


🚀 Özellikler
Kimlik Doğrulama ve Yetkilendirme
Kullanıcıların giriş yapmasını ve rollerine göre yetkiler almasını sağlayan JWT tabanlı sistem.

Rol Bazlı Yetkilendirme

Admin: Tüm kullanıcıları ve görevleri yönetebilir.
Kullanıcı: Sadece kendi görevlerini görüntüleyebilir ve yönetebilir.
Görev Yönetimi

Görev ekleme, düzenleme, silme ve listeleme.
Adminler tüm görevleri görebilir; kullanıcılar yalnızca kendilerine ait görevleri görebilir.
API Tabanlı İletişim
RESTful API yapısıyla frontend uygulamalar için hızlı ve güvenilir bir iletişim sunar.



🛠️ Kullanılan Teknolojiler
.NET Core 6.0
Modern ve hızlı backend geliştirme platformu.

Entity Framework Core
Veritabanı işlemleri için kullanılan ORM aracı.

MSSQL Server
Veritabanı yönetimi için tercih edilen ilişkisel veritabanı sistemi.

JWT (JSON Web Token)
Kimlik doğrulama ve yetkilendirme için güvenli token mekanizması.



📂 Proje Yapısı
bash
Kodu kopyala
📦 src
├── 📁 Controllers       # API uç noktalarını yöneten denetleyiciler
│   ├── AuthController.cs
│   ├── UsersController.cs
│   ├── TasksController.cs
├── 📁 Models            # Veri modelleri
│   ├── User.cs
│   ├── Task.cs
├── 📁 Services          # İş mantığı ve servis katmanı
│   ├── AuthService.cs
│   ├── TaskService.cs
│   ├── UserService.cs
├── 📁 Data              # Veritabanı bağlamı ve ilk yapılandırma
│   ├── AppDbContext.cs
│   ├── SeedData.cs
├── Program.cs           # Uygulama başlangıç noktası
├── Startup.cs           # Uygulama yapılandırma
└── appsettings.json     




🔧 Kurulum ve Çalıştırma
1️⃣ MSSQL Veritabanını Hazırlayın
MSSQL Server'da bir veritabanı oluşturun (örneğin: TaskManagementDb).
appsettings.json dosyasında aşağıdaki bağlantı dizgesini güncelleyin:

"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TaskManagementDb;Trusted_Connection=True;"
}


2️⃣ Projeyi Çalıştırın
Proje bağımlılıklarını yükleyin:
dotnet restore

Veritabanını migrate edin ve seed data ekleyin:
dotnet ef database update

Projeyi başlatın:
dotnet run
3️⃣ API Dokümantasyonuna Erişim
Projeyi çalıştırdıktan sonra Swagger dokümantasyonu şu adreste erişilebilir olacaktır:

http://localhost:5000/swagger


 API Uç Noktaları
Kullanıcı Girişi
http
Kodu kopyala
POST /api/auth/login
Girdi:

json
Kodu kopyala
{
  "tcKimlikNo": "12345678901",
  "password": "password123"
}
Çıktı:

json
Kodu kopyala
{
  "message": "Giriş başarılı.",
  "user": {
    "id": 1,
    "name": "Ahmet Yılmaz",
    "role": "admin"
  },
  "token": "jwt-token-string"
}
Yeni Kullanıcı Oluşturma
http

POST /api/Users
Girdi:
{
  "id": 0,
  "name": "Ahmet Yılmaz",
  "email": "ahmet@example.com",
  "tcKimlikNo": "12345678901",
  "role": "user",
  "password": "password123"
}
Çıktı:
{
  "message": "Kullanıcı başarıyla oluşturuldu."
}
Görev Ekleme
http
Kodu kopyala
POST /api/tasks
Girdi:


{
  "title": "Yeni Görev",
  "description": "Görev açıklaması",
  "userId": 1,
  "dueDate": "2024-11-30"
}
Çıktı:


{
  "message": "Görev başarıyla eklendi."
}
📝 Katkıda Bulunma
Katkıda bulunmak için bir Pull Request gönderin veya bir Issue oluşturun. Daha fazla bilgi için CONTRIBUTING.md dosyasına göz atabilirsiniz.

📄 Lisans
Bu proje MIT Lisansı ile lisanslanmıştır.
