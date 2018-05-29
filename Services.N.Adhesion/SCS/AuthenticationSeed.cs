using System;

namespace BGBA.Services.N.Enrollment.SCS
{
    internal class AuthenticationSeed
    {
        public String Key { get; set; }
        public String Id { get; set; }

        public AuthenticationSeed(String key, String id)
        {
            Key = key;
            Id = id;
        }

        public AuthenticationSeed() { }
    }
}
