-- This statement will assign 'Blank' to the variable CompoundsViewScript if the system
-- has RegIntegration.  We make this determination by looking for the column comments on the 
-- INV_VW_REG_BATCHES view.  No comments indicates the view was created as the default, meaning
-- no integration.  When there are no comments, no record is returned and we use the current 
-- variable setting, which was initially set to Inv_VW_Compounds.  When there are comments, 
-- we instead assign the value of 'Blank' to the variable and essentially no script is executed...
-- bypassing the view creation until a later stage.
select decode(comments,null,'&CompoundsViewScript.','Blank')
as comments_col
from user_col_comments where table_name = 'INV_VW_REG_BATCHES'
and column_name = 'REGNUMBER';