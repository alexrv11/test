using System.Collections.Generic;
namespace Models.N.Location
{
    public interface ILocality
    {
        string Name { get; set; }
        List<BranchOffice> BranchOffices { get; set; }

    }
}