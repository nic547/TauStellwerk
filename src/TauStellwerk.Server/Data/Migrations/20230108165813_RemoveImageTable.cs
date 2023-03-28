// <copyright file="20230108165813_RemoveImageTable.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TauStellwerk.Server.Database.Migrations;

/// <inheritdoc />
public partial class RemoveImageTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "EngineImages");

        migrationBuilder.AddColumn<string>(
            name: "ImageSizes",
            table: "Engines",
            type: "TEXT",
            nullable: false,
            defaultValue: string.Empty);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ImageSizes",
            table: "Engines");

        migrationBuilder.CreateTable(
            name: "EngineImages",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                EngineId = table.Column<int>(type: "INTEGER", nullable: true),
                Filename = table.Column<string>(type: "TEXT", nullable: false),
                Width = table.Column<int>(type: "INTEGER", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EngineImages", x => x.Id);
                table.ForeignKey(
                    name: "FK_EngineImages_Engines_EngineId",
                    column: x => x.EngineId,
                    principalTable: "Engines",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateIndex(
            name: "IX_EngineImages_EngineId",
            table: "EngineImages",
            column: "EngineId");
    }
}
