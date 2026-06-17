using Logistics.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Persistence.Migrations.Tenant
{
    /// <summary>
    /// Adds company website, contact person, MC# and DOT# to customers.
    /// Hand-written (additive nullable columns).
    /// </summary>
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260617130000_AddCustomerCompanyFields")]
    public partial class AddCustomerCompanyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(name: "website", table: "customers", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "contact_person", table: "customers", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "mc_number", table: "customers", type: "text", nullable: true);
            migrationBuilder.AddColumn<string>(name: "dot_number", table: "customers", type: "text", nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "website", table: "customers");
            migrationBuilder.DropColumn(name: "contact_person", table: "customers");
            migrationBuilder.DropColumn(name: "mc_number", table: "customers");
            migrationBuilder.DropColumn(name: "dot_number", table: "customers");
        }
    }
}
