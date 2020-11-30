/*
	
Приложенный код находится в Application слое приложения, построенного по Onion Architecture. 
Другие слои опущены для простоты 
В этом слое расположены по большей части запросы (query и commands) CQRS (используем MediatR) и хендлеры к ним

Проект направлен на ведение неких банковских сделок. 
Сделки необходимо согласовывать в некоторой workflow-системе. 
В нашем контексте не может быть сделки, у которой не запущено согласование и не бывает согласования без сделки. 
Задача была решена с помощью Mediatr-пайплайна с транзакционным поведением. 


*/

/* Общее */
namespace Service.Application.Deals.Interfaces
{
	/* Интерфейс для доступа к транзакциям, реализован в Infrastructure слое */
    public interface ITransactionAccessor
    {
        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        void RollbackTransaction();
    }
	
	//интерйефс транзакционного запроса, чтобы Mediatr применял транзакционное поведение только к нужным коммандам
	public interface ITransactionalRequest
    {
	
    }
}

namespace Service.Application.Common.CommandBehaviour
{
	/* Транзакционное повеение для хендлера */
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ITransactionalRequest
    {
        private readonly ITransactionAccessor _transactionAccessor;

        public TransactionBehaviour(ITransactionAccessor transactionAccessor)
        {
            _transactionAccessor = transactionAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            TResponse response = default;

            try
            {
                await _transactionAccessor.BeginTransactionAsync();

                response = await next();

                await _transactionAccessor.CommitTransactionAsync();
                return response;
            }
            catch (Exception e)
            {
                _transactionAccessor.RollbackTransaction();
                throw e;
            }
        }
    }
}


/* 
	Хендлеры для создания сделок. DTO и команды MediatR опущены для простоты. 

*/
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
	
	public class CreateWorkflowCommandHandler : IRequestHandler<CreateWorkflowCommand>
    {
        private readonly IWorkflowSystem workflowSystem;

        public CreateWorkflowCommandHandler(IWorkflowSystem workflowSystem)
        {
            this.workflowSystem = workflowSystem;
        }

        public Task<Unit> Handle(CreateWorkflowCommand request, CancellationToken cancellationToken)
        {
            /* просто отдаем вызов в некую workflow систему */
            workflowSystem.CreateWorkflow(/*...*/);
            return Task.FromResult(default(Unit));
        }
    }
