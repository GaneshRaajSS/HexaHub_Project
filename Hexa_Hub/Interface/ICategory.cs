using Microsoft.EntityFrameworkCore;
namespace Hexa_Hub.Interface
{

    public interface ICategory
    {
        Task<List<Category>> GetAllCategories();

        Task<Category> AddCategory(Category category);
        Task<Category> UpdateCategory(Category category);
        Task DeleteCategory(int id);
        Task Save();
    }

}

