using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        //to add message
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        //get individual message
        Task<Message> GetMesaage(int id);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recepientUsername);
        Task<bool> SaveAllAsync();
    }
}