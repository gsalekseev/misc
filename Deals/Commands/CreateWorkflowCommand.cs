using MediatR;

namespace Service.Application.Deals.Commands
{
    public class CreateWorkflowCommand : IRequest
    {
        public CreateWorkflowCommand(string workflowCode, string userWinLogin)
        {
            WorkflowCode = workflowCode;
            UserWinLogin = userWinLogin;
            //... другие параметры
        }

        public string WorkflowCode { get; }

        public string UserLogin { get; }
		
		//... другие параметры
    }
}
