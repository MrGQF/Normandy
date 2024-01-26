using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Normandy.Identity.UserData.Domain.Models;

namespace Normandy.Identity.UserData.EnityFrameworkCore.DbContexts
{
    public partial class UserDataContext : DbContext
    {
        public UserDataContext()
        {
        }

        public UserDataContext(DbContextOptions<UserDataContext> options)
        : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(new EventId[] { RelationalEventId.MultipleCollectionIncludeWarning }));
            base.OnConfiguring(optionsBuilder);            
        }

        public virtual DbSet<AcBase> AcBases { get; set; }
        public virtual DbSet<AppPaEmail> AppPaEmails { get; set; }
        public virtual DbSet<AppPaMobile> AppPaMobiles { get; set; }
        public virtual DbSet<AppPaMobile2> AppPaMobile2s { get; set; }
        public virtual DbSet<VerifyUserinfo> VerifyUserinfos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AcBase>(entity =>
            {
                entity.HasKey(e => e.Thsid)
                    .HasName("PRIMARY");

                entity.ToTable("ac_base");

                entity.HasIndex(e => e.Userid, "userid");

                entity.Property(e => e.Thsid)
                    .HasColumnType("bigint(64) unsigned")
                    .HasColumnName("thsid");

                entity.Property(e => e.Appid)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("appid");

                entity.Property(e => e.Deleted)
                    .HasColumnType("tinyint(8)")
                    .HasColumnName("deleted")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.DeletedTime)
                    .HasColumnName("deleted_time")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Email)
                    .HasMaxLength(255)
                    .HasColumnName("email")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Identifier)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Note)
                    .HasMaxLength(255)
                    .HasColumnName("note")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .HasColumnName("phone")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Status)
                    .HasColumnType("tinyint(8)")
                    .HasColumnName("status")
                    .HasDefaultValueSql("'0'");

                entity.Property(e => e.UpdatedTime)
                    .HasColumnName("updated_time")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Userid)
                    .HasColumnType("int(11)")
                    .HasColumnName("userid")
                    .HasDefaultValueSql("'NULL'");
            });

            modelBuilder.Entity<AppPaEmail>(entity =>
            {
                entity.HasKey(e => e.Userid)
                    .HasName("PRIMARY");

                entity.ToTable("app_pa_email");

                entity.HasIndex(e => e.Email, "email");

                entity.Property(e => e.Userid)
                    .HasColumnType("int(11)")
                    .HasColumnName("userid");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("email")
                    .HasDefaultValueSql("''''''");
            });

            modelBuilder.Entity<AppPaMobile>(entity =>
            {
                entity.HasKey(e => e.Userid)
                    .HasName("PRIMARY");

                entity.ToTable("app_pa_mobile");

                entity.HasIndex(e => e.Mobile, "mobile");

                entity.Property(e => e.Userid)
                    .HasColumnType("int(11)")
                    .HasColumnName("userid");

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("mobile")
                    .HasDefaultValueSql("''''''");
            });

            modelBuilder.Entity<AppPaMobile2>(entity =>
            {
                entity.ToTable("app_pa_mobile2");

                entity.HasIndex(e => e.Country, "idx_country");

                entity.HasIndex(e => e.Mobile, "idx_mobile");

                entity.HasIndex(e => e.Userid, "uinq_userid");

                entity.Property(e => e.Id)
                    .HasColumnType("int(10) unsigned")
                    .HasColumnName("id");

                entity.Property(e => e.Country)
                    .HasColumnType("int(11)")
                    .HasColumnName("country")
                    .HasDefaultValueSql("'86'");

                entity.Property(e => e.Ctime)
                    .HasColumnName("ctime")
                    .HasDefaultValueSql("'''0000-00-00 00:00:00'''");

                entity.Property(e => e.Mobile)
                    .IsRequired()
                    .HasMaxLength(32)
                    .HasColumnName("mobile")
                    .HasDefaultValueSql("''''''");

                entity.Property(e => e.Mtime)
                    .HasColumnName("mtime")
                    .HasDefaultValueSql("'''0000-00-00 00:00:00'''");

                entity.Property(e => e.Userid)
                    .HasColumnType("int(11)")
                    .HasColumnName("userid");
            });

            modelBuilder.Entity<VerifyUserinfo>(entity =>
            {
                entity.HasKey(e => e.Userid)
                    .HasName("PRIMARY");

                entity.ToTable("verify_userinfo");

                entity.HasIndex(e => new { e.Account, e.Qsid }, "ix_verify_userinfo");

                entity.HasIndex(e => e.Msgcode, "ix_verify_userinfo_msgcode");

                entity.HasIndex(e => e.Nocase, "ix_verify_userinfo_nocase");

                entity.HasIndex(e => e.Regtime, "ix_verify_userinfo_regtime");

                entity.Property(e => e.Userid)
                    .HasColumnType("int(11)")
                    .HasColumnName("userid");

                entity.Property(e => e.Account)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("account")
                    .HasDefaultValueSql("''''''");

                entity.Property(e => e.Msgcode)
                    .HasColumnType("int(11)")
                    .HasColumnName("msgcode");

                entity.Property(e => e.Nocase)
                    .IsRequired()
                    .HasMaxLength(250)
                    .HasColumnName("nocase")
                    .HasDefaultValueSql("''''''");

                entity.Property(e => e.Passwd)
                    .IsRequired()
                    .HasMaxLength(64)
                    .HasColumnName("passwd")
                    .HasDefaultValueSql("''''''");

                entity.Property(e => e.Qsid)
                    .HasColumnType("int(11)")
                    .HasColumnName("qsid")
                    .HasDefaultValueSql("'800'");

                entity.Property(e => e.Qsuserclass)
                    .HasColumnType("int(11)")
                    .HasColumnName("qsuserclass")
                    .HasDefaultValueSql("'NULL'");

                entity.Property(e => e.Regtime)
                    .HasColumnName("regtime")
                    .HasDefaultValueSql("'''0000-00-00 00:00:00'''");

                entity.Property(e => e.Userclass)
                    .HasColumnType("int(11)")
                    .HasColumnName("userclass")
                    .HasDefaultValueSql("'10000'");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
