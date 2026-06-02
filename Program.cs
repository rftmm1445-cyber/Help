//namespace WebApplication4
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.
//            builder.Services.AddControllersWithViews();
//            // إضافة التخزين المؤقت للجلسة
//            builder.Services.AddDistributedMemoryCache();

//            // إضافة الجلسة نفسها
//            builder.Services.AddSession(options =>
//            {
//                options.IdleTimeout = TimeSpan.FromMinutes(30); // مدة انتهاء الجلسة
//                options.Cookie.HttpOnly = true; // حماية الكوكيز
//                options.Cookie.IsEssential = true; // لتعمل حتى إذا رفض المستخدم بعض الكوكيز
//            });

//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (!app.Environment.IsDevelopment())
//            {
//                app.UseExceptionHandler("/Home/Error");
//                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//                app.UseHsts();
//            }

//            app.UseHttpsRedirection();
//            app.UseStaticFiles();

//            app.UseRouting();
//            app.UseSession(); // ✅ تفعيل الجلسة


//            app.UseAuthorization();

//            app.MapControllerRoute(
//                name: "default",
//                pattern: "{controller=Home}/{action=Index}/{id?}");

//            app.Run();
//        }
//    }
//}
namespace WebApplication4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // 1️⃣ السماح لملفات العرض (Views / Layout) بالوصول إلى الجلسة ديناميكياً
            builder.Services.AddHttpContextAccessor();

            // إضافة التخزين المؤقت للجلسة
            builder.Services.AddDistributedMemoryCache();

            // إضافة الجلسة نفسها وتكوينها
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // مدة انتهاء الجلسة
                options.Cookie.HttpOnly = true; // حماية الكوكيز من الاختراق عبر السكريبتات
                options.Cookie.IsEssential = true; // لتعمل الجلسة بكفاءة أساسية في المتصفح
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // ✅ تفعيل الجلسة في المكان الصحيح بعد التوجيه وقبل الصلاحيات
            app.UseSession();

            app.UseAuthorization();

            // 2️⃣ جعل صفحة تسجيل الدخول (Login) هي نقطة انطلاق المشروع الأولى تلقائياً
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Account}/{action=Login}/{id?}");

            app.Run();
        }
    }
}
