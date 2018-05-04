namespace BGBA.Models.N.Core.Trace
{
    /// <summary>
    /// Esta interfaz nos permite tener los datos para logear los llamados
    /// </summary>
    public interface ITraceable
    {
        string Request { get; set; }
        string Response { get; set; }
        long ElapsedTime { get; set; }
    }
}
