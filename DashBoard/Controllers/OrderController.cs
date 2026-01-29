using AccountDAL.config;
using AccountDAL.Eentiti.Order_Aggregate;
using DashBoard.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DashBoard.Controllers
{
    public class OrderController : Controller
    {
        private readonly CozaStoreContext _context;

        public OrderController(CozaStoreContext context)
        {
            _context = context;
        }

        public IActionResult GetAllOrde()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TotalCount()
        {
            var count = await _context.Orders.CountAsync();
            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> ListAll()
        {
            var orders = await _context.Orders
                .Include(o => o.DeliveryCost)
                .Select(o => new
                {
                    id = o.Id,
                    buyerEmail = o.BuyerEmail,
                    orderDate = o.OrderDate,
                    status = o.Status.ToString(),
                    total = o.SubTotal + (o.DeliveryCost != null ? o.DeliveryCost.Cost : 0),
                    shippingStatus = o.ShippingStatus.ToString()
                })
                .OrderByDescending(o => o.orderDate)
                .ToListAsync();

            return Json(orders);
        }
        // ✅ تحديث حالة الشحن
        [HttpPost]
        public async Task<IActionResult> UpdateShippingStatus([FromBody] ShippingUpdateModel model)
        {
            if (model == null) return BadRequest("Invalid data.");

            var order = await _context.Orders.FindAsync(model.Id);
            if (order == null) return NotFound();

            if (Enum.TryParse(model.Status, out ShippingStatus newStatus))
            {
                order.ShippingStatus = newStatus;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Shipping status updated." });
            }

            return BadRequest("Invalid status value.");
        }

        // ✅ حذف الطلب
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return NotFound(new { success = false, message = "Order not found." });

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Order deleted successfully." });
        }

        // ✅ فلترة حسب حالة الدفع والشحن (اختياري)
        [HttpGet]
        public async Task<IActionResult> ListFiltered(string? paymentStatus, string? shippingStatus)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrEmpty(paymentStatus))
                query = query.Where(o => o.Status.ToString() == paymentStatus);

            if (!string.IsNullOrEmpty(shippingStatus))
                query = query.Where(o => o.ShippingStatus.ToString() == shippingStatus);

            var orders = await query
                .Include(o => o.DeliveryCost)
                .Select(o => new
                {
                    id = o.Id,
                    buyerEmail = o.BuyerEmail,
                    orderDate = o.OrderDate,
                    status = o.Status.ToString(),
                    total = o.SubTotal + (o.DeliveryCost != null ? o.DeliveryCost.Cost : 0),
                    shippingStatus = o.ShippingStatus.ToString()
                })
                .OrderByDescending(o => o.orderDate)
                .ToListAsync();

            return Json(orders);
        }
    }
}

