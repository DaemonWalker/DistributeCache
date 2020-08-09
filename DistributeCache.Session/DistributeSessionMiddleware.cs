using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DistributeCache.Session
{
    public class DistributeSessionMiddleware
    {
        private readonly RequestDelegate _next;
        public DistributeSessionMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var sessionId = string.Empty;
            var session = context.RequestServices.GetService<ISession>();
            context.Session = session;
            await _next(context);
        }
    }
}
