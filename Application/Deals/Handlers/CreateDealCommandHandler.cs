
namespace Service.Application.Deals.Handlers
{
       public class CreateDealCommandHandler : IRequestHandler<CreateDealCommand, CreationResult>
    {
        public CreateDealCommandHandler(IDealRepository dealRepository)
        {
            DealRepository = dealRepository;
        }

        private IDealRepository DealRepository { get; }

        public async Task<CreationResult> Handle(CreateDealCommand request, CancellationToken cancellationToken)
        {
           
            Deal deal = CreateDeal(/*...*/);

            await this.DealRepository.InsertDealAsync(deal);
            return new CreationResult(deal.IdDeal);
        }

        private Deal CreateDeal(/*...*/)
        {
            return new Deal(/*...*/);
        }
    }
}
