// <copyright file="20221030221725_EngineIndices.cs" company="Dominic Ritz">
// Copyright (c) Dominic Ritz. All rights reserved.
// Licensed under the GNU GPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TauStellwerk.Server.Database.Migrations;

public partial class EngineIndices : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Engines_Created",
            table: "Engines");

        migrationBuilder.DropIndex(
            name: "IX_Engines_LastUsed",
            table: "Engines");

        migrationBuilder.CreateIndex(
            name: "IX_Engines_Created_IsHidden",
            table: "Engines",
            columns: new[] { "Created", "IsHidden" });

        migrationBuilder.CreateIndex(
            name: "IX_Engines_LastUsed_IsHidden",
            table: "Engines",
            columns: new[] { "LastUsed", "IsHidden" });

        migrationBuilder.CreateIndex(
            name: "IX_Engines_Name_IsHidden",
            table: "Engines",
            columns: new[] { "Name", "IsHidden" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Engines_Created_IsHidden",
            table: "Engines");

        migrationBuilder.DropIndex(
            name: "IX_Engines_LastUsed_IsHidden",
            table: "Engines");

        migrationBuilder.DropIndex(
            name: "IX_Engines_Name_IsHidden",
            table: "Engines");

        migrationBuilder.CreateIndex(
            name: "IX_Engines_Created",
            table: "Engines",
            column: "Created");

        migrationBuilder.CreateIndex(
            name: "IX_Engines_LastUsed",
            table: "Engines",
            column: "LastUsed");
    }
}