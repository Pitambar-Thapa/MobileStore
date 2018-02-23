using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace StoreWeb.DataContract
{
    public class ProductContract
    {
        public int ProductID { get; set; }
        public string PName { get; set; }
        public string PDetails { get; set; }
        public Nullable<int> VId { get; set; }
        public string ModelNumber { get; set; }
        public Nullable<int> ColorId { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<System.DateTime> DeletedDate { get; set; }
        public int Status { get; set; }
        public int CategoryId { get; set; }

        public int Quantity { get; set; }

        public decimal Rate { get; set; }

        public string CategoryName { get; set; }

        public string VendorName { get; set; }
        public string ColorName { get; set; }
    }
}