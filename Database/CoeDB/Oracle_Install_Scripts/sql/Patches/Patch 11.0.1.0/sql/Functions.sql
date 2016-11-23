--Copyright 1999-2010 CambridgeSoft Corporation. All rights reserved

CREATE OR REPLACE FUNCTION Normalize(AChemName varchar2) RETURN VARCHAR2 deterministic IS
    TYPE TReplacements IS Varray(600) OF Varchar2(300);
    chemName     Varchar2(2000);
    replacements TReplacements;
    newname Varchar2(2000):='';
    offset Integer:= 1;
    i Integer:= 1;
    numReplacements Integer;
    didReplace boolean:= false;
    repl integer;
    CharChemName Varchar2(1);
    BeforeCharChemName Varchar2(1);
    AfterCharChemName Varchar2(1);

    thisOrig Varchar2(2000);
    thisReplacement Varchar2(2000);
	deltaOffset Integer;
    ChemNameLength Integer;

    FUNCTION CleanNormalized(AChemName VARCHAR2) RETURN VARCHAR2 IS
        TYPE TReplacements IS Varray(100) OF Varchar2(50);
        replacements TReplacements;
        ChemName     Varchar2(2000);
        newname Varchar2(2000);
        offset Integer;
        CharChemName Varchar2(1);
        numReplacements Integer;
        didReplace boolean;
        repl Integer;
    BEGIN
        ChemName:='('||Lower(AChemName);

        replacements:=TReplacements(
                        '11dimethylethyl',                        'tbutyl',
                        '1methylpropyl',                        'sbutyl',
                        '1methylethyl',                            'isopropyl',
                        '135triazine',                            'striazin',
                        'arachid',                                'eicosan',
                        'butyr',                                'butan',
                        'capro',                                'hexan',
                        'd+',                                    'd',
                        'd-',                                    'd',
                        'ethylenedinitrilotetraaceticacid',        'edta',
                        'ethylenediaminetetraaceticacid',        'edta',
                        'ethylenediaminetetraacetate',            'edta',
                        'edetate',                                'edta',
                        'enath',                               'heptan',
                        'laur',                                    'dodecan',
                        'l+',                                    'l',
                        'l-',                                    'l',
                        'myrist',                                'tetradecan',
                        'naphthalenyl',                            'naphthyl',
                        'pelargon',                                'nonan',
                        'propion',                                'propan',
                        'palmit',                                'hexadecan',
                        'silveri',                                'silver',
                        'stear',                                'octadecan',
                        'valer',                                'pentan');
        newname := '';
        offset := 1;
        LOOP
            EXIT WHEN  (offset>=Length(chemname)); --while (offset < chemname.Length)
            CharChemName :=  SUBSTR(chemName,offset+1,1);
            IF (ASCII(CharChemName) < 32) THEN
                offset := offset + 1;    -- skip this character
            ELSE
                --now do some cleanup of the genrated name

                numReplacements := replacements.count;

                didReplace := false;
                repl := 1;
                LOOP
                    EXIT WHEN  (repl>numReplacements);
                    -- Make sure the first character is a match; if not, no point in checking anything else
                    IF ((CharChemName = SUBSTR(replacements(repl),1,1)) AND  (Length(chemname) - offset >= Length(replacements(repl))) AND (SUBSTR(chemname,offset+1, Length(replacements(repl))) = replacements(repl))) THEN
                        newname := newname || replacements(repl + 1);
                        offset := offset + Length(replacements(repl));
                        didReplace := true;
                        EXIT;
                    END IF;
                    repl := repl + 2;
                END LOOP;
                IF  NOT(didReplace) THEN
                    newname := newname || CharChemName;  --added this
                    offset := offset + 1;
                END IF;
            END IF;
        END LOOP;
        RETURN newname;
    END;

    FUNCTION FixIcAte(AChemName VARCHAR2) RETURN VARCHAR2 IS
        TYPE TReplacements IS Varray(100) OF Varchar2(50);
        LChemName       Varchar2(2000);
        clen            Integer;
        numReplacements Integer;
        repl            Integer;
        replacements    TReplacements;
    BEGIN
        LChemName:=AChemName;
        clen := Length(AChemName);

        replacements:=TReplacements(
                            'pelargon',        'nonano',
                            'propion',        'propano',
                            'arachid',        'eicosano',
                            'methano',        'form',
                            'myrist',        'tetradecano',
                            'ethano',        'acet',
                            'palmit',        'hexadecano',
                            'stear',        'octadecano',
                            'valer',        'pentano',
                            'butyr',        'butano',
                            'capro',        'hexano',
                            'enath',        'heptano',
                            'laur',            'dodecano',
                            'capr',            'decano'
                         );

        numReplacements := replacements.count;
        repl := 1;
        LOOP
            EXIT WHEN  (repl>numReplacements);
            -- Make sure the first character is a match; if not, no point in checking anything else
            IF (Length(LChemName) >= Length(replacements(repl)) AND
                SUBSTR(LChemName,(Length(LChemName) - Length(replacements(repl)))+1) = replacements(repl)) THEN

                LChemName := Replace(LChemName,replacements(repl), replacements(repl + 1));
                Exit;
            END IF;
            repl:=repl+2;
        END LOOP;

        RETURN LChemName;
    END;


    FUNCTION IsLetter(AChar Varchar2) RETURN boolean IS
        LLetters Varchar2(50):='abcdefghijklmnopqrstuvwxyz';
    BEGIN
        IF INSTR(LLetters,AChar)>0 THEN
            RETURN TRUE;
        ELSE
            RETURN FALSE;
        END IF;
    END;
