using DocEsignAPI.Domain.Services;
using DocEsignAPI.Models;
using DocEsignAPI.Models.RequestModels;
using DocEsignAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace DocEsignAPI.Controllers
{
    public class EsignController : Controller
    {

        private EsignService _esignService;
        public EsignController(EsignService esignService)
        {
            _esignService = esignService;
        }

        [HttpPost("Esign")]
        public async Task<IActionResult> IndexAsync([FromForm] EsignRM model, [FromForm] IFormFile pdf)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EsignProp prop = new EsignProp();
                    prop.Cordinates = new Cordinates(model.Cordinates_X, model.Cordinates_Y, model.Cordinates_Width, model.Cordinates_Height);
                    prop.Reason = model.Reason ?? "";
                    prop.Location = model.Location ?? "Mumbai";
                    prop.Page_No = model.Page_NO;
                    prop.EsignDate = model.EsignDate;
                    byte[] fileContent = await _esignService.EsignDocAsync(prop, pdf);
                    return new FileContentResult(fileContent, "application/pdf");
                }
                else
                {
                    return BadRequest(model);
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return new JsonResult(new
                {
                    status_code = 500,
                    ErrorMessage = ex.Message
                });
            }
        }

        [HttpPost("Test")]
        public async Task<IActionResult> TestAsync()
        {

            var request = HttpContext.Request;
            return await Task.FromResult( new JsonResult(new
            {
                message = "test"
            }));
        }
    }
}
