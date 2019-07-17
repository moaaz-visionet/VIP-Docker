using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tour_Of_Heroes_Server.Models;

namespace Tour_Of_Heroes_Server
{
    public static class ElasticsearchExtensions
    {
        public static void AddElasticSearch(this IServiceCollection services, IConfiguration configuration)
        {
            var url = configuration["ElasticServer"];
            var defaultIndex = configuration["Index"];

            var settings = new ConnectionSettings(new Uri(url)).DefaultIndex(defaultIndex);

            settings.DefaultMappingFor<Hero>(m => m.PropertyName(h => h.id, "id"));

            var client = new ElasticClient(settings);

            while (!client.Ping().IsValid);

            services.AddSingleton<IElasticClient>(client);

            var createIndexResponse = client.CreateIndex(defaultIndex, c => c.Mappings(x => x.Map<Hero>(m => m.AutoMap().Properties(p => p))));

            var result = client.IndexMany(GetHeroes());
        }

        private static List<Hero> GetHeroes()
        {
            return new List<Hero>
            {
                new Hero { id = 11, name= "Dr Nice" },
                new Hero { id = 12, name= "Narco" },
                new Hero { id = 13, name= "Bombasto" },
                new Hero { id = 14, name= "Celeritas" },
                new Hero { id = 15, name= "Magneta" },
                new Hero { id = 16, name= "RubberMan" },
                new Hero { id = 17, name= "Dynama" },
                new Hero { id = 18, name= "Dr IQ" },
                new Hero { id = 19, name= "Magma" },
                new Hero { id = 20, name= "Tornado" }
            };
        }
    }
}
