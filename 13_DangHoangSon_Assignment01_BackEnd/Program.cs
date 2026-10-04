using _13_DangHoangSon_Assignment01_BackEnd.Models;
using _13_DangHoangSon_Assignment01_BackEnd.Repositories;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using System.Text.Json.Serialization;

namespace _13_DangHoangSon_Assignment01_BackEnd
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. Cấu hình DbContext sử dụng chuỗi kết nối "MyCnn" từ appsettings.json
            builder.Services.AddDbContext<FunewsManagementContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyCnn")));

            // 2. Đăng ký Dependency Injection cho các Repositories
            builder.Services.AddScoped<ISystemAccountRepository, SystemAccountRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
            builder.Services.AddScoped<ITagRepository, TagRepository>();

            // 3. Cấu hình Controllers + OData + Bỏ qua lỗi vòng lặp JSON khi Include thực thể liên quan
            var modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntityType<SystemAccount>().HasKey(a => a.AccountId);
            modelBuilder.EntityType<NewsArticle>().HasKey(n => n.NewsArticleId);
            modelBuilder.EntityType<Category>().HasKey(c => c.CategoryId);
            modelBuilder.EntityType<Tag>().HasKey(t => t.TagId);

            modelBuilder.EntitySet<NewsArticle>("NewsArticles");
            modelBuilder.EntitySet<Category>("Categories");
            modelBuilder.EntitySet<SystemAccount>("SystemAccounts");
            modelBuilder.EntitySet<Tag>("Tags");

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                })
                .AddOData(options => options
                    .Select()
                    .Filter()
                    .OrderBy()
                    .Expand()
                    .Count()
                    .SetMaxTop(100)
                    .AddRouteComponents("odata", modelBuilder.GetEdmModel()));

            // 4. Cấu hình CORS để FrontEnd gọi API không bị chặn
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            // Cấu hình Swagger
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Kích hoạt CORS
            app.UseCors("AllowAll");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}