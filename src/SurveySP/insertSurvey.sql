/**************************
Handle survey insert 
***************************/
--DROP TYPE Survey.SurveyTableType;
--DROP PROCEDURE Survey.sp_InsertSurvey;
CREATE TYPE Survey.SurveyTableType AS TABLE (
    --Id UNIQUEIDENTIFIER NOT NULL,
    survey_type_id UNIQUEIDENTIFIER NOT NULL,
    user_id UNIQUEIDENTIFIER NOT NULL,
    name_en NVARCHAR(100) NOT NULL,
    name_ar NVARCHAR(100) NOT NULL,
    description_en NVARCHAR(500) NULL,
    description_ar NVARCHAR(500) NULL,
    closing_address_en NVARCHAR(100) NULL,
    closing_address_ar NVARCHAR(100) NULL,
    closing_statement_en NVARCHAR(500) NULL,
    closing_statement_ar NVARCHAR(500) NULL,
    start_date DATETIMEOFFSET NOT NULL,
    end_date DATETIMEOFFSET NOT NULL,
    timezone NVARCHAR(100) NOT NULL,
    image_url NVARCHAR(500) NULL,
    is_required BIT NOT NULL,
    is_active BIT NOT NULL
    --created_at DATETIME2 NULL,
    --updated_at DATETIME2 NULL,
    --row_version ROWVERSION
);

CREATE TYPE Survey.SurveyQuestionsType AS TABLE
(
	QuestionEn NVARCHAR(300) NOT NULL,
    QuestionAr NVARCHAR(300) NOT NULL,
    QuestionType INT NOT NULL
);

CREATE TYPE Survey.SurveyChoicesType AS TABLE
(
    QuestionIndex INT NOT NULL,
    text_ar NVARCHAR(300) NOT NULL,
    text_en NVARCHAR(300) NOT NULL
);

CREATE TYPE Survey.EvaluateChoicesType AS TABLE
(
    QuestionIndex INT NOT NULL,
    text_ar NVARCHAR(300) NOT NULL,
    text_en NVARCHAR(300) NOT NULL,
	emotion NVARCHAR(10) NOT NULL
);

/********************
Handle Procedure to insert
*********************/
CREATE PROC Survey.sp_InsertSurvey
	@SurveyDetails Survey.SurveyTableType READONLY,
    @SurveyQuestions Survey.SurveyQuestionsType READONLY,
    @SurveyChoices Survey.SurveyChoicesType READONLY,
    @EvaluateChoices Survey.EvaluateChoicesType READONLY
AS
BEGIN
	SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

		DECLARE @SurveyId UNIQUEIDENTIFIER = NEWID();
        DECLARE @Now DATETIME2 = SYSUTCDATETIME();
		-- insert survey
		INSERT INTO Survey.surveys (
            Id, name_ar, name_en, description_ar, description_en,
            closing_address_ar, closing_address_en, closing_statement_ar, closing_statement_en,
            start_date, end_date, image_url, is_active, is_required, user_id, timezone,
            created_at, updated_at, survey_type_id, reminder_sent
        )
        SELECT 
            @SurveyId,
            name_ar, name_en, description_ar, description_en,
            closing_address_ar, closing_address_en, closing_statement_ar, closing_statement_en,
            start_date, end_date, image_url, is_active, is_required, user_id, timezone,
            @Now, @Now, survey_type_id,0
        FROM @SurveyDetails;

		-- Insert Questions
        /*INSERT INTO Survey.survey_questions (
            Id, SurveyId, QuestionAr, QuestionEn, QuestionType, created_at, updated_at
        )
        SELECT 
            NEWID(), 
            @SurveyId, 
            QuestionAr, 
            QuestionEn, 
            ISNULL(QuestionType,0), 
            @Now, 
            @Now
        FROM @SurveyQuestions;*/
		-- another approach using merge
		DECLARE @QuestionMap TABLE (
			TempId INT IDENTITY(1,1),
			QuestionId UNIQUEIDENTIFIER,
			QuestionType INT
		);

		MERGE INTO Survey.survey_questions AS target
		USING @SurveyQuestions AS src
		on 1=0
		WHEN NOT MATCHED THEN
			INSERT (Id, SurveyId, QuestionAr, QuestionEn, QuestionType, created_at, updated_at)
			VALUES (NEWID(), @SurveyId, QuestionAr, QuestionEn, ISNULL(QuestionType,0), @Now, @Now )
		OUTPUT  inserted.Id, inserted.QuestionType
			INTO @QuestionMap(QuestionId, QuestionType);
		
		-- Insert Choices (for question types 1 and 2)
        INSERT INTO Survey.survey_choises (
            Id, question_id, text_ar, text_en
        )
        SELECT 
            NEWID(), 
            qm.QuestionId, 
            sc.text_ar, 
            sc.text_en
        FROM @SurveyChoices sc
        JOIN @QuestionMap qm ON sc.QuestionIndex = qm.TempId
        WHERE qm.QuestionType IN (1, 2);

		 -- Insert Evaluate Choices (for question type 3)
        INSERT INTO Survey.survey_evaluate_choises (
            Id, question_id, text_ar, text_en, emotion
        )
        SELECT 
            NEWID(), 
            qm.QuestionId, 
            ec.text_ar, 
            ec.text_en, 
            ec.emotion
        FROM @EvaluateChoices ec
        JOIN @QuestionMap qm ON ec.QuestionIndex = qm.TempId
        WHERE qm.QuestionType = 3;

		COMMIT TRANSACTION;
        
        -- Return the complete survey data
        SELECT * FROM vw_getSurveyDetails WHERE Id = @SurveyId;
    END TRY
	BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END;