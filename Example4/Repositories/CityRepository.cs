using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmplaCore.Models;
using Dapper;
namespace AmplaCore.Repositories
{
    public class CityRepository
    {
        private readonly PostgresConnection _connection;

        public CityRepository(PostgresConnection connection)
        {
            _connection = connection;
        }

        /// <summary>
        /// Получить список популярных городов.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public List<City> GetTopCities(int count)
        {
            // TODO пока первые 4 города.
            List<City> res = _connection.DB.Query<City>("SELECT * FROM cities  where id>=24 ORDER BY id LIMIT(" + count + ")").ToList();
            return res;
        }

        public City GetCityByName(string cityName)
        {
            City res = _connection.DB.Query<City>("SELECT * FROM cities WHERE LOWER(name)=@name;",
                  new
                  {
                      name = new DbString { Value = cityName.Trim().ToLower() }
                  }).FirstOrDefault();
            return res;
        }

        public List<City> GetCityList()
        {
            return _connection.DB.Query<City>("select * from cities").ToList();
        }

        public int AddCity(City city)
        {
            var sqlQuery = "INSERT INTO cities (create_date,name) VALUES(now(),@name) RETURNING ID;";
            int resID = _connection.DB.Query<int>(sqlQuery, city).FirstOrDefault();
            city.ID = resID;
            return resID;
        }
        public List<City> GetCities()
        {
            List<City> res = _connection.DB.Query<City>("SELECT * FROM cities ORDER BY name").ToList();
            return res;

        }

        public City GetCity(int cityID)
        {
            City res = _connection.DB.Query<City>("SELECT * FROM cities WHERE id=@ID;",
                new
                {
                    ID = cityID
                }).FirstOrDefault();
            return res;
        }

        public List<City> GetCitiesByIDs(IEnumerable<int> ids)
        {
            List<City> res = _connection.DB
              .Query<City>("SELECT * FROM cities WHERE id = ANY(@ids)", new
              {
                  ids = ids.ToArray()
              }).ToList();

            return res;
        }
    }
}
