using Logistics.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Persistence.Migrations.Master
{
    /// <summary>Adds company website + contact person to tenants. Hand-written (additive nullable columns).</summary>
    [DbContext(typeof(MasterDbContext))]
    [Migration("20260617120000_AddTenantWebsiteContactPerson")]
    public partial class AddTenantWebsiteContactPerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "website",
                table: "tenants",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "contact_person",
                table: "tenants",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "website", table: "tenants");
            migrationBuilder.DropColumn(name: "contact_person", table: "tenants");
        }
    }
}
