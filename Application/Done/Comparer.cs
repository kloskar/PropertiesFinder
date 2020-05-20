using System;
using System.Collections.Generic;
using System.Text;
using Models;
using System.Diagnostics.CodeAnalysis;

namespace Application.Done
{
    class Comparer : IEqualityComparer<Entry>
    {
        public bool Equals(Entry x, Entry y)
        {
            if (x.OfferDetails.Url.Equals(y.OfferDetails.Url))
                return true;
            return false;
        }

        public int GetHashCode([DisallowNull] Entry obj)
        {
            return obj.PropertyAddress.City.GetHashCode() +
                   obj.PropertyAddress.StreetName.GetHashCode() +
                   obj.PropertyAddress.District.GetHashCode();
        }
    }
}
