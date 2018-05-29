namespace BGBA.MS.N.Adhesion.Models
{
    public class EnrollResult
    {
        public EnrollState? EnrollState { get; set; }
        public string EnrollNumber { get; set; }
        public AlphanumericState? AlphanumericState { get; set; }
    }

    public enum EnrollState
    {
        OK,
        ALREADY_ENROLL,
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
