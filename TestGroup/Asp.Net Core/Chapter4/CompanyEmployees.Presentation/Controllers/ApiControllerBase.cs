using Entities.ErrorModel;
using Entities.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Presentation.Controllers
{
    public class ApiControllerBase : ControllerBase
    {
        /// <summary>
        /// HandleError is a public method in your base class, right? Change it to [protected] and try again.
        /// 否则swagger报错
        /// </summary>
        /// <param name="baseResponse"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        protected IActionResult ProcessError(ApiBaseResponse baseResponse)
        {
            return baseResponse switch
            {
                ApiNotFoundResponse => NotFound(new ErrorDetails
                {
                    Message = ((ApiNotFoundResponse)baseResponse).Message,
                    StatusCode = StatusCodes.Status404NotFound
                }),
                ApiBadRequestResponse => BadRequest(new ErrorDetails
                {
                    Message = ((ApiBadRequestResponse)baseResponse).Message,
                    StatusCode = StatusCodes.Status400BadRequest
                }),
                _ => throw new NotImplementedException()
            };
        }
    }
}
