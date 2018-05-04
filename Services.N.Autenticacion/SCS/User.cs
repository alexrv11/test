namespace BGBA.Services.N.Autenticacion.SCS
{
    internal class User
    {
        public User(string userId, string password)
        {
            this.UserId = userId;
            this.Password = password;
        }

        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
