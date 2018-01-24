using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace CambridgeSoft.COE.Framework.Common.SqlGenerator.Utils
{
    public enum WhereClauseTypes
    {
        TextCriteria,
        DateCriteria,
        NumericalCriteria,
    }

    public class XmlTranslation
    {
        public static string GetOperation(XmlNode operationNode, WhereClauseTypes whereClauseType)
        {
            string value = operationNode.InnerText.Trim();

            if (!string.IsNullOrEmpty(value))
            {
                if (value.ToLower() == "null" || value.ToLower() == "not null")
                {
                    return "equal";
                }

                if (value.Trim().StartsWith("*") || value.Trim().EndsWith("*") || value.Contains("?") || value.Contains("_") || value.Trim().StartsWith("%") || value.Trim().EndsWith("%"))
                {
                    if (operationNode.Attributes["operator"] != null && !string.IsNullOrEmpty(operationNode.Attributes["operator"].Value) &&
                        (operationNode.Attributes["operator"].Value.ToLower().Contains("contains") || operationNode.Attributes["operator"].Value.ToLower().Contains("like")))
                    {
                        return operationNode.Attributes["operator"].Value.ToLower();
                    }
                    else
                        return "like";
                }

                if ((value[0] == '\'' && value[value.Length - 1] == '\'' && value.Length > 1) ||
                    value[0] == '\"' && value[value.Length - 1] == '\"' && value.Length > 1)
                {
                    operationNode.InnerText = value.Substring(1, value.Length - 2);
                    return "equal";
                }

                if (value[0] == '<')
                {
                    if (value.Length > 1 && value[1] == '>')
                    {
                        operationNode.InnerText = value.Substring(2);
                        return "notequal";
                    }
                    else if (value.Length > 1 && value[1] == '=')
                    {
                        operationNode.InnerText = value.Substring(2);
                        return "lte";
                    }
                    else
                    {
                        operationNode.InnerText = value.Substring(1);
                        return "lt";
                    }
                }
                else if (value[0] == '>')
                {
                    if (value.Length > 1 && value[1] == '=')
                    {
                        operationNode.InnerText = value.Substring(2);
                        return "gte";
                    }
                    else
                    {
                        operationNode.InnerText = value.Substring(1);
                        return "gt";
                    }
                }
                else if (value[0] == '=')
                {
                    operationNode.InnerText = value.Substring(1);
                    return "equal";
                }
            }

            if (operationNode.Attributes["operator"] == null ||
                string.IsNullOrEmpty(operationNode.Attributes["operator"].Value))
                if (whereClauseType == WhereClauseTypes.TextCriteria)
                    return "like";
                else
                    return "equal";

            return operationNode.Attributes["operator"].Value.ToLower();
        }

        public static XmlDocument Transform(XmlDocument searchCriteriaXMLDocument)
        {
            XmlDocument resultDocument = (XmlDocument)searchCriteriaXMLDocument.Clone();
            XmlNamespaceManager nspManager = new XmlNamespaceManager(resultDocument.NameTable);

            nspManager.AddNamespace("coe", resultDocument.DocumentElement.NamespaceURI);

            XmlNodeList criteriaNodeList = resultDocument.SelectNodes("//coe:searchCriteriaItem", nspManager);

            for (int currentNodeIndex = 0; currentNodeIndex < criteriaNodeList.Count; currentNodeIndex++)
            {
                XmlNode currentNode = criteriaNodeList[currentNodeIndex];

                if (currentNode.FirstChild.Name.ToLower() == "textcriteria" ||
                currentNode.FirstChild.Name.ToLower() == "numericalcriteria" ||
                currentNode.FirstChild.Name.ToLower() == "datecriteria")
                {
                    List<XmlNode> transformedList = ParseCriteria(currentNode);

                    ReplaceNodeByList(currentNode, transformedList);
                }
            }

            return resultDocument;
        }

        private static void ReplaceNodeByList(XmlNode originalNode, List<XmlNode> replacementList)
        {
            /*//If using this more code-readable loop, the comment ends up below the translation.
             * foreach (XmlNode currentReplacement in replacementList)
                originalNode.ParentNode.InsertBefore(TransformCriteria(currentReplacement), originalNode);
             */

            for (int currentIndex = replacementList.Count - 1; currentIndex >= 0; currentIndex--)
                originalNode.ParentNode.InsertAfter(replacementList[currentIndex], originalNode);

#if DEBUG
            XmlComment comment = originalNode.OwnerDocument.CreateComment(originalNode.OuterXml);
            originalNode.ParentNode.ReplaceChild(comment, originalNode);
#else
            originalNode.ParentNode.RemoveChild(originalNode);
#endif

        }

        public class Tokenizer
        {
            #region variables
            private string[] separators;
            private string[] operations;
            private string[] tokens;
            private int currentIndex;
            private string text;
            #endregion

            #region Properties

            public string Text
            {

                get
                {
                    return text;
                }
                set
                {
                    this.text = value;
                    this.tokens = Split(text, Separators);
                    this.currentIndex = 0;
                }

            }
            public string[] Separators
            {
                get
                {
                    return this.separators;
                }
                set
                {
                    if (value == null)
                        this.separators = new string[] { };
                    else
                        this.separators = value;
                }
            }

            public string[] Operations
            {
                get
                {
                    return this.operations;
                }
                set
                {
                    if (value == null)
                        this.operations = new string[] { };
                    else
                        this.operations = value;
                }
            }
            #endregion

            #region Methods
            public Tokenizer(string[] separators, string[] operations)
            {
                this.Separators = separators;
                this.Operations = operations;
            }

            /// <summary>
            /// Assumes operations = separators
            /// </summary>
            /// <param name="sentence"></param>
            /// <param name="separators"></param>
            public Tokenizer(string sentence, string[] separators)
                : this(sentence, separators, separators)
            {
            }

            public Tokenizer(string sentence, string[] separators, string[] operations)
                : this(separators, operations)
            {
                this.Text = sentence;
            }

            public string ViewNextToken()
            {
                int originalIndex = this.currentIndex;

                string token = GetNextToken();

                this.currentIndex = originalIndex;

                return token;
            }

            public string GetNextToken()
            {
                string currentToken = string.Empty;

                for (int index = currentIndex; index < tokens.Length; index++, currentIndex++)
                {
                    if (ArrayContains(operations, tokens[index].ToLower()))
                    {
                        if (currentToken == string.Empty)
                        {
                            currentIndex++;
                            return tokens[index];
                        }
                        else
                        {
                            return currentToken;
                        }
                    }
                    else
                        currentToken += tokens[index];
                }
                return currentToken;
            }

            public static string[] Split(string originalString, string[] separators)
            {
                List<string> results = new List<string>();

                int currentPosition = 0;

                bool matchedAny = true;

                if (!string.IsNullOrEmpty(originalString.Trim()))
                {
                    do
                    {
                        matchedAny = false;

                        string nextToken = string.Empty;
                        int matchingPosition = int.MaxValue;

                        foreach (string token in separators)
                        {
                            if (0 < originalString.IndexOf(token, currentPosition) &&
                                originalString.IndexOf(token, currentPosition) < matchingPosition)
                            {
                                matchingPosition = originalString.IndexOf(token, currentPosition);
                                nextToken = token;
                                matchedAny = true;
                            }
                        }

                        if (matchedAny)
                        {
                            if (originalString.Substring(currentPosition, matchingPosition - currentPosition).Trim() != string.Empty)
                                results.Add(originalString.Substring(currentPosition, matchingPosition - currentPosition));
                            results.Add(nextToken);
                            currentPosition = matchingPosition + nextToken.Length;
                        }
                    } while (matchedAny);
                }
                if (originalString.Substring(currentPosition, originalString.Length - currentPosition).Trim() != string.Empty)
                    results.Add(originalString.Substring(currentPosition, originalString.Length - currentPosition));

                if (results.Count == 0)
                    results.Add(string.Empty);

                return results.ToArray();
            }
            private static bool ArrayContains(string[] tokenArray, string token)
            {
                foreach (string currentToken in tokenArray)
                    if (currentToken == token)
                        return true;

                return false;
            }

            public override string ToString()
            {
                return this.ToString(true);
            }

            public string ToString(bool includeMarkers)
            {
                StringBuilder builder = new StringBuilder();

                for (int currentToken = 0; currentToken < this.tokens.Length; currentToken++)
                {
                    if (includeMarkers)
                    {
                        if (currentToken == currentIndex)
                            builder.Append("|");
                        else if (currentToken != 0)
                            builder.Append("·");
                    }

                    builder.Append(tokens[currentToken]);
                }

                return builder.ToString();
            }


            /// <summary>
            /// Reads a string delimited by '
            /// </summary>
            /// <returns></returns>
            public string GetRemainingString()
            {
                string result = string.Empty;
                string token = string.Empty;

                while ((token = GetNextToken()) != string.Empty && token != "'")
                    result += token;

                return result;
            }

            public string GetString()
            {
                string token = GetNextToken();

                if (token == "'")
                    return GetRemainingString();

                return null;
            }

            public string GetRemainingText()
            {
                string result = string.Empty;

                for (int index = currentIndex; index < tokens.Length; index++)
                    result += tokens[index];

                return result;
            }

            Stack<int> _indexStack = null;
            public void PushPosition()
            {
                if (_indexStack == null)
                    _indexStack = new Stack<int>();

                _indexStack.Push(currentIndex);
            }
            public void PopPosition()
            {
                if (_indexStack != null && _indexStack.Count > 0)
                {
                    currentIndex = _indexStack.Pop();
                }
            }

            public void RemoveTopPosition()
            {
                if (_indexStack != null && _indexStack.Count > 0)
                    _indexStack.Pop();
            }
            #endregion
        }


        public static bool ParseAssertion(XmlNode node, Tokenizer tokenizer)
        {
            string token = string.Empty;

            while ((token = tokenizer.GetNextToken()) != string.Empty && token.Trim() == string.Empty) ;

            if (token.Trim() != string.Empty && token.Trim()[0] == '\'')
                token += tokenizer.GetRemainingString();

            switch (token.ToLower().Trim())
            {
                case "not":
                    GetNodeAttribute(node.FirstChild, "negate").Value = "yes";
                    return ParseAssertion(node, tokenizer);
                    break;

                //case "(":
                //    return true;
                //    break;

                default:
                    node.FirstChild.InnerText = token.Trim();
                    node = TransformCriteria(node);
                    break;
            }
            return true;
        }

        public static List<XmlNode> ParseCriteria(XmlNode criteriaNode)
        {
            Tokenizer tokenizer = new Tokenizer(criteriaNode.FirstChild.InnerText,
                                                                        new string[] { " ", "(", ")", "'" },
                                                                        new string[] { "not", "and", "or", });

            List<XmlNode> resultList = new List<XmlNode>();

            XmlNode currentNode = criteriaNode.Clone();

            string currentToken = string.Empty;
            SearchCriteria.LogicalCriteria logicalCriteria = null;

            ParseAssertion(currentNode, tokenizer);

            do
            {

                //Get the operator
                currentToken = tokenizer.GetNextToken();

                if (currentToken != string.Empty)
                {
                    switch (currentToken.ToLower())
                    {
                        case "and":
                            resultList.Add(currentNode);

                            currentNode = criteriaNode.Clone();
                            ParseAssertion(currentNode, tokenizer);

                            break;
                        case "or":
                            logicalCriteria = new SearchCriteria.LogicalCriteria();
                            logicalCriteria.LogicalOperator = SearchCriteria.COELogicalOperators.Or;

                            XmlNode logicalNode = currentNode.OwnerDocument.ReadNode(XmlReader.Create(new StringReader(logicalCriteria.ToString())));

                            logicalNode.AppendChild(currentNode);

                            currentNode = criteriaNode.Clone();
                            ParseAssertion(currentNode, tokenizer);
                            logicalNode.AppendChild(currentNode);

                            currentNode = logicalNode;

                            break;
                        default:
                            throw new Exception(string.Format("Unexpected token: {0}", currentToken));
                            break;
                    }
                }
                else
                {
                    //if (resultList.Count == 0)
                    resultList.Add(currentNode);
                }
            } while (currentToken != string.Empty);

            return resultList;
        }

        public static XmlNode TransformCriteria(XmlNode criteriaNode)
        {
            XmlNode operationNode = criteriaNode.FirstChild;
            string value = operationNode.InnerText.Trim();

            if (value.ToLower() == "null" || value.ToLower() == "not null")
            {
                GetNodeAttribute(operationNode, "operator").Value = "equal";
                return criteriaNode;
            }

            if (operationNode.Attributes["operator"] == null || string.IsNullOrEmpty(operationNode.Attributes["operator"].Value))
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (value.Contains("*") || value.Contains("?"))
                    {
                        GetNodeAttribute(operationNode, "operator").Value = "like";
                        return criteriaNode;
                    }

                    if (value[0] == '\'' && value[value.Length - 1] == '\'' && value.Length > 1)
                    {
                        operationNode.InnerText = value.Substring(1, value.Length - 2);
                        GetNodeAttribute(operationNode, "operator").Value = "equal";
                        return criteriaNode;
                    }

                    if (value[0] == '<')
                    {
                        if (value.Length > 1 && value[1] == '>')
                        {
                            operationNode.InnerText = value.Substring(2);
                            GetNodeAttribute(operationNode, "operator").Value = "notequal";
                            return criteriaNode;
                        }
                        else if (value.Length > 1 && value[1] == '=')
                        {
                            operationNode.InnerText = value.Substring(2);
                            GetNodeAttribute(operationNode, "operator").Value = "lte";
                            return criteriaNode;
                        }
                        else
                        {
                            operationNode.InnerText = value.Substring(1);
                            GetNodeAttribute(operationNode, "operator").Value = "lt";
                            return criteriaNode;
                        }
                    }
                    else if (value[0] == '>')
                    {
                        if (value.Length > 1 && value[1] == '=')
                        {
                            operationNode.InnerText = value.Substring(2);
                            GetNodeAttribute(operationNode, "operator").Value = "gte";
                            return criteriaNode;
                        }
                        else
                        {
                            operationNode.InnerText = value.Substring(1);
                            GetNodeAttribute(operationNode, "operator").Value = "gt";
                            return criteriaNode;
                        }
                    }
                    else if (value[0] == '=')
                    {
                        operationNode.InnerText = value.Substring(1);
                        GetNodeAttribute(operationNode, "operator").Value = "equal";
                        return criteriaNode;
                    }
                }

                //Default values
                if (operationNode.Name.ToLower() == "textcriteria")
                {
                    GetNodeAttribute(operationNode, "operator").Value = "like";
                    return criteriaNode;
                }
                else
                {
                    GetNodeAttribute(operationNode, "operator").Value = "equal";
                    return criteriaNode;
                }
            }
            return criteriaNode;
        }
        public static XmlAttribute GetNodeAttribute(XmlNode parentNode, string attributeName)
        {
            if (parentNode.Attributes[attributeName] == null)
            {
                XmlAttribute attribute = parentNode.OwnerDocument.CreateAttribute(attributeName);
                parentNode.Attributes.Append(attribute);
            }

            return parentNode.Attributes[attributeName];

        }
    }
}
