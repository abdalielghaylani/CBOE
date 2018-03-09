using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormDBLib;
using System.Xml.Serialization;

namespace ChemBioViz.NET
{
    public class TreeNode : ITreeNode
    {
        #region Variables
        private List<ITreeNode> m_nodes;
        #endregion

        #region Properties
        public string Key { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public CBVConstants.NodeType Type { get; set; }
        public List<ITreeNode> Nodes
        {
            get { return m_nodes; }
            set { m_nodes = value; }
        }
        #endregion

        #region Constructors
        public TreeNode(string key, string name, CBVConstants.NodeType type)
        {
            this.Key = key;
            this.Name = name;
            this.Type = type;
            m_nodes = new List<ITreeNode>();
        }

        public TreeNode(string key, string name, CBVConstants.NodeType type, List<ITreeNode> nodes)
        {
            this.Key = key;
            this.Name = name;
            this.Type = type;
            this.m_nodes = nodes;
        }
        #endregion

        #region Methods
        public void Add(ITreeNode node)
        {
            m_nodes.Add(node);
        }

        public void Remove(ITreeNode node)
        {
            m_nodes.Remove(node);
        }

        /// <summary>
        ///  Create key for a folder node
        /// </summary>
        /// <param name="nodekey"></param>
        /// <returns></returns>
        public string GetFolderKey(string nodeKey)
        {
            return string.Concat(nodeKey, "~");
        }
        /// <summary>
        ///  Get a certain node with <paramref name="nKey"/> from the list
        /// </summary>
        /// <param name="nKey">It's the key assigned to each tree node</param>
        /// <returns></returns>
        public ITreeNode GetNodeFromListByKey(string nodeKey)
        {
            ITreeNode result = null;
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].Key.Equals(nodeKey) || (m_nodes[i] is TreeNode && nodeKey.Equals(GetFolderKey(m_nodes[i].Key))))
                {
                    result = m_nodes[i];
                    break;
                }
                else
                {
                    // search inside the node if it's a folder
                    if (m_nodes[i] is TreeNode && (((TreeNode)m_nodes[i]).Nodes.Count > 0))
                        result = ((TreeNode)m_nodes[i]).GetNodeFromListByKey(nodeKey);
                    if (result != null)
                        break;
                }
            }
            return result;
        }
        /// <summary>
        ///  This method is used to get nodes with reserved words as name
        /// </summary>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public ITreeNode GetNodeFromListByName(string nodeName)
        {
            ITreeNode result = null;
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].Name.Equals(nodeName))
                {
                    result = m_nodes[i];
                    break;
                }
                else
                {
                    // search inside the node if it's a folder
                    if (m_nodes[i] is TreeNode && (((TreeNode)m_nodes[i]).Nodes.Count > 0))
                        result = ((TreeNode)m_nodes[i]).GetNodeFromListByName(nodeName);
                    if (result != null)
                        break;
                }
            }
            return result;
        }

        /// <summary>
        ///  Get the form with the <paramref name="id"/>
        /// </summary>
        /// <param name="id">It's the ID of each form in the DB</param>
        /// <returns></returns>
        public ITreeNode GetLeafFromListById(int id)
        {
            ITreeNode result = null;
            
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i] is TreeLeaf)
                {
                    if (((TreeLeaf)m_nodes[i]).Id == id)
                    {
                        result = (TreeLeaf)m_nodes[i];
                        break;
                    }
                }
                else
                {
                    // search inside the node if it's a folder
                    if (m_nodes[i] is TreeNode && (((TreeNode)m_nodes[i]).Nodes.Count > 0))
                        result = ((TreeNode)m_nodes[i]).GetLeafFromListById(id);
                    if (result != null)
                        break;
                }
            }
            return result;
        }
        /// <summary>
        ///  Get the object ID from the db by the given node key (from the UI tree)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetObjectID(string key)
        {
            int result = -1;
            ITreeNode node = GetNodeFromListByKey(key);
            if (node is TreeLeaf)
            {
                result = ((TreeLeaf)node).Id;
            }
            return result;
        }
        /// <summary>
        ///  Reports the index of the first occurrence of the specified node in the List
        /// </summary>
        /// <param name="nKey"></param>
        /// <returns></returns>
        public int IndexOf (string nodeKey)
        {
            int nodeIndex = -1;
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].Name.Equals(nodeKey) || (m_nodes[i] is TreeNode && nodeKey.Equals(GetFolderKey(m_nodes[i].Key))))
                {
                    nodeIndex = i;
                    break;
                }
                else
                {
                    // search inside the node if it's a folder
                    if (m_nodes[i] is TreeNode && (((TreeNode)m_nodes[i]).Nodes.Count > 0))
                        nodeIndex = ((TreeNode)m_nodes[i]).IndexOf(nodeKey);
                }
            }
            return nodeIndex;
        }
        /// <summary>
        ///  Verify that all leaves exist on the db (as forms or queries)
        ///  If not, remove from tree structure
        /// </summary>
        /// <param name="dbNodeNames">Forms that exist on the db</param>
        /// <param name="tree">Tree structure list from settings</param>
        public void VerifyListAgainstDBNodes(Dictionary<int, string> genObjects)
        {
            List<ITreeNode> tempNList = new List<ITreeNode>(m_nodes);
            foreach (ITreeNode n in tempNList)
            {
                // Forms are retrieved by the name
                if (!n.Type.Equals(CBVConstants.NodeType.Folder) && !genObjects.ContainsKey(((TreeLeaf)n).Id))
                {
                    m_nodes.Remove(n);
                }
                else
                {
                    //Verify inner nodes
                    if (n is TreeNode && ((TreeNode)n).Nodes.Count > 0)
                    {
                        ((TreeNode)n).VerifyListAgainstDBNodes(genObjects);
                    }
                }
            }
        }
        /// <summary>
        ///  Verifies if the node exists on the DB bank
        ///  Check by its <paramref name="nodeName"/>
        /// </summary>
        /// <param name="nodeName"></param>
        /// <param name="dbNodeNames"></param>
        /// <returns></returns>
        public bool ExistOnDB(string nodeName, List<string> dbNodeNames)
        {
            bool result = false;

            for (int i = 0; i < dbNodeNames.Count; i++)
            {
                if (dbNodeNames[i].Equals(nodeName))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
        /// <summary>
        ///  Adds new node to the tree list
        /// </summary>
        /// <param name="path"></param>
        /// <param name="nKey"></param>
        /// <param name="nodeName"></param>
        /// <param name="groupName"></param>
        /// <param name="isFolder"></param>
        public void AddNewNodeToList(string[] path, ITreeNode node, string groupName)
        {
            if (path.Length == 0)
                return;

            string[] subPath = path;  //for example: \UserForms\\TestFolder\\Cs_Demo_Props
            String firstToken = path[0];

            // look through children for match on first path token
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].Name.Equals(firstToken))
                {
                    if (path.Length == 2)  // current node is the parent //SUB
                    {
                        if (node is TreeNode)
                        {
                            string key = string.Empty;
                            if (node.Key.Contains("~"))
                                key = node.Key.Remove(node.Key.IndexOf("~"), 1);
                            else
                                key = node.Key;
                            ((TreeNode)m_nodes[i]).Nodes.Add(new TreeNode(key, node.Name, CBVConstants.NodeType.Folder));
                        }
                        else
                        {
                            ((TreeNode)m_nodes[i]).Nodes.Add(node);
                        }
                        break;
                    }
                    else
                    {
                        string[] tempPath = new string[(subPath.Length - 1)];
                        Array.Copy(subPath, 1, tempPath, 0, subPath.Length - 1);
                        ((TreeNode)m_nodes[i]).AddNewNodeToList(tempPath, node, groupName);
                        break;
                    }
                }
            }
        }
        /// <summary>
        ///  Get a specific node from its given <paramref name="path"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ITreeNode GetNodeByPath(string[] path)
        {
            ITreeNode node = null;
            string[] subPath = path;  //for example: \My Forms\\TestFolder\\Cs_Demo_Props
            String firstToken = path[0];

            // look through children for match on first path token
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].Name.Equals(firstToken))
                    {
                    if (path.Length == 1)
                    {
                        node = m_nodes[i];
                        break;
                    }
                    else
                    {
                        string[] tempPath = new string[(subPath.Length - 1)];
                        Array.Copy(subPath, 1, tempPath, 0, subPath.Length - 1);
                        node = ((TreeNode)m_nodes[i]).GetNodeByPath(tempPath);
                        break;
                    }
                }
            }
            return node;
        }
        /// <summary>
        ///  Reposition node
        /// </summary>
        /// <param name="sNode"></param>
        /// <param name="dNode"></param>
        /// <param name="list"></param>
        public void MoveNodeInList(ITreeNode sNode, ITreeNode dNode)
        {
            foreach (ITreeNode n in m_nodes)
            {
                if (n is TreeNode)
                {
                    if (n.Name.Equals(dNode.Name))
                    {
                        ((TreeNode)n).Nodes.Add(sNode);
                        break;
                    }
                    else
                        ((TreeNode)n).MoveNodeInList(sNode, dNode);
                }
            }
        }
        /// <summary>
        ///  Remove node from the list
        /// </summary>
        /// <param name="nKey"></param>
        public void RemoveNodeFromList(string nodeKey)
        {
            for (int i = 0; i < m_nodes.Count; i++)
            {
                if (m_nodes[i].Key.Equals(nodeKey) || (m_nodes[i] is TreeNode && nodeKey.Equals(
                    ((TreeNode)m_nodes[i]).GetFolderKey(m_nodes[i].Key))))
                {
                    m_nodes.Remove(m_nodes[i]);
                    break;
                }
                else
                {
                    // search inside the node if it's a folder
                    if (m_nodes[i] is TreeNode && (((TreeNode)m_nodes[i]).Nodes.Count > 0))
                        ((TreeNode)m_nodes[i]).RemoveNodeFromList(nodeKey);
                }
            }
        }
        #endregion
    }

    public class TreeLeaf : ITreeNode
    {
        #region Properties
        // Identifies the node in the whole tree
        public string Key { get; set; }
        // Identifies the leaf in the db
        public int Id { get; set; }
        public string Name { get; set; }
        public string Comments { get; set; }
        public CBVConstants.NodeType Type { get; set; }
        public int ParentId { get; set; }
        #endregion

        #region Constructor
        public TreeLeaf(string key, string name, CBVConstants.NodeType type)
        {
            this.Key = key;
            this.Name = name;
            this.Type = type;
            this.Id = 0;
            this.ParentId = 0;
        }
        #endregion
    }
}
