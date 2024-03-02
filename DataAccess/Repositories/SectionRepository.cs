using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
   public class SectionRepository:GenericRepository<Section>
    {
        CategoryRepository _categoryRepository = new CategoryRepository();

        public async Task<IEnumerable<Section>> GetSectionsByCountryId(long CountryId)
        {

            var Sections = await GetAll(" Where CountryId=" + CountryId + " ");
            foreach (var section in Sections)
            {
                var categories = await _categoryRepository.GetAll(" where SectionId=" + section.Id + "");
                section.Categories = categories;
            }

            return Sections;

        }
    }
}
