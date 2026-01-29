using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications.Size;
using AutoMapper;
using DashBoard.Helpers;
using DashBoard.ViewModel.CozaMaster;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DashBoard.Controllers
{
    public class SizeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SizeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> GetAllSize()
        {
            var socdsize = new SizeGetByTypeSpecifications();
            var dsize = await _unitOfWork.Repository<DifferentSize>().GetAllWithSpecAsync(socdsize);
            var dmap = _mapper.Map<List<DifferentSize>, List<DifferentSizeViewModel>>(dsize.ToList());
            return View(dmap);
        }

        public IActionResult CreatNameSize()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> CreatNameSize(NameSizeViewModel NSM)
        {
            var Nsmmap = _mapper.Map<NameSizeViewModel, SizeName>(NSM);
            _unitOfWork.Repository<SizeName>().AddAsync(Nsmmap);
            await _unitOfWork.Complete();
            return View();
        }

        #region CreatSize
        public IActionResult CreatSize()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatSize(DifferentSizeViewModel Dfsize)
        {
            if (!ModelState.IsValid)
                return View(Dfsize);

            var size = _mapper.Map<DifferentSizeViewModel, DifferentSize>(Dfsize);

            //if (Dfsize.Type == SizeType.Dimension && Dfsize.Dimensions != null)
            //{
            //    var dimensions = _mapper.Map<DimensionSizeViewModel, DimensionSize>(Dfsize.Dimensions);
            //    size.Dimensions = dimensions;
            //}

            await _unitOfWork.Repository<DifferentSize>().AddAsync(size);
            await _unitOfWork.Complete();

            return RedirectToAction(nameof(Index), "Users");
        }
        #endregion

        public async Task<IActionResult> GetAllNameSize()
        {
            var data = await _unitOfWork.Repository<SizeName>().GetAllAsync();
            var result = data.Select(x => new {
                value = x.Id,
                text = x.SizeNames
            });

            return Json(result);
        }


        #region EditSize
        public IActionResult EditSize(int id)
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EditSize(DifferentSizeViewModel Dfsize)
        {
            if (!ModelState.IsValid)
                return View(Dfsize);

            var size = _mapper.Map<DifferentSizeViewModel, DifferentSize>(Dfsize);

            //if (Dfsize.Type == SizeType.Dimension && Dfsize.Dimensions != null)
            //{
            //    var dimensions = _mapper.Map<DimensionSizeViewModel, DimensionSize>(Dfsize.Dimensions);
            //    size.Dimensions = dimensions;
            //}

            await _unitOfWork.Repository<DifferentSize>().AddAsync(size);
            return View(Dfsize);
        }
        #endregion
        #region DeleteSize
        public async Task<IActionResult> DeleteSize(int id)
        {
            var specData = new GetIdSizeSpecifications(id);
            var data = await _unitOfWork.Repository<DifferentSize>().GetByIdWithSpecAsync(specData);
            var category = _mapper.Map<DifferentSize, DifferentSizeViewModel>(data);


            return View(category);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteSizefn(int id)
        {
            if (ModelState.IsValid)
            {
                var specData = new GetIdSizeSpecifications(id);
                var DataById =await _unitOfWork.Repository<DifferentSize>().GetByIdWithSpecAsync(specData);
                //await _unitOfWork.Complete();
                //if (DataById.Type == SizeType.Dimension && DataById.Dimensions != null)
                //{
                //    _unitOfWork.Repository<DimensionSize>().Delete(DataById.Dimensions);

                //}
                _unitOfWork.Repository<DifferentSize>().Delete(DataById);
               await _unitOfWork.Complete();
            }
            return RedirectToAction(nameof(GetAllSize), "Size");
        }
        #endregion
        [HttpGet]
        public async Task<IActionResult> GetByTypeSize(string type)
        {
            if (!Enum.TryParse<SizeType>(type, true, out var parsedType))
                return BadRequest("Invalid size type");

            var spec = new SizeGetByTypeSpecifications(parsedType);

            var size = await _unitOfWork.Repository<DifferentSize>().GetAllWithSpecAsync(spec);
            if (parsedType == SizeType.Dimension)
            {
                var data = size
                     .Where(s => s.Dimensions != null)
                     .Select(s => new
                     {
                         Id = s.Id,
                         Label = $"{s.Dimensions.Width}x{s.Dimensions.Height}x{s.Dimensions.Depth}"

                     }).ToList();
                     
                return Json(data);
            }
            else
            {
                var data = size.Select(s => new
                {
                    Id = s.Id,
                    Label = s.Label
                }).ToList();
                return Json(data);
            }
           
        }
    }
}
