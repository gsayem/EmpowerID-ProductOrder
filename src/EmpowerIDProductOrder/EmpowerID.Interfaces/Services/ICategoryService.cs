using EmpowerID.Models;

namespace EmpowerID.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IList<Category>> GetCategoriesAsync(CancellationToken cancellationToken);
        Task SaveCategoriesAsync(IList<Category> categories, CancellationToken cancellationToken);
        Task SyncCDCCategoriesAsync(CancellationToken cancellationToken);
    }
}
