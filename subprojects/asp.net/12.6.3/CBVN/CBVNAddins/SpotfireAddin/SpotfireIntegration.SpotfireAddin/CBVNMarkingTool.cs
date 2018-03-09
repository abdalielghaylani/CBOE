using Spotfire.Dxp.Application;
using Spotfire.Dxp.Application.Extension;

namespace SpotfireIntegration.SpotfireAddin
{
    public sealed class CBVNMarkingTool : CustomTool<Document>
    {
        public CBVNMarkingTool() : base("Connect marking with ChemBioViz.NET")
        {
        }

        protected override void ExecuteCore(Document context)
        {
            SynchronizeMarking(context);
        }

        static internal void SynchronizeMarking(Document context)
        {
            CBVNMarkingNode node;
            if (context.CustomNodes.TryGetNode<CBVNMarkingNode>(out node))
            {
                node.Reconfigure();
            }
            else
            {
                node = new CBVNMarkingNode();
                context.CustomNodes.Add(node);
            }
        }

        protected override bool IsVisibleCore(Document context)
        {
            /* This tool doesn't actually initiate a connection with ChemBioViz.NET.
             * It only adds the necessary machinery to the document for us to be notified
             * of interesting Spotfire events and send them back when we already have a
             * connection.  Since this is already invoked by the extension code when it
             * loads a COE table, its presence in the menu would be confusing to the user,
             * so we hide it.
             */
            return false;
        }
    }
}
