using API.Extensions;
using API.Helpers;
using API.Middleware;
using AutoMapper;
using Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddScoped<IProductRepository, ProductRepository>();
            // services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddAutoMapper(typeof(MappingProfiles));
            services.AddControllers();
            services.AddDbContext<StoreContext>(x =>
                    x.UseSqlite(_configuration.GetConnectionString("DefaultConnection")));

            // services.Configure<ApiBehaviorOptions>(options =>
            //     options.InvalidModelStateResponseFactory = actionContext =>
            //      {
            //          var errors = actionContext.ModelState
            //          .Where(e => e.Value.Errors.Count > 0)
            //          .SelectMany(x => x.Value.Errors)
            //          .Select(x => x.ErrorMessage).ToArray();


            //          var errorResponse = new ApiValidationErrorResponse
            //          {
            //              Errors = errors
            //          };

            //          return new BadRequestObjectResult(errorResponse);
            //      }
            // );
            services.AddCors(policy => policy.AddPolicy("CorsPolicy", builder =>
               {
                   builder.WithOrigins("https://localhost:4200").AllowAnyMethod().AllowAnyHeader();
               }));
            services.AddApplicationServices();
            services.AddSwaggerDocumentation();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // if (env.IsDevelopment())
            // {
            //     app.UseDeveloperExceptionPage();
            // }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseStatusCodePagesWithReExecute("/errors/{0}");
            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseSwaggerDocumentation();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
