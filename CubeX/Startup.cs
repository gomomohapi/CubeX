using CubeX.Models;
using CubeX.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CubeX.Startup))]
namespace CubeX
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            CreateRolesAndUsers();
        }

        // In this method we will create default User roles and Admin user for login  
        private void CreateRolesAndUsers()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            //Creating Admin role
            //Creating default admin user
            if (!roleManager.RoleExists("Admin"))
            {
                // first we create Admin role    
                var role = new IdentityRole();
                role.Name = "Admin";
                roleManager.Create(role);

                //Here we create a Admin super user who will maintain the website           
                var user = new ApplicationUser();
                user.FirstName = "Admin";
                user.Surname = "User";
                user.UserName = "admin@cubex.com";
                user.Email = "admin@cubex.com";

                string userPWD = "@Cube2020.";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Admin    
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Admin");

                }

            }

            // Creating Staff role     
            if (!roleManager.RoleExists("Staff"))
            {
                var role = new IdentityRole();
                role.Name = "Staff";
                roleManager.Create(role);

                //Here we create a Staff user who will maintain the website           
                var user = new ApplicationUser();
                user.FirstName = "Tholithemba";
                user.Surname = "Mkhwanazi";
                user.UserName = "tory@cubex.com";
                user.Email = "tory@cubex.com";

                string userPWD = "@Cube2020.";

                var chkUser = UserManager.Create(user, userPWD);

                //Add default User to Role Staff    
                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, "Staff");

                }
            }

            // Creating Customer role     
            if (!roleManager.RoleExists(SD.EndUserRole))
            {
                var role = new IdentityRole();
                role.Name = "Customer";
                roleManager.Create(role);
            }
        }
    }
}
