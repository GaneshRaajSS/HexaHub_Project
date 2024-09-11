using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hexa_Hub.Interface;
using Hexa_Hub.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hexa_Hub.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoriesController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly ISubCategory _subcategory;

        public SubCategoriesController(DataContext context, ISubCategory subCategory)
        {
            _context = context;
            _subcategory = subCategory;
        }

        // GET: api/SubCategories
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<SubCategory>>> GetSubCategories()
        {
            return await _subcategory.GetAllSubCategories();
        }


        // PUT: api/SubCategories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutSubCategory(int id, SubCategory subCategory)
        {
            if (id != subCategory.SubCategoryId)
            {
                return BadRequest();
            }


            _subcategory.UpdateSubCategory(subCategory);

            try
            {

                await _subcategory.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubCategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/SubCategories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubCategory>> PostSubCategory(SubCategory subCategory)
        {

            _subcategory.AddSubCategory(subCategory);
            await _subcategory.Save();

            return CreatedAtAction("GetSubCategory", new { id = subCategory.SubCategoryId }, subCategory);
        }

        // DELETE: api/SubCategories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {

            try
            {
                await _subcategory.DeleteSubCategory(id);
                await _subcategory.Save();
                return NoContent();
            }
            catch (Exception)
            {
                if (id == null)
                    return NotFound();
                return BadRequest();
            }
        }

        private bool SubCategoryExists(int id)
        {
            return _context.SubCategories.Any(e => e.SubCategoryId == id);
        }
    }
}
