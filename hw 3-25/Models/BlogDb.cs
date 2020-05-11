using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace hw_3_25.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string BlogAuthor { get; set; }
        public DateTime BlogDate { get; set; }
        public string BlogText { get; set; }
        public List<Comment> Comments = new List<Comment>();
    }
    public class Comment
    {
        public string CommentAuthor { get; set; }
        public DateTime CommentDate { get; set; }
        public string CommentText { get; set; }
    }
    public class BlogDb
    {
        private string _conStr = @"Data Source=.\sqlexpress;Initial Catalog=Blog;Integrated Security=True;";
        public List<BlogPost> GetBlogs()
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT *, LEFT(BlogText, 180) AS BegText FROM BlogPosts";
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                List<BlogPost> posts = new List<BlogPost>();
                while (reader.Read())
                {
                    posts.Add(new BlogPost
                    {
                        Id = (int)reader["Id"],
                        Title = (string)reader["Title"],
                        BlogAuthor = (string)reader["BlogAuthor"],
                        BlogDate = (DateTime)reader["BlogDate"],
                        BlogText = (string)reader["BegText"]
                    });
                }
                return posts.OrderByDescending(p => p.BlogDate).ToList();
            }
        }
        public int AddBlogPost(BlogPost bp)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO BlogPosts(Title, BlogAuthor, BlogDate, BlogText)
                                    VALUES(@title, 'Mrs. Sew & Sew', GETDATE(), @text)
                                    SELECT SCOPE_IDENTITY()";
                cmd.Parameters.AddWithValue("@title", bp.Title);
                cmd.Parameters.AddWithValue("@text", bp.BlogText);
                conn.Open();
                return (int)(decimal)cmd.ExecuteScalar();
            }
        }
        public BlogPost GetPostToDisplay(int blogId)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"SELECT * FROM BlogPosts bp
                                    LEFT JOIN Comments c
                                    ON bp.Id = c.BlogId
                                    WHERE bp.id=@id";
                cmd.Parameters.AddWithValue("@id", blogId);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                BlogPost post = new BlogPost();
                while (reader.Read())
                {
                    if (post.Id==0)
                    {
                        post = new BlogPost
                        {
                            Id = (int)reader["Id"],
                            Title = (string)reader["Title"],
                            BlogAuthor = (string)reader["BlogAuthor"],
                            BlogDate = (DateTime)reader["BlogDate"],
                            BlogText = (string)reader["BlogText"]
                        };
                    }
                    if (reader["CommentText"] != DBNull.Value)
                    {
                        post.Comments.Add(new Comment
                        {
                            CommentAuthor = (string)reader["CommentAuthor"],
                            CommentDate = (DateTime)reader["CommentDate"],
                            CommentText = (string)reader["CommentText"]
                        });
                    }
                }
                return post;
            }
        }
        public void AddComment(Comment comment, int blogId)
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = @"INSERT INTO Comments(BlogId, CommentAuthor, CommentDate, CommentText)
                                    VALUES(@blogId, @author, GETDATE(), @text)";
                cmd.Parameters.AddWithValue("@blogId", blogId);
                cmd.Parameters.AddWithValue("@author", comment.CommentAuthor);
                cmd.Parameters.AddWithValue("@text", comment.CommentText);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
        public decimal GetPostsCount()
        {
            using (SqlConnection conn = new SqlConnection(_conStr))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(*) FROM BlogPosts";
                conn.Open();
                return (int)cmd.ExecuteScalar();
            }
        }

    }
}
