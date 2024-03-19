using AutoMapper;
using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Models.ViewModel;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace MagicVilla_Web.Controllers;

public class VillaNumberController : Controller
{
    private readonly IVillaNumberService _villaNumberService;
    private readonly IVillaService _villaService;
    private readonly IMapper _mapper;

    public VillaNumberController(IVillaNumberService villaNumberService, IMapper mapper, IVillaService villaService)
    {
        _villaNumberService = villaNumberService;
        _villaService = villaService;
        _mapper = mapper;
    }

    public async Task<IActionResult> IndexVillaNumber()
    {
        List<VillaNumberDto> list = new();
        var response = await _villaNumberService.GetAllAsync<ApiResponse>(HttpContext.Session.GetString(Sd.SessionToken));
        if (response != null)
        {
            list = JsonConvert.DeserializeObject<List<VillaNumberDto>>(Convert.ToString(response.Result));
        }

        return View(list);
    }
    
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateVillaNumber()
    {
        VillaNumberCreateViewModel villaNumberCreateViewModel = new();
        var response = await _villaService.GetAllAsync<ApiResponse>(HttpContext.Session.GetString(Sd.SessionToken));
        
        if (response != null && response.IsSuccess)
        {
            villaNumberCreateViewModel.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result)).Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }
        return View(villaNumberCreateViewModel);
    }
    
    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateViewModel villa)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaNumberService.CreateAsync<ApiResponse>(villa.VillaNumber, HttpContext.Session.GetString(Sd.SessionToken));
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }else 
            {
                if (response != null && response.ErrorMessages.Count > 0)
                {
                    ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault()!);   
                }
            }
        }
        
        var responseIfError = await _villaService.GetAllAsync<ApiResponse>(HttpContext.Session.GetString(Sd.SessionToken));
        
        if (responseIfError != null && responseIfError.IsSuccess)
        {
            villa.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(responseIfError.Result)).Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }
        return View(villa);
    }
    
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> UpdateVillaNumber(int villaNo)
    {
        VillaNumberUpdateViewModel villaNumberUpdateViewModel = new();
        var response = await _villaNumberService.GetAsync<ApiResponse>(villaNo, HttpContext.Session.GetString(Sd.SessionToken));
        if (response != null && response.IsSuccess)
        {
            VillaNumberDto model = JsonConvert.DeserializeObject<VillaNumberDto>(Convert.ToString(response.Result));
            villaNumberUpdateViewModel.VillaNumber = _mapper.Map<VillaNumberUpdateDto>(model);
        }
        
        response = await _villaService.GetAllAsync<ApiResponse>(HttpContext.Session.GetString(Sd.SessionToken));
        
        if (response != null && response.IsSuccess)
        {
            villaNumberUpdateViewModel.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result)).Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(villaNumberUpdateViewModel);
        }
        return NotFound();
    }

    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateViewModel model)
    {
        if (ModelState.IsValid)
        {
            var response = await _villaNumberService.UpdateAsync<ApiResponse>(model.VillaNumber, HttpContext.Session.GetString(Sd.SessionToken));
            if (response != null && response.IsSuccess)
            {
                return RedirectToAction(nameof(IndexVillaNumber));
            }else 
            {
                if (response != null && response.ErrorMessages.Count > 0)
                {
                    ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault()!);   
                }
            }
        }
        
        var responseIfError = await _villaService.GetAllAsync<ApiResponse>(HttpContext.Session.GetString(Sd.SessionToken));
        
        if (responseIfError != null && responseIfError.IsSuccess)
        {
            model.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(responseIfError.Result)).Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
        }
        return View(model);
    }
    
    
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> DeleteVillaNumber(int villaNo)
    {
        VillaNumberDeleteViewModel villaNumberDeleteViewModel = new();
        var response = await _villaNumberService.GetAsync<ApiResponse>(villaNo, HttpContext.Session.GetString(Sd.SessionToken));
        if (response != null && response.IsSuccess)
        {
            VillaNumberDto model = JsonConvert.DeserializeObject<VillaNumberDto>(Convert.ToString(response.Result));
            villaNumberDeleteViewModel.VillaNumber = model;
        }
        
        response = await _villaService.GetAllAsync<ApiResponse>(HttpContext.Session.GetString(Sd.SessionToken));
        
        if (response != null && response.IsSuccess)
        {
            villaNumberDeleteViewModel.VillaList = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result)).Select(i => new SelectListItem()
            {
                Text = i.Name,
                Value = i.Id.ToString()
            });
            return View(villaNumberDeleteViewModel);
        }
        return NotFound();
    }

    
    [Authorize(Roles = "admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteViewModel model)
    {
        var response = await _villaNumberService.DeleteAsync<ApiResponse>(model.VillaNumber.VillaNo, HttpContext.Session.GetString(Sd.SessionToken));
        if (response != null && response.IsSuccess)
        {
            return RedirectToAction(nameof(IndexVillaNumber));
        }

        return View(model);
    }
}