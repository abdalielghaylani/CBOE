using System;
namespace CambridgeSoft.COE.DocumentManager.Services.Types
{
    public interface IDocument
    {
        PropertyList PropertyList { get; }
        ExternalLinkList ExternalLinkList { get; }
        StructureList StructureList { get; }
        int ID { get; set; }
        string Name { get; set; }
        string Title { get; set; }
        //string Content { get; }
        string Location { get; set; }
        string Author { get; set; }
        int Size { get; }
        string Submitter { get; }
        string Comments { get; set; }
        void InitializeFromXml(string xml, bool isNew, bool isClean);
        bool IsDirty { get; }
        bool IsValid { get; }
        bool IsNew { get; }
        string Xml { get; set; }
        byte[] BinaryContent{get; set;}
    }
}
