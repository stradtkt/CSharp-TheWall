using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Wall.Models;

namespace Wall.Controllers
{
    public class HomeController : Controller
    {
  private WallContext _wContext;

        public HomeController(WallContext context)
        {
            _wContext = context;    
        }
        private User ActiveUser 
        {
            get 
            {
                return _wContext.users.Where(u => u.user_id == HttpContext.Session.GetInt32("user_id")).FirstOrDefault();
            }
        }
        [HttpGet("")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpGet("login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("registeruser")]
        public IActionResult RegisterUser(RegisterUser newuser)
        {
            User CheckEmail = _wContext.users
                .Where(u => u.email == newuser.email)
                .SingleOrDefault();

            if(CheckEmail != null)
            {
                ViewBag.errors = "That email already exists";
                return RedirectToAction("Register");
            }
            if(ModelState.IsValid)
            {
                PasswordHasher<RegisterUser> Hasher = new PasswordHasher<RegisterUser>();
                User newUser = new User
                {
                    user_id = newuser.user_id,
                    first_name = newuser.first_name,
                    last_name = newuser.last_name,
                    email = newuser.email,
                    password = Hasher.HashPassword(newuser, newuser.password)
                  };
                _wContext.Add(newUser);
                _wContext.SaveChanges();
                ViewBag.success = "Successfully registered";
                return RedirectToAction("Login");
            }
            else
            {
                return View("Register");
            }
        }

        [HttpPost("loginuser")]
        public IActionResult LoginUser(LoginUser loginUser) 
        {
            User CheckEmail = _wContext.users
                .SingleOrDefault(u => u.email == loginUser.email);
            if(CheckEmail != null)
            {
                var Hasher = new PasswordHasher<User>();
                if(0 != Hasher.VerifyHashedPassword(CheckEmail, CheckEmail.password, loginUser.password))
                {
                    HttpContext.Session.SetInt32("user_id", CheckEmail.user_id);
                    HttpContext.Session.SetString("first_name", CheckEmail.first_name);
                    return RedirectToAction("Dashboard");
                }
                else
                {
                    ViewBag.errors = "Incorrect Password";
                    return View("Register");
                }
            }
            else
            {
                ViewBag.errors = "Email not registered";
                return View("Register");
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        [HttpGet("Dashboard")]
        public IActionResult Dashboard()
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                List<Message> messages = _wContext.messages
                .Include(u => u.Users)
                .Include(c => c.Comments)
                .OrderByDescending(d => d.created_at)
                .ToList();
                List<Comment> comments = _wContext.comments
                .Include(u => u.Users)
                .OrderByDescending(d => d.created_at)
                .ToList();
                ViewBag.user = ActiveUser;
                ViewBag.messages = messages;
                ViewBag.comments = comments;
                return View();
            }
        }

        [HttpPost("process_message")]
        public IActionResult ProcessPost(Message mess)
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
            if(ModelState.IsValid)
            {
                Message newMessage = new Message
                {
                    user_id = ActiveUser.user_id,
                    message = mess.message
                };
                _wContext.messages.Add(newMessage);
                _wContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
                List<Message> messages = _wContext.messages
                .Include(u => u.Users)
                .OrderByDescending(d => d.created_at)
                .ToList();
                List<Comment> comments = _wContext.comments
                .Include(u => u.Users)
                .OrderByDescending(d => d.created_at)
                .ToList();
                ViewBag.messages = messages;
                ViewBag.comments = comments;
                return View();
            }
        }

        [HttpPost("delete/{message_id}")]
        public IActionResult DeleteMessage(int messageid)
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                Message message = _wContext.messages.Where(m => m.message_id == messageid).SingleOrDefault();
                _wContext.messages.Remove(message);
                _wContext.SaveChanges();
                return RedirectToAction("Dashboard");
            }
            
        }

        [HttpGet("comment/{id}")]
        public IActionResult Comment(int id)
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                Message message = _wContext.messages.Include(u => u.Users).Include(c => c.Comments).Where(m => m.message_id == id).SingleOrDefault();
                List<Comment> comments = _wContext.comments.Include(u => u.Users).Include(m => m.Messages).ThenInclude(c => c.Comments).ToList();
                ViewBag.message = message;
                ViewBag.comments = comments;
                ViewBag.user = ActiveUser;
                return View();
            }
        }

        [HttpPost("ProcessComment")]
        public IActionResult ProcessComment(string comm, int message_id)
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Login");
            }
            else
            {
                Comment com = new Comment
                {
                    comment = comm,
                    message_id = message_id,
                    user_id = ActiveUser.user_id
                };
                _wContext.Add(com);
                _wContext.SaveChanges();
                return Redirect("/comment/" + message_id);
            }
        }
        [HttpPost("delete_comment/{comment_id}")]
        public IActionResult DeleteComment(int commentid)
        {
            Comment comment = _wContext.comments.Where(c => c.comment_id == commentid).SingleOrDefault();
            _wContext.comments.Remove(comment);
            _wContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
