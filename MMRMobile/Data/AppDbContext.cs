using System;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MMRMobile.Models;

namespace MMRMobile.Data;

public class AppDbContext : DbContext
{
    protected AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dbPath = Path.Combine(folder, "MMRMobile", "MMRMobile.db");
            Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

            // 禁用外键约束
            optionsBuilder.UseSqlite($"Data Source={dbPath};Foreign Keys=False");
        }
    }

    public DbSet<TagModel> Tags { get; set; }
    public DbSet<ContactModel> Contacts { get; set; }
    public DbSet<ContactTagModel> ContactTags { get; set; }
    public DbSet<WorkContactModel> WorkContacts { get; set; }
    public DbSet<WorkTagModel> WorkTags { get; set; }
    public DbSet<WorkModel> Works { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<ContactTagModel>(entity =>
        {
            entity.HasKey(ct => new { ct.ContactId, ct.TagId });
            entity.HasOne<ContactModel>(ct => ct.Contact).WithMany(c => c.ContactTags)
                .HasForeignKey(ct => ct.ContactId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<TagModel>(ct => ct.Tag).WithMany(t => t.ContactTags).HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(ct => ct.CreateTime).IsRequired();
            entity.Property(ct => ct.DateModified).IsRequired();
        });
        modelBuilder.Entity<WorkTagModel>(entity =>
        {
            entity.HasKey(ct => new { ct.WorkId, ct.TagId });
            entity.HasOne<WorkModel>(wt => wt.Work).WithMany(w => w.WorkTags).HasForeignKey(t => t.WorkId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<TagModel>(t => t.Tag).WithMany(t => t.WorkTags).HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.Property(ct => ct.CreateTime).IsRequired();
            entity.Property(ct => ct.DateModified).IsRequired();
        });
        modelBuilder.Entity<WorkContactModel>(entity =>
        {
            entity.HasKey(ct => new { ct.WorkId, ct.ContactId });

            entity.HasOne<WorkModel>(wc => wc.Work).WithMany(w => w.WorkContacts).HasForeignKey(ct => ct.WorkId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<ContactModel>(c => c.Contact).WithMany(c => c.WorkContacts).HasForeignKey(ct => ct.ContactId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}