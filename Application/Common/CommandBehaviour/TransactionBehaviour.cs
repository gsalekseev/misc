using Service.Application.Deals.Interfaces;
using MediatR;
using Service.Application.Workflow.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Service.Application.Common.CommandBehaviour
{
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
