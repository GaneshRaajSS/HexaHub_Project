namespace Hexa_Hub.Interface
{
    public interface ISubCategory
    {
        Task<List<SubCategory>> GetAllSubCategories();

        Task AddSubCategory(SubCategory subcategory);
        Task<SubCategory> UpdateSubCategory(SubCategory subcategory);
        Task DeleteSubCategory(int id);

        Task Save();
    }
}
