using AccountDAL.Eentiti;
using AccountDAL.Eentiti.Order_Aggregate;
using AccountDAL.Repositories;
using AccountDAL.Services;
using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = AccountDAL.Eentiti.CozaStore.Product;

namespace BAL.Service
{
    public class PaymentService: IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration,
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }



        public async Task<CustomerBasket> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var basket = await _basketRepo.GetBasketAsync(basketId);

            if (basket == null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryCostId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.Repository<DeliveryCost>().GetByIdAsync(basket.DeliveryCostId.Value);
                shippingPrice = deliveryMethod.Cost;
                basket.ShippingPrice = shippingPrice;
            }
           
            //foreach (var item in basket.Items)
            //{
            //    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
            //    if (item.Price != productpro)
            //        item.Price = product.Price;
            //}

            var service = new PaymentIntentService();

            PaymentIntent intent;
            // ✅ هذا هو الكود المضاف
            if (!string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                // استرجع حالة PaymentIntent من Stripe
                var existingIntent = await service.GetAsync(basket.PaymentIntentId);

                // إذا كان الدفع قد تم بنجاح، لا تفعل شيئًا سوى إعادة السلة
                if (existingIntent.Status == "succeeded")
                {
                    return basket;
                }
            }

            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                intent = await service.CreateAsync(options);
                basket.PaymentIntentId = intent.Id;
                basket.ClientSecret = intent.ClientSecret;
            }
            else
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket.Items.Sum(item => item.Quantity * (item.Price * 100)) + (long)shippingPrice * 100,
                };
                await service.UpdateAsync(basket.PaymentIntentId, options);
            }

            await _basketRepo.UpdateBasketAsync(basket);
            return basket;
        }
    }

}
