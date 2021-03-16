using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        //return userLike; get specific user like 
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        //return appUser; 
        Task<AppUser> GetUserWithLikes(int userId);
        //return IEnumerable
        Task<PagedList<LikeDto>> GetUserLikes (LikesParams likesParams);
    }
}