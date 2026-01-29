using AccountDAL.Eentiti;
using AccountDAL.Eentiti.Order_Aggregate;
using AccountDAL.Repositories;
using AccountDAL.Services;
using AccountDAL.Specifications.Delivery;
using AutoMapper;
using cozastore.ViewModel.Ordes;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace cozastore.Controllers
{
    public class OrdersController:BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;


        public OrdersController(IOrderService orderService, IMapper mapper, IBasketRepository basketRepo,IUnitOfWork unitOfWork)
        {
            _orderService = orderService;
            _mapper = mapper;
            _basketRepo = basketRepo;
            _unitOfWork=unitOfWork;
        }
        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody]OrderViewModel orderDto)
        {
            string buyerEmail = User.FindFirstValue(ClaimTypes.Email);

            var orderAddress = _mapper.Map<AddressViewModel, AccountDAL.Eentiti.Order_Aggregate.Address>(orderDto.ShippingAddress);

            var basket = await _basketRepo.GetBasketAsync(orderDto.BasketId);

            var order = await _orderService.CreateOrderAsync(buyerEmail, orderDto.BasketId, orderDto.DeliveryCostId, orderAddress, orderDto.PaymentMethod);

            if (order == null) return BadRequest(/*new ApiResponse(400, "An error occured during the creation of he order")*/);
            if (order != null) _basketRepo.DeleteBasketAsync(orderDto.BasketId);

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersForUser()
        {
            string buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var orders = await _orderService.GetOrdersForUser(buyerEmail);
            var PeFor = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnViewModel>>(orders);
            return View(PeFor);
        }
        [HttpGet]
        public async Task<ActionResult<OrderToReturnViewModel>> GetOrderForUser(int id)
        {
            string buyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var order = await _orderService.GetOrderById(id, buyerEmail);
            return Ok(_mapper.Map<Order, OrderToReturnViewModel>(order));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethods()
        {
            var deliveryMethods = await _orderService.GetDeliveryMethodAsync();
            return Ok(deliveryMethods);
        }
        [HttpPost]
        public async Task<ActionResult<IReadOnlyList<DeliveryCost>>> GetDeliveryCost([FromBody] DliverySpecParams orderDelivery)
        {
            var specDelivery = new FilterDliverryCostSpecification(orderDelivery);
            var deliveryMethods = await _unitOfWork.Repository<DeliveryCost>().GetAllWithSpecAsync(specDelivery);
            return Ok(deliveryMethods);
        }
    }
}
