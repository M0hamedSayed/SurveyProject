
/*************************************
survey view
*************************************/
ALTER view vw_getSurveyDetails 
As
select 
	s.Id As Id,
    s.name_ar,
	s.name_en,
	s.description_ar,
	s.description_en,
	s.closing_address_ar,
	s.closing_address_en,
	s.closing_statement_ar,
	s.closing_statement_en,
	s.start_date,
	s.end_date,
	s.reminder_sent,
	s.image_url,
	s.is_active,
	s.is_required,
	s.user_id,
	s.timezone,
	s.RowVersion,
	s.created_at,
	s.updated_at,
	-- survey type
	st.Id As surveyTypeId,
	st.name_ar As surveyTypeNameAr,
	st.name_en As surveyTypeNameEn,
	-- survey questions
	q.Id As questionId,
	q.QuestionAr,
	q.QuestionEn,
	q.QuestionType,
	SurveyCount = 1,
	-- choices
    CASE WHEN q.QuestionType IN (1, 2) THEN c.Id END AS ChoiceId,
    CASE WHEN q.QuestionType IN (1, 2) THEN c.text_en END AS ChoiceTextEn,
    CASE WHEN q.QuestionType IN (1, 2) THEN c.text_ar END AS ChoiceTextAr,

    CASE WHEN q.QuestionType = 3 THEN ec.Id END AS EvaluateChoiceId,
    CASE WHEN q.QuestionType = 3 THEN ec.text_ar END AS EvaluateChoiceTextAr,
    CASE WHEN q.QuestionType = 3 THEN ec.text_en END AS EvaluateChoiceTextEn,
    CASE WHEN q.QuestionType = 3 THEN ec.emotion END AS EvaluateChoiceEmoji

from Survey.surveys s
	Left Join Survey.survey_types st
		on st.Id = s.survey_type_id
	Left Join Survey.survey_questions q
		on q.SurveyId = s.Id
	Left Join Survey.survey_choises c
		on q.Id = c.question_id And q.QuestionType In (1,2)
	Left Join Survey.survey_evaluate_choises ec
		on q.Id = ec.question_id And q.QuestionType = 3




/*************************************
get on survey procedure
*************************************/
ALTER proc getOneSurvey 
	@SurveyId UNIQUEIDENTIFIER,
	@UserId UNIQUEIDENTIFIER,
    @IsAdmin BIT
As
Begin
select * 
from vw_getSurveyDetails
where 
	(
		(@IsAdmin = 1 AND user_id = @UserId)
        OR
        (@IsAdmin = 0 AND user_id IN (
             SELECT Id FROM AspNetUsers WHERE ManagerId = @UserId
         ))
     )
	AND
	@SurveyId = Id
End


/*************************************
get survey list with pagination procedure
*************************************/
ALTER PROCEDURE sp_GetFilteredSurveys
    @UserId UNIQUEIDENTIFIER,
    @IsAdmin BIT,
    @Search NVARCHAR(255) = NULL,
    @StartDate DATETIME2 = NULL,
    @EndDate DATETIME2 = NULL,
    @SurveyTypeId UNIQUEIDENTIFIER = NULL,
    @IsActive BIT = NULL,
    @Page INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;

	 -- Validate page parameters
    IF @Page < 1 SET @Page = 1;
    IF @PageSize < 1 OR @PageSize > 100 SET @PageSize = 10;

    -- CTE to get distinct Survey IDs with filtering
    ;WITH FilteredSurveys AS (
        SELECT 
            s.Id, 
            s.start_date
        FROM 
            vw_getSurveyDetails s
        WHERE 
            -- Role-based ownership
            (
                (@IsAdmin = 1 AND s.user_id = @UserId)
                OR
                (@IsAdmin = 0 AND s.user_id IN (
                    SELECT Id FROM AspNetUsers WHERE ManagerId = @UserId
                ))
            )
            -- Admin bypasses activation/start date logic
            AND (
                @IsAdmin = 1
                OR s.is_active = 1
                OR s.start_date <= SYSUTCDATETIME()
            )
            -- Search filtering
            AND (
                @Search IS NULL
                OR s.name_ar LIKE '%' + @Search + '%'
                OR s.name_en LIKE '%' + @Search + '%'
            )
            -- Date range filtering
            AND (
                (@StartDate IS NULL OR s.start_date >= @StartDate)
                AND (@EndDate IS NULL OR s.end_date <= @EndDate)
            )
            -- SurveyType filtering
            AND (@SurveyTypeId IS NULL OR s.surveyTypeId = @SurveyTypeId)
            -- IsActive flag
            AND (@IsActive IS NULL OR s.is_active = @IsActive)
        GROUP BY 
            s.Id, 
            s.start_date  -- Ensure distinct surveys
    ),
    -- Get Total Count
    TotalCount AS (
        SELECT COUNT(*) AS TotalCount FROM FilteredSurveys
    ),
    -- Paginated Survey IDs
    PaginatedSurveys AS (
        SELECT 
            fs.Id,
            fs.start_date,
            ROW_NUMBER() OVER (ORDER BY fs.start_date DESC) AS RowNum
        FROM 
            FilteredSurveys fs
    )
    -- Select results
    SELECT 
        tc.TotalCount As SurveyCount,
		v.Id,
		v.name_ar,
		v.name_en,
		v.description_ar,
		v.description_en,
		v.closing_address_ar,
		v.closing_address_en,
		v.closing_statement_ar,
		v.closing_statement_en,
		v.start_date,
		v.end_date,
		v.image_url,
		v.is_active,
		v.is_required,
		v.user_id,
		v.timezone,
		v.created_at,
		v.updated_at,
		v.surveyTypeId,
		v.surveyTypeNameAr,
		v.surveyTypeNameEn,
		v.questionId,
		v.QuestionAr,
		v.QuestionEn,
		v.QuestionType,
		v.ChoiceId,
		v.ChoiceTextEn,
		v.ChoiceTextAr,
		v.EvaluateChoiceId,
		v.EvaluateChoiceTextAr,
        v.EvaluateChoiceTextEn,
		v.EvaluateChoiceEmoji,
		v.RowVersion,
		v.reminder_sent

    FROM 
        vw_getSurveyDetails v
    INNER JOIN 
        PaginatedSurveys ps ON v.Id = ps.Id
    CROSS JOIN 
        TotalCount tc
    WHERE 
        ps.RowNum BETWEEN ((@Page - 1) * @PageSize) + 1 AND @Page * @PageSize
    ORDER BY 
        ps.start_date DESC;
END


