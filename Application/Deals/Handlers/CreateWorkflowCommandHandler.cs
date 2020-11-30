
namespace Service.Application.Deals.Handlers
{
    public class CreateWorkflowCommandHandler : IRequestHandler<CreateProcessCommand>
    {
        private readonly IWorkflowHost workflowHost;

        public CreateWorkflowCommandHandler(IWorkflowHost workflowHost)
        {
            this.workflowHost = workflowHost;
        }

        public Task<Unit> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
        {
            /* просто отдаем вызов в некую workflow систему */
            workflowHost.CreateWorkflow(/*...*/);
            return Task.FromResult(default(Unit));
        }
    }
}
