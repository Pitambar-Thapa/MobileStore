using StoreDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StoreService.Services
{
    public class VendorService:GenericService<Vendor>
    {
        private MobileStoreEntities context = null;
        public VendorService()
        {
            this.context = new MobileStoreEntities();
        }
        public void Save(Vendor entitiy)
        {
            entitiy.CreatedDate = DateTime.Now;
            entitiy.CreatedBy = 2;
            entitiy.Status = 10;
            this.Add(entitiy);
        }
        public void Edit(Vendor entity)
        {
            this.Update(entity);
        }
        public void Delete(object VendorId)
        {

        }
        public List<Vendor> ListAll()
        {
           return this.LoadAll().Where(x=>!x.DeletedDate.HasValue).ToList();
        }
        public List<SelectListItem> GetVendorListItem()
        {
            List<SelectListItem> vendorListItem = new List<SelectListItem>();
            var vendorList = this.LoadAll().Where(x=>!x.DeletedDate.HasValue);
            foreach (var item in vendorList)
            {
                vendorListItem.Add(new SelectListItem() { Text = item.VName, Value = item.VendorID.ToString() });
            }
            return vendorListItem;
        }

        public bool IsExistVendor(string vendorName)
        {
            var count = this.LoadAll().Where(x => x.VName.ToLower().Equals(vendorName.ToLower())).Count();
            if(count==0)
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
