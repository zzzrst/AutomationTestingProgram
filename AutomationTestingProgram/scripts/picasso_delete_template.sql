SET SERVEROUTPUT ON FORMAT WRAPPED;

-- 4 variables to overwrite (are to be overwritten)
define formCode = 'VARIABLE_OVERWRITE_FORM_NAME'
define formYear = VARIABLE_OVERWRITE_FORM_YEAR
define orgCode = 'VARIABLE_OVERWRITE_ORG_CODE'
define dateOfExecution = 'VARIABLE_OVERWRITE_DATE_TIME'

CREATE OR REPLACE PROCEDURE print_table_data (v_org_code VARCHAR2, v_form_year VARCHAR2, v_form_code VARCHAR2) IS
    max_length NUMBER := 0;
BEGIN
   FOR rec IN (SELECT FIELD_DEFINITION_CODE, FIELD_DATA_VALUE_TEXT 
                    FROM EDCS_PICASSO.FIELD_DATA FIELD_DATA WHERE FIELD_DATA.FORM_DATA_ID = ( SELECT * FROM get_data_id(v_org_code, v_form_year, v_form_code))) LOOP
            max_length := GREATEST(max_length, LENGTH(rec.FIELD_DEFINITION_CODE));
   END LOOP;
        
   FOR rec IN (SELECT FIELD_DEFINITION_CODE, FIELD_DATA_VALUE_TEXT 
                    FROM EDCS_PICASSO.FIELD_DATA FIELD_DATA_DEL WHERE FIELD_DATA_DEL.FORM_DATA_ID = ( SELECT * FROM get_data_id(v_org_code, v_form_year, v_form_code))) LOOP
          DBMS_OUTPUT.PUT_LINE('FIELD_DEFINITION_CODE: ' || RPAD(rec.FIELD_DEFINITION_CODE, max_length) || '    FIELD_DATA_VALUE_TEXT: ' || rec.FIELD_DATA_VALUE_TEXT);
   END LOOP;
END print_table_data;
/


CREATE OR REPLACE FUNCTION get_data_id(v_org_code VARCHAR2, v_form_year VARCHAR2, v_form_code VARCHAR2)
	return VARCHAR2 SQL_MACRO
IS
BEGIN
	return q'{
	   SELECT FDATA_DEL.FORM_DATA_ID
       FROM   EDCS_PICASSO.FORM_DATA FDATA_DEL
       WHERE  FDATA_DEL.FORM_DEFINITION_ID = (
              SELECT FDEF_DEL.FORM_DEFINITION_ID
              FROM   EDCS_PICASSO.FORM_DEFINITION FDEF_DEL
              WHERE  FDEF_DEL.FORM_DEFINITION_CODE = v_form_code
			  AND    FDEF_DEL.FORM_ITERATION_CODE = v_form_year)
       AND    FDATA_DEL.DATASET_CODE = v_org_code
       ORDER BY FORM_DATA_ID desc
       FETCH FIRST 1 ROWS ONLY
	}';
END;
/

SPOOL "K:\ESIP\EDCS QA\QTP\SQL Scripts\spool_logs\Delete_&&formCode._&&formYear._&&orgCode._&&dateOfExecution..log" APPEND;
    BEGIN
        DBMS_OUTPUT.PUT_LINE('----------------------------------------------------------[NEW DELETE ENTRY]---------------------------------------------------------');
        DBMS_OUTPUT.PUT_LINE(TO_CHAR(SYSDATE, 'MM-DD-YYYY-HH24-MI-SS'));
    END;
    /
    
    SELECT NAME AS Database_Name
    FROM V$DATABASE;
    
    BEGIN
        DBMS_OUTPUT.PUT_LINE('-----------------------------------------------------------[ORIGINAL DATA]-----------------------------------------------------------');
    END;
    /
    
    EXECUTE print_table_data('&&orgCode', &&formYear, '&&formCode');
SPOOL OFF

UPDATE EDCS_PICASSO.FIELD_DATA FIELD_DATA_DEL
SET FIELD_DATA_VALUE_TEXT = NULL,
    FIELD_DATA_VALUE_FILE = NULL,
    LAST_UPDATE_TS = LOCALTIMESTAMP(6), 
    LAST_UPDATE_USER_ID = 'QA-SQL-REG'
WHERE  FIELD_DATA_DEL.FORM_DATA_ID = ( SELECT * FROM get_data_id('&&orgCode', &&formYear, '&&formCode'));
INSERT INTO EDCS_PICASSO.FIELD_DATA_AUDIT (
		FIELD_DATA_ID,
		FORM_DATA_ID,
		FLD_DT_SECTION_REPEAT_SEQUENCE,
		FLD_DT_ROW_REPEAT_SEQUENCE,
		LAST_UPDATE_TS,
		LAST_UPDATE_USER_ID,
		FIELD_DEFINITION_CODE,
		FLD_DT_PAGE_REPEAT_SEQUENCE,
		FIELD_DEFINITION_ID,
		FIELD_DATA_VALUE_TEXT,
		FIELD_DATA_AUDIT_ID,
		AUDIT_OPERATION,
		AUDIT_CREATE_TS,
		AUDIT_SENT_TS
	)
SELECT  
		FIELD_DATA_ID,
		FORM_DATA_ID,
		FLD_DT_SECTION_REPEAT_SEQUENCE,
		FLD_DT_ROW_REPEAT_SEQUENCE,
		LAST_UPDATE_TS,
		LAST_UPDATE_USER_ID,
		FIELD_DEFINITION_CODE,
		FLD_DT_PAGE_REPEAT_SEQUENCE,
		FIELD_DEFINITION_ID,
		FIELD_DATA_VALUE_TEXT,
		NULL,
		'UPDATE',
		LOCALTIMESTAMP(6), 
        NULL
FROM EDCS_PICASSO.FIELD_DATA FIELD_DATA
WHERE  FIELD_DATA.FORM_DATA_ID = ( SELECT * FROM get_data_id('&&orgCode', &&formYear, '&&formCode'));

SPOOL "K:\ESIP\EDCS QA\QTP\SQL Scripts\spool_logs\Delete_&&formCode._&&formYear._&&orgCode._&&dateOfExecution..log" APPEND;
    BEGIN
        DBMS_OUTPUT.PUT_LINE('-----------------------------------------------------------[UPDATED DATA]------------------------------------------------------------');
    END;
    /
    
    EXECUTE print_table_data('&&orgCode', &&formYear, '&&formCode');
SPOOL OFF              

COMMIT;
