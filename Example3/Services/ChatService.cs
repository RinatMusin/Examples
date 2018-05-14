using Crm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Crm.Services
{
    public class ChatService : BaseService
    {
        public List<ChatMessage> GetMessagesByUserID(int userID)
        {
            return db.ChatMessages.Where(n => n.UserID == userID).ToList();
        }
        public List<ChatMessage> GetMessagesByObject(string objectType, int objectID, int userID, int limit, int offset)
        {
            //только публичные
            //if (userID == 0)
            return db.ChatMessages.Include("User").Include("Party").Where(n => n.ParentID == objectID && n.ParentType.ToLower() == objectType.ToLower()).OrderByDescending(c => c.ID).ToList().Skip(offset).Take(limit).ToList();
            //    return db.Notes.Include("User").Where(n => n.ObjectID == objectID && n.ObjectType.ToLower() == objectType.ToLower() && (n.IsPublic == 1 || n.IsPublic==0 && n.UserID==userID)).OrderBy(c => c.ID).Skip(offset).Take(limit).ToList();
            /*else
            {
                //публичные и пользователя
                return db.ChatMessages.Include("User").Where(n => n.ParentID == objectID && n.ObjectType.ToLower() == objectType.ToLower() && (n.IsPublic == 1 || n.IsPublic == 0 && n.UserID == userID)).OrderBy(c => c.ID).Skip(offset).Take(limit).ToList();
            }*/
        }
        public void AddChatMessage(ChatMessage message)
        {
            message.CreateDate = DateTime.Now;
            //      note.UpdateDate = DateTime.Now;
            db.ChatMessages.Add(message);
            db.SaveChanges();
        }

        internal ChatMessage GetMessageByID(int id)
        {
            return db.ChatMessages.Find(id);
        }

        internal List<ChatView> GetChatViewList(int projectID, int partyID, int limit, int offset)
        {
            return db.ChatViews.Where(c => c.ProjectID == projectID && c.PartyID == partyID).Skip(offset).Take(limit).ToList();
        }


        internal List<ChatView> GetChatViewListByChatID(int projectID, int chatID)
        {
            return db.ChatViews.Where(c => c.ProjectID == projectID && c.ChatID == chatID).ToList();
        }

        internal void AddChat(Chat chat)
        {
            chat.CreateDate = DateTime.Now;
            chat.LastUpdateDate = DateTime.Now;
            db.Chats.Add(chat);
            db.SaveChanges();
        }

        internal void AddChatParty(ChatParty chatParty)
        {
            db.ChatParties.Add(chatParty);
            db.SaveChanges();
        }

        /*   public int GetPrivateChatMessageCount(int projectID,int partyID)
          {
             SELECT * FROM
  chat_messages WHERE party_id IN(

  SELECT DISTINCT( party_id)
  FROM chat_parties 
  WHERE chat_id IN(
  SELECT  chat_id
    FROM chat_parties where party_id=5)) AND parent_type='chat' AND project_id=14
          }
  */
    }
}