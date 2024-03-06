using Application.Commands;
using Application.Notifications;
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
    internal sealed class DeleteCompanyHandler : IRequestHandler<DeleteCompanyCommand/*, Unit*/>
    {
        private readonly IRepositoryManager _repository;
        public DeleteCompanyHandler(IRepositoryManager repository) => _repository = repository;
        public async Task/*<Unit>*/ Handle(DeleteCompanyCommand request, CancellationToken cancellationToken)
        {
            var company = await _repository.Company.GetCompanyAsync(request.Id, request.TrackChanges) ?? throw new CompanyNotFoundException(request.Id);
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
            //return Unit.Value;
        }
    }

    internal sealed class DeleteCompanyHandler2 : INotificationHandler<CompanyDeletedNotification>
    {
        private readonly IRepositoryManager _repository;
        public DeleteCompanyHandler2(IRepositoryManager repository) => _repository = repository;
        public async Task Handle(CompanyDeletedNotification notification, CancellationToken cancellationToken)
        {
            var company = await _repository.Company.GetCompanyAsync(notification.Id,
            notification.TrackChanges);
            if (company is null)
                throw new CompanyNotFoundException(notification.Id);
            _repository.Company.DeleteCompany(company);
            await _repository.SaveAsync();
        }
    }
}
