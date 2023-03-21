using System;
using Customer.Api.Utils;
using Customer.Api.DataLayer.EntityFramework.Context;
using Customer.Api.DataLayer.Utils;
using Customer.Api.DataLayer.Domain.Customer;

namespace Customer.Api
{
    public class Startup
    {
        /// <summary>
        /// Gets the application configuration object
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup" /> class.
        /// </summary>
        /// <param name="configuration">The application configuration</param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// This method gets called by the runtime. Use this method to add services to the container
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //Added to allow us to return entire object from DB with children without UI complaining https://stackoverflow.com/questions/59199593/net-core-3-0-possible-object-cycle-was-detected-which-is-not-supported
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );
            services.AddSwaggerGen();
            services.AddHttpContextAccessor();
            services.AddMemoryCache();
            //    services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            //    {
            //        builder
            //               .SetIsOriginAllowedToAllowWildcardSubdomains()
            //               .WithOrigins(
            //                   AppSettingsUtil.EnsureNoTrailingSlash(Configuration.GetValue<string>("AppSettings:FrontEndCorsOrigin"))
            //               )
            //               .AllowAnyHeader()
            //               .AllowAnyMethod()
            //               .AllowCredentials();
            //    }));

            //
            // Services
            //
            services.AddTransient<IBaseAppSettingsUtil, AppSettingsUtil>();
            services.AddTransient<IAppSettingsUtil, AppSettingsUtil>();
            services.AddTransient<ICustomerService, CustomerService>();
            services.AddScoped<IRepository, Repository>();
        }

        /// <summary>
        /// Configure 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IBaseAppSettingsUtil appSettingsUtil)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            //This default one has to be here for the default wwwroot folder setup
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseRouting();
            //app.UseCors("MyPolicy");
            //app.UseMiddleware<JwtMiddleware>(); //JWT Auth flows through here

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            RegisterDbContext(appSettingsUtil);
        }

        public void RegisterDbContext(IBaseAppSettingsUtil appSettingsUtil)
        {
            DataContextConfig.Contexts.Add("DefaultConnection", new DataContextConfig("DefaultConnection", new string[] { "Customer.Api.DataLayer" }, appSettingsUtil));
        }
    }
}
