using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace TelegramBotForSchool
{
    public class CategoryCollection
    {
        private List<Category> categories;

        public CategoryCollection(List<Category> categories)
        {
            this.categories = categories;
        }

        public string[] GetCategoriesNames()
        {
            string[] categoriesNames = new string[categories.Count];

            for (int i = 0; i < categories.Count; i++)
            {
                categoriesNames[i] = categories[i].Name;
            }

            return categoriesNames;
        }

        public Category GetCategoryWithName(string categoryName)
        {
            Category result = null;
            foreach (Category category in categories)
            {
                if (category.Name == categoryName)
                {
                    result = category;
                    break;
                }
            }
            return result;
        }
    }
}

