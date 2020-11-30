using System.Threading.Tasks;

namespace Service.Application.Deals.Interfaces
{
    public interface ITransactionAccessor
    {
        Task BeginTransactionAsync();

        Task CommitTransactionAsync();

        void RollbackTransaction();
    }
}
