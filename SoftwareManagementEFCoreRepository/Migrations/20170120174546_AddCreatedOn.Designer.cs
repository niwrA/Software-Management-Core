using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using SoftwareManagementEFCoreRepository;

namespace SoftwareManagementEFCoreRepository.Migrations
{
    [DbContext(typeof(MainContext))]
    [Migration("20170120174546_AddCreatedOn")]
    partial class AddCreatedOn
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

                    b.Property<DateTime>("CreatedOn");

                    b.Property<string>("Name");

                    b.Property<DateTime>("UpdatedOn");

                    b.HasKey("Guid");

                    b.ToTable("ProductStates");
                });
        }
    }
}
