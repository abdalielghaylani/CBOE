using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChemBioViz.NET
{
    public interface ITreeNode
    {
        string Key { get; set; } 
        string Name { get; set; }
        string Comments { get; set; }
        FormDBLib.CBVConstants.NodeType Type { get; set; }
    }

}
