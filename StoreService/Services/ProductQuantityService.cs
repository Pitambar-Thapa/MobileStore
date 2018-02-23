using StoreDomain;
using StoreWeb.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreService.Services
{
    public class ProductQuantityService : GenericService<ProductQuantity>
    {
        private MobileStoreEntities context = null;
        public ProductQuantityService()
        {
            this.context = new MobileStoreEntities();
        }
        public void Save(ProductContract entitiy,int ProductId)
        {
            ProductQuantity productQuantity = new ProductQuantity();
            productQuantity.PId = ProductId;
            productQuantity.Quantity = entitiy.Quantity;
            productQuantity.Rate = entitiy.Rate;
            productQuantity.CreatedBy = 2;
            productQuantity.CreatedDate = DateTime.Now;
            productQuantity.Status = 10;
            this.Add(productQuantity);
        } 

    }
}
