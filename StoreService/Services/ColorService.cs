using StoreDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StoreService.Services
{
    public class ColorService:GenericService<Color>
    {
        private MobileStoreEntities context = null;
        public ColorService()
        {
            this.context = new MobileStoreEntities();
        }
        public void Save(Color entitiy)
        {
            entitiy.CreatedDate = DateTime.Now;
            entitiy.CreatedBy = 2;
            entitiy.Status = 10;
            this.Add(entitiy);
        }
        public void Edit(Color entity)
        {
            this.Update(entity);
        }
        public void Delete(object ColorId)
        {

        }
        public List<Color> ListAll()
        {
            return this.LoadAll().ToList();
        }
        public bool IsExistColor(string colorName)
        {
            var count = this.LoadAll().Where(x => x.ColorName.ToLower().Equals(colorName.ToLower())).Count();
            if (count == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

        }
        public List<SelectListItem> GetColorListItem()
        {
            return this.LoadAll().Where(x => !x.DeletedDate.HasValue).Select(x => new SelectListItem { Text = x.ColorName.ToString(), Value = x.ColorID.ToString() }).ToList();
        }
    }
}
