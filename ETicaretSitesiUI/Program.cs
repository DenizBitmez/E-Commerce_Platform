using ETicaretDal.Abstract;
using ETicaretDal.Concreate;
using ETicaretData.Context;
using ETicaretData.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Add services to the container.
builder.Services.AddControllersWithViews();

//Ba��ml�l�k Enjeksiyonu
builder.Services.AddDbContext<ETicaretContext>();
builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<IProductDal, ProductDal>();
builder.Services.AddScoped<IOrderDal, OrderDal>();
builder.Services.AddScoped<IOrderLineDal, OrderLineDal>();
builder.Services.AddScoped<IReviewDal, ReviewDal>();
//builder.Services.AddScoped<IEmployeeDal, EmployeeDal>();



//Identity kimlik do�rulama
builder.Services.AddIdentity<AppUser, AppRole>(opistion =>
{
	//kilitleme s�resi
	opistion.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
	//max ba�ar�s�z giri�
	opistion.Lockout.MaxFailedAccessAttempts = 5;
	//rakam gereklili�i false
	opistion.Password.RequireDigit = false;
	//�ifrelerde �zel karakter gereklili�i false
	opistion.Password.RequireNonAlphanumeric = false;
	//�ifrede k���k harf gereklili�i false
	opistion.Password.RequireLowercase = false;
	//�ifrede b�y�k harf gereklili�ini kald�r
	opistion.Password.RequireUppercase = false;
}).AddEntityFrameworkStores<ETicaretContext>()// EF ile veri taban� ba�lant�s�n� sa�lar
.AddDefaultTokenProviders();//default olarak token sa�lay�c� ekler

builder.Services.ConfigureApplicationCookie(opistion =>
{
	//giri� yap�lmad���nda y�nlendirilen sayfa
	opistion.LoginPath = "/Account/Login";
	//yetkisiz eri�im oldu�unda y�nlendirilecek sayfa
	opistion.AccessDeniedPath = "/";
	//
	opistion.Cookie = new CookieBuilder
	{
		Name = "AspNetCoreIdentityExampleCookie",  //�erez ismi
		HttpOnly = false,  //sadece http �zerinden eri�ilsin
		SameSite = SameSiteMode.Lax, //�erez ayn� sitede yap�lacak isteklerde ge�erli
		SecurePolicy = CookieSecurePolicy.Always  //�erez yaln�zca https �zerinde SSL sertifikas� olan istekler iletilecek
	};
	//�erez ge�erlilik s�resi dolduk�a yenilenir
	opistion.SlidingExpiration = true;
	//�erez ge�erlilik s�resi 15 dk
	opistion.ExpireTimeSpan = TimeSpan.FromMinutes(15);

});

builder.Services.AddSingleton<IBotFrameworkHttpAdapter, BotFrameworkHttpAdapter>();
builder.Services.AddSingleton<IBot, EchoBot>(); // <-- EchoBot.cs kendi botun

builder.Services.AddSingleton<ICredentialProvider>(sp =>
	new SimpleCredentialProvider(
		builder.Configuration["MicrosoftAppId"],
		builder.Configuration["MicrosoftAppPassword"]
	)
);

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: MyAllowSpecificOrigins,
					  policy =>
					  {
						  policy.WithOrigins("http://localhost:44395", "http://localhost:port")
								.AllowAnyHeader()
								.AllowAnyMethod();
					  });
});

// Oturum Y�netimi ��lemleri
//oturum y�netim servisi
builder.Services.AddSession();
//hata verirse yerini de�i�tir
var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}


app.UseHttpsRedirection(); //http den y�nlendirme yap�l�r
app.UseStaticFiles(); //,

app.UseRouting(); //http isteklerini y�nlendirme

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication(); //kimlik do�rulama i�lemleri

app.UseAuthorization(); //yetkilendirme i�lemleri

app.UseSession();

app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
});

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=List}/{id?}");

app.Run();
