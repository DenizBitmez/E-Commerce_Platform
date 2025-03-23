using ETicaretDal.Abstract;
using ETicaretDal.Concreate;
using ETicaretData.Context;
using ETicaretData.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Add services to the container.
builder.Services.AddControllersWithViews();

//Baðýmlýlýk Enjeksiyonu
builder.Services.AddDbContext<ETicaretContext>();
builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<IProductDal, ProductDal>();
builder.Services.AddScoped<IOrderDal, OrderDal>();
builder.Services.AddScoped<IOrderLineDal, OrderLineDal>();
builder.Services.AddScoped<IReviewDal, ReviewDal>();
//builder.Services.AddScoped<IEmployeeDal, EmployeeDal>();



//Identity kimlik doðrulama
builder.Services.AddIdentity<AppUser, AppRole>(opistion =>
{
    //kilitleme süresi
    opistion.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    //max baþarýsýz giriþ
    opistion.Lockout.MaxFailedAccessAttempts = 5;
    //rakam gerekliliði false
    opistion.Password.RequireDigit = false;
    //þifrelerde özel karakter gerekliliði false
    opistion.Password.RequireNonAlphanumeric = false;
    //þifrede küçük harf gerekliliði false
    opistion.Password.RequireLowercase = false;
    //þifrede büyük harf gerekliliðini kaldýr
    opistion.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ETicaretContext>()// EF ile veri tabaný baðlantýsýný saðlar
.AddDefaultTokenProviders();//default olarak token saðlayýcý ekler

builder.Services.ConfigureApplicationCookie(opistion =>
{
    //giriþ yapýlmadýðýnda yönlendirilen sayfa
    opistion.LoginPath = "/Account/Login";
    //yetkisiz eriþim olduðunda yönlendirilecek sayfa
    opistion.AccessDeniedPath = "/";
    //
    opistion.Cookie = new CookieBuilder
    {
        Name = "AspNetCoreIdentityExampleCookie",  //çerez ismi
        HttpOnly = false,  //sadece http üzerinden eriþilsin
        SameSite = SameSiteMode.Lax, //çerez ayný sitede yapýlacak isteklerde geçerli
        SecurePolicy = CookieSecurePolicy.Always  //çerez yalnýzca https üzerinde SSL sertifikasý olan istekler iletilecek
    };
    //çerez geçerlilik süresi doldukça yenilenir
    opistion.SlidingExpiration = true;
    //çerez geçerlilik süresi 15 dk
    opistion.ExpireTimeSpan = TimeSpan.FromMinutes(15);

});

// Oturum Yönetimi Ýþlemleri
//oturum yönetim servisi
builder.Services.AddSession();
//hata verirse yerini deðiþtir
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection(); //http den yönlendirme yapýlýr
app.UseStaticFiles(); //,

app.UseRouting(); //http isteklerini yönlendirme

app.UseAuthentication(); //kimlik doðrulama iþlemleri

app.UseAuthorization(); //yetkilendirme iþlemleri

app.UseSession(); //oturum yönetimi aktifleþtirilir

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=List}/{id?}");

app.Run();


