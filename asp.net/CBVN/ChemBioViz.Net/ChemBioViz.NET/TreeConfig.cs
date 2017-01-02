using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

using Infragistics.Win.UltraWinTree;

using FormDBLib;
using ChemBioViz.NET.Utilities;
using System.Collections.Specialized;
using CBVUtilities;
using System.Xml.XPath;

namespace ChemBioViz.NET
{
    public class TreeConfig
    {
        #region Variables
        // Generate keys for tree nodes. UI tree demands unique keys for the entire set of nodes.
        private KeyGenerator m_kGenerator;
        private TreeNode m_mainTree;
        // Reserved words for defined nodes
        private StringCollection m_rWords;
        #endregion

        #region Properties
        public TreeNode MainTree
        {
            get { return m_mainTree; }
            set { m_mainTree = value; }
        }
        public TreeNode FirstNode
        {
            get { return (MainTree == null || MainTree.Nodes.Count == 0) ? null : MainTree.Nodes[0] as TreeNode; }
        }
        public KeyGenerator KGenerator
        {
            get { return m_kGenerator; }
        }
        public StringCollection RWords
        {
            get { return m_rWords; }
        }
        #endregion

        #region Constructor
        public TreeConfig()
        {
            m_kGenerator = new KeyGenerator();
            m_mainTree = new TreeNode(m_kGenerator.GetKey(), "MainTree", CBVConstants.NodeType.Folder);
            m_rWords = new StringCollection();
        }
        #endregion

