using System;
using Microsoft.Extensions.Configuration;

namespace Services.N.Afip
{
    public class AfipServices
    {
        private readonly IConfiguration _configuration;

        public AfipServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
    }
}
