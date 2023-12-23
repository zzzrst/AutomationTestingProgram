SET SERVEROUTPUT ON FORMAT WRAPPED;


define formCode = 'VARIABLE_OVERWRITE_FORM_NAME'
define formYear = VARIABLE_OVERWRITE_FORM_YEAR
define orgCode = 'VARIABLE_OVERWRITE_ORG_CODE'
define dateOfExecution = 'VARIABLE_OVERWRITE_DATE_TIME'
define stateCode = 'VARIABLE_OVERWRITE_STATE_CODE'

CREATE OR REPLACE PROCEDURE print_table_data (v_org_code VARCHAR2, v_form_year VARCHAR2, v_form_code VARCHAR2) IS
    max_length1 NUMBER := 0;
    max_length2 NUMBER := 0;
BEGIN
   FOR rec IN (SELECT * FROM EDCS_PICASSO.WORKFLOW_STATE WORKFLOW_STATE 
               WHERE form_data_id IN (
                    SELECT form_data_id
                    FROM TABLE(get_data_id('&&orgCode', &&formYear, '&&formCode')))) LOOP
          max_length1 := GREATEST(max_length1, LENGTH(rec.WORKFLOW_STATE_ID));
          max_length2 := GREATEST(max_length2, LENGTH(rec.FORM_DATA_ID));
   END LOOP;
        
   FOR rec IN (SELECT * FROM EDCS_PICASSO.WORKFLOW_STATE WORKFLOW_STATE 
               WHERE form_data_id IN (
                    SELECT form_data_id
                    FROM TABLE(get_data_id('&&orgCode', &&formYear, '&&formCode')))) LOOP
          DBMS_OUTPUT.PUT_LINE('WORKFLOW_STATE_ID: ' || RPAD(rec.WORKFLOW_STATE_ID, max_length1) || '    FORM_DATA_ID: ' || RPAD(rec.FORM_DATA_ID, max_length2) || '    CURRENT_WORKFLOW_STEP_ID: ' || rec.CURRENT_WORKFLOW_STEP_ID);
   END LOOP;
END print_table_data;
/


CREATE OR REPLACE FUNCTION get_data_id(v_org_code VARCHAR2, v_form_year VARCHAR2, v_form_code VARCHAR2)
	return VARCHAR2 SQL_MACRO
IS
BEGIN
	return q'{
	   SELECT form_data_id
       FROM edcs_picasso.form_data fdata
            JOIN edcs_core.organization_view org ON (fdata.dataset_code = org.organization_code)
            JOIN edcs_picasso.form_definition fd USING (form_definition_id)
       WHERE 
            fd.FORM_DEFINITION_CODE = '&&formCode'
            AND fd.form_iteration_code = '&&formYear'
            AND org.organization_code IN ('&&orgCode')
	}';
END;
/


SPOOL "K:\ESIP\EDCS QA\QTP\SQL Scripts\spool_logs\Revert_&&formCode._&&formYear._&&orgCode._&&dateOfExecution..log" APPEND;
    BEGIN
        DBMS_OUTPUT.PUT_LINE('----------------------------------------------------------[NEW REVERT ENTRY]---------------------------------------------------------');
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

UPDATE edcs_picasso.workflow_state
  SET CURRENT_WORKFLOW_STEP_ID = (
    SELECT workflow_step_id
    FROM edcs_picasso.workflow_step
    WHERE workflow_step_code = '&&stateCode'  -- target workflow step code (string)
  )
WHERE
  form_data_id IN (
    SELECT form_data_id
    FROM TABLE(get_data_id('&&orgCode', &&formYear, '&&formCode')));

COMMIT;

SPOOL "K:\ESIP\EDCS QA\QTP\SQL Scripts\spool_logs\Revert_&&formCode._&&formYear._&&orgCode._&&dateOfExecution..log" APPEND;
    BEGIN
        DBMS_OUTPUT.PUT_LINE('-----------------------------------------------------------[UPDATED DATA]------------------------------------------------------------');
    END;
    /
    
    EXECUTE print_table_data('&&orgCode', &&formYear, '&&formCode');
SPOOL OFF    