        #region Methods
        /// <summary>
        ///  Check if the node is a folder by its <paramref name="nKey"/>
        /// </summary>
        /// <param name="nKey"></param>
        /// <returns></returns>
        public bool IsFolder(string nodeKey)
        {
            return nodeKey.EndsWith("~");
        }
        /// <summary>
        ///  Loads the tree list from the xml. Deserializes the tree structure just for the selected group.
        /// </summary>
        /// <param name="groupName"></param>
        public virtual void DeserializeTreeFromXML(string groupName, string sXml)
        {
            if (!string.IsNullOrEmpty(sXml))
            {
                XmlDocument xdoc = new XmlDocument();
                //There is a tree structure stored 
                xdoc.LoadXml(sXml);
                XmlNode node = xdoc.DocumentElement;
                if (groupName == CBVConstants.PUBLIC_GROUPNAME || groupName == CBVConstants.PRIVATE_GROUPNAME)
                {
                    node = SortXMLByFormName(xdoc);
                }


                // There is at least a group serialized in the settings file
                if (node.ChildNodes.Count > 0)
                {
                    // Check if the tree is already stored on the list
                    if (m_mainTree.GetNodeFromListByName(groupName) == null)
                    {
                        // Get the xml node for the current group
                        XmlNode mainNode = GetMainXMLNode(groupName, node);
                        // If there are forms for this group
                        if (mainNode != null)
                        {
                            // Create temp keys
                            TreeNode mainTreeNode = new TreeNode(KGenerator.GetKey(), mainNode.Attributes["name"].Value, CBVConstants.NodeType.Folder);
                            if (mainNode.Attributes["comments"] != null)
                                mainTreeNode.Comments = mainNode.Attributes["comments"].Value;
                            // Add its children
                            mainTreeNode.Nodes = DeserializeTreeFromXML(groupName, mainNode, new List<ITreeNode>());
                            m_mainTree.Nodes.Add(mainTreeNode);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sort the TreeStructure xml by form name
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public XmlNode SortXMLByFormName(XmlDocument doc)
        {
            XmlDocument XmlDocSorted = new XmlDocument();
            XmlNode root = doc.DocumentElement;
            XmlDocSorted.LoadXml(root.CloneNode(false).OuterXml);
            XmlNode publicForms = root.SelectSingleNode(string.Format(@"Folder[@name='{0}']", CBVConstants.PUBLIC_GROUPNAME));
            XmlNode publicFormsNew = null;
            if (publicForms != null)
                publicFormsNew = XmlDocSorted.ImportNode(publicForms, false);
            XmlNode privateForms = root.SelectSingleNode(string.Format(@"Folder[@name='{0}']", CBVConstants.PRIVATE_GROUPNAME));
            XmlNode privateFormsNew = null;
            if (privateForms != null)
                privateFormsNew = XmlDocSorted.ImportNode(privateForms, false);
            XPathNavigator nav = doc.CreateNavigator();
            //Coverity Bug Fix CID  10883 (Local Analysis)
            if (nav != null)
            {
                XPathExpression exp = nav.Compile(string.Format(@"//TreeStructure/Folder[@name='{0}']/Form", CBVConstants.PUBLIC_GROUPNAME));
                XPathExpression exp1 = nav.Compile(string.Format(@"//TreeStructure/Folder[@name='{0}']/Folder", CBVConstants.PUBLIC_GROUPNAME));
                exp.AddSort("@name", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
                //sort the public forms by name
                XPathNodeIterator nodeIterator = nav.Select(exp);
                // Public Forms Iteration
                while (nodeIterator.MoveNext())
                {
                    if (nodeIterator.Current is IHasXmlNode)
                    {
                        XmlNode formNode = ((IHasXmlNode)nodeIterator.Current).GetNode();
                        publicFormsNew.AppendChild(XmlDocSorted.ImportNode(formNode, true));
                    }
                }
                exp1.AddSort("@name", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
                XPathNodeIterator nodeIterator1 = nav.Select(exp1);
                // Folders in Public Forms Iteration
                while (nodeIterator1.MoveNext())
                {
                    if (nodeIterator1.Current is IHasXmlNode)
                    {
                        XmlNode formNode1 = ((IHasXmlNode)nodeIterator1.Current).GetNode();
                        publicFormsNew.AppendChild(XmlDocSorted.ImportNode(formNode1, true));
                    }
                }
                if (publicFormsNew != null)
                    XmlDocSorted.DocumentElement.AppendChild(publicFormsNew);
                exp = nav.Compile(string.Format(@"//TreeStructure/Folder[@name='{0}']/Form", CBVConstants.PRIVATE_GROUPNAME));
                exp1 = nav.Compile(string.Format(@"//TreeStructure/Folder[@name='{0}']/Folder", CBVConstants.PRIVATE_GROUPNAME));
                exp.AddSort("@name", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
                //sort the private forms by name
                nodeIterator = nav.Select(exp);
                // Private Forms Iteration
                while (nodeIterator.MoveNext())
                {
                    if (nodeIterator.Current is IHasXmlNode)
                    {
                        XmlNode formNode = ((IHasXmlNode)nodeIterator.Current).GetNode();
                        privateFormsNew.AppendChild(XmlDocSorted.ImportNode(formNode, true));
                    }
                }
                exp1.AddSort("@name", XmlSortOrder.Ascending, XmlCaseOrder.None, "", XmlDataType.Text);
                nodeIterator1 = nav.Select(exp1);
                // Folders in Private Forms Iteration
                while (nodeIterator1.MoveNext())
                {
                    if (nodeIterator1.Current is IHasXmlNode)
                    {
                        XmlNode formNode1 = ((IHasXmlNode)nodeIterator1.Current).GetNode();
                        privateFormsNew.AppendChild(XmlDocSorted.ImportNode(formNode1, true));
                    }
                }
                if (privateFormsNew != null)
                    XmlDocSorted.DocumentElement.AppendChild(privateFormsNew);
            }
            return XmlDocSorted.DocumentElement;
        }

        /// <summary>
        ///  Deserializes the tree structure just for the selected group.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="node"></param>
        /// <param name="subList"></param>
        /// <returns></returns>
        private List<ITreeNode> DeserializeTreeFromXML(string groupName, XmlNode node, List<ITreeNode> subList)
        {
            if (node.ChildNodes.Count > 0)
            {
                ITreeNode newNode = null;
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.Name.Equals(CBVConstants.NodeType.Folder.ToString()))   //<Folder name="Public Forms"> ... </Folder>
                    {
                        newNode = new TreeNode(KGenerator.GetKey(), n.Attributes["name"].Value, CBVConstants.NodeType.Folder);
                        if (n.ChildNodes.Count > 0)
                        {
                            DeserializeTreeFromXML(groupName, n, ((TreeNode)newNode).Nodes);
                        }
                        subList.Add(newNode);
                    }
                    else
                    {
                        if (n.Name.Equals(CBVConstants.NodeType.Form.ToString()))
                            newNode = new TreeLeaf(KGenerator.GetKey(), n.Attributes["name"].Value, CBVConstants.NodeType.Form);
                        else if (n.Name.Equals(CBVConstants.NodeType.Query.ToString()))
                            newNode = new TreeLeaf(KGenerator.GetKey(), n.Attributes["name"].Value, CBVConstants.NodeType.Query);
                        else if (n.Name.Equals(CBVConstants.NodeType.MergedQuery.ToString()))
                            newNode = new TreeLeaf(KGenerator.GetKey(), n.Attributes["name"].Value, CBVConstants.NodeType.MergedQuery);

                        if (newNode != null)
                        {
                            if (n.Attributes["id"] != null)
                                ((TreeLeaf)newNode).Id = int.Parse(n.Attributes["id"].Value);
                            if (n.Attributes["comments"] != null)
                                ((TreeLeaf)newNode).Comments = n.Attributes["comments"].Value;
                            if (n.Attributes["parent_id"] != null)
                                ((TreeLeaf)newNode).ParentId = CBVUtil.StrToInt(n.Attributes["parent_id"].Value);

                            subList.Add(newNode);
                        }
                    }

                }
            }
            return subList;
        }
        /// <summary>
        ///  Gets the main node which contains a complete tree.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private XmlNode GetMainXMLNode(string groupName, XmlNode node)
        {
            return node.SelectSingleNode("Folder[@name='" + groupName + "']");
        }
        //---------------------------------------------------------------------
        private bool IsChildQuery(TreeLeaf node, QueryCollection queries)
        {
            if (queries == null || node == null) return false;
            Query q = queries.FindByID(node.Id);
            return q != null && q.ParentQuery != null && q.ParentQueryID > 0;
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Verifies that all nodes on the db are on the list. 
        ///  If not, add them.
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="dbNodeNames"></param>
        public TreeNode VerifyDBAgainstListNodes(string groupName, Dictionary<int, string> genObjects,
            QueryCollection queries)
        {
            TreeNode tree = (TreeNode)m_mainTree.GetNodeFromListByName(groupName);
            if (genObjects.Count > 0)
            {
                // tree structure was not mapped
                if (tree == null)
                    tree = CreateRoot(groupName);

                foreach (KeyValuePair<int, string> gObj in genObjects)
                {
                    string nName = gObj.Value;
                    // search for the node inside the group list 
                    if (tree.GetLeafFromListById(gObj.Key) == null)
                    {
                        CBVConstants.NodeType nType;
                        if (groupName.Equals(CBVConstants.QUERIES_GROUPNAME))
                        {
                            if (nName.EndsWith(CBVConstants.TREE_QUERY_MERGED_SUFFIX))
                            {
                                nName = gObj.Value.Remove(gObj.Value.Length - CBVConstants.TREE_QUERY_MERGED_SUFFIX.Length);
                                nType = CBVConstants.NodeType.MergedQuery;
                            }
                            else
                                nType = CBVConstants.NodeType.Query;
                        }
                        else
                            nType = CBVConstants.NodeType.Form;

                        ITreeNode node = new TreeLeaf(m_kGenerator.GetKey(), nName, nType);
                        if (node is TreeLeaf)
                            ((TreeLeaf)node).Id = gObj.Key;
                        // Add non mapped nodes straight to the root.
                        // ! do this only if query is not a child of another
                        bool bIsChildQuery = IsChildQuery(node as TreeLeaf, queries);
                        if (!bIsChildQuery)
                            tree.Add(node);
                    }
                    else
                    {
                        if (gObj.Value.EndsWith(CBVConstants.TREE_QUERY_MERGED_SUFFIX))
                            nName = gObj.Value.Remove(gObj.Value.Length - CBVConstants.TREE_QUERY_MERGED_SUFFIX.Length);

                        // Exist on the DB but with other name

                        // if (!tree.GetLeafFromListById(gObj.Key).Name.Equals(nName))  // crash if GLFLBI returns null
                        //     tree.GetLeafFromListById(gObj.Key).Name = nName;
                        ITreeNode tnode = tree.GetLeafFromListById(gObj.Key);
                        if (tnode == null) continue;
                        if (!tnode.Name.Equals(nName))
                            tnode.Name = nName;
                    }
                }
                if (m_mainTree.IndexOf(groupName) != -1)
                {
                    // update group tree
                    m_mainTree.Nodes[m_mainTree.IndexOf(groupName)] = tree;
                }
                else
                    m_mainTree.Nodes.Add(tree);
            }

            return tree;
        }
        /// <summary>
        ///  Verify that all nodes exist on the db (as forms or queries)
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="dbNodeNames"></param>
        public void VerifyListAgainstDBNodes(string groupName, Dictionary<int, string> genObjects)
        {
            TreeNode tree = (TreeNode)m_mainTree.GetNodeFromListByName(groupName);
            //Coverity Bug Fix CID :13104 
            if (tree != null)
                tree.VerifyListAgainstDBNodes(genObjects);
        }

        public TreeNode CreateRoot(string groupName)
        {
            TreeNode root = (TreeNode)m_mainTree.GetNodeFromListByName(groupName);
            if (root == null)
            {
                root = new TreeNode(m_kGenerator.GetKey(), groupName, CBVConstants.NodeType.Folder);
                m_mainTree.Add(root);
            }
            return root;
        }
        /// <summary>
        ///  Adds new node to the tree structure
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="nKey"></param>
        /// <param name="nodeName"></param>
        /// <param name="groupName"></param>
        public void AddNewNodeToList(string fullPath, ITreeNode newNode, string groupName)
        {
            string[] path = fullPath.Split(new string[] { CBVConstants.TREE_PATH_SEPARATOR }, StringSplitOptions.None);
            m_mainTree.AddNewNodeToList(path, newNode, groupName);
        }

        private int DepthBelowFolder(TreeConfig treeConf, ITreeNode newNode)
        {
            ITreeNode node = newNode;
            int nDepth = 0;
            while (node != null && node is TreeLeaf && (node as TreeLeaf).ParentId != 0)
            {
                ++nDepth;
                node = treeConf.MainTree.GetLeafFromListById((node as TreeLeaf).ParentId);
            }
            return nDepth;
        }

        public void AddNewChildLeafToList(TreeConfig treeConf,
            UltraTreeNode parentUINode, UltraTreeNode newUINode, ITreeNode newNode, string groupName)
        {
            // puts newNode one level up in the path, at same level as parent (leaf)
            String fullPath = newUINode.FullPath;
            string[] path = fullPath.Split(new string[] { CBVConstants.TREE_PATH_SEPARATOR }, StringSplitOptions.None);

            int nDepth = DepthBelowFolder(treeConf, newNode);
            // remove this many levels from array

            int nTokens = path.Count(), iToken = 0, iLast = nTokens - 1;
            if (nTokens > (nDepth + 1))
            {
                string[] newpath = new string[nTokens - nDepth];
                for (int i = 0; i < nTokens; ++i)
                {
                    bool bSkip = (i >= (iLast - nDepth)) && (i < iLast);
                    if (!bSkip)
                        newpath[iToken++] = path[i];
                }
                m_mainTree.AddNewNodeToList(newpath, newNode, groupName);
            }
            else
            {
                m_mainTree.AddNewNodeToList(path, newNode, groupName);
            }
        }
        /// <summary>
        ///  Serialize the tree structure from the list to xml according to <paramref name="groupName"/>
        /// </summary>
        /// <param name="groupName"></param>
        public string SerializeTreeConfig(string groupName, string sXml)
        {
            XmlDocument xdoc = new XmlDocument();
            //there is a tree structure stored
            if (!string.IsNullOrEmpty(sXml))
            {
                xdoc.LoadXml(sXml);
                foreach (ITreeNode n in m_mainTree.Nodes)
                {
                    if (n.Name.Equals(groupName))
                    {
                        List<ITreeNode> restrictedList = new List<ITreeNode>();
                        restrictedList.Add(n);
                        // Get just the group tag
                        XmlElement root = (XmlElement)xdoc.SelectSingleNode(CBVConstants.TREE_STRUCTURE);

                        if (root != null)
                        {
                            //Backwards compatibility
                            if (root.SelectSingleNode("Folder[@name='" + CBVConstants.OLD_USERFORMS_GROUPNAME + "']") != null)
                                root.RemoveChild(root.SelectSingleNode("Folder[@name='" + CBVConstants.OLD_USERFORMS_GROUPNAME + "']"));

                            // Replace old sctructure and write the new one based on the list
                            if (root.SelectSingleNode("Folder[@name='" + groupName + "']") != null)
                                root.RemoveChild(root.SelectSingleNode("Folder[@name='" + groupName + "']"));
                            CreateXElements(xdoc, root, restrictedList);
                        }
                    }
                }
            }
            else
            {
                //the xml is empty
                CreateMainNode(xdoc, groupName);
            }
            return xdoc.OuterXml;
        }
        /// <summary>
        ///  Creates PublicForm or PrivateForm nodes on the xml
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="treeGroup"></param>
        private void CreateMainNode(XmlDocument xdoc, string groupName)
        {
            XmlElement root = xdoc.CreateElement(CBVConstants.TREE_STRUCTURE);

            List<ITreeNode> restrictedList = new List<ITreeNode>();
            restrictedList.Add(m_mainTree.GetNodeFromListByName(groupName));

            // If the list already has elements, then, add these too
            CreateXElements(xdoc, root, restrictedList);
            xdoc.AppendChild(root);
        }
        /// <summary>
        ///  Serialize each node in the list to an xml node
        /// </summary>
        /// <param name="xdoc"></param>
        /// <param name="root"></param>
        /// <param name="mainNode"></param>
        private void CreateXElements(XmlDocument xdoc, XmlElement root, List<ITreeNode> mainNode)
        {
            foreach (ITreeNode node in mainNode)
            {
                if (node == null) continue;

                XmlElement element = xdoc.CreateElement(node.Type.ToString());
                element.SetAttribute("name", node.Name);

                if (node is TreeNode)
                {
                    if (((TreeNode)node).Nodes.Count > 0)
                        CreateXElements(xdoc, element, ((TreeNode)node).Nodes);
                }
                else
                {
                    if (node is TreeLeaf && (node as TreeLeaf).Id == 0)
                        continue;
                    element.SetAttribute("id", ((TreeLeaf)node).Id.ToString());
                    if (((TreeLeaf)node).ParentId != 0)
                        element.SetAttribute("parent_id", ((TreeLeaf)node).ParentId.ToString());
                }
                element.SetAttribute("comments", node.Comments);

                if (!node.Name.EndsWith("*"))
                {
                    if (root == null)
                        xdoc.AppendChild(element);
                    else
                        root.AppendChild(element);
                }
            }
        }
        /// <summary>
        ///  Reposition node
        /// </summary>
        /// <param name="sourceUINode"></param>
        /// <param name="destNode"></param>
        /// <param name="groupName"></param>
        public bool MoveNodeInList(string sourceNode, string destNode)
        {
            bool nodeMoved = false;
            // Get tree from the group
            ITreeNode sNode = m_mainTree.GetNodeFromListByKey(sourceNode);
            ITreeNode dNode = m_mainTree.GetNodeFromListByKey(destNode);
            // Destination node could only be a folder
            if (dNode is TreeNode && sNode != null && dNode != null)
            {
                m_mainTree.RemoveNodeFromList(sNode.Key);
                m_mainTree.MoveNodeInList(sNode, dNode);
                nodeMoved = true;
            }
            return nodeMoved;
        }
        /// <summary>
        ///  This gets the structure from the UI tree and store it on the tree list
        ///  This method is used when the list was not updated when there were changes in the UI tree.
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="nodes"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public List<ITreeNode> PopulateListFromUI(List<ITreeNode> tree, TreeNodesCollection nodes, string groupName)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].HasNodes || nodes[i].Key.EndsWith("~"))
                {
                    if (nodes[i].HasNodes)
                    {
                        List<ITreeNode> subNodes = new List<ITreeNode>();
                        PopulateListFromUI(subNodes, nodes[i].Nodes, groupName);
                        TreeNode tnode = new TreeNode(KGenerator.GetKey(), nodes[i].Text, CBVConstants.NodeType.Folder, subNodes);
                        tree.Add(tnode);
                    }
                    else
                        tree.Add(new TreeNode(KGenerator.GetKey(), nodes[i].Text, CBVConstants.NodeType.Folder));
                }
                else
                {
                    if (groupName.Equals(CBVConstants.PUBLIC_GROUPNAME) || groupName.Equals(CBVConstants.PRIVATE_GROUPNAME))
                        tree.Add(new TreeLeaf(KGenerator.GetKey(), nodes[i].Text, CBVConstants.NodeType.Form));
                    else
                        tree.Add(new TreeLeaf(KGenerator.GetKey(), nodes[i].Text, CBVConstants.NodeType.Query));
                }
            }
            return tree;
        }

        public TreeNode CreateFolder(string name)
        {
            return new TreeNode(this.KGenerator.GetKey(), name, CBVConstants.NodeType.Folder);
        }
        /// <summary>
        ///  Add <paramref name="newUINode"/> to <paramref name="sourceUINode"/>
        /// </summary>
        /// <param name="sourceUINode"></param>
        /// <param name="newUINode"></param>
        /// <param name="groupName"></param>
        public void AddNode(UltraTreeNode sourceUINode, UltraTreeNode newUINode, ITreeNode newNode, string groupName)
        {
            if (sourceUINode != null && newUINode != null && newNode != null)
            {
                // Add new node to UI tree
                AddUINode(sourceUINode, newUINode);
                // Add new node to the structure
                AddNewNodeToList(newUINode.FullPath, newNode, groupName);
            }
        }

        public void AddUINode(UltraTreeNode sourceNode, UltraTreeNode newNode)
        {
            // Add new node to UI tree
            sourceNode.Nodes.Add(newNode);
            sourceNode.ExpandAll(ExpandAllType.OnlyNodesWithChildren);
        }

        public void RemoveNode(UltraTreeNode childNode, UltraTreeNode parentNode)
        {
            if (childNode != null && parentNode != null)
            {
                // Remove node from the structure
                m_mainTree.RemoveNodeFromList(childNode.Key);
                // Remove node from the UI tree
                RemoveUINode(childNode, parentNode);
            }
        }

        private void RemoveUINode(UltraTreeNode childNode, UltraTreeNode parentNode)
        {
            // Remove node from the UI tree
            parentNode.Nodes.Remove(childNode);
            parentNode.ExpandAll(ExpandAllType.OnlyNodesWithChildren);
        }

        public void UpdateNode(string nodeKey, string newName, string comments)
        {
            ITreeNode node = m_mainTree.GetNodeFromListByKey(nodeKey);
            //Coverity Bug Fix : CID 13103 
            if (node != null)
            {
                if (!String.IsNullOrEmpty(newName) && newName.CompareTo(node.Name) != 0)
                    node.Name = newName;
                if (!String.IsNullOrEmpty(comments))
                    node.Comments = comments;
            }
        }
        /// <summary>
        ///  Check if there is other node with the same name at the same level.
        /// </summary>
        /// <param name="childName"></param>
        /// <param name="key"></param>
        /// <param name="isLeaf"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool ExistSiblingWithSameName(string childName, string key, bool isLeaf, UltraTreeNode parent)
        {
            bool result = false;
            foreach (UltraTreeNode n in parent.Nodes)
            {
                if (n.Text.Equals(childName, StringComparison.CurrentCultureIgnoreCase) && !key.Equals(n.Key))
                {
                    // verify if exist a sibling  
                    // discard if different type (folder vs form)
                    if (isLeaf && !IsFolder(n.Key) || !isLeaf && IsFolder(n.Key))
                    {
                        result = true;
                        break;
                    }

                }
            }
            return result;
        }
        /// <summary>
        ///  Check if <paramref name="word"/> is a reserved word
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsRWord(string word)
        {
            bool result = false;
            // Reserved words are stored with lower case
            //Coverity Bug Fix CID 12956 
            if (!string.IsNullOrEmpty(word) && m_rWords.Count > 0)
                result = m_rWords.Contains(word.ToLower());
            return result;
        }
        /// <summary>
        ///  Add reserve words for Forms tree
        /// </summary>
        public void AddReserveWords(string[] words)
        {
            // Make sure that every reserved word has lower case
            foreach (string word in words)
            {
                RWords.Add(word.ToLower());
            }
        }

        /// <summary>
        ///  Creates the UI tree based on the given tree structure
        /// </summary>
        /// <param name="bPublic"></param>
        /// <param name="tStructure"></param>
        /// <param name="tree"></param>
        /// <returns></returns>
        public UltraTree BuildUITree(string groupName, TreeNode tStructure, UltraTree tree)
        {
            UltraTreeNode root = new UltraTreeNode(tStructure.GetFolderKey(tStructure.Key), tStructure.Name);
            root.LeftImages.Add(ChemBioViz.NET.Properties.Resources.Folder_Yellow);
            tree.Nodes.Add(root);
            BuildUITree(groupName, tStructure, root, tree);
            root.ExpandAll(ExpandAllType.OnlyNodesWithChildren);
            return tree;
        }

        /// <summary>
        ///  Creates the UI tree based on the given tree structure
        /// </summary>
        /// <param name="bPublic"></param>
        /// <param name="tStructure"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private void BuildUITree(string groupName, TreeNode tStructure, UltraTreeNode root, UltraTree tree)
        {
            // Populate the tree
            foreach (ITreeNode node in tStructure.Nodes)
            {
                UltraTreeNode uNode = CreateUINode(node, groupName);
                if (node is TreeNode && ((TreeNode)node).Nodes.Count > 0)
                {
                    // Folder node
                    BuildUITree(groupName, (TreeNode)node, uNode, tree);
                }
                root.Nodes.Add(uNode);
            }
        }
        //---------------------------------------------------------------------
        public UltraTree BuildUITreeEx(string groupName, TreeNode tStructure, UltraTree tree)
        {
            // this version for query tree, having child leaves
            UltraTreeNode root = new UltraTreeNode(tStructure.GetFolderKey(tStructure.Key), tStructure.Name);
            root.LeftImages.Add(ChemBioViz.NET.Properties.Resources.Folder_Yellow);
            tree.Nodes.Add(root);

            List<ITreeNode> children = new List<ITreeNode>();
            BuildUITreeParents(groupName, tStructure, root, tree, children);
            if (children.Count > 0)
                AddUITreeChildren(groupName, tStructure, root, tree, children);

            root.ExpandAll(ExpandAllType.OnlyNodesWithChildren);
            return tree;
        }
        //---------------------------------------------------------------------
        private void BuildUITreeParents(string groupName, TreeNode tStructure, UltraTreeNode root, UltraTree tree,
                                        List<ITreeNode> children)
        {
            // Create all UI tree nodes except leaves with parents; save those on list
            foreach (ITreeNode node in tStructure.Nodes)
            {
                UltraTreeNode uNode = CreateUINode(node, groupName);
                if (node is TreeNode && ((TreeNode)node).Nodes.Count > 0)
                {
                    BuildUITreeParents(groupName, (TreeNode)node, uNode, tree, children);
                }
                else if (node is TreeLeaf && (node as TreeLeaf).ParentId != 0)
                {
                    children.Add(node);
                    continue;
                }
                if (node is TreeLeaf && (node as TreeLeaf).Id == 0)
                {
                    root.Nodes.Insert(0, uNode);
                }
                else
                {
                    root.Nodes.Add(uNode);
                }
            }
        }
        //---------------------------------------------------------------------
        private static int CompareLeafs(ITreeNode tn1, ITreeNode tn2)
        {
            TreeLeaf leaf1 = tn1 as TreeLeaf, leaf2 = tn2 as TreeLeaf;
            //Coverity Bug Fix CID 13013 
            if (leaf1 != null && leaf2 != null)
            {
                if (leaf1.ParentId < leaf2.ParentId)
                    return -1;
                if (leaf1.ParentId > leaf2.ParentId)
                    return 1;
            }
            return 0;
        }
        //---------------------------------------------------------------------
        private void AddUITreeChildren(string groupName, TreeNode tStructure, UltraTreeNode root, UltraTree tree,
                                        List<ITreeNode> children)
        {
            // sort by parent id, then add leaves in order so parents are sure to be already created
            children.Sort(CompareLeafs);
            foreach (ITreeNode node in children)
            {
                UltraTreeNode uNode = CreateUINode(node, groupName);

                ITreeNode nodeParent = tStructure.GetLeafFromListById((node as TreeLeaf).ParentId);
                UltraTreeNode uNodeParent = (nodeParent == null) ? root : tree.GetNodeByKey(nodeParent.Key);
                Debug.Assert(uNodeParent != null);
                uNodeParent.Nodes.Add(uNode);
            }
        }
        //---------------------------------------------------------------------
        /// <summary>
        ///  Create node in the UI
        /// </summary>
        /// <param name="newUINode"></param>
        /// <param name="bPublic"></param>
        /// <returns></returns>
        public UltraTreeNode CreateUINode(ITreeNode newNode, string groupName)
        {
            UltraTreeNode node = null;
            System.Drawing.Bitmap image = null;
            StringBuilder nText = new StringBuilder(newNode.Name);
            if (newNode is TreeNode)
            {
                node = new UltraTreeNode(((TreeNode)newNode).GetFolderKey(newNode.Key), newNode.Name);
                image = ChemBioViz.NET.Properties.Resources.Folder_Yellow;
                node.LeftImages.Add(image);
            }
            else
            {
                if (newNode.Type.ToString().Equals(CBVConstants.NodeType.Form.ToString()))
                {
                    image = groupName.Equals(CBVConstants.PUBLIC_GROUPNAME)
                        ? ChemBioViz.NET.Properties.Resources.FileOrange : ChemBioViz.NET.Properties.Resources.File_User;
                }
                else if (newNode.Type.ToString().Equals(CBVConstants.NodeType.Query.ToString()))
                    image = ChemBioViz.NET.Properties.Resources.File_Right;
                else if (newNode.Type.ToString().Equals(CBVConstants.NodeType.MergedQuery.ToString()))
                    image = ChemBioViz.NET.Properties.Resources.File_Left;
                if (groupName.Equals(CBVConstants.QUERIES_GROUPNAME))
                {
                    nText.Append(": ");
                    nText.Append(newNode.Comments);
                }
                node = new UltraTreeNode(newNode.Key, nText.ToString());
                node.LeftImages.Add(image);

                if (newNode is TreeLeaf)    // JDD .. tag UI node with obj (query) id
                    node.Tag = (newNode as TreeLeaf).Id;
            }
            return node;
        }
        #endregion
    }

    public class FormsTreeConfig : TreeConfig
    {
        #region Constructor
        public FormsTreeConfig()
            : base()
        {
            base.AddReserveWords(new string[] { CBVConstants.TREE_FORMS_ROOT, CBVConstants.PUBLIC_GROUPNAME, CBVConstants.PRIVATE_GROUPNAME });
        }
        #endregion

        #region Methods
        public void DeserializeTreeFromXML(string groupName)
        {
            if (!string.IsNullOrEmpty(PreferencesHelper.PreferencesHelperInstance.TreeConfig))
                base.DeserializeTreeFromXML(groupName, PreferencesHelper.PreferencesHelperInstance.TreeConfig);

            //else
            //{
            //    // There aren't forms on the server or the TreeConfig tag is empty
            //    // Create and write it back to the settings.
            //}
        }

        public TreeLeaf CreateLeaf(string name, int leafID)
        {
            TreeLeaf leaf = new TreeLeaf(this.KGenerator.GetKey(), name, CBVConstants.NodeType.Form);
            leaf.Id = leafID;
            return leaf;
        }
        #endregion
    }

    public class QueriesTreeConfig : TreeConfig
    {
        #region Constructor
        public QueriesTreeConfig()
            : base()
        {
            base.AddReserveWords(new string[] { CBVConstants.TREE_QUERIES_ROOT, CBVConstants.RETRIEVE_ALL });
        }
        #endregion

        #region Methods
        public override void DeserializeTreeFromXML(string groupName, string sXml)
        {
            if (!string.IsNullOrEmpty(sXml))
            {
                XmlDocument xdoc = new XmlDocument();
                //There is a tree structure stored 
                xdoc.LoadXml(sXml);
                XmlNode node = xdoc.DocumentElement;

                // There is at least a group serialized in the settings file
                if (node.ChildNodes.Count > 0)
                    node = node.SelectSingleNode("//queries/TreeStructure");

                // In this case we'll always need to remove the current QTree structure from the list.
                //Coverity Bug Fix CID 13102 
                ITreeNode treeNodeToRemove = MainTree.GetNodeFromListByName(CBVConstants.QUERIES_GROUPNAME);
                if (MainTree != null && treeNodeToRemove != null)
                    MainTree.RemoveNodeFromList(treeNodeToRemove.Key);
                base.DeserializeTreeFromXML(groupName, (node == null ? string.Empty : node.OuterXml));
            }
            //else ??
        }

        public Dictionary<int, string> GetIdValuePairList(QueryCollection qCollection)
        {
            Dictionary<int, string> gObjCollection = new Dictionary<int, string>();
            foreach (Query q in qCollection)
            {
                string name = q.IsMergeQuery ? string.Concat(q.Name, "|MergedQuery") : q.Name;

                // new 1/11: add * here and not only in CreateLeaf
                if (q.IsFlagged(Query.QueryFlag.kfDiscard) && !CBVUtil.EndsWith(name, "*"))
                    name += "*";

                gObjCollection.Add(q.ID, name);
            }
            return gObjCollection;
        }

        /// <summary>
        ///  Extract the query name when it's inside a "name+description" text 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string ExtractQueryName(string text)
        {
            String queryName = string.Empty;
            if (!String.IsNullOrEmpty(text) && text.Contains(":"))
            {
                queryName = text.Substring(0, text.IndexOf(':')).Trim();
                if (queryName.EndsWith("*"))
                    queryName = queryName.Substring(0, queryName.Length - 1);
            }
            return queryName;
        }

        public void AddComments(QueryCollection qCollection, ITreeNode node)
        {
            // Please don't confuse ID with Key. ID if for the Query ID in the DB and Key is for the UI tree node.
            if (((TreeNode)node).Nodes.Count > 0)
            {
                foreach (ITreeNode n in ((TreeNode)node).Nodes)
                {
                    if (n is TreeLeaf)
                    {
                        n.Comments = (from k in qCollection
                                      where (k.ID == ((TreeLeaf)n).Id)
                                      select k).FirstOrDefault<Query>().GetQueryDesc();
                    }
                    else
                    {
                        if (((TreeNode)n).Nodes.Count > 0)
                            AddComments(qCollection, n);
                    }
                }
            }
        }

        public TreeLeaf CreateLeaf(QueryCollection qCollection, int leafID)
        {
            Query q = qCollection.FindByID(leafID);
            string name = q.Name;
            if (q.IsFlagged(Query.QueryFlag.kfDiscard) && !CBVUtil.EndsWith(name, "*"))
                name += "*";
            TreeLeaf leaf = new TreeLeaf(this.KGenerator.GetKey(), name,
                q.IsMergeQuery ? CBVConstants.NodeType.MergedQuery : CBVConstants.NodeType.Query);
            leaf.Comments = q.GetQueryDesc();
            leaf.Id = leafID;
            return leaf;
        }

        /// <summary>
        ///  Check if there is at least one non-kept query
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public bool HasNonKeptQueries(ITreeNode node)
        {
            bool result = false;
            if (node is TreeNode)
            {
                foreach (ITreeNode n in ((TreeNode)node).Nodes)
                {
                    if (n is TreeNode)
                    {
                        if (((TreeNode)n).Nodes.Count > 0)
                            result = HasNonKeptQueries(n);
                    }
                    else
                    {
                        if (((TreeLeaf)n).Name.EndsWith("*"))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        ///  Check if the node has children and remove them from the form body
        /// </summary>
        /// <param name="node"></param>
        public void RemoveChildQueries(QueryCollection qCollection, ITreeNode node)
        {
            if (node is TreeLeaf)
                RemoveQuery(qCollection, node);
            else
            {
                if (((TreeNode)node).Nodes.Count > 0)
                {
                    foreach (ITreeNode n in ((TreeNode)node).Nodes)
                    {
                        if (n is TreeLeaf)
                            RemoveQuery(qCollection, n);
                        else
                            RemoveChildQueries(qCollection, n);
                    }
                }
            }
        }

        private void RemoveQuery(QueryCollection qCollection, ITreeNode n)
        {
            Query q = qCollection.Find(n.Name);
            if (q != null)
            {
                if (q.IsSaved)
                    q.DeleteSavedHitlist();
                qCollection.Remove(q);
            }
        }
        #endregion
    }
}
