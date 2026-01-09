using System;
using System.Collections.Generic;
using StageSpotter.Business.Interfaces;
using StageSpotter.Data.Interfaces;
using StageSpotter.Domain.Models;

namespace StageSpotter.Business.Services
{
    public class SavedItemService : ISavedItemService
    {
        private readonly ISavedAnalysisRepository _analysisRepo;
        private readonly ISavedVacatureRepository _vacatureRepo;

        public SavedItemService(ISavedAnalysisRepository analysisRepo, ISavedVacatureRepository vacatureRepo)
        {
            _analysisRepo = analysisRepo;
            _vacatureRepo = vacatureRepo;
        }

        public int SaveAnalysis(int userId, string fileName, string result)
        {
            var sa = new SavedAnalysis
            {
                UserId = userId,
                FileName = fileName,
                Result = result,
                CreatedAt = DateTime.UtcNow
            };
            return _analysisRepo.Create(sa);
        }

        public IEnumerable<SavedAnalysis> GetAnalysesForUser(int userId)
        {
            return _analysisRepo.GetByUserId(userId);
        }

        public SavedAnalysis? GetAnalysisById(int id)
        {
            return _analysisRepo.GetById(id);
        }

        public int SaveVacature(int userId, int vacatureId)
        {
            var sv = new SavedVacature
            {
                UserId = userId,
                VacatureId = vacatureId,
                CreatedAt = DateTime.UtcNow
            };
            return _vacatureRepo.Create(sv);
        }

        public IEnumerable<SavedVacature> GetVacaturesForUser(int userId)
        {
            return _vacatureRepo.GetByUserId(userId);
        }

        public int RemoveVacature(int userId, int vacatureId)
        {
            return _vacatureRepo.Delete(userId, vacatureId);
        }
    }
}
