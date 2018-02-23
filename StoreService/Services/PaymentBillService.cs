using StoreDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace StoreService.Services
{

    
    public class PaymentBillService:GenericService<PaymentBill>
    {
        private MobileStoreEntities context = null;
        public PaymentBillService()
        {
            this.context = new MobileStoreEntities();
        }
            public void Save(PaymentBill entitiy)
        {
            entitiy.CreatedDate = DateTime.Now;
            entitiy.CreatedBy = 2;
            entitiy.Status = 10;
            this.Add(entitiy);
        }
        public void Edit(PaymentBill entity)
        {
            this.Update(entity);
        }
        public void Delete(object PaymentBillId)
        {

        }

      
    }
}
