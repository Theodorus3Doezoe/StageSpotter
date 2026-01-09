using System.Collections.Generic;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Interfaces
{
    public interface IReviewService
    {
        int AddReview(Review review);
        IEnumerable<Review> GetReviewsForBedrijf(int bedrijfId);
        double GetAverageRating(int bedrijfId);
    }
}
