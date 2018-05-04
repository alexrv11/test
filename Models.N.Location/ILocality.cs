using System.Collections.Generic;
namespace BGBA.Models.N.Location
{
    public interface ILocality
    {
        string Name { get; set; }
        List<BranchOffice> BranchOffices { get; set; }

    }
}