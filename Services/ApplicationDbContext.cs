using Microsoft.EntityFrameworkCore;
using TeamAceProject.Models.Entities;

namespace TeamAceProject.Services
{
    // EF Core database context — registers all entity sets and configures relationships
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Team> Teams { get; set; } = null!;
        public DbSet<TeamMember> TeamMembers { get; set; } = null!;
        public DbSet<TeamMemberMove> TeamMemberMoves { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Reaction> Reactions { get; set; } = null!;

        // Defines keys, column constraints, indexes, and cascade rules for all entities
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(user => user.Id);
            modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();
            modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();
            modelBuilder.Entity<User>().Property(user => user.Username).IsRequired().HasMaxLength(50);
            modelBuilder.Entity<User>().Property(user => user.Email).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<User>().Property(user => user.PasswordHash).IsRequired();
            modelBuilder.Entity<User>().Property(user => user.FavoritePokemonName).HasMaxLength(100);
            modelBuilder.Entity<User>().Property(user => user.Bio).HasMaxLength(500);

            modelBuilder.Entity<Team>().HasKey(team => team.Id);
            modelBuilder.Entity<Team>().Property(team => team.Name).IsRequired().HasMaxLength(100);
            modelBuilder
                .Entity<Team>()
                .HasOne(team => team.User)
                .WithMany(user => user.Teams)
                .HasForeignKey(team => team.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMember>().HasKey(teamMember => teamMember.Id);
            modelBuilder.Entity<TeamMember>().Property(teamMember => teamMember.PokemonName).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<TeamMember>().Property(teamMember => teamMember.AbilityName).HasMaxLength(100);
            modelBuilder.Entity<TeamMember>().Property(teamMember => teamMember.NatureName).HasMaxLength(100);
            modelBuilder.Entity<TeamMember>().Property(teamMember => teamMember.HeldItemName).HasMaxLength(100);
            modelBuilder
                .Entity<TeamMember>()
                .HasIndex(teamMember => new { teamMember.TeamId, teamMember.SlotIndex })
                .IsUnique();
            modelBuilder
                .Entity<TeamMember>()
                .HasOne(teamMember => teamMember.Team)
                .WithMany(team => team.TeamMembers)
                .HasForeignKey(teamMember => teamMember.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TeamMemberMove>().HasKey(teamMemberMove => teamMemberMove.Id);
            modelBuilder.Entity<TeamMemberMove>().Property(teamMemberMove => teamMemberMove.MoveName).IsRequired().HasMaxLength(100);
            modelBuilder
                .Entity<TeamMemberMove>()
                .HasIndex(teamMemberMove => new { teamMemberMove.TeamMemberId, teamMemberMove.MoveSlot })
                .IsUnique();
            modelBuilder
                .Entity<TeamMemberMove>()
                .HasOne(teamMemberMove => teamMemberMove.TeamMember)
                .WithMany(teamMember => teamMember.TeamMemberMoves)
                .HasForeignKey(teamMemberMove => teamMemberMove.TeamMemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Post>().HasKey(post => post.Id);
            modelBuilder.Entity<Post>().Property(post => post.Caption).IsRequired().HasMaxLength(1000);
            modelBuilder
                .Entity<Post>()
                .HasOne(post => post.Team)
                .WithMany(team => team.Posts)
                .HasForeignKey(post => post.TeamId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder
                .Entity<Post>()
                .HasOne(post => post.User)
                .WithMany(user => user.Posts)
                .HasForeignKey(post => post.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>().HasKey(comment => comment.Id);
            modelBuilder.Entity<Comment>().Property(comment => comment.Body).IsRequired().HasMaxLength(1000);
            modelBuilder
                .Entity<Comment>()
                .HasOne(comment => comment.Post)
                .WithMany(post => post.Comments)
                .HasForeignKey(comment => comment.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder
                .Entity<Comment>()
                .HasOne(comment => comment.User)
                .WithMany(user => user.Comments)
                .HasForeignKey(comment => comment.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reaction>().HasKey(reaction => new { reaction.PostId, reaction.UserId });
            modelBuilder
                .Entity<Reaction>()
                .HasOne(reaction => reaction.Post)
                .WithMany(post => post.Reactions)
                .HasForeignKey(reaction => reaction.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder
                .Entity<Reaction>()
                .HasOne(reaction => reaction.User)
                .WithMany(user => user.Reactions)
                .HasForeignKey(reaction => reaction.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
