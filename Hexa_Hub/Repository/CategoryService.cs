using Hexa_Hub.Interface;
using Microsoft.EntityFrameworkCore;
namespace Hexa_Hub.Repository
{
    public class CategoryService : ICategory
    {
        private readonly DataContext _context;

        public CategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetAllCategories()
        {
            return await _context.Categories.Include(c => c.Assets).Include(c => c.SubCategories).ToListAsync();
        }

        public async Task<Category?> GetCategoryById(int id)
        {
            return await _context.Categories
                                 .Include(c => c.Assets)
                                 .Include(c => c.SubCategories)
                                 .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        public async Task<Category> AddCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<Category>UpdateCategory(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                throw new Exception("Category not Found");
            }

            _context.Categories.Remove(category);

        }
        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }

    }
}

