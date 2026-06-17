using Logistics.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Persistence.Migrations.Tenant
{
    /// <summary>
    /// Adds booked rate-per-mile and commodity fields (estimated weight, material type, packaging)
    /// to loads. Hand-written (additive nullable columns).
    /// </summary>
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260617100000_AddLoadCommodityFields")]
    public partial class AddLoadCommodityFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "rate_per_mile",
                table: "loads",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "estimated_weight",
                table: "loads",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "material_type",
                table: "loads",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "packaging",
                table: "loads",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(name: "rate_per_mile", table: "loads");
            migrationBuilder.DropColumn(name: "estimated_weight", table: "loads");
            migrationBuilder.DropColumn(name: "material_type", table: "loads");
            migrationBuilder.DropColumn(name: "packaging", table: "loads");
        }
    }
}
