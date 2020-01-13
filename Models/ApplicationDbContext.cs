using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Linq;

namespace Apoa.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Vierasavain kategoria -> kysymys
            modelBuilder.Entity<Category>()
                .HasMany(c => c.Assessments);
            // // Vieras avain Käyttäjä->Rooli
            // modelBuilder.Entity<User>()
            //     .HasOne(u => u.Role);

            modelBuilder.Entity<Assessment>()
                    .HasOne(a => a.ResponseOptions);

            // Vieras avain vastaus->kysymys
            modelBuilder.Entity<Response>()
                .HasOne(r => r.Assessment);

            // Vieras avain vastaus->käyttäjä
            modelBuilder.Entity<Response>()
                .HasOne(r => r.User);

            // monta moneen käyttäjä->ryhmä
            modelBuilder.Entity<UserGroup>()
                .HasKey(ug => new { ug.UserId, ug.GroupId });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany(u => u.UserGroups)
                .HasForeignKey(ug => ug.UserId);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany(g => g.GroupUsers)
                .HasForeignKey(pt => pt.GroupId);

            // Monta moneen kategoria->ryhmä
            modelBuilder.Entity<CategoryGroup>()
                .HasKey(cg => new { cg.GroupId, cg.CategoryId });

            modelBuilder.Entity<CategoryGroup>()
                .HasOne(cg => cg.Group)
                .WithMany(c => c.CategoryGroups)
                .HasForeignKey(cg => cg.GroupId);

            modelBuilder.Entity<CategoryGroup>()
                .HasOne(cg => cg.Category)
                .WithMany(c => c.CategoryGroups)
                .HasForeignKey(cg => cg.CategoryId);


            // Syötetään roolit tietokantaan, kun ajetaan migraatio
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Opettaja", NormalizedName = "OPETTAJA" });

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole() { Name = "Opiskelija", NormalizedName = "OPISKELIJA" });

            // Syötetään vastausvaihtoehdot automaattisesti tietokantaan kun ajetaan db_update

            modelBuilder.Entity<ResponseOptions>().HasData(new ResponseOptions() { Id = 1, Min = 0, Max = 2 },
                                                            new ResponseOptions() { Id = 2, Min = 0, Max = 5 },
                                                            new ResponseOptions() { Id = 3, Min = 0, Max = 10 });
        }

        // public User GetOrCreateUser(string mail) {

        //     var user = this.Users.Include(u => u.Role)
        //                     .Include(u => u.UserGroups)
        //                     .FirstOrDefault(u => u.Mail == mail);

        //      if (user == null) {
        //        user = new User();
        //        user.Mail = mail;

        //        string[] emailParts = mail.Split('@');

        //        if (emailParts[1] == "savonia.fi") {
        //            user.Role = this.Roles.FirstOrDefault(r => r.Id == 1);
        //            user.RoleId = 1;
        //        }
        //        else {
        //            user.Role = this.Roles.FirstOrDefault(r => r.Id == 2);
        //            user.RoleId = 2;
        //        }

        //        string[] userName = emailParts[0].Split(".");
        //        user.Name = string.Format("{0} {1}", userName[0], userName[1]);

        //         this.Users.Add(user);
        //         this.SaveChanges();
        //    }
        //    return user;
        // }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Assessment> Assessments { get; set; }
        public DbSet<Response> Responses { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<CategoryGroup> CategoryGroups { get; set; }

        public DbSet<ResponseOptions> ResponseOptions { get; set; }
    }


}