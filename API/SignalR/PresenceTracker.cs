using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.SignalR
{
    public class PresenceTracker
    { 
        //this dictionary is going to be shared amogst everyone who connets to our server
        private static readonly Dictionary<string, List<string>> OnlineUsers = 
            new Dictionary<string, List<string>>();

            public Task<bool> UserConnected(string username, string connectionId)
            {
                bool isOnline = false;
                lock(OnlineUsers) 
                {
                    //if already have dictionary element with key of the currently loggedIn username
                    if(OnlineUsers.ContainsKey(username))
                    {
                        //acces to dictionary element with key-useranme, and add it to List
                        OnlineUsers[username].Add(connectionId);
                    }else
                    {
                        OnlineUsers.Add(username, new List<string> {connectionId});
                        isOnline = true;
                    }
                }

                return Task.FromResult(isOnline);
            }

            public Task<bool> UserDisconnected(string username, string connectionId)
            {
                bool isOffline = false;
                lock(OnlineUsers)
                {
                    if (!OnlineUsers.ContainsKey(username)) return Task.FromResult(isOffline);

                    OnlineUsers[username].Remove(connectionId);
                    if(OnlineUsers[username].Count == 0)
                    {
                        OnlineUsers.Remove(username);
                        isOffline = true;
                    }
                }
                return Task.FromResult(isOffline);
            }

            //get all users that are currently connected
            public Task<string[]> GetOnlineUsers()
            {
                string[] onlineUsers;
                lock(OnlineUsers)
                {
                    onlineUsers = OnlineUsers.OrderBy(k => k.Key).Select(k => k.Key).ToArray();
                }

                return Task.FromResult(onlineUsers);
            }

            //list of connection of particulat uer that is connected
            public Task<List<string>> GetConnectionsForUser(string username)
            {
                List<string> connectionIds;
                lock(OnlineUsers)
                {
                    //if we have a dictionary element with the key of username
                        //then this is gona return the list of connectionIds fot that particulat user
                    connectionIds = OnlineUsers.GetValueOrDefault(username);
                }

                return Task.FromResult(connectionIds);
            }
    }
}