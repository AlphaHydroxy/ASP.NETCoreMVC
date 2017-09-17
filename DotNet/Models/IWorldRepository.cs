using System.Collections.Generic;

namespace DotNet.Models
{
    public interface IWorldRepository
    {
        IEnumerable<Trip> GetAllTrips();
    }
}