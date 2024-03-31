grammar PythonComments;

file : (NEWLINE | statement)* EOF;

statement : COMMENT
     | MULTILINE_COMMENT_DOUBLE_QUOTE
     | MULTILINE_COMMENT_SINGLE_QUOTE
     ;

COMMENT : SHARP ~[\r\n]*;
SHARP : '#';
DOUBLE_QUOTES : '"""';
SINGLE_QUOTES : '\'\'\'';

MULTILINE_COMMENT_DOUBLE_QUOTE : DOUBLE_QUOTES .*? DOUBLE_QUOTES;
MULTILINE_COMMENT_SINGLE_QUOTE : SINGLE_QUOTES .*? SINGLE_QUOTES;

NEWLINE : ('\r'? '\n' | '\r')+ -> skip;

WS : [ \t]+ -> skip;
