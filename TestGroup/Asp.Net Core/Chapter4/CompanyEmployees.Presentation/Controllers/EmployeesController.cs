﻿using Service.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DataTransferObjects;
using Microsoft.AspNetCore.JsonPatch;
using Shared.RequestFeatures;
using System.Text.Json;

namespace CompanyEmployees.Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager service) => _service = service;
        //[HttpGet]
        //public IActionResult GetEmployeesForCompany(Guid companyId)
        //{
        //    var employees = _service.EmployeeService.GetEmployees(companyId, trackChanges: false);
        //    return Ok(employees);
        //}

        //[HttpGet("{id:guid}")]
        [HttpGet("{id:guid}", Name = "GetEmployeeForCompany")]
        public IActionResult GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var employee = _service.EmployeeService.GetEmployee(companyId, id,
            trackChanges: false);
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            if (employee is null)
                return BadRequest("EmployeeForCreationDto object is null");
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges: false);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                id = employeeToReturn.Id
            },
            employeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            _service.EmployeeService.DeleteEmployeeForCompany(companyId, id, trackChanges: false);
            return NoContent();
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            if (employee is null)
                return BadRequest("EmployeeForUpdateDto object is null");
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            _service.EmployeeService.UpdateEmployeeForCompany(companyId, id, employee,
            compTrackChanges: false, empTrackChanges: true);
            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        public IActionResult PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id,
            [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");
            var result = _service.EmployeeService.GetEmployeeForPatch(companyId, id, compTrackChanges: false, empTrackChanges: true);
            //patchDoc.ApplyTo(result.employeeToPatch);
            patchDoc.ApplyTo(result.employeeToPatch, ModelState);
            TryValidateModel(result.employeeToPatch);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            _service.EmployeeService.SaveChangesForPatch(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }

        //[HttpGet]
        //public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
        //{
        //    var employees = await _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
        //    return Ok(employees);
        //}

        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> GetEmployeesForCompany(Guid companyId, [FromQuery] EmployeeParameters employeeParameters)
        {
            //var t1 = _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //var t2 = _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //var t3 = _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //var t4 = _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //var t5 = _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //var t6 = _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //var t7 = _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //await Task.WhenAll(t1, t2, t3, t4,t5,t6,t7);
            var pagedResult = await _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.employees);
        }
    }
}
