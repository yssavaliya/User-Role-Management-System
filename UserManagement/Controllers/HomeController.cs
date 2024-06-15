using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Linq;
using UserManagement.Models;
using UserManagementSystem.Data;

namespace UserManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment environment;

        public HomeController(ApplicationDbContext context,IWebHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }

        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModelClass loginModel)
        {
            var userWithPassword = _context.LoginDetails.Include(l => l.UserModeClass)
                                                      .FirstOrDefault(x => x.UserModeClass.Username == loginModel.Username);

            if (userWithPassword != null && userWithPassword.Password == loginModel.Password)
            {
                // Set session variables for login
                HttpContext.Session.SetString("userID", userWithPassword.UserId.ToString());
                HttpContext.Session.SetString("role", userWithPassword.UserModeClass.Role);
                HttpContext.Session.SetString("username", userWithPassword.UserModeClass.Username);
                HttpContext.Session.SetString("isLogin", "Loggedin");

                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Login Failed";
                return View();
            }
        }

        public IActionResult Logout()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                HttpContext.Session.Remove("username");
                HttpContext.Session.Remove("role");
                HttpContext.Session.Remove("isLogin");
            }
            return RedirectToAction("Login");
        }

        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewBag.MySession = HttpContext.Session.GetString("username");
                ViewBag.Role = HttpContext.Session.GetString("role");
                ViewBag.isLogin = HttpContext.Session.GetString("isLogin");
                int userId = Int32.Parse(HttpContext.Session.GetString("userID"));
                var loginUserDetail = _context.UserDetails.FirstOrDefault(user => user.UserId == userId);

                if (HttpContext.Session.GetString("role") == "Admin")
                {
                    ViewData["isAdmin"] = true;
                }
                else if (HttpContext.Session.GetString("role") == "Employee")
                {
                    ViewData["isEmployee"] = true;
                }

                return View(loginUserDetail);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        public IActionResult UserList()
        {
            if (HttpContext.Session.GetString("username") != null)
            {
                ViewBag.isLogin = HttpContext.Session.GetString("isLogin");
                if (HttpContext.Session.GetString("role") == "Admin")
                {
                    ViewData["isAdmin"] = true;
                }
                else if (HttpContext.Session.GetString("role") == "Employee")
                {
                    ViewData["isEmployee"] = true;
                }

                var allUsers = _context.UserDetails.ToList();
                return View(allUsers);
            }
            return RedirectToAction("Login");
        }

        public IActionResult AddOrEdit(int? id)
        {
            ViewBag.Role = HttpContext.Session.GetString("role");
            ViewBag.isLogin = HttpContext.Session.GetString("isLogin");

            if (HttpContext.Session.GetString("role") == "Admin")
            {
                ViewData["isAdmin"] = true;
            }
            else if (HttpContext.Session.GetString("role") == "Employee")
            {
                ViewData["isEmployee"] = true;
            }

            if (id == null)
            {
                return View(new UserClass()); 
            }
            else
            {
                var user = _context.UserDetails.FirstOrDefault(u => u.UserId == id);
                if (user == null)
                {
                    return NotFound();
                }

                var userModel = new UserClass
                {
                    UserId = user.UserId,
                    Name = user.Name,
                    Email = user.Email,
                    Gender = user.Gender,
                    Age = user.Age,
                    City = user.City,
                    Username = user.Username,
                    Role = user.Role
                };

                return View(userModel);
            }
        }

            [HttpPost]
            public IActionResult AddOrEdit(UserClass user)
            {
                if (ModelState.IsValid)
                {
                    var existingUser = _context.UserDetails.FirstOrDefault(u => u.Username == user.Username && u.UserId != user.UserId);

                    if (existingUser != null)
                    {
                        ModelState.AddModelError("Username", "Username is already taken.");
                        return View(user);
                    }

                    if (user.UserId == 0)
                    {
                        string newFileName = "";

                        if (user.profileImage != null)
                        {
                            String uploadFolder = Path.Combine(environment.WebRootPath, "ProfilePicture");
                            newFileName = Guid.NewGuid().ToString() + "_" + user.profileImage.FileName;
                            String filePath = Path.Combine(uploadFolder, newFileName);

                            if (!Directory.Exists(uploadFolder))
                            {
                                Directory.CreateDirectory(uploadFolder);
                            }

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                user.profileImage.CopyTo(fileStream);
                            }

                           
                        }
                        UserModeClass newUser = new UserModeClass
                        {
                            Name = user.Name,
                            Email = user.Email,
                            Gender = user.Gender,
                            Age = user.Age,
                            City = user.City,
                            Role = user.Role,
                            Username = user.Username,
                            profileImage = newFileName 
                        };
                        _context.UserDetails.Add(newUser);
                        _context.SaveChanges();

                        int userId = newUser.UserId; 

                        InsertOrUpdateLogin(userId, user.Username);
                    }
                    else
                    {
                        var userToUpdate = _context.UserDetails.FirstOrDefault(u => u.UserId == user.UserId);
                        if (userToUpdate == null)
                        {
                            return NotFound();
                        }

                        string newFileName = userToUpdate.profileImage == null ? "no image" : "has image";

                        if (user.profileImage != null)
                        {
                            String uploadFolder = Path.Combine(environment.WebRootPath, "ProfilePicture");
                            newFileName = Guid.NewGuid().ToString() + "_" + user.profileImage.FileName;
                            String filePath = Path.Combine(uploadFolder, newFileName);

                            if (!Directory.Exists(uploadFolder))
                            {
                                Directory.CreateDirectory(uploadFolder);
                            }

                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                user.profileImage.CopyTo(fileStream);
                            }


                        }

                        userToUpdate.Name = user.Name;
                        userToUpdate.Email = user.Email;
                        userToUpdate.Gender = user.Gender;
                        userToUpdate.Age = user.Age;
                        userToUpdate.City = user.City;
                        userToUpdate.Role = user.Role;
                        userToUpdate.Username = user.Username;
                        userToUpdate.profileImage = newFileName; 

                        _context.UserDetails.Update(userToUpdate);
                        _context.SaveChanges();

                        InsertOrUpdateLogin(user.UserId, user.Username);
                    }

                    return RedirectToAction("UserList");
                }
                return View(user);
            }


        public void InsertOrUpdateLogin(int id, string username, string password = "123")
        {
            var login = _context.LoginDetails.FirstOrDefault(l => l.UserId == id);

            if (login == null)
            {
                login = new LoginModelClass
                {
                    UserId = id,
                    Username = username,
                    Password = password
                };
                _context.LoginDetails.Add(login);
            }
            else
            {
                login.Username = username;
                login.Password = password;
                _context.LoginDetails.Update(login);
            }

            _context.SaveChanges();
        }

        public IActionResult Delete(int id)
        {
            var user = _context.UserDetails.Find(id);
            var login = _context.LoginDetails.Find(id);
            if (user == null)
            {
                return RedirectToAction("UserList");
            }
            _context.UserDetails.Remove(user);
            _context.LoginDetails.Remove(login);

            _context.SaveChanges();
            return RedirectToAction("UserList");
        }

