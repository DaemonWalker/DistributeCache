using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributeCache.Session.Test.Web
{
    public static class Utils
    {
        public static string GetRedisConnectionString(this IConfiguration config)
        {
            var redisConfig = config.GetSection("Redis");
            return $"{redisConfig["Address"]},defaultDatabase={redisConfig["DefaultDatabase"]},password={redisConfig["Password"]}";
        }
    }
}
