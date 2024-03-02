using Dapper;
using DataAccess.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class CategoryRepository : GenericRepository<Category>
    {
        public async Task<List<Category>> GetCategoriesWithSubCategoriesBySectionId(long SectionId, int Page)
        {
            Page = Page * 30;


            string sql = "SELECT  * FROM Category  INNER JOIN SubCategory  ON Category.Id = SubCategory.CategoryId where Category.SectionId="+ SectionId+ ";";
            var CategoryDictionary = new Dictionary<long, Category>();
            var list = _connectionFactory.GetConnection.Query<Category, SubCategory, Category>(
                sql,
                (category, subCategory) =>
                {
                    Category categoryEntry;

                    if (!CategoryDictionary.TryGetValue(category.Id, out categoryEntry))
                    {
                        categoryEntry = category;
                        categoryEntry.SubCategories = new List<SubCategory>();
                        CategoryDictionary.Add(categoryEntry.Id, categoryEntry);
                    }

                    categoryEntry.SubCategories.Add(subCategory);
                    return categoryEntry;
                },
                splitOn: "Id")
            .Distinct().Skip(Page)
            .ToList();
            

           return list;
        }
    }
}
