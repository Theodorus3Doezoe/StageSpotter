using System.Collections.Generic;
using StageSpotter.Domain.Models;

namespace StageSpotter.Data.Interfaces
{
    public interface IReviewRepository
    {
        int Create(Review review);
        IEnumerable<Review> GetByBedrijfId(int bedrijfId);
        double GetAverageRating(int bedrijfId);
    }
}
