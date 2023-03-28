// <copyright file="20230106135238_EngineTagAsSingleString.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TauStellwerk.Server.Database.Migrations;

/// <inheritdoc />
public partial class EngineTagsAsSingleString : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Tags",
            table: "Engines",
            type: "TEXT",
            nullable: false,
            defaultValue: string.Empty);

        migrationBuilder.Sql(@"
                UPDATE Engines
                SET  Tags = old_table.tags
                FROM
                (
                SELECT EnginesId as id, GROUP_CONCAT(Name,'') AS tags  FROM EngineTag
                LEFT OUTER JOIN Tags ON Tags.Id = EngineTag.TagsId
                GROUP BY EnginesId
                ) AS old_table
                WHERE Engines.Id = old_table.id
                ");

        migrationBuilder.DropTable(
            name: "EngineTag");

        migrationBuilder.DropTable(
            name: "Tags");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Tags",
            table: "Engines");

        migrationBuilder.CreateTable(
            name: "Tags",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Name = table.Column<string>(type: "TEXT", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tags", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "EngineTag",
            columns: table => new
            {
                EnginesId = table.Column<int>(type: "INTEGER", nullable: false),
                TagsId = table.Column<int>(type: "INTEGER", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_EngineTag", x => new { x.EnginesId, x.TagsId });
                table.ForeignKey(
                    name: "FK_EngineTag_Engines_EnginesId",
                    column: x => x.EnginesId,
                    principalTable: "Engines",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_EngineTag_Tags_TagsId",
                    column: x => x.TagsId,
                    principalTable: "Tags",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_EngineTag_TagsId",
            table: "EngineTag",
            column: "TagsId");

        migrationBuilder.CreateIndex(
            name: "IX_Tags_Name",
            table: "Tags",
            column: "Name",
            unique: true);
    }
}