using Microsoft.EntityFrameworkCore;
using OrderFlow.Infrastructure.Entities;

namespace OrderFlow.Infrastructure.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdminLog> AdminLogs { get; set; }

    public virtual DbSet<Advantage> Advantages { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<BlogCategory> BlogCategories { get; set; }

    public virtual DbSet<BlogPost> BlogPosts { get; set; }

    public virtual DbSet<ContactMessage> ContactMessages { get; set; }

    public virtual DbSet<EmailTemplate> EmailTemplates { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<PaymentDetail> PaymentDetails { get; set; }

    public virtual DbSet<PortfolioCategory> PortfolioCategories { get; set; }

    public virtual DbSet<PortfolioItem> PortfolioItems { get; set; }

    public virtual DbSet<SeoSetting> SeoSettings { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<SiteSetting> SiteSettings { get; set; }

    public virtual DbSet<SubscriptionEmail> SubscriptionEmails { get; set; }

    public virtual DbSet<SystemNotification> SystemNotifications { get; set; }

    public virtual DbSet<Tag> Tags { get; set; }

    public virtual DbSet<Technology> Technologies { get; set; }

    public virtual DbSet<Testimonial> Testimonials { get; set; }

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
    {
        //optionsBuilder.UseSqlServer("Data Source=localhost\\DB;Initial Catalog=OrderFlowDb;User ID=testdb;Password=B1242fMK;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False;Command Timeout=30");

        // Проверяем, если конфигурация уже настроена (например, через DI в Program.cs), то не перезаписываем её
        if (!optionsBuilder.IsConfigured)
        {
            // Строим конфигурацию из файла appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // Извлекаем строку подключения
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Инициализируем провайдер БД (в примере используется PostgreSQL, для SQL Server замените на UseSqlServer)
            optionsBuilder.UseSqlServer(connectionString);
        }
    }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdminLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AdminLog__3214EC0791B9584E");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<Advantage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Advantag__3214EC07C28E10A7");
        });

        modelBuilder.Entity<AspNetRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC070981861C");

            entity.HasIndex(e => e.NormalizedName, "IX_AspNetRoles_Name")
                .IsUnique()
                .HasFilter("([NormalizedName] IS NOT NULL)");
        });

        modelBuilder.Entity<AspNetRoleClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetRo__3214EC077A2F73CF");

            entity.HasOne(d => d.Role).WithMany(p => p.AspNetRoleClaims).HasConstraintName("FK_AspNetRoleClaims_Roles");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC0798798A87");

            entity.HasIndex(e => e.NormalizedEmail, "IX_AspNetUsers_Email")
                .IsUnique()
                .HasFilter("([NormalizedEmail] IS NOT NULL)");

            entity.HasIndex(e => e.NormalizedUserName, "IX_AspNetUsers_UserName")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.LockoutEnabled).HasDefaultValueSql("((1))");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AspNetUserRole",
                    r => r.HasOne<AspNetRole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_AspNetUserRoles_Roles"),
                    l => l.HasOne<AspNetUser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_AspNetUserRoles_Users"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                        j.HasIndex(new[] { "UserId" }, "IX_AspNetUserRoles_UserId");
                    });
        });

        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__AspNetUs__3214EC07F4416016");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasConstraintName("FK_AspNetUserClaims_Users");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasConstraintName("FK_AspNetUserLogins_Users");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasConstraintName("FK_AspNetUserTokens_Users");
        });

        modelBuilder.Entity<BlogCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogCate__3214EC0769B8D288");
        });

        modelBuilder.Entity<BlogPost>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__BlogPost__3214EC0767F1B07B");

            entity.HasOne(d => d.Category).WithMany(p => p.BlogPosts)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_BlogPosts_Category");

            entity.HasMany(d => d.Tags).WithMany(p => p.BlogPosts)
                .UsingEntity<Dictionary<string, object>>(
                    "BlogPostTag",
                    r => r.HasOne<Tag>().WithMany()
                        .HasForeignKey("TagId")
                        .HasConstraintName("FK_BlogPostTags_Tag"),
                    l => l.HasOne<BlogPost>().WithMany()
                        .HasForeignKey("BlogPostId")
                        .HasConstraintName("FK_BlogPostTags_Post"),
                    j =>
                    {
                        j.HasKey("BlogPostId", "TagId");
                    });
        });

        modelBuilder.Entity<ContactMessage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ContactM__3214EC072C18F465");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__EmailTem__3214EC07D89F4948");

            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC07B6239BAA");

            entity.ToTable(tb => tb.HasTrigger("trg_Orders_Audit"));

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<PaymentDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentD__3214EC078528E22E");

            entity.Property(e => e.Id).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<PortfolioCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Portfoli__3214EC07BED3C0AC");
        });

        modelBuilder.Entity<PortfolioItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Portfoli__3214EC0762442689");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");

            entity.HasMany(d => d.Categories).WithMany(p => p.PortfolioItems)
                .UsingEntity<Dictionary<string, object>>(
                    "PortfolioItemCategory",
                    r => r.HasOne<PortfolioCategory>().WithMany()
                        .HasForeignKey("CategoryId")
                        .HasConstraintName("FK_PortfolioItemCategories_Category"),
                    l => l.HasOne<PortfolioItem>().WithMany()
                        .HasForeignKey("PortfolioItemId")
                        .HasConstraintName("FK_PortfolioItemCategories_Item"),
                    j =>
                    {
                        j.HasKey("PortfolioItemId", "CategoryId");
                    });

            entity.HasMany(d => d.TechnologiesNavigation).WithMany(p => p.PortfolioItems)
                .UsingEntity<Dictionary<string, object>>(
                    "PortfolioTechnology",
                    r => r.HasOne<Technology>().WithMany()
                        .HasForeignKey("TechnologyId")
                        .HasConstraintName("FK_PortfolioTechnologies_Technology"),
                    l => l.HasOne<PortfolioItem>().WithMany()
                        .HasForeignKey("PortfolioItemId")
                        .HasConstraintName("FK_PortfolioTechnologies_Item"),
                    j =>
                    {
                        j.HasKey("PortfolioItemId", "TechnologyId");
                    });
        });

        modelBuilder.Entity<SeoSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SeoSetti__3214EC0710C5AAD3");

            entity.Property(e => e.OgType).HasDefaultValueSql("('website')");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Services__3214EC0740E55150");

            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<SiteSetting>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SiteSett__3214EC070F2C7DC5");

            entity.ToTable(tb => tb.HasTrigger("trg_SiteSettings_UpdateTimestamp"));

            entity.Property(e => e.Id).HasDefaultValueSql("((1))");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<SubscriptionEmail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Subscrip__3214EC07FA3D9CE5");

            entity.Property(e => e.IsActive).HasDefaultValueSql("((1))");
            entity.Property(e => e.SubscribedAt).HasDefaultValueSql("(getutcdate())");
        });

        modelBuilder.Entity<SystemNotification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__SystemNo__3214EC072DF42D96");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.ForAdminOnly).HasDefaultValueSql("((1))");
        });

        modelBuilder.Entity<Tag>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Tags__3214EC07B954C5AB");
        });

        modelBuilder.Entity<Technology>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Technolo__3214EC0730707CBB");
        });

        modelBuilder.Entity<Testimonial>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Testimon__3214EC070A6751C9");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
