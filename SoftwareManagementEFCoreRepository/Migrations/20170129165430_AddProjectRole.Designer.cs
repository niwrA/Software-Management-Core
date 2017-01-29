using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SoftwareManagementEFCoreRepository;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20170129165430_AddProjectRole")]
    partial class AddProjectRole
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.0-rtm-22752")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.CommandState", b =>
                {
                    b.Property<Guid>("Guid")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommandTypeId");

                    b.Property<DateTime>("CreatedOn");

                    b.Property<Guid>("EntityGuid");

                    b.Property<DateTime?>("ExecutedOn");

                    b.Property<string>("ParametersJson");

                    b.Property<DateTime?>("ReceivedOn");

                    b.Property<string>("UserName");

                    b.HasKey("Guid");

                    b.ToTable("CommandStates");
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

            modelBuilder.Entity("SoftwareManagementEFCoreRepository.ProjectRoleState", b =>
                {
                    b.HasOne("SoftwareManagementEFCoreRepository.ProjectState")
                        .WithMany("ProjectRoleStates")
                        .HasForeignKey("ProjectGuid")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
