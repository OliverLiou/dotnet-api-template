namespace TemplateApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

// public class TemplateContext(DbContextOptions<TemplateContext> options) : DbContext(options)
public class TemplateContext(DbContextOptions<TemplateContext> options) : IdentityDbContext<User, Role, string>(options)
{
    //Tables
    public DbSet<Table1> Table1 { get; set; } = null!;
    public DbSet<User> User { get; set; } = null!;
    public DbSet<Role> Role { get; set; } = null!;

    //Logs Table
    public DbSet<Table1Log> Table1Log { get; set; } = null!;
    public DbSet<UserLog> UserLog { get; set; } = null!;
    // public DbSet<Log> Log { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        //Identity
        // builder.Entity<User>().ToTable("User");
        // builder.Entity<Role>().ToTable("Role");
        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaim");
        builder.Entity<IdentityUserToken<string>>().ToTable("UserToken");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogin");
        builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaim");
        builder.Entity<IdentityUserRole<string>>().ToTable("UserRole");

        //DataSeed
        string superAdminName = "SuperAdmin";
        string superAdminRoleDesc = "超級管理員";
        var superAdminRole = new Role() { Id = superAdminName, Name = superAdminName, NormalizedName = superAdminName.ToUpper(), RoleDesc = superAdminRoleDesc };
        superAdminRole.ConcurrencyStamp = "ConcurrencyStamp"; //為了防止再移轉
        builder.Entity<Role>().HasData(superAdminRole);

        string userName = "sadmin";
        string email = "sadmin@hcmfgroup.com";
        var user = new User() { Id = userName, DepartmentId = "部門代碼", EmployeeId = userName, EmployeeName = "超級管理員", UserName = userName, NormalizedUserName = userName.ToUpper(), Email = email, NormalizedEmail = email.ToUpper(), SecurityStamp = Guid.NewGuid().ToString() };
        user.PasswordHash = "AQAAAAEAACcQAAAAEOtRfNDmY3fKqd9iqJINpOVUiLz8JFKzKEz/Xt46A/eIfMdpdMjueu4xYIYFRncnXg=="; //為了防止再移轉
        user.SecurityStamp = "SecurityStamp"; //為了防止再移轉
        user.ConcurrencyStamp = "ConcurrencyStamp"; //為了防止再移轉
        builder.Entity<User>().HasData(user);
        builder.Entity<IdentityUserRole<string>>().HasData(
            new IdentityUserRole<string>() { UserId = user.Id, RoleId = superAdminRole.Id }
        );
    }
}