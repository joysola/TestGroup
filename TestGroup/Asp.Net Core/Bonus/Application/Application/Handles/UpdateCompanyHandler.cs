using Application.Commands;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handles
{
    internal sealed class UpdateCompanyHandler1 : IRequestHandler<UpdateCompanyCommand1>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        public UpdateCompanyHandler1(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task Handle(UpdateCompanyCommand1 request, CancellationToken cancellationToken)
        {
            var companyEntity = await _repository.Company.GetCompanyAsync(request.Id, request.TrackChanges);
            if (companyEntity is null)
                throw new CompanyNotFoundException(request.Id);
            _mapper.Map(request.Company, companyEntity);
            await _repository.SaveAsync();
        }
    }


    internal sealed class UpdateCompanyHandler2 : IRequestHandler<UpdateCompanyCommand2, Unit>
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        public UpdateCompanyHandler2(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateCompanyCommand2 request, CancellationToken cancellationToken)
        {
            var companyEntity = await _repository.Company.GetCompanyAsync(request.Id, request.TrackChanges);
            if (companyEntity is null)
                throw new CompanyNotFoundException(request.Id);
            _mapper.Map(request.Company, companyEntity);
            await _repository.SaveAsync();
            return Unit.Value;
        }

    }
}
