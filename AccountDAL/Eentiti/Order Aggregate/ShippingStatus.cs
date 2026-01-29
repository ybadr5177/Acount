using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.Order_Aggregate
{
    public enum ShippingStatus
    {
        Pending = 0,       // قيد التجهيز
        Shipped = 1,       // تم الإرسال
        OutForDelivery = 2, // في الطريق
        Delivered = 3,     // تم التوصيل
        Cancelled = 4
    }
}
