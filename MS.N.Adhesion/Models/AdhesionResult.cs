namespace BGBA.MS.N.Adhesion.Models
{
    public class AdhesionResult
    {
        public AdhesionState? AdhesionState { get; set; }
        public string IdAdhesion { get; set; }
        public AlphanumericState? AlphanumericState { get; set; }
    }

    public enum AdhesionState
    {
        OK,
        ALREDY_REGISTERED,
        INVALID_PIN
    }

    public enum AlphanumericState
    {
        OK,
        NOT_INFORMED,
        CONSECUTIVE_CHARACTERS,
        INCORRECT_CHARACTERS
    }
}
