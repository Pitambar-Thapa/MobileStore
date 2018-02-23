using StoreDomain;
using StoreWeb.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StoreService.Services
{
    public class ProductService:GenericService<Product>
    {
        private MobileStoreEntities context = null;
        public ProductService()
        {
            this.context = new MobileStoreEntities();
        }
        public void Save(ProductContract entitiy)
        {
            Product product = new Product();
            product.PName = entitiy.PName;
            product.PDetails = entitiy.PDetails;
            product.CreatedDate = DateTime.Now;
            product.VId = entitiy.VId;
            product.CategoryId = entitiy.CategoryId;
            product.ColorId = entitiy.ColorId;
            product.CreatedBy = 2;
            product.Status = 10;
            this.Add(product);
            int pId = this.context.Product.OrderByDescending(x => x.ProductID).Select(x => x.ProductID).FirstOrDefault();

            ProductQuantityService productQuantityService = new ProductQuantityService();
            productQuantityService.Save(entitiy, pId);
        }
        public void Edit(Product entity)
        {
            this.Update(entity);
        }
        public void Delete(object ProductId)
        {

        }
        public List<ProductContract> GetAllProducts()
        {
            var products = this.context.spGetProductDetails().ToList();
            List<ProductContract> productContractList = new List<ProductContract>();
            foreach (var item in products)
            {
                ProductContract productContract = new ProductContract();
                productContract.ProductID = item.ProductID;
                productContract.PName = item.PName;
                productContract.PDetails = item.PDetails;
                productContract.VendorName = item.VName;
                productContract.ColorName = item.ColorName;
                productContract.Quantity = item.Quantity;
                productContract.Rate = item.Rate;
                productContractList.Add(productContract);
            }
             return productContractList;
        }
        public List<Product> GetAllProducts(int? vendorId,int? categoryId)
        {
            List<Product> productList = new List<Product>();
            var searchList = this.LoadAll().Where(x => x.VId == vendorId && !x.DeletedDate.HasValue).ToList();
            return productList;
        }
        public List<SelectListItem> GetProductListItem()
        {
            return this.LoadAll().Where(x => !x.DeletedDate.HasValue).Select(x => new SelectListItem { Text = x.PName.ToString(), Value = x.ProductID.ToString() }).ToList();
        }
    }
}
