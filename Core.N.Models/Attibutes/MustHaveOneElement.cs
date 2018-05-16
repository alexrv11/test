using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Models.N.Core.Attibutes
{
    public class MustHaveOneElementAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var collection = value as ICollection;
            if (collection != null)
            {
                return collection.Count > 0;
            }
            return false;
        }
    }
}
