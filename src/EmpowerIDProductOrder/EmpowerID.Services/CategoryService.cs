using EmpowerID.Common.Enums;
using EmpowerID.Interfaces.Repository;
using EmpowerID.Interfaces.Services;
using EmpowerID.Models;
using EmpowerID.Models.CDC;
using Microsoft.Extensions.Logging;

namespace EmpowerID.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ILogger<CategoryService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepositoryAsync<Category> _categoryRepositoryAsync;
        private readonly IRepositoryAsync<CDC_Category> _cdc_CategoryRepositoryAsync;
        public CategoryService(ILogger<CategoryService> logger, IUnitOfWork unitOfWork, IRepositoryAsync<Category> categoryRepositoryAsync, IRepositoryAsync<CDC_Category> cdc_CategoryRepositoryAsync)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _categoryRepositoryAsync = categoryRepositoryAsync;
            _cdc_CategoryRepositoryAsync = cdc_CategoryRepositoryAsync;
        }
        public async Task SyncCDCCategoriesAsync(CancellationToken cancellationToken)
        {
            //Get the CDC Product From Primary/Source Database
            var cdcCategories = await _cdc_CategoryRepositoryAsync.GetAllFromRawSqlAsync("exec sp_get_CDC_Data_For_Categories", cancellationToken);

            _unitOfWork.ChangeDatabase(DatabaseConnection.Secondary);
            var uowCategoryRepository = _unitOfWork.GetRepositoryAsync<Category>();
            //Change the connection to Secondary/Destination Database
            uowCategoryRepository.ChangeDatabase(DatabaseConnection.Secondary);
            _logger.LogInformation($"Total CDC Categories found : {cdcCategories.Count}");

            if (cdcCategories.Any())
            {
                //Deleted Categories
                var deletedCategoryIds = cdcCategories.Where(p => p.DataStatus == DataStatus.Deleted).Select(s => s.Id);
                var deletedCategories = await uowCategoryRepository.FindAllAsync(s => deletedCategoryIds.Contains(s.Id), cancellationToken);

                if (deletedCategories != null && deletedCategories.Any())
                {
                    _logger.LogInformation($"Total CDC deleted categories found : {deletedCategories.Count}");
                    await uowCategoryRepository.RemoveRangeAsync(cancellationToken, [.. deletedCategories]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Inserted Categories
                var insertedCategories = cdcCategories.Where(p => p.DataStatus == DataStatus.Inserted).ToList();

                if (insertedCategories != null && insertedCategories.Any())
                {
                    _logger.LogInformation($"Total CDC inserted categories found : {insertedCategories.Count}");
                    var iCategories = new List<Category>();
                    insertedCategories.ForEach(ic =>
                    {
                        iCategories.Add(new Category { Id = ic.Id, Name = ic.Name });
                    });
                    await uowCategoryRepository.AddRangeAsync(cancellationToken, [.. iCategories]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Updated Categories
                var cdcUpdatedCategories = cdcCategories.Where(p => p.DataStatus == DataStatus.Updated);
                var updatedCategoryIds = cdcUpdatedCategories.Select(s => s.Id);
                var updatedCategories = await uowCategoryRepository.FindAllAsync(s => updatedCategoryIds.Contains(s.Id), cancellationToken);

                if (updatedCategories != null && updatedCategories.Any())
                {
                    _logger.LogInformation($"Total CDC updated categories found : {updatedCategories.Count}");
                    updatedCategories.ToList().ForEach(u =>
                    {
                        u.Name = cdcUpdatedCategories.First(s => s.Id == u.Id).Name;
                    });

                    await uowCategoryRepository.UpdateRangeAsync(cancellationToken, [.. updatedCategories]);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }

                //Cleanup CDC Table
                _logger.LogInformation($"Cleanup CDC Categories in source database");
                _unitOfWork.ChangeDatabase(Common.Enums.DatabaseConnection.Primary);
                _cdc_CategoryRepositoryAsync.ChangeDatabase(Common.Enums.DatabaseConnection.Primary);
                await _cdc_CategoryRepositoryAsync.GetAllFromRawSqlAsync("exec sp_CDC_Categories_Table_Cleanup", cancellationToken);
            }
        }

        public async Task<IList<Category>> GetCategoriesAsync(CancellationToken cancellationToken)
        {
            var categories = await _categoryRepositoryAsync.GetAllAsync(cancellationToken);
            return categories;
        }

        public async Task SaveCategoriesAsync(IList<Category> categories, CancellationToken cancellationToken)
        {
            await _categoryRepositoryAsync.AddRangeAsync(cancellationToken, [.. categories]);
        }
    }
}