BEGIN
    chemName:='('||Lower(AChemName);

    replacements:=TReplacements(
                         ' potassium',              'potassium',
                         ' sodium',                 'sobium',
                         ' and ',                   '+',
                         ' na+',                    'sobium',
                         ' k+',                     'potassium',
                         ' ',                       '',                    --this might be wrong
                         '`',                       '''',                --this might be wrong or might be able to moved up
                         '&',                       '+',                --this might be wrong or might be able to moved up
                         ',0''',                    ',O''',
                         ',',                       '',
                         '-0-',                     '-O-',
                         '-',                       '',
                         '; ',                      '',                    --what is this
                         '0,0',                     '22',
                         '1+',                      'i',
                         '2hcl',                    'bihydrochlorid',
                         '2+',                      'ii',
                         '3+',                      'iii',
                         '4+',                      'iv',
                         '5+',                      'v',
                         '6+',                      'vi',
                         '(-)',                     '-',
                         '('    ,                   '',                    --this might be wrong
                         '#',                       'no',
                         '+/-',                     '+-',
                         '+,-',                     '+-',
                         '+-',                      '+-',
                         'antimony tri',            'antimonyiii',
                         'antimonious',             'antimonyiii',
                         'antimonous',              'antimonyiii',
                         'antimonius',              'antimonyiii',
                         'aluminium',               'aluminum',
                         'anhydrous',               '',
                         'argentous',               'silveri',
                         'arsenious',               'arseniciii',
                         'anisidine',               'methoxybenzenamin',
                         'arsenous',                'arseniciii',
                         'aniline',                 'benzenamin',
                         'ammine',                  'amin',
                         'alpha',                   'a',
                         'benzeneamine',            'benzenamin',
                         'bismuth tri',             'bismuthiii',
                         'bromo',                   'brom',
                         'beta',                    'b',
                         'bis',                     'bi',
                         'carbamodithioato-S,S',    'bithiocarbamato',
                         'columbium',               'niobium',
                         'cobaltous',               'cobaltii',
                         'carbamoyl',               'carbamyl',
                         'complex',                 '',
                         'chromic',                 'chromiumiii',
                         'caesium',                 'cesium',
                         'cuprous',                 'copperi',
                         'capryl ',                 'octyl',
                         'chloro',                  'chlor',
                         'capryl',                  'octano',
                         'cresol',                  'methylphenol',
                         'cupric',                  'copperii',
                         'cerous',                  'ceriumiii',
                         'choro',                   'chlor',
                         'ceric',                   'ceriumiv',
                         'dodecahydrate',           '12h2o',
                         'decahydrate' ,            '10h2o',
                         'derivative' ,             '',
                         'dihydrate' ,              '2h2o',
                         'dextro' ,                 'd',
                         'delta',                   'd',
                         'des',                     'de',
                         'di',                      'bi',
                         'enneahydrate',            '9h2o',
                         'enathyl',                 'heptyl',
                         'eico',                    'ico',
                         'ehty',                    'ethy',
                         'ferrous',                 'ironii',
                         'ferric',                  'ironiii',
                         'fluoro',                  'fluor',
                         'flouro',                  'fluor',
                         'flour',                   'fluor',
                         'gamma',                   'y',
                         'hemipentahydrate',        '2.5h2o',
                         'heptahydrate',            '7h2o',
                         'hemihydrate',             '1/2h2o',
                         'hexahydrate',             '6h2o',
                         'hexadecyl',               'cetyl',
                         'hydrate',                 'h2o',
                         'hyro',                    'hydro',
                         'hcl',                     'hydrochlorid',
                         'hbr',                     'hydrobromid',
                         'icacid',                  'at',
                         'i'    ,                   'i',
                         'levo',                    'l',
                         'mercaptan',               'thiol',
                         'manganous',               'manganesii',
                         'mercurous',               'mercuryi',
                         'mercuric',                'mercuryii',
                         'yl alcohol',              'anol',
                         'ylalcohol',               'anol',
                         'methyl',                  'methyl',
                         'methy',                   'methy',            --these are out of order on purpose
                         'methyoxy',                'methoxy',
                         'meta',                    '3',                --missing some other weritd thing
                         'nonahydrate',             '9h2o',
                         'nickelous',               'nickelii',
                         'napth',                   'naphth',
                         'octadecahydrate',         '18h2o',
                         'octahydrate',             '8h2o',                --some ous acid stuff missing after this
                         'ortho' ,                  '2',
                         'omega',                   'w',
                         'phosphorothioate',        'thiophosphat',
                         'pentahydrate' ,           '5h2o',
                         'phosphorous',             'phosphorus',
                         'plumbous',                'leadi',
                         'pyrine' ,                 'pyren',
                         'para',                    '4',
                         'phth',                    'phth',
                         'pht',                     'phth',
                         'p',                       'p',
                         'sesquihydrate',           '3/2h2o',
                         'salicycl',                'salicyl',
                         'stannane',                'tin',
                         'stannous',                'tinii',
                         'stannic',                 'tiniv',
                         'sulph',                   'sulf',
                         'salts',                   '',
                         'sec-',                    's',
                         'salt',                    '',
                         'tetrahydrate',            '4h2o',
                         'trihydrate',              '3h2o',
                         'toluidine',               'methylbenzenamin',
                         'thallous',                'thalliumi',
                         'terthiop',                'terthiop',
                         'thallic',                 'thalliumiii',
                         'tert',                    't',
                         'tris',                    'tri',
                         't',                       't',
                         'water',                   'h2o',
                         'yl mercaptan',            'anethiol',
                         'ylmercaptan',             'anethiol');

    newname := '';
    offset := 1;
    ChemNameLength:=LENGTH(chemName);
    LOOP
        EXIT WHEN offset >=ChemNameLength;
        CharChemName :=  SUBSTR(chemName,offset+1,1);
        BeforeCharChemName :=  SUBSTR(chemName,offset,1);
        AfterCharChemName :=  SUBSTR(chemName,offset+2,1);

        IF (ASCII(CharChemName) < 32) THEN
            offset:=offset+1;    -- skip this character
        ELSIF (CharChemName = '.'
            OR CharChemName = ')' OR CharChemName = ']' OR CharChemName = '}'
            OR CharChemName = '(' OR CharChemName = '[' OR CharChemName = '{'
            OR CharChemName = '/' OR CharChemName = '_' OR CharChemName = ':'
            OR CharChemName = '\\') THEN

            --we are supposed to be maintaining (-) as - and (+) as +
            IF ((offset + 3 <= ChemNameLength) AND ((SUBSTR(chemname,offset+1,3) = '(-)') OR (SUBSTR(chemname,offset+1,3) = '(+)'))) THEN
                newname := newname || AfterCharChemName;
                offset:=offset + 3;
            ELSE
                offset:=offset + 1;    --    skip this character
            END IF;
        ELSIF (offset + 3 <= ChemNameLength AND CharChemName = 'a' AND SUBSTR(chemname,offset+1,3) = 'ate') THEN
            newname := FixIcAte(newname) || 'a';
            offset := offset + 1;
        ELSIF (offset + 4 <= ChemNameLength AND CharChemName = 'a' AND SUBSTR(chemname,offset+1, 4) = 'amyl') THEN
            newname := newname || 'pent';
            offset := offset + 2;
        ELSIF (offset > 0 AND BeforeCharChemName = 'y' AND CharChemName = 'c' AND ChemNameLength - offset > 5 AND SUBSTR(chemname, offset+1, 5) = 'ceric') THEN
            newname := newname || 'cer';
            offset := offset + 3;
            -- ceric --> cer
        ELSIF (ChemNameLength>5 AND offset = ChemNameLength - 1 AND offset > 0 AND CharChemName = 'e' AND IsLetter(BeforeCharChemName)) THEN
            -- drop an 'e' at the very end of the name
            offset := offset + 1;    --    skip this character
        ELSIF (ChemNameLength>5 AND offset > 0 AND offset + 1 < ChemNameLength AND CharChemName = 'e' AND IsLetter(BeforeCharChemName) AND NOT(IsLetter(AfterCharChemName))) THEN
            -- drop an 'e' after a letter and before a non-letter
            offset := offset + 1;    --    skip this character
        ELSIF (offset + 7 <= ChemNameLength AND CharChemName = 'i' AND SUBSTR(chemname, offset+1, 7) = 'ic acid') THEN
            IF INSTR(newname,'yl')<>0 THEN
                newname := newname || 'icacid';
            ELSE
                newname := FixIcAte(newname) || 'at';
            END IF;
            offset := offset + 7;
        ELSIF (offset + 6 <= ChemNameLength AND CharChemName = 'i' AND SUBSTR(chemname, offset+1, 6) = 'icacid') THEN
            newname := FixIcAte(newname) || 'at';
            offset := offset + 6;
        ELSIF (offset + 8 <= ChemNameLength AND CharChemName = 'o' AND SUBSTR(chemname, offset+1, 8) = 'ous acid') THEN
            IF INSTR(newname,'yl')<>0 THEN
                newname := newname || 'ousacid';
            ELSE
                newname := newname || 'it';
            END IF;
            offset := offset + 8;
        ELSIF (offset + 6 <= ChemNameLength AND CharChemName = 'i' AND SUBSTR(chemname, offset+1, 6) = 'icacid') THEN
            newname := FixIcAte(newname) || 'at';
            offset := offset + 6;
        ELSIF (offset > 0 AND offset + 1 < ChemNameLength AND CharChemName = 'm'
            AND INSTR('-' ||' ' ||',' || '''',AfterCharChemName)>0
            AND INSTR('-' || ' ' || ',' || '''' || '(' || '[' || '{',BeforeCharChemName)>0) THEN
            newname := newname || '3';
            offset := offset + 1;    --    skip this character
        ELSIF (offset + 3 <= ChemNameLength AND CharChemName = 'o' AND SUBSTR(chemname, offset+1, 3) = 'oyl') THEN
            newname := FixIcAte(newname) || 'o';
            --                    newname = FixIcAte(newsyns);
            --                    if (newname.LENGTH() > 0 AND newname[newname.LENGTH - 1] != 'o')
            --                        newname ||= 'o';
            offset := offset + 1;
        ELSIF (offset > 0 AND offset + 1 < ChemNameLength AND CharChemName = 'o'
              AND INSTR('-'||' '||','|| '''',AfterCharChemName)>0
              AND INSTR('-'|| ' '|| ','|| ''''|| '('|| '['|| '{',BeforeCharChemName)>0 )  THEN
            newname := newname || '2';
            offset := offset + 1;    --    skip this character
        ELSIF (offset > 0 AND offset + 1 < ChemNameLength AND CharChemName = 'p'
              AND INSTR('-'||' '||','|| '''',AfterCharChemName)>0
              AND INSTR('-'|| ' '|| ','|| ''''|| '('|| '['|| '{',BeforeCharChemName)>0 )  THEN
            newname := newname || '4';
            offset := offset + 1;    --    skip this character
            --don't replace des at end of string
       ELSIF (offset + 3 = ChemNameLength AND CharChemName = 'd' AND SUBSTR(chemname, offset+1, 3) = 'des') THEN
			newname := newname || 'des';
			offset :=  offset + 3;

        ELSE

            numReplacements := replacements.count;
            didReplace := false;
            repl := 1;
            LOOP
                EXIT WHEN  (repl>numReplacements);
                -- Make sure the first character is a match; if not, no point in checking anything else

                IF ((CharChemName = SUBSTR(replacements(repl),1,1)) AND
                     (ChemNameLength - offset >= LENGTH(replacements(repl))) AND
                    (SUBSTR(chemname, offset+1, LENGTH(replacements(repl))) = replacements(repl))) THEN
                        thisOrig := replacements(repl);
            			thisReplacement := replacements(repl + 1);
            			deltaOffset :=Length(thisOrig);
            			IF (NOT(replacements(repl) = ' potassium') AND NOT(replacements(repl) = ' sodium')
                           AND NOT(replacements(repl) = ' na+') AND NOT(replacements(repl) = ' k+')) THEN
            				-- If both the original and the replacement end in 'yl',
            				-- then don't replace that part.  We only checked it to confirm the match,
            				-- but we might still want to replace it further.
            				-- enanthyl alcohol --> heptyl alcohol --> heptanol
            				IF (deltaOffset > 2
            					AND SUBSTR(thisOrig,deltaOffset - 1,1) = 'y'
                                AND SUBSTR(thisOrig,deltaOffset,1) = 'l'
            					AND SUBSTR(thisReplacement,LENGTH(thisReplacement) - 1,1) = 'y'
                                AND SUBSTR(thisReplacement,LENGTH(thisReplacement),1) = 'l') THEN
            					thisReplacement := SUBSTR(thisReplacement,1, LENGTH(thisReplacement) - 2);
            					deltaOffset := deltaOffset - 2;
                            END IF;
            				newname := newname || thisReplacement;
                        ELSE
            				newname := thisReplacement || newname;
                        END IF;
            			offset := offset + deltaOffset;
                        didReplace := TRUE;
                        EXIT;
                END IF;
                repl := repl + 2;
            END LOOP;
            IF NOT (didReplace)
            THEN
                newname := newname || CharChemName;  --added this
                offset := offset + 1;
            END IF;
        END IF;
    END LOOP;
    newname := CleanNormalized(newname);

    RETURN newname;
END;
/
