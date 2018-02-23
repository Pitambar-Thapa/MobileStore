using StoreDomain;
using StoreService.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StoreService.Services
{
   public class CategoryService:GenericService<Category>
    {
        private MobileStoreEntities context = null;
        public CategoryService()
        {
            this.context = new MobileStoreEntities();
        }
        public void Save(Category entitiy)
        {
            entitiy.CreatedDate = DateTime.Now;
            entitiy.CreatedBy = 2;
            entitiy.Status = 10;
             this.Add(entitiy);
        }
        public void Edit(Category entity)
        {
            this.Update(entity);
        }
        public void Delete(object CategoryId)
        {

        }
        public List<Category> ListAll()
        {
            return this.LoadAll().Where(x=>!x.DeletedDate.HasValue).ToList();
        }
        public List<SelectListItem> CategoryListItem()
        {
            List<SelectListItem> categoryListItem = new List<SelectListItem>();
            var vendorList = this.LoadAll().Where(x => x.DeletedDate == null);
            foreach (var item in vendorList)
            {
                categoryListItem.Add(new SelectListItem() { Text = item.CName, Value = item.CategoryID.ToString() });
            }
            return categoryListItem;
        }
        public bool IsExistCategory(string categoryName)
        {
            var count = this.LoadAll().Where(x => x.CName.ToLower().Equals(categoryName.ToLower())).Count();
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
    }
}