public IActionResult UpdateProfile()
{



    // Get user ID from session
    var userIdSession = HttpContext.Session.GetString("userID");
    if (string.IsNullOrEmpty(userIdSession))
    {
        return RedirectToAction("Login");
    }

    if (!int.TryParse(userIdSession, out int userId))
    {
        return RedirectToAction("Login");
    }

    // Fetch user details from the database
    var user = _context.UserDetails.FirstOrDefault(u => u.UserId == userId);

    if (user == null)
    {
        return NotFound();
    }

    // Populate UserClass with user data
    var userModel = new UserClass
    {
        UserId = user.UserId,
        Name = user.Name,
        Email = user.Email,
        Gender = user.Gender,
        Age = user.Age,
        City = user.City,
        Username = user.Username,
        Role = user.Role // Assuming Role is fetched from the database for the current user
    };

    return View(userModel);
}

        [HttpPost]
        public IActionResult UpdateProfile(UserClass userclass)
        {
            ViewBag.MySession = HttpContext.Session.GetString("username");
            ViewBag.Role = HttpContext.Session.GetString("role");
            ViewBag.isLogin = HttpContext.Session.GetString("isLogin");
            int userId1 = Int32.Parse(HttpContext.Session.GetString("userID"));
            var loginUserDetail = _context.UserDetails.FirstOrDefault(user => user.UserId == userId1);

            if (HttpContext.Session.GetString("role") == "Admin")
            {
                ViewData["isAdmin"] = true;
            }
            else if (HttpContext.Session.GetString("role") == "Employee")
            {
                ViewData["isEmployee"] = true;
            }

            var userIdSession = HttpContext.Session.GetString("userID");
            if (string.IsNullOrEmpty(userIdSession))
            {
                return RedirectToAction("Login");
            }

            if (!int.TryParse(userIdSession, out int userId))
            {
                return RedirectToAction("Login");
            }

            var user = _context.UserDetails.Find(userId);

            if (user == null)
            {
                return NotFound();
            }

            string newFileName = user.profileImage;

            if (userclass.profileImage != null)
            {
                String uploadFolder = Path.Combine(environment.WebRootPath, "ProfilePicture");
                newFileName = Guid.NewGuid().ToString() + "_" + userclass.profileImage.FileName;
                String filePath = Path.Combine(uploadFolder, newFileName);
        
                // Ensure the directory exists
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Save the new profile image
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    userclass.profileImage.CopyTo(fileStream);
                }

                // Delete old profile image if it exists and is not the default image
                if (!string.IsNullOrEmpty(user.profileImage))
                {
                    string oldPath = Path.Combine(environment.WebRootPath, "ProfilePicture", user.profileImage);
                    if (System.IO.File.Exists(oldPath))
                    {
                        System.IO.File.Delete(oldPath);
                    }
                }
            }

                    // Update user details
                    user.Name = userclass.Name;
                    user.Email = userclass.Email;
                    user.Gender = userclass.Gender;
                    user.Age = userclass.Age;
                    user.City = userclass.City;
                    user.Role = userclass.Role;
                    user.Username = userclass.Username;
                    user.profileImage = newFileName;

                    _context.UserDetails.Update(user);
                    _context.SaveChanges();

            return RedirectToAction("Index");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
