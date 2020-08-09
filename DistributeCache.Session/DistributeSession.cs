using CSRedis;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace DistributeCache.Session
{
    public class DistributeSession : ISession
    {
        private readonly CSRedisClient redisDB;
        private readonly ILogger<DistributeSession> logger;
        private readonly DistributeSessionConfig config;
        private const string CACHE_SET_NAME = "NSESSIONID_";
        const string SESSION_NAME = "NSessionId";

        public string Id { get; internal set; }

        public bool IsAvailable => true;

        public IEnumerable<string> Keys => redisDB.HKeys(this.Id);

        public DistributeSession(IHttpContextAccessor httpContextAccessor, ILogger<DistributeSession> logger, DistributeSessionConfig sessionConfig)
        {
            this.logger = logger;
            this.config = sessionConfig;
            redisDB = new CSRedisClient(sessionConfig.RedisConnectionString);
            RedisHelper.Initialization(redisDB);

            if (sessionConfig.IsApiMode)
            {
                this.Id = this.GetSessionIdByHeader(httpContextAccessor.HttpContext);
            }
            else
            {
                this.Id = this.GetSessionIdByCookies(httpContextAccessor.HttpContext);
            }
        }
        public static string GenerateSessionId()
        {
            return $"{CACHE_SET_NAME}{Guid.NewGuid().ToString("N")}";
        }

        public void Clear()
        {
            redisDB.Del(this.Id);
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task LoadAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            redisDB.HDel(this.Id, key);
        }

        public void Set(string key, byte[] value)
        {
            if (key.IsValidSessionKey() == false)
            {
                throw new InvalidOperationException("SessionKey is invalid, cannot set value");
            }
            redisDB.HSet(this.Id, key, value);
        }

        public bool TryGetValue(string key, out byte[] value)
        {
            if (redisDB.HExists(this.Id, key))
            {
                value = redisDB.HGet<byte[]>(this.Id, key);
                redisDB.ExpireAsync(this.Id, config.ExpireSeconds);
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        protected virtual string GetSessionIdByCookies(HttpContext context)
        {
            var cookies = context.Request.Cookies;
            string sessionId = default;
            if (cookies.ContainsKey(SESSION_NAME))
            {
                sessionId = cookies[SESSION_NAME];
            }
            else
            {
                sessionId = DistributeSession.GenerateSessionId();
                context.Response.Cookies.Append(SESSION_NAME, sessionId);
            }
            return sessionId;
        }
        protected virtual string GetSessionIdByHeader(HttpContext context)
        {
            var authId = context?.Request.Headers[HttpRequestHeader.Authorization.ToString()];
            if (string.IsNullOrWhiteSpace(authId))
            {
                authId = DistributeSession.GenerateSessionId();
            }
            return authId;
        }
    }
}
