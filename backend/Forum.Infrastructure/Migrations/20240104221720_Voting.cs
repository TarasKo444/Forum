using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Voting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UniqueCommentVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsUpVote = table.Column<bool>(type: "INTEGER", nullable: false),
                    CommentId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniqueCommentVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniqueCommentVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniqueCommentVotes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UniquePostVotes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsUpVote = table.Column<bool>(type: "INTEGER", nullable: false),
                    PostId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UniquePostVotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UniquePostVotes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UniquePostVotes_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UniqueCommentVotes_CommentId",
                table: "UniqueCommentVotes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_UniqueCommentVotes_UserId",
                table: "UniqueCommentVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UniquePostVotes_PostId",
                table: "UniquePostVotes",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_UniquePostVotes_UserId",
                table: "UniquePostVotes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UniqueCommentVotes");

            migrationBuilder.DropTable(
                name: "UniquePostVotes");
        }
    }
}
