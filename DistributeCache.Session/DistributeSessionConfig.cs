using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DistributeCache.Session
{
    public class DistributeSessionConfig
    {
        public bool IsApiMode { get; set; } = false;
        public string RedisConnectionString { get; set; }
        public int ExpireSeconds { get; set; } = 30 * 60;
    }
}
