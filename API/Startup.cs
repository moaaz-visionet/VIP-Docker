using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tour_Of_Heroes_Server.Common;
using Tour_Of_Heroes_Server.Models;
using System.Data.SqlClient;
using System.Data;
using Nest;

namespace Tour_Of_Heroes_Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            AppConfig.ConnectionString = configuration.GetSection("ConnectionString").Value;
            AppConfig.ElasticServer = configuration.GetSection("ElasticServer").Value;
            AppConfig.Index = configuration.GetSection("Index").Value;

            //var connectionString = new ConnectionSettings(new Uri(AppConfig.ElasticServer));
            //var _elasticClient = new ElasticClient(connectionString);

            //if (_elasticClient.IndexExists(AppConfig.Index).Exists)
            //{
            //    _elasticClient.DeleteIndex(AppConfig.Index);
            //}
            //else
            //{
            //    var newIndex = new CreateIndexDescriptor(AppConfig.Index).Mappings(x => x.Map<Hero>(m => m.AutoMap().Properties(p => p)));
            //}

            //var result = _elasticClient.IndexMany(GetHeroes(), AppConfig.Index);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(options =>
            {
                options.AddPolicy("cors",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .DisallowCredentials();
                    });



            });
            services.AddMvc();

            services.AddElasticSearch(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("cors");
            app.UseHttpsRedirection();
            app.UseMvc();
        }

        private List<Hero> GetHeroes()
        {
            using (var sqlConnection = new SqlConnection(AppConfig.ConnectionString))
            {
                var heroes = new List<Hero>();
                using (var sqlCommand = new SqlCommand("Get_Heroes", sqlConnection))
                {
                    sqlCommand.CommandTimeout = 180;
                    sqlCommand.CommandType = CommandType.StoredProcedure;

                    try
                    {
                        sqlCommand.Connection.Open();

                        using (var reader = sqlCommand.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                heroes.Add(new Hero()
                                {
                                    id = Convert.ToInt32(reader["id"]),
                                    name = reader["name"].ToString()
                                });
                            }

                            return heroes;
                        }
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                }
            }
        }
    }
}
