using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeFix.Domain.Context;
using WeFix.Domain.Entities;

namespace WeFix.WebUI.Hubs
{
    public class MessageDetail
    {
        public string FromUserID { get; set; }
        public string FromUserName { get; set; }
        public string ToUserID { get; set; }
        public string ToUserName { get; set; }
        public string Message { get; set; }
    }

    public class UserDetail
    {
        public string ConnectionId { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
    }
    [HubName("chatHub")]
    public class ChatHub:Hub
    {
        #region---Data Members---
        static List<UserDetail> ConnectedUsers = new List<UserDetail>();
        static List<MessageDetail> CurrentMessage = new List<MessageDetail>();
        private EFDbContext db = new EFDbContext();
        #endregion

        #region---Methods---
        public void Connect(string UserName, string UserID)
        {
            var id = Context.ConnectionId;

            if (ConnectedUsers.Count(x => x.ConnectionId == id) == 0)
            {               
                ConnectedUsers.Add(new UserDetail { ConnectionId = id, UserName = UserName, UserID = UserID });
            }
            UserDetail CurrentUser = ConnectedUsers.Where(u => u.ConnectionId == id).FirstOrDefault();
            var user = db.Users.Find(UserID);
            user.ConnectionId = id;
            user.IsOnline = true;
            user.LastOnline = DateTime.Now;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            // send to caller           
            Clients.Caller.onConnected(CurrentUser.UserID.ToString(), CurrentUser.UserName, ConnectedUsers, CurrentMessage, CurrentUser.UserID);
            // send to all except caller client           
            Clients.AllExcept(CurrentUser.ConnectionId).onNewUserConnected(CurrentUser.UserID.ToString(), CurrentUser.UserName, CurrentUser.UserID);
        }

        public void Send(string userName, string message)
        {
            // store last 100 messages in cache
            //MessageDetail md = new MessageDetail()
            //{
            //    FromUserID = userId,
            //    FromUserName = userName,
            //    Message = message,
            //    ToUserID = null,
            //    ToUserName = null
            //};
            //AddMessageinCache(md);

            // Broad cast message
            Clients.All.addNewMessageToPage(userName, message);
        }

        public void SendPrivateMessage(string toUserId, string message)
        {
            try
            {
                string fromconnectionid = Context.ConnectionId;
                string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault()).ToString();
                string _fromUserId = "";
                _fromUserId = strfromUserId;
                string _toUserId = "";
                _toUserId = toUserId;
                List<UserDetail> FromUsers = ConnectedUsers.Where(u => u.UserID == _fromUserId).ToList();
                List<UserDetail> ToUsers = ConnectedUsers.Where(x => x.UserID == _toUserId).ToList();
                ChatMessage chat = new ChatMessage();

                if (FromUsers.Count != 0 && ToUsers.Count() != 0)
                {
                    foreach (var ToUser in ToUsers)
                    {
                        chat.Body = message;
                        chat.Time_Stamp = DateTime.Now;
                        chat.toUserId = toUserId;
                        // send to                                                                                            //Chat Title
                        Clients.Client(ToUser.ConnectionId).sendPrivateMessage(_fromUserId.ToString(), FromUsers[0].UserName, FromUsers[0].UserName, message);
                    }


                    foreach (var FromUser in FromUsers)
                    {
                        // send to caller user 
                        //Chat Title
                        chat.fromUserId = FromUser.UserID;
                        Clients.Client(FromUser.ConnectionId).sendPrivateMessage(_toUserId.ToString(), FromUsers[0].UserName, ToUsers[0].UserName, message);
                    }
                    // send to caller user
                    //Clients.Caller.sendPrivateMessage(_toUserId.ToString(), FromUsers[0].UserName, message);
                    //ChatDB.Instance.SaveChatHistory(_fromUserId, _toUserId, message);
                    MessageDetail _MessageDeail = new MessageDetail { FromUserID = _fromUserId, FromUserName = FromUsers[0].UserName, ToUserID = _toUserId, ToUserName = ToUsers[0].UserName, Message = message };
                    db.ChatMessages.Add(chat);
                    db.SaveChanges();
                    AddMessageinCache(_MessageDeail);
                }
            }
            catch { }
        }

        public void RequestLastMessage(string FromUserID, string ToUserID)
        {
            List<MessageDetail> CurrentChatMessages = (from u in CurrentMessage where ((u.FromUserID == FromUserID && u.ToUserID == ToUserID) || (u.FromUserID == ToUserID && u.ToUserID == FromUserID)) select u).ToList();
            //send to caller user
            Clients.Caller.GetLastMessages(ToUserID, CurrentChatMessages);
        }

        public void SendUserTypingRequest(string toUserId)
        {
            string strfromUserId = (ConnectedUsers.Where(u => u.ConnectionId == Context.ConnectionId).Select(u => u.UserID).FirstOrDefault()).ToString();

            string _toUserId = "";
            _toUserId = toUserId;
            List<UserDetail> ToUsers = ConnectedUsers.Where(x => x.UserID == _toUserId).ToList();

            foreach (var ToUser in ToUsers)
            {
                // send to                                                                                            
                Clients.Client(ToUser.ConnectionId).ReceiveTypingRequest(strfromUserId);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var item = ConnectedUsers.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            if (item != null)
            {
                ConnectedUsers.Remove(item);
                if (ConnectedUsers.Where(u => u.UserID == item.UserID).Count() == 0)
                {
                    var id = item.UserID.ToString();
                    Clients.All.onUserDisconnected(id, item.UserName);
                    try
                    {
                        var user = db.Users.Find(item.UserID);
                        user.ConnectionId = "";
                        user.IsOnline = false;
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }                   
                }
            }
            return base.OnDisconnected(stopCalled);
        }
        #endregion

        #region---private Messages---
        private void AddMessageinCache(MessageDetail _MessageDetail)
        {
            CurrentMessage.Add(_MessageDetail);
            if (CurrentMessage.Count > 100)
                CurrentMessage.RemoveAt(0);
        }
        #endregion
    }
}
