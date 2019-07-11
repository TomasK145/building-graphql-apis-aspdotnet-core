using CarvedRock.Api.Data;
using CarvedRock.Api.GraphQL;
using CarvedRock.Api.Repositories;
using GraphQL;
using GraphQL.Server;
using GraphQL.Server.Ui.Playground;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarvedRock.Api
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IHostingEnvironment _env;

        public Startup(IConfiguration config, IHostingEnvironment env)
        {
            _config = config;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddDbContext<CarvedRockDbContext>(options => options.UseInMemoryDatabase("CarvedRock"));
                //options.UseSqlServer(_config["ConnectionStrings:CarvedRock"]));
            services.AddScoped<ProductRepository>();

            services.AddScoped<IDependencyResolver>(s => new FuncDependencyResolver(s.GetRequiredService)); //DI konfiguracia pre ziskanie IDependencyResolver
            services.AddScoped<CarvedRockSchema>();

            services.AddGraphQL(options => 
                {
                    options.ExposeExceptions = true; //--> urcenie ci JSON response ma obsahovat detailne info o chybe (bolo by vhodne nastavovat podla prostredia (DEV/PROD))
                    //options.ComplexityConfiguration //--> moznost nastavenia komplexnosti query (max nesting level pre query)
                    
                    //options.EnableMetrics //--> trackuje execution time pre queries, defaultne TRUE
                    //Metrics data je mozne ziskat pouzitim Context objektu pri vytvarani Resolve metody pre GraphType
                })
                .AddGraphTypes(ServiceLifetime.Scoped); //pre ziskanie vsetkych GraphTypes (scanuje assembly)
        }

        public void Configure(IApplicationBuilder app, CarvedRockDbContext dbContext)
        {
            app.UseGraphQL<CarvedRockSchema>(); //pridanie GraphQL middlewaru --> ako parameter je mozne uviest ENDPOINT, ak nie je uvedeny tak bude /graphql
            app.UseGraphQLPlayground(new GraphQLPlaygroundOptions()); //middleware pre GraphQL playground, options defaultne definuju playgroud dostupne na /ui/playground  a je ocakavane, ze GraphQL je na endpointe /graphql (default pre GraphQL middleware)
            dbContext.Seed();
        }
    }
}