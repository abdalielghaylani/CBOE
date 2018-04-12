using System;
using System.Collections.Generic;
using System.Text;

namespace CambridgeSoft.COE.DataLoader.Calculation.Parser
{
    class Tokenizer
    {
        public enum TokenClass {
            Error,
            Empty,
            Whitespace,
            Name,
            Integer,
            Decimal,
            String,
            Indirect,
            Operator,
            Illegal,
            Char,
        };

        private string strSource = "";
        private int nSrc = 0;
        private TokenClass[] tokenClass = {
/* @ */	TokenClass.Empty,
/* A */	TokenClass.Whitespace,
/* B */	TokenClass.Name,
/* C */	TokenClass.Integer,     // #
/* D */	TokenClass.Decimal,     // #.
/* E */	TokenClass.Decimal,     // #.#
/* F */	TokenClass.Operator,    // .
/* G */	TokenClass.Decimal,     // .#
/* H */	TokenClass.Error,       // "
/* I */	TokenClass.Error,       // "\
/* J */	TokenClass.String,      // "..."
/* K */	TokenClass.Error,       // [
/* L */	TokenClass.Error,       // [...
/* M */	TokenClass.Indirect,    // [...]
/* N */	TokenClass.Operator,    // various single
/* O */	TokenClass.Operator,    // ! < >
/* P */	TokenClass.Operator,    // != <= >=
/* Q */	TokenClass.Error,       // =
/* R */	TokenClass.Operator,    // ==
/* S */	TokenClass.Operator,    // &
/* T */	TokenClass.Operator,    // &&
/* U */	TokenClass.Operator,    // |
/* V */	TokenClass.Operator,    // ||
/* W */	TokenClass.Illegal,     // various
/* X */	TokenClass.Error,       // '
/* Y */	TokenClass.Error,       // '\
/* Z */	TokenClass.Error,       // '.
/* [ */	TokenClass.Char,        // '.'
        };
        private string[] strFSA = {
/*       @ABCDEFGHIJKLMNOPQRSTUVWXYZ[ */
/*   */	"AA@@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* ! */	"O@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* " */	"H@@@@@@@JH@@@@@@@@@@@@@@ZZ@@",
/* # */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* $ */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* % */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* & */	"S@@@@@@@HH@@@@@@@@@T@@@@ZZ@@",
/* ' */	"X@@@@@@@HH@@@@@@@@@@@@@@@Z[@",
/* ( */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* ) */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* * */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* + */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* , */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* - */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* . */	"F@@D@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* / */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* 0 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 1 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 2 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 3 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 4 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 5 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 6 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 7 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 8 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* 9 */	"C@BCEEGGHH@@L@@@@@@@@@@@ZZ@@",
/* : */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* ; */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* < */	"O@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* = */	"Q@@@@@@@HH@@@@@P@R@@@@@@ZZ@@",
/* > */	"O@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* ? */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* @ */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* A */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* B */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* C */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* D */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* E */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* F */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* G */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* H */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* I */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* J */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* K */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* L */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* M */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* N */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* O */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* P */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* Q */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* R */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* S */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* T */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* U */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* V */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* W */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* X */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* Y */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* Z */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* [ */	"K@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* \ */	"W@@@@@@@IH@@@@@@@@@@@@@@YZ@@",
/* ] */	"W@@@@@@@HH@@M@@@@@@@@@@@ZZ@@",
/* ^ */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* _ */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* ` */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* a */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* b */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* c */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* d */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* e */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* f */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* g */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* h */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* i */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* j */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* k */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* l */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* m */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* n */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* o */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* p */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* q */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* r */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* s */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* t */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* u */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* v */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* w */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* x */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* y */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* z */	"B@B@@@@@HH@LL@@@@@@@@@@@ZZ@@",
/* { */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* | */	"U@@@@@@@HH@@@@@@@@@@@V@@ZZ@@",
/* } */	"N@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/* ~ */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
/*  */	"W@@@@@@@HH@@@@@@@@@@@@@@ZZ@@",
        };

        public string Source
        {
            set
            {
                strSource = value;
                nSrc = 0;
                return;
            } // set
        } // Source

        public void ReadToken(ref string rstrToken, ref TokenClass rTokenClass, ref int nStartingPosition)
        {
            nStartingPosition = nSrc;   // Starting position
            int nState = 0;
            int nLft = nSrc;
            while (nSrc < strSource.Length)
            {
                char chSrc = strSource[nSrc++];
                if (chSrc < ' ') chSrc = ' ';
                if (chSrc > '\x7F') chSrc = '\x7F';
                int nNewState = strFSA[chSrc - ' '][nState] - 64;
                //char chState = (char)(nState + 64);
                //char chNewState = (char)(nNewState + 64);
                if (nNewState == 0)
                {
                    nSrc--;
                    break;  // End of token
                }
                nState = nNewState;
            } // while (nSrc < strSource.Length)
            rstrToken = strSource.Substring(nLft, nSrc - nLft); // Token
            rTokenClass = tokenClass[nState];   // TokenClass
            return;
        } // ReadToken()
    } // class Tokenizer
}
