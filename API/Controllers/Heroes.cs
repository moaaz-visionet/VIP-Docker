using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tour_Of_Heroes_Server.Models;
using Tour_Of_Heroes_Server.Common;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using Nest;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Tour_Of_Heroes_Server.Controllers
{
    [Route("api/[controller]")]
    public class Heroes : Controller
    {
        // GET: api/values
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = await Search();
                return Ok(result.OrderBy(x => x.id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var heroes = await Search();

                return Ok(heroes.FirstOrDefault(x => x.id == id));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        public async Task<IActionResult> Get(string name)
        {
            try
            {
                var heroes = await Search();

                return Ok(heroes.FirstOrDefault(x => x.name == name));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private async Task<List<Hero>> Search(string value = "")
        {
            var connectionString = new ConnectionSettings(new Uri(AppConfig.ElasticServer));
            var _elasticClient = new ElasticClient(connectionString);

            if (_elasticClient.IndexExists(AppConfig.Index).Exists)
            {
                _elasticClient.DeleteIndex(AppConfig.Index);
            }
            else
            {
                var newIndex = new CreateIndexDescriptor(AppConfig.Index).Mappings(x => x.Map<Hero>(m => m.AutoMap().Properties(p => p)));
            }

            _elasticClient.IndexMany(GetHeroes(), AppConfig.Index);

            var result = await _elasticClient.SearchAsync<Hero>(x => x
                        .Index(AppConfig.Index)
                        .Explain(false)
                        .Query(q => q.QueryString(m => m.Query(value + "*"))));

            return (from x in result.Hits select x.Source).ToList();
        }

        private List<Hero> GetHeroes()
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

            //using (var sqlConnection = new SqlConnection(AppConfig.ConnectionString))
            //{
            //    var heroes = new List<Hero>();
            //    using (var sqlCommand = new SqlCommand("Get_Heroes", sqlConnection))
            //    {
            //        sqlCommand.CommandTimeout = 180;
            //        sqlCommand.CommandType = CommandType.StoredProcedure;

            //        try
            //        {
            //            sqlCommand.Connection.Open();

            //            using (var reader = sqlCommand.ExecuteReader())
            //            {
            //                while (reader.Read())
            //                {
            //                    heroes.Add(new Hero()
            //                    {
            //                        id = Convert.ToInt32(reader["id"]),
            //                        name = reader["name"].ToString()
            //                    });
            //                }

            //                return heroes;
            //            }
            //        }
            //        catch (Exception e)
            //        {
            //            throw;
            //        }
            //    }
            //}
        }
    }
}
