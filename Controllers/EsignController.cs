/*
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
    OF THIRD PARTY RIGHTS
    
    This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    http://itextpdf.com/terms-of-use/
    
    The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
    
    In accordance with Section 7(b) of the GNU Affero General Public License,
    a covered work must retain the producer line in every PDF that is created
    or manipulated using iText.
    
    You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the iText software without
    disclosing the source code of your own applications.
    These activities include: offering paid services to customers as an ASP,
    serving PDFs on the fly in a web application, shipping iText with a closed
    source product.
    
    For more information, please contact iText Software Corp. at this
    address: sales@itextpdf.com

 */

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
                    prop.Location = model.Location ?? "<Location Name>";
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
