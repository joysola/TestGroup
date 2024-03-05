using MediatR;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands
{
    public sealed record UpdateCompanyCommand1(Guid Id, CompanyForUpdateDto Company, bool TrackChanges) : IRequest;
    public sealed record UpdateCompanyCommand2(Guid Id, CompanyForUpdateDto Company, bool TrackChanges) : IRequest<Unit>;
}
