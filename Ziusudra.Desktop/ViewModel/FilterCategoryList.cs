using System.ComponentModel;

namespace Ziusudra.Desktop.ViewModel
{

    public class FilterCategoryList:
        BindingList<FilterCategory>
    {

        public FilterCategoryList()
        { }

        public FilterCategoryList(IEnumerable<FilterCategory> filterCategories):
            base(filterCategories.ToList())
        { }

        public void Update(IEnumerable<FilterCategory> filterCategories)
        {
            if (Count > 0)
            {
                foreach (FilterCategory category in Items.ToList())
                    if (!filterCategories.Contains(category))
                        Remove(category);

                foreach (FilterCategory category in filterCategories)
                {
                    FilterCategory? existingCategory = Items.FirstOrDefault(fc => fc == category);
                    if (existingCategory is not null)
                    {
                        foreach (Filter fil in existingCategory.Except(category).ToList())
                            existingCategory.Remove(fil);
                        foreach (Filter fil in category)
                        {
                            Filter? f = existingCategory.FirstOrDefault(f => f == fil);
                            if (f is null)
                                existingCategory.Add(fil);
                            else
                                f.Update(fil);
                        }
                    } else
                        Add(category);
                }
            } else
                foreach (FilterCategory category in filterCategories)
                    Add(category);
        }
    }
}
