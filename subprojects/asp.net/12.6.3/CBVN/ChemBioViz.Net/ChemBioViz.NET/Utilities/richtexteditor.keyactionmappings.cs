#region Using directives

using System;
using System.Drawing;
using Infragistics.Win;
using System.Windows.Forms;
using System.Diagnostics;

#endregion Using directives

namespace Infragistics.Win.EmbeddableEditors
{

	#region RichTextEditorKeyActionMappings
    /// <summary>
    /// Collection of key-action mappings for the <see cref="RichTextEditor"/>.
    /// </summary>
    public class RichTextEditorKeyActionMappings : KeyActionMappingsBase
    {
		#region Constructor
		/// <summary>
		/// Creates a new instance of the <see cref="RichTextEditorKeyActionMappings"/> class.
		/// </summary>
        public RichTextEditorKeyActionMappings( ) : base( 5 )
        {
        }
		#endregion Constructor

		#region LoadDefaultMappings
        /// <summary>
        /// Loads the default key-action mappings for the <see cref="RichTextEditor"/>.
        /// </summary>
        public override void LoadDefaultMappings() 
        {
            RichTextEditorKeyActionMapping [] defaultMappings = new RichTextEditorKeyActionMapping [] 
            {
				#region OpenFileThroughDialog
				//
				//	Maps to the Ctrl+O keystroke...launches an OpenFileDialog, from which
				//	the end user can browse for an existing RTF file, and open in the editor.
				//
				new RichTextEditorKeyActionMapping(	Keys.O /*keyCode*/,
													RichTextEditorAction.OpenFileThroughDialog/*action*/,
													0/*disallowedState*/,
													RichTextEditorStates.InEditMode/*requiredState*/,
													SpecialKeys.AltShift/*disallowedSpecialKeys*/,
													SpecialKeys.Ctrl/*requiredSpecialKeys*/ ),
				#endregion OpenFileThroughDialog

				#region SaveFileThroughDialog
				//
				//	Maps to the Ctrl+S keystroke...launches a SaveFileDialog, from which
				//	the end user can create/open an RTF file, to which the editor's current
				//	contents can be saved.
				//
				new RichTextEditorKeyActionMapping(	Keys.S /*keyCode*/,
													RichTextEditorAction.SaveFileThroughDialog/*action*/,
													0/*disallowedState*/,
													RichTextEditorStates.InEditMode/*requiredState*/,
													SpecialKeys.AltShift/*disallowedSpecialKeys*/,
													SpecialKeys.Ctrl/*requiredSpecialKeys*/ ),
				#endregion SaveFileThroughDialog

           };

            // loop over the default mappings array and add each one to the
            // collection
            //
            for( int i = 0; i < defaultMappings.Length; i++ )
            {
                this.Add( defaultMappings[i] );
            }

        }
		#endregion LoadDefaultMappings

		#region Indexer
		/// <summary>
		/// indexer 
		/// </summary>
        public RichTextEditorKeyActionMapping this[ int index ] 
        {
            get
            {
				this.VerifyMappingsAreLoaded();
                
				return (RichTextEditorKeyActionMapping)base.GetItem( index );
            }
            set
            {
				this.VerifyMappingsAreLoaded();

				base.List[index] = value;
            }
        }
		#endregion Indexer
             
		#region CopyTo
		/// <summary>
		/// Copies the elements of the collection into the array.
		/// </summary>
		/// <param name="array">The array to copy to</param>
		/// <param name="index">The index to begin copying to.</param>
		public void CopyTo( RichTextEditorKeyActionMapping[] array, int index)
		{
			CopyTo( (System.Array)array, index );
		}
		#endregion	CopyTo
 		
