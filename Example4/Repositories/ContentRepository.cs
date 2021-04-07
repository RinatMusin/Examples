using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AmplaCore.Models;
using Dapper;
namespace AmplaCore.Repositories
{
    public class ContentRepository
    {
        private readonly PostgresConnection _connection;

        public ContentRepository(PostgresConnection connection)
        {
            _connection = connection;
        }
        public int GetContentListTotalCount(int profileID)
        {
            return _connection.DB.Query<int>("SELECT COUNT(id) FROM contents WHERE profile_id=@id", new { id = profileID }).FirstOrDefault<int>();
        }
        public List<Content> GetContentList(int profileID, int page, int limit)
        {
            if (page < 1) page = 1;
            if (limit < 1) limit = 20;

            List<Content> res = _connection.DB.Query<Content>(
                string.Format(@"SELECT c.*,p.name AS profile_name,p.image AS profile_image 
                    FROM contents c JOIN profiles p ON p.id = c.profile_id 
                    WHERE c.profile_id=@id ORDER BY c.publish_date DESC OFFSET {0} LIMIT({1})", (page - 1) * limit, limit),
                new { id = profileID }).ToList();

            return res;
        }
        /// <summary>
        /// Получить список последних записей.
        /// </summary>
        /// <returns></returns>
        public List<Content> GetLastContentList()
        {
            List<Content> res = _connection.DB.Query<Content>("SELECT c.*,p.name AS profile_name,p.image AS profile_image FROM contents c JOIN profiles p ON p.id = c.profile_id ORDER BY publish_date DESC LIMIT(10)").ToList();
            return res;
        }

        public int AddContent(Content content)
        {


            var sqlQuery = "INSERT INTO contents (create_date,update_date, publish_date,title,text,profile_id,image) VALUES(@createdate,@updatedate,@publishdate,@title,@text,@profileid,@image) RETURNING ID;";
            int resID = _connection.DB.Query<int>(sqlQuery, content).FirstOrDefault();
            content.ID = resID;

            return resID;
        }
        public void UpdateContentImage(Content content)
        {


            var sqlQuery = "UPDATE contents SET image=@image WHERE id=@id";
            _connection.DB.Query(sqlQuery, content);



        }
        public Content GetContent(int id)
        {
            return _connection.DB.Query<Content>("SELECT c.*,p.name AS profile_name,p.image AS profile_image FROM contents c JOIN profiles p ON p.id = c.profile_id WHERE c.id=" + id).FirstOrDefault();

        }


        #region Комментарии
        /// <summary>
        /// Получить список комментариев
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectID"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public List<Comment> GetCommentList(string objectType, int objectID, int lastID)
        {
            //TODO листование GetCommentList(string objectType, int objectID, int page, int limit)
            string whereLastID = "";
            if (lastID > 0)
            {
                whereLastID = string.Format(" AND c.id<{0} ", lastID);
            }
            List<Comment> res = _connection.DB.Query<Comment>(
                @"SELECT c.*,p.name AS profile_name,p.image AS profile_image FROM comments c 
                    JOIN profiles p ON p.id = c.profile_id 
                    WHERE LOWER(c.object_type)=@ObjectType AND object_id=@ObjectID " + whereLastID + "ORDER BY c.id DESC ",
                new
                {
                    ObjectID = objectID,
                    ObjectType = new DbString
                    {
                        Value = objectType.ToLower().Trim()
                    }
                }).ToList();
            return res;
        }
        public List<Comment> GetContentCommentList(int objectID, int lastID)
        {
            //GetContentCommentList(int objectID, int page, int limit)
            return GetCommentList("content", objectID, lastID);
        }
        //SELECT * FROM public.comments WHERE object_id IN(2,6,56) AND index=1;
        /// <summary>
        /// Сохранить комментарий.
        /// </summary>
        /// <param name="comment"></param>
        /// <returns></returns>
        public int AddComment(Comment comment)
        {
            if (comment != null && !string.IsNullOrEmpty(comment.ObjectType))
            {
                comment.ObjectType = comment.ObjectType.ToLower().Trim();
                // index  +1  от максимума
                var sqlQuery = "INSERT INTO comments (create_date,text,profile_id,object_type,object_id) VALUES(@CreateDate,@Text,@ProfileID,@ObjectType,@ObjectID) RETURNING ID;";
                int resID = _connection.DB.Query<int>(sqlQuery, comment).FirstOrDefault();
                comment.ID = resID;
                return resID;
            }
            return 0;
        }

        public List<Comment> GetLastContentCommentList(List<int> lists, int rowCount = 6)
        {
            if (lists != null && lists.Count > 0)
            {
                List<Comment> res = _connection.DB.Query<Comment>(
                    string.Format(@"SELECT * FROM(SELECT c.*,p.name AS profile_name,p.image AS profile_image ,
                        ROW_NUMBER() OVER(PARTITION BY c.object_id ORDER BY c.id desc ) AS row_number
                        FROM comments c JOIN profiles p ON p.id = c.profile_id
                        WHERE LOWER(c.object_type) = @ObjectType AND object_id IN({0})  ORDER BY c.create_date DESC)AS data
                        WHERE row_number < {1} ", string.Join(',', lists), rowCount),
                    new
                    {

                        ObjectType = new DbString { Value = "content" }
                    }).ToList();
                return res;
            }
            return null;
        }


        #endregion


    }
}
