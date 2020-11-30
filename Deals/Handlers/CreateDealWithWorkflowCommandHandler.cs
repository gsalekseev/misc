
namespace Service.Application.Deals.Handlers
{
    public class CreateDealWithWorkflowCommandHandler : IRequestHandler<CreateDealWithWorkflowCommand>
    {
        private const string WORKFLOW_CODE = "WRKFL_DEAL";

        public CreateDealWithProcessCommandHandler(
            IRequestHandler<CreateDealCommand, DealCreationResult> dealCommandHandler,
            IRequestHandler<CreateWorkflowCommand> processCommandHandler)
        {
            CreateDealCommandHandler = dealCommandHandler;
            CreateWorkflowCommandHandler = processCommandHandler;
        }

        public IRequestHandler<CreateDealCommand, DealCreationResult> CreateDealCommandHandler { get; }

        public IRequestHandler<CreateWorkflowCommand> CreateWorkflowCommandHandler { get; }

        public async Task<Unit> Handle(CreateDealWithProcessCommand request, CancellationToken cancellationToken)
        {
            var dealCreationResult = await CreateDealCommandHandler.Handle(
                new CreateDealCommand(request.DealDto, request.UserDto),
                cancellationToken);

            var workflowRequest = new CreateWorkflowCommand(dealCreationResult.IdDeal, WORKFLOW_CODE /*...*/);

            await CreateWorkflowCommandHandler.Handle(workflowRequest, cancellationToken);
            return await Task.FromResult(default(Unit));
        }
    }
}
