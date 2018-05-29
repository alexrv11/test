using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BGBA.Models.N.Client.Enrollment;
using BGBA.Models.N.Core.Trace;

namespace BGBA.Services.N.Enrollment
{
    public interface IEnrollmentServices : ITraceService
    {
        string ERROR_PIN_SCS { get; }
        string ERROR_ALREDY_REGISTERED { get; }

        string NOT_INFORMED { get; }
        string CONSECUTIVE_CHARACTERS { get; }
        string INCORRECT_CHARACTERS { get; }

        Task<string> EnrollClientAsync(EnrollmentData data);
        Task<string> EnrollAlphanumericAsync(EnrollmentData data);
        Task<List<EnrolledClient>> GetEnrolledClientsAsync(string documentNumber);
        Task<string> GetSCSCipherPasswordAsync(String userId, String password);
    }
}
