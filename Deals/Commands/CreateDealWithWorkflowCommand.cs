using Service.Application.Deals.DTO;
using MediatR;
using Service.Application.Workflow.Interfaces;

namespace Service.Application.Deals.Commands
{
    public class CreateDealWithWorkflowCommand : IRequest, ITransactionalRequest
    {
        public CreateDealWitWorkflowCommand(DealDto dto, UserDto userDto)
        {
            DealDto = dto;
            UserDto = userDto;
        }

        public DealDto DealDto { get; }

        public UserDto UserDto { get; }
    }
}
