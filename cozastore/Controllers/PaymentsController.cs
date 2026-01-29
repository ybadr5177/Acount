using AccountDAL.Eentiti;
using AccountDAL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace cozastore.Controllers
{
   
    public class PaymentsController : BaseController
    {
        private readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent( string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);

            if (basket == null) return BadRequest(/*new ApiResponse(400, "A Problem With Your Basket")*/);

            return Ok(basket);
        }
    }
}
