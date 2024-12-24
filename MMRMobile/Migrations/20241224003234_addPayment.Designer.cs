﻿// <auto-generated />
using System;
using MMRMobile.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace MMRMobile.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241224003234_addPayment")]
    partial class addPayment
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("MMRMobile.Models.ContactModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("Phone")
                        .HasColumnType("TEXT");

                    b.Property<int>("TagsId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Wechat")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Phone")
                        .IsUnique();

                    b.HasIndex("TagsId");

                    b.ToTable("Contacts");
                });

            modelBuilder.Entity("MMRMobile.Models.ContactTagModel", b =>
                {
                    b.Property<int>("ContactId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.HasKey("ContactId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ContactTags");
                });

            modelBuilder.Entity("MMRMobile.Models.TagModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Active")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("tags");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkContactModel", b =>
                {
                    b.Property<int>("WorkId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("ContactId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.HasKey("WorkId", "ContactId");

                    b.HasIndex("ContactId");

                    b.ToTable("WorkContacts");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("EndAt")
                        .HasColumnType("TEXT");

                    b.Property<decimal>("Funds")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Status")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Works");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkPaymentModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Amount")
                        .HasColumnType("decimal(18,2)");

                    b.Property<int?>("ContactId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.Property<bool>("HasInvoice")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsIncome")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("PaymentDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Remark")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("TEXT");

                    b.Property<int>("WorkId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ContactId");

                    b.HasIndex("WorkId");

                    b.ToTable("WorkPayments");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkTagModel", b =>
                {
                    b.Property<int>("WorkId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("TEXT");

                    b.HasKey("WorkId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("WorkTags");
                });

            modelBuilder.Entity("MMRMobile.Models.ContactModel", b =>
                {
                    b.HasOne("MMRMobile.Models.TagModel", "Tags")
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tags");
                });

            modelBuilder.Entity("MMRMobile.Models.ContactTagModel", b =>
                {
                    b.HasOne("MMRMobile.Models.ContactModel", "Contact")
                        .WithMany("ContactTags")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MMRMobile.Models.TagModel", "Tag")
                        .WithMany("ContactTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contact");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkContactModel", b =>
                {
                    b.HasOne("MMRMobile.Models.ContactModel", "Contact")
                        .WithMany("WorkContacts")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MMRMobile.Models.WorkModel", "Work")
                        .WithMany("WorkContacts")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contact");

                    b.Navigation("Work");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkPaymentModel", b =>
                {
                    b.HasOne("MMRMobile.Models.ContactModel", "Contact")
                        .WithMany("WorkPayments")
                        .HasForeignKey("ContactId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("MMRMobile.Models.WorkModel", "Work")
                        .WithMany("WorkPayments")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Contact");

                    b.Navigation("Work");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkTagModel", b =>
                {
                    b.HasOne("MMRMobile.Models.TagModel", "Tag")
                        .WithMany("WorkTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("MMRMobile.Models.WorkModel", "Work")
                        .WithMany("WorkTags")
                        .HasForeignKey("WorkId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tag");

                    b.Navigation("Work");
                });

            modelBuilder.Entity("MMRMobile.Models.ContactModel", b =>
                {
                    b.Navigation("ContactTags");

                    b.Navigation("WorkContacts");

                    b.Navigation("WorkPayments");
                });

            modelBuilder.Entity("MMRMobile.Models.TagModel", b =>
                {
                    b.Navigation("ContactTags");

                    b.Navigation("WorkTags");
                });

            modelBuilder.Entity("MMRMobile.Models.WorkModel", b =>
                {
                    b.Navigation("WorkContacts");

                    b.Navigation("WorkPayments");

                    b.Navigation("WorkTags");
                });
#pragma warning restore 612, 618
        }
    }
}
