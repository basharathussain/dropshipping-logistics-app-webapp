using Logistics.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Logistics.Infrastructure.Persistence.Migrations.Tenant
{
    /// <summary>
    /// Adds the captured finalized LLM prompt to AI dispatch sessions.
    /// Hand-written (additive, single nullable column).
    /// </summary>
    [DbContext(typeof(TenantDbContext))]
    [Migration("20260616093000_AddAiDispatchSessionPrompt")]
    public partial class AddAiDispatchSessionPrompt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "prompt",
                table: "ai_dispatch_sessions",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "prompt",
                table: "ai_dispatch_sessions");
        }
    }
}