		#region CreateActionStateMappingsCollection
		/// <summary>
		/// Creates an instance of an ActionStateMappingsCollection derived class
		/// </summary>
		/// <returns></returns>
		protected override Infragistics.Win.KeyActionMappingsBase.ActionStateMappingsCollection CreateActionStateMappingsCollection()
		{
			return new ActionStateMappingsCollection
			(
				new ActionStateMapping[] 
				{

                
					#region OpenFileThroughDialog
					new ActionStateMapping(	RichTextEditorAction.OpenFileThroughDialog,
											(long)0,
											(long)RichTextEditorStates.InEditMode),
					#endregion OpenFileThroughDialog

					#region OpenFileThroughDialog
					new ActionStateMapping(	RichTextEditorAction.SaveFileThroughDialog,
											(long)0,
											(long)RichTextEditorStates.InEditMode)
					#endregion OpenFileThroughDialog

				} 
			);
		}
		#endregion	CreateActionStateMappingsCollection

	}
	#endregion RichTextEditorKeyActionMappings

	#region RichTextEditorKeyActionMapping class
	/// <summary>
	/// Key-action mapping class for the <see cref="RichTextEditor"/>.
	/// </summary>
	public class RichTextEditorKeyActionMapping : KeyActionMappingBase
	{

		#region Constructor
		/// <summary>
		/// Creates a new instance of the <see cref="RichTextEditorKeyActionMapping"/> class.
		/// </summary>
		/// <param name="keyCode">The keystroke for which this action is to be mapped</param>
		/// <param name="actionCode">The <see cref="RichTextEditorAction"/> to perform when the keystroke is received</param>
		/// <param name="stateDisallowed">The <see cref="RichTextEditorStates"/> representing conditions that must <b>not</b> be true in order for the action to be performed</param>
		/// <param name="stateRequired">The <see cref="RichTextEditorStates"/> representing conditions that <b>must</b> be true in order for the action to be performed</param>
		/// <param name="specialKeysDisallowed">The special keys (such as ALT, CTRL or SHIFT) that can <b>not</b> be pressed in order for the action to be performed</param>
		/// <param name="specialKeysRequired">The special keys (such as ALT, CTRL or SHIFT) that <b>must</b> be pressed in order for the action to be performed</param>
		public RichTextEditorKeyActionMapping(	Keys keyCode,
												RichTextEditorAction actionCode,
												RichTextEditorStates stateDisallowed,
												RichTextEditorStates stateRequired,
												SpecialKeys specialKeysDisallowed,
												SpecialKeys specialKeysRequired )

		: base (keyCode,
				actionCode,
				(long)stateDisallowed,
				(long)stateRequired,
				specialKeysDisallowed,
				specialKeysRequired )
		{
		}
		#endregion Constructor

		#region ActionCode
		/// <summary>
		/// Gets/sets the action code
		/// </summary>
        public new RichTextEditorAction ActionCode
        {
            get 
            {
                return (RichTextEditorAction)base.ActionCode;
            }
            set
            {
				base.ActionCode = value;
            }
        }
		#endregion ActionCode

	}
	#endregion RichTextEditorKeyActionMapping class

	#region Enumerations

		#region RichTextEditorStates enumeration
    /// <summary>
    /// Bit flags that describe the <see cref="RichTextEditor"/>'s current state
    /// </summary>
    [Flags] public enum RichTextEditorStates
    {
		/// <summary>
		/// The <see cref="RichTextEditor"/> is in edit mode.
		/// </summary>
        InEditMode = 0x00000001,
	}
		#endregion	RichTextEditorStates enumeration

		#region RichTextEditorAction enumeration
	/// <summary>
	/// Enumerates the actions that can be performed on the <see cref="RichTextEditor"/>.
	/// </summary>
	public enum RichTextEditorAction
	{
		/// <summary>
		/// Lanches an OpenFileDialog and opens the selected RTF file.
		/// </summary>
		OpenFileThroughDialog,

		/// <summary>
		/// Lanches a SaveFileDialog and saves the editor's contents as an RTF file.
		/// </summary>
		SaveFileThroughDialog,

	}
		#endregion RichTextEditorAction enumeration

	#endregion Enumerations
}