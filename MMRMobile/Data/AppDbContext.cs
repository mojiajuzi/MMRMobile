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
        if (optionsBuilder.IsConfigured) return;
        var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        var dbPath = Path.Combine(folder, "MMRMobile", "MMRMobile.db");
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    public DbSet<TagModel> Tags { get; set; }
    public DbSet<ContactTagModel> ContactTags { get; set; }
    public DbSet<ContactModel> Contacts { get; set; }

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
    }
}