using System.Collections.Generic;
using StageSpotter.Business.Interfaces;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepo;

        public ReviewService(IReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        public int AddReview(Review review)
        {
            review.CreatedAt = System.DateTime.UtcNow;
            return _reviewRepo.Create(review);
        }

        public IEnumerable<Review> GetReviewsForBedrijf(int bedrijfId)
        {
            return _reviewRepo.GetByBedrijfId(bedrijfId);
        }

        public double GetAverageRating(int bedrijfId)
        {
            return _reviewRepo.GetAverageRating(bedrijfId);
        }
    }
}
