using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using hw_3_25.Models;

namespace hw_3_25.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(int pagetodisplay)
        {
            IndexViewModel vm = new IndexViewModel(); //to be used in all scopes here, returned in the View at the end
            BlogDb db = new BlogDb();
            decimal totalBlogs = db.GetPostsCount();  //this gives me total number of posts
            vm.TotalPages = (int)Math.Ceiling(totalBlogs / 5); //this gives the amount of valid pages
            if (pagetodisplay <= 1 || pagetodisplay > vm.TotalPages)  //this sets if the query string was for an invalid page or no query, it'll go do default
            {
                vm.Posts = db.GetBlogs().Take(5).ToList();  //the currentpage is defaulted to one whether user selected pg one, went to home, or invalid query
            }
            else //if valid query string came in, it should skip all previous page contents and take the current page's
            {
                vm.CurrentPage = pagetodisplay;
                pagetodisplay--;
                vm.Posts = db.GetBlogs().Skip(pagetodisplay * 5).Take(5).ToList();
            }
            return View(vm);
        }
        public IActionResult DisplayPost(int blogId)
        {
            BlogDb db = new BlogDb();
            BlogPost blogpost = db.GetPostToDisplay(blogId);
            if (blogpost.BlogText == null)
            {
                return Redirect("/home/index");  //if either '0', or an id that doesn't exist was sent it, redirects
            }
            DisplayPostViewModel vm = new DisplayPostViewModel();
            vm.BlogPost = blogpost;
            vm.CommenterName = Request.Cookies["commenterName"];  
            return View(vm);
        }
        public IActionResult MostRecent()
        {
            BlogDb db = new BlogDb();
            BlogPost post = db.GetBlogs().FirstOrDefault();
            if (post == null)
            {
                return Redirect("/home/index");
            }
            return Redirect($"/home/displaypost?blogid={post.Id}");
        }
        public IActionResult AddPost()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddPost(BlogPost post)
        {
            BlogDb db = new BlogDb();
            int id = db.AddBlogPost(post);
            return Redirect($"/home/displaypost?blogId={id}");
        }

        [HttpPost]
        public IActionResult AddComment(Comment comment, int blogId)
        {
            BlogDb db = new BlogDb();
            db.AddComment(comment, blogId);
            Response.Cookies.Append("commenterName", $"{comment.CommentAuthor}");
            return Redirect($"/home/displaypost?blogid={blogId}");
        }
    }
}