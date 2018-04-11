CREATE OR REPLACE TYPE &&schemaname..t_wm_concat AS OBJECT (
    g_string CLOB
  , STATIC FUNCTION odciaggregateinitialize( sctx IN OUT t_wm_concat ) 
    RETURN NUMBER
  , MEMBER FUNCTION odciaggregateiterate( SELF IN OUT t_wm_concat
                                        , VALUE IN CLOB )
    RETURN NUMBER
  , MEMBER FUNCTION odciaggregateterminate( SELF IN t_wm_concat
                                          , returnvalue OUT CLOB
                                          , flags IN NUMBER )
    RETURN NUMBER
  , MEMBER FUNCTION odciaggregatemerge( SELF IN OUT t_wm_concat
                                      , ctx2 IN t_wm_concat )
    RETURN NUMBER ) ;
/

CREATE OR REPLACE TYPE BODY &&schemaname..t_wm_concat IS
STATIC FUNCTION odciaggregateinitialize( sctx IN OUT t_wm_concat )
RETURN NUMBER 
IS
BEGIN
    sctx := t_wm_concat( NULL ) ;
    RETURN odciconst.success ;
END ;

MEMBER FUNCTION odciaggregateiterate( SELF IN OUT t_wm_concat
                                    , VALUE IN CLOB )
RETURN NUMBER 
IS
BEGIN
    self.g_string := self.g_string || ',' || VALUE ;
    RETURN odciconst.success ;
END ;

MEMBER FUNCTION odciaggregateterminate( SELF IN t_wm_concat
                                      , returnvalue OUT CLOB
                                      , flags IN NUMBER )
RETURN NUMBER 
IS
BEGIN
    returnvalue := RTRIM( LTRIM( self.g_string, ',' ), ',' ) ;
    RETURN odciconst.success ;
END ;

MEMBER FUNCTION odciaggregatemerge( SELF IN OUT t_wm_concat
                                  , ctx2 IN t_wm_concat )
RETURN NUMBER 
IS
BEGIN
    self.g_string := self.g_string || ',' || ctx2.g_string ;
    RETURN odciconst.success ;
END ;

END ;
/

CREATE OR REPLACE FUNCTION &&schemaname..wm_concat( p_input IN VARCHAR2 ) 
RETURN CLOB
PARALLEL_ENABLE AGGREGATE USING t_wm_concat ;
/

create or replace public synonym wm_concat for &&schemaname..wm_concat;
