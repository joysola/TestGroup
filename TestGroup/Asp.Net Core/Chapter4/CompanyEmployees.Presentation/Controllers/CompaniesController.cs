using Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DataTransferObjects;
using CompanyEmployees.Presentation.ModelBinders;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Entities.Responses;
using CompanyEmployees.Presentation.Extensions;
using MediatR;

namespace CompanyEmployees.Presentation.Controllers
{
    //[ApiVersion("1.0")]
    [Route("api/companies")]
    [ApiController]
    [ResponseCache(CacheProfileName = "120SecondsDuration")]
    [ApiExplorerSettings(GroupName = "v1")]
    public class CompaniesController : ApiControllerBase//ControllerBase
    {
        private readonly IServiceManager _service;
        private readonly ISender _sender;

        public CompaniesController(IServiceManager service) => _service = service;
        public CompaniesController(ISender sender) => _sender = sender;

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST");
            return Ok(); 
        }

        [HttpGet("2",Name = "GetCompanies2")]
        public IActionResult GetCompanies2()
        {
            var baseResult = _service.CompanyService.GetAllCompanies2(trackChanges: false);
            var companies = baseResult.GetResult<IEnumerable<CompanyDto>>();
            return Ok(companies);
        }

        [HttpGet("2/{id:guid}" ,Name = "CompanyById2")]
        public IActionResult GetCompany2(Guid id)
        {
            var baseResult = _service.CompanyService.GetCompany2(id, trackChanges: false);
            if (!baseResult.Success)
                return ProcessError(baseResult);
            var company = baseResult.GetResult<CompanyDto>();
            return Ok(company);
        }



        /// <summary>
        /// Gets the list of all companies
        /// </summary>
        /// <returns>The companies list</returns>
        [HttpGet(Name = "GetCompanies")]
        //[Authorize]
        [Authorize(Roles = "Manager")] // 需要在userrole中出现
        public IActionResult GetCompanies()
        {
            //throw new Exception("Exception");
            //try
            //{
            var companies = _service.CompanyService.GetAllCompanies(trackChanges: false);
            return Ok(companies);
            //}
            //catch
            //{
            //    return StatusCode(500, "Internal server error");
            //}
        }

        //[HttpGet("{id:guid}", Name = "CompanyById")]
        //public IActionResult GetCompany(Guid id)
        //{
        //    var company = _service.CompanyService.GetCompany(id, trackChanges: false);
        //    return Ok(company);
        //}



        //[ResponseCache(Duration = 60)]
        [HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
        [HttpCacheValidation(MustRevalidate = false)]
        [HttpGet("{id:guid}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }

        /// <summary>
        /// Creates a newly created company
        /// </summary>
        /// <param name="company"></param>
        /// <returns>A newly created company</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="422">If the model is invalid</response>
        [HttpPost(Name = "CreateCompany")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(422)]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id },
            createdCompany);
        }

        //[HttpPost]
        //public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
        //{
        //    if (company is null)
        //        return BadRequest("CompanyForCreationDto object is null");
        //    //var xx = System.Text.Json.JsonSerializer.Serialize(company);
        //    var createdCompany = _service.CompanyService.CreateCompany(company);
        //    return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        //}

        //[HttpGet("collection/({ids})", Name = "CompanyCollection")]
        //public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        //{
        //    var companies = _service.CompanyService.GetByIds(ids, trackChanges: false);
        //    return Ok(companies);
        //}

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges: false);
            return Ok(companies);
        }


        //[HttpPost("collection")]
        //public IActionResult CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        //{
        //    var result = _service.CompanyService.CreateCompanyCollection(companyCollection);
        //    return CreatedAtRoute("CompanyCollection", new { result.ids }, result.companies);
        //}

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids },
            result.companies);
        }


        //[HttpDelete("{id:guid}")]
        //public IActionResult DeleteCompany(Guid id)
        //{
        //    _service.CompanyService.DeleteCompany(id, trackChanges: false);
        //    return NoContent();
        //}

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);
            return NoContent();
        }


        //[HttpPut("{id:guid}")]
        //public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        //{
        //    if (company is null)
        //        return BadRequest("CompanyForUpdateDto object is null");
        //    _service.CompanyService.UpdateCompany(id, company, trackChanges: true);
        //    return NoContent();
        //}
        [HttpPut("{id:guid}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            //if (company is null)
            //    return BadRequest("CompanyForUpdateDto object is null");
            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
            return NoContent();
        }
    }

}
