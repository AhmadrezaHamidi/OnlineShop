using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Base.Application.Common;
using Base.Application.Common.Response;
using Base.Application.Contracts;
using Base.Domain.Entities;
using Base.Domain.Entities.TallarEntities;
using MediatR;


namespace Base.Application.Features.command.EmployeeCommands
{

    public class CreateTallarCommand : IRequest<OperationResult<string>>
    {
        public CreateTallarCommand(string name, string image, int capability, long price)
        {
            Name = name;
            Image = image;
            Capability = capability;
            Price = price;
        }

        public string Name { get; set; }
        public string Image { get; set; }
        public int Capability { get; set; }
        public long Price { get; set; }
    }

    public class CreateTallarCommandHandler : IRequestHandler<CreateTallarCommand, OperationResult<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateTallarCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<OperationResult<string>> Handle(CreateTallarCommand request, CancellationToken cancellationToken)
        {
            var isExsist = _unitOfWork.GetRepository<Tallar>().FirstOrDefault(x => x.Name == request.Name);

            var res = new OperationResult<string>();
            return Task.FromResult(res);
        }
    }


}
