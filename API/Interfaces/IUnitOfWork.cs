using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository{get;}

        IMessageRepository MessageRepository {get;}

        ILikesRepository LikesRepository {get; }

        Task<bool> Complete();

        //if entity framwork has any chandes
        bool HasChanges();
    }
}