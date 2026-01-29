using AccountDAL.Eentiti;
using AccountDAL.Repositories;
using AccountDAL.Specifications.Delivery;
using AutoMapper;
using DashBoard.ViewModel;
using DashBoard.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DashBoard.Controllers
{
    public class DeliveryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public DeliveryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper=mapper;
        }
        public async Task<IActionResult> GetAllDelivery()
        {
            var specDelivery = new GetAllDeliverySpecification();
            var Data =await _unitOfWork.Repository<DeliveryCost>().GetAllWithSpecAsync(specDelivery);
            var MapDelivery = _mapper.Map<IReadOnlyList<DisplayDeliveryCostViewModel>>(Data);
            return View(MapDelivery);
        }
        public IActionResult DeliveryCountry()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeliveryCountry(DeliveryCountryViewModel deliveryCountry)
        {
            var data = _mapper.Map<DeliveryCountryViewModel, DeliveryCountry>(deliveryCountry);
            await _unitOfWork.Repository<DeliveryCountry>().AddAsync(data);
           await  _unitOfWork.Complete();
            return View();
        }
        public async Task< IActionResult> DeliveryCharge()
        {
            var deliveryCountry = await _unitOfWork.Repository<DeliveryCountry>().GetAllAsync();
            var viewModel = new DeliveryCostViewModel
            {
                DeliveryCountry = _mapper.Map<List<DeliveryCountryViewModel>>(deliveryCountry),

            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task< IActionResult> DeliveryCharge(DeliveryCostViewModel deliveryCostView)
        {
            var deliveryCostMap = _mapper.Map<DeliveryCostViewModel, DeliveryCost>(deliveryCostView);
           await _unitOfWork.Repository<DeliveryCost>().AddAsync(deliveryCostMap);
          await  _unitOfWork.Complete();
            return RedirectToAction("DeliveryCharge");
        }
    }
}
