using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Survey.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class surveyIntial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Survey");

            migrationBuilder.CreateTable(
                name: "InboxState",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Received = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiveCount = table.Column<int>(type: "int", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Consumed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InboxState", x => x.Id);
                    table.UniqueConstraint("AK_InboxState_MessageId_ConsumerId", x => new { x.MessageId, x.ConsumerId });
                });

            migrationBuilder.CreateTable(
                name: "OutboxState",
                columns: table => new
                {
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Delivered = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastSequenceNumber = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxState", x => x.OutboxId);
                });

            migrationBuilder.CreateTable(
                name: "survey_types",
                schema: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name_en = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name_ar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessage",
                columns: table => new
                {
                    SequenceNumber = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EnqueueTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SentTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Headers = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Properties = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InboxMessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InboxConsumerId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OutboxId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    MessageType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConversationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    InitiatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SourceAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    DestinationAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ResponseAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    FaultAddress = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ExpirationTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessage", x => x.SequenceNumber);
                    table.ForeignKey(
                        name: "FK_OutboxMessage_InboxState_InboxMessageId_InboxConsumerId",
                        columns: x => new { x.InboxMessageId, x.InboxConsumerId },
                        principalTable: "InboxState",
                        principalColumns: new[] { "MessageId", "ConsumerId" });
                    table.ForeignKey(
                        name: "FK_OutboxMessage_OutboxState_OutboxId",
                        column: x => x.OutboxId,
                        principalTable: "OutboxState",
                        principalColumn: "OutboxId");
                });

            migrationBuilder.CreateTable(
                name: "surveys",
                schema: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    survey_type_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name_en = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    name_ar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    description_ar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    closing_address_en = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    closing_address_ar = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    closing_statement_en = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    closing_statement_ar = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    timezone = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    image_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    is_required = table.Column<bool>(type: "bit", nullable: false),
                    is_active = table.Column<bool>(type: "bit", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_surveys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_surveys_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_surveys_survey_types_survey_type_id",
                        column: x => x.survey_type_id,
                        principalSchema: "Survey",
                        principalTable: "survey_types",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "survey_questions",
                schema: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    QuestionAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_survey_questions_surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalSchema: "Survey",
                        principalTable: "surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "survey_responses",
                schema: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_responses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_survey_responses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_survey_responses_surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalSchema: "Survey",
                        principalTable: "surveys",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "survey_choises",
                schema: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    question_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    text_en = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    text_ar = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_choises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_survey_choises_survey_questions_question_id",
                        column: x => x.question_id,
                        principalSchema: "Survey",
                        principalTable: "survey_questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "survey_evaluate_choises",
                schema: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Emotion = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    question_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    text_en = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    text_ar = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_evaluate_choises", x => x.Id);
                    table.ForeignKey(
                        name: "FK_survey_evaluate_choises_survey_questions_question_id",
                        column: x => x.question_id,
                        principalSchema: "Survey",
                        principalTable: "survey_questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "survey_response_answers",
                schema: "Survey",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResponseId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextAnswer = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ChoiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MultipleChoices = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Evaluation = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_survey_response_answers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_survey_response_answers_survey_choises_ChoiceId",
                        column: x => x.ChoiceId,
                        principalSchema: "Survey",
                        principalTable: "survey_choises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_survey_response_answers_survey_evaluate_choises_Evaluation",
                        column: x => x.Evaluation,
                        principalSchema: "Survey",
                        principalTable: "survey_evaluate_choises",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_survey_response_answers_survey_questions_QuestionId",
                        column: x => x.QuestionId,
                        principalSchema: "Survey",
                        principalTable: "survey_questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_survey_response_answers_survey_responses_ResponseId",
                        column: x => x.ResponseId,
                        principalSchema: "Survey",
                        principalTable: "survey_responses",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_InboxState_Delivered",
                table: "InboxState",
                column: "Delivered");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_EnqueueTime",
                table: "OutboxMessage",
                column: "EnqueueTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_ExpirationTime",
                table: "OutboxMessage",
                column: "ExpirationTime");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_InboxMessageId_InboxConsumerId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "InboxMessageId", "InboxConsumerId", "SequenceNumber" },
                unique: true,
                filter: "[InboxMessageId] IS NOT NULL AND [InboxConsumerId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxMessage_OutboxId_SequenceNumber",
                table: "OutboxMessage",
                columns: new[] { "OutboxId", "SequenceNumber" },
                unique: true,
                filter: "[OutboxId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OutboxState_Created",
                table: "OutboxState",
                column: "Created");

            migrationBuilder.CreateIndex(
                name: "IX_survey_choises_Id",
                schema: "Survey",
                table: "survey_choises",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_survey_choises_question_id",
                schema: "Survey",
                table: "survey_choises",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_survey_evaluate_choises_Id",
                schema: "Survey",
                table: "survey_evaluate_choises",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_survey_evaluate_choises_question_id",
                schema: "Survey",
                table: "survey_evaluate_choises",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_survey_questions_SurveyId",
                schema: "Survey",
                table: "survey_questions",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_survey_response_answers_ChoiceId",
                schema: "Survey",
                table: "survey_response_answers",
                column: "ChoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_survey_response_answers_Evaluation",
                schema: "Survey",
                table: "survey_response_answers",
                column: "Evaluation");

            migrationBuilder.CreateIndex(
                name: "IX_survey_response_answers_QuestionId",
                schema: "Survey",
                table: "survey_response_answers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_survey_response_answers_ResponseId",
                schema: "Survey",
                table: "survey_response_answers",
                column: "ResponseId");

            migrationBuilder.CreateIndex(
                name: "IX_survey_responses_SurveyId",
                schema: "Survey",
                table: "survey_responses",
                column: "SurveyId");

            migrationBuilder.CreateIndex(
                name: "IX_survey_responses_UserId",
                schema: "Survey",
                table: "survey_responses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_surveys_end_date",
                schema: "Survey",
                table: "surveys",
                column: "end_date");

            migrationBuilder.CreateIndex(
                name: "IX_surveys_start_date",
                schema: "Survey",
                table: "surveys",
                column: "start_date");

            migrationBuilder.CreateIndex(
                name: "IX_surveys_survey_type_id",
                schema: "Survey",
                table: "surveys",
                column: "survey_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_surveys_user_id",
                schema: "Survey",
                table: "surveys",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutboxMessage");

            migrationBuilder.DropTable(
                name: "survey_response_answers",
                schema: "Survey");

            migrationBuilder.DropTable(
                name: "InboxState");

            migrationBuilder.DropTable(
                name: "OutboxState");

            migrationBuilder.DropTable(
                name: "survey_choises",
                schema: "Survey");

            migrationBuilder.DropTable(
                name: "survey_evaluate_choises",
                schema: "Survey");

            migrationBuilder.DropTable(
                name: "survey_responses",
                schema: "Survey");

            migrationBuilder.DropTable(
                name: "survey_questions",
                schema: "Survey");

            migrationBuilder.DropTable(
                name: "surveys",
                schema: "Survey");

            migrationBuilder.DropTable(
                name: "survey_types",
                schema: "Survey");
        }
    }
}
