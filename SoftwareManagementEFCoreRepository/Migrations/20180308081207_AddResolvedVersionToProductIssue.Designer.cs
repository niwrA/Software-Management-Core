﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using SoftwareManagementEFCoreRepository;
using System;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20180308081207_AddResolvedVersionToProductIssue")]
    partial class AddResolvedVersionToProductIssue
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CommandElementState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid>("DesignGuid");

                    b.Property<Guid>("EntityElementGuid");

                    b.Property<Guid>("EpicElementGuid");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("EntityElementGuid");

                    b.ToTable("CommandElementStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CommandState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Command");

                    b.Property<string>("CommandVersion");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Entity");

                    b.Property<Guid>("EntityGuid");

                    b.Property<string>("EntityRoot");

                    b.Property<Guid>("EntityRootGuid");

                    b.Property<DateTime?>("ExecutedOn");

                    b.Property<string>("ParametersJson");

                    b.Property<DateTime?>("ReceivedOn");

                    b.Property<string>("UserName");

                    b.HasKey("Guid");

                    b.ToTable("CommandStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentAccountState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CompanyGuid");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("EnvironmentGuid");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("EnvironmentGuid");

                    b.ToTable("CompanyEnvironmentAccountStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentDatabaseState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CompanyGuid");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("EnvironmentGuid");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("EnvironmentGuid");

                    b.ToTable("CompanyEnvironmentDatabaseStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentHardwareState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CompanyGuid");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("EnvironmentGuid");

                    b.Property<string>("IpAddress");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("EnvironmentGuid");

                    b.ToTable("CompanyEnvironmentHardwareStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CompanyGuid");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<string>("Url");

                    b.HasKey("Guid");

                    b.HasIndex("CompanyGuid");

                    b.ToTable("CompanyEnvironmentStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyRoleState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CompanyGuid");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("CompanyGuid");

                    b.ToTable("CompanyRoleStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("ExternalId");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("CompanyStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ContactState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AvatarFileGuid");

                    b.Property<string>("AvatarUrl");

                    b.Property<DateTime?>("BirthDate");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Email");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("ContactStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.DesignElementState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid>("DesignGuid");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("DesignElementStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.DesignState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("DesignStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.EmploymentState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CompanyRoleGuid");

                    b.Property<Guid>("ContactGuid");

                    b.Property<string>("ContactName");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("EndDate");

                    b.Property<DateTime?>("StartDate");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("EmploymentStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.EntityElementState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid>("DesignGuid");

                    b.Property<Guid>("EpicElementGuid");

                    b.Property<bool>("IsCollection");

                    b.Property<string>("Name");

                    b.Property<Guid?>("ParentGuid");

                    b.Property<string>("PluralName");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("EpicElementGuid");

                    b.ToTable("EntityElementStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.EpicElementState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid>("DesignGuid");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("DesignGuid");

                    b.ToTable("EpicElementStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.FileState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentType");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid>("EntityGuid");

                    b.Property<string>("FileName");

                    b.Property<string>("FolderName");

                    b.Property<Guid>("ForGuid");

                    b.Property<string>("ForType");

                    b.Property<string>("Name");

                    b.Property<long>("Size");

                    b.Property<string>("Type");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("ForGuid");

                    b.ToTable("FileStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.LinkState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid>("EntityGuid");

                    b.Property<Guid>("ForGuid");

                    b.Property<string>("ImageUrl");

                    b.Property<string>("Name");

                    b.Property<string>("SiteName");

                    b.Property<DateTime>("UpdatedOn");

                    b.Property<string>("Url");

                    b.HasKey("Guid");

                    b.HasIndex("ForGuid");

                    b.ToTable("LinkStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductConfigOptionState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("DefaultValue");

                    b.Property<string>("Description");

                    b.Property<bool>("IsDefaultOption");

                    b.Property<bool>("IsOptionForParent");

                    b.Property<string>("Name");

                    b.Property<Guid?>("ParentGuid");

                    b.Property<string>("Path");

                    b.Property<Guid?>("ProductFeatureGuid");

                    b.Property<Guid>("ProductGuid");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("ProductGuid");

                    b.ToTable("ProductConfigOptionStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductFeatureState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid?>("FirstVersionGuid");

                    b.Property<bool>("IsRequest");

                    b.Property<string>("Name");

                    b.Property<Guid>("ProductGuid");

                    b.Property<Guid?>("RequestedForVersionGuid");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("ProductGuid");

                    b.ToTable("ProductFeatureStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductInstallationState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("CompanyEnvironmentGuid");

                    b.Property<Guid>("CompanyGuid");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("ExternalId");

                    b.Property<Guid>("ProductGuid");

                    b.Property<Guid?>("ProductVersionGuid");

                    b.Property<DateTime?>("StartDate");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("ProductInstallationStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductIssueState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<Guid>("FirstVersionGuid");

                    b.Property<string>("Name");

                    b.Property<Guid>("ProductGuid");

                    b.Property<Guid?>("ResolvedVersionGuid");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("ProductGuid");

                    b.ToTable("ProductIssueStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BusinessCase");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("ProductStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductVersionState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Build");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<int>("Major");

                    b.Property<int>("Minor");

                    b.Property<string>("Name");

                    b.Property<Guid>("ProductGuid");

                    b.Property<int>("Revision");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("ProductGuid");

                    b.ToTable("ProductVersionStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProjectRoleAssignmentState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ContactGuid");

                    b.Property<string>("ContactName");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("EndDate");

                    b.Property<Guid>("ProjectGuid");

                    b.Property<Guid>("ProjectRoleGuid");

                    b.Property<DateTime?>("StartDate");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("ProjectRoleAssignmentStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProjectRoleState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<Guid>("ProjectGuid");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("ProjectGuid");

                    b.ToTable("ProjectRoleStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProjectState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<DateTime?>("EndDate");

                    b.Property<string>("Name");

                    b.Property<DateTime?>("StartDate");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("ProjectStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.PropertyElementState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("DataType");

                    b.Property<string>("Description");

                    b.Property<Guid>("DesignGuid");

                    b.Property<Guid>("EntityElementGuid");

                    b.Property<Guid>("EpicElementGuid");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.HasIndex("EntityElementGuid");

                    b.ToTable("PropertyElementStates");
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CommandElementState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.EntityElementState")
                        .WithMany("CommandElementStates")
                        .HasForeignKey("EntityElementGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentAccountState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.CompanyEnvironmentState")
                        .WithMany("AccountStates")
                        .HasForeignKey("EnvironmentGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentDatabaseState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.CompanyEnvironmentState")
                        .WithMany("DatabaseStates")
                        .HasForeignKey("EnvironmentGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentHardwareState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.CompanyEnvironmentState")
                        .WithMany("HardwareStates")
                        .HasForeignKey("EnvironmentGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyEnvironmentState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.CompanyState")
                        .WithMany("CompanyEnvironmentStates")
                        .HasForeignKey("CompanyGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CompanyRoleState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.CompanyState")
                        .WithMany("CompanyRoleStates")
                        .HasForeignKey("CompanyGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.EntityElementState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.EpicElementState")
                        .WithMany("EntityElementStates")
                        .HasForeignKey("EpicElementGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.EpicElementState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.DesignState")
                        .WithMany("EpicElementStates")
                        .HasForeignKey("DesignGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductConfigOptionState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.ProductState")
                        .WithMany("ProductConfigOptionStates")
                        .HasForeignKey("ProductGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductFeatureState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.ProductState")
                        .WithMany("ProductFeatureStates")
                        .HasForeignKey("ProductGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductIssueState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.ProductState")
                        .WithMany("ProductIssueStates")
                        .HasForeignKey("ProductGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProductVersionState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.ProductState")
                        .WithMany("ProductVersionStates")
                        .HasForeignKey("ProductGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProjectRoleState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.ProjectState")
                        .WithMany("ProjectRoleStates")
                        .HasForeignKey("ProjectGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.PropertyElementState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.EntityElementState")
                        .WithMany("PropertyElementStates")
                        .HasForeignKey("EntityElementGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
