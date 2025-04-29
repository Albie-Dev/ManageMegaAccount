using Microsoft.EntityFrameworkCore;

namespace MMA.Service
{
    partial class MMADbContext : DbContext
    {
        public MMADbContext(DbContextOptions<MMADbContext> options) : base(options: options)
        {
            
        }

        #region dbset -> tables in database
        public DbSet<UserEntity> Users { get; set; } = null!;
        public DbSet<RoleEntity> Roles { get; set; } = null!;
        public DbSet<UserRoleEntity> UserRoles { get; set; } = null!;
        public DbSet<UserTokenEntity> UserTokens { get; set; } = null!;

        #region Cloud
        public DbSet<MegaAccountEntity> MegaAccounts { get; set; } = null!;
        #endregion Cloud


        #region chat
        #endregion chat

        #region Movie
        public DbSet<MovieEntity> Movies { get; set; } = null!;
        public DbSet<ActorEntity> Actors { get; set; } = null!;
        #endregion Movie

        public DbSet<NotificationEntity> Notifications { get; set; } = null!;

        public DbSet<LinkHelperEntity> Links { get; set; } = null!;
        #endregion dbset




        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(configuration: new UserRoleEntityConfiguration());
            builder.ApplyConfiguration(configuration: new UserEntityConfiguration());
            builder.ApplyConfiguration(configuration: new RoleEntityConfiguration());
        }
    }
}