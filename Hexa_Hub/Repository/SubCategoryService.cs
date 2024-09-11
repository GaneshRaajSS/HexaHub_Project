using Hexa_Hub.Interface;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Repository
{
    public class SubCategoryService : ISubCategory
    {
        private readonly DataContext _context;

        public SubCategoryService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<SubCategory>> GetAllSubCategories()
        {
            return await _context.SubCategories.ToListAsync();
        }




        public async Task AddSubCategory(SubCategory subCategory)
        {
            _context.SubCategories.Add(subCategory);

        }

        public Task<SubCategory> UpdateSubCategory(SubCategory subCategory)
        {
            _context.SubCategories.Update(subCategory);
            return Task.FromResult(subCategory);
        }

        public async Task DeleteSubCategory(int subCategoryId)
        {
            var subCategory = await _context.SubCategories.FindAsync(subCategoryId);
            if (subCategory == null)
            {

                throw new Exception("SubCategory not Found");
            }
            _context.SubCategories.Remove(subCategory);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
