using Service.Application.Deals.DTO;
using MediatR;

namespace Service.Application.Deals.Commands
{
    public class CreateDealCommand : IRequest<DealCreationResult>
    {
        public CreateDealCommand(DealDto dealDto, UserDto dtoUser)
        {
            DealDto = dealDto;
            UserDto = dtoUser;
        }

        public DealDto DealDto { get; }

        public UserDto UserDto { get; }
    }
}
