
#region Using directives

using System;
using System.Drawing;
using Infragistics.Win;
using System.Windows.Forms;
using System.Diagnostics;
using System.ComponentModel;

#endregion Using directives

namespace Infragistics.Win.EmbeddableEditors
{
	#region RichTextEditor class
	/// <summary>
	/// The RichTextEditor is a custom EmbeddableEditorBase-derived editor that
	/// provides editing functionality like that of the MS RichTextBox control.
	/// </summary>
	/// <remarks>
	/// <p class="body">
	/// To understand how the editor works with an owner
	/// such as the UltraGrid, it is helpful to use the following analogy:
	/// The EmbeddableEditorBase-derived editor is typically used by a column,
	/// and serves instances of the associated EmbeddableUIElementBase-derived
	/// UIElement (see <see cref="RichTextEditorEmbeddableUIElement"/>) to each cell in that column.
	/// </p>
	/// </remarks>
	/// <seealso cref="RichTextEditorEmbeddableUIElement"/>
	/// <seealso cref="EmbeddableRichTextBox"/>
	public class RichTextEditor : EmbeddableEditorBase
	{
		#region Constants

		private const string NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE = "The property or method cannot be accessed when the editor in not in edit mode.";
		private const string VALUE_PROPERTY_WRONG_DATA_TYPE_EXCEPTION_MESSAGE = "The Value property can only be set to a value that is of type string.";
		private const string CURRENT_VALUE_INVALID_EXCEPTION_MESSAGE = "The current value is not valid.";
		private static readonly Size IDEAL_SIZE = new Size( 100, 96 );
		
		#endregion Constants

		#region Member variables

		private EmbeddableRichTextBox				richTextBoxEditor = null;
		private EmbeddableRichTextBox				richTextBoxRenderer = null;
		private RichTextEditorKeyActionMappings		keyActionMappings = null;

		#endregion Member variables

		#region Constructors

		/// <summary>
		/// Creates a new instance of the <see cref="RichTextEditor"/> class.
		/// </summary>
		public RichTextEditor()
		{
		}

		/// <summary>
		/// Creates a new instance of the <see cref="RichTextEditor"/> class.
		/// </summary>
		/// <param name="defaultOwner">An EmbeddableEditorOwnerBase-derived instance which serves as a second point of contact between the editor and the primary owner (see remarks)</param>
		/// <remarks><p class="body">
		/// The default owner serves as a secondary source of information for the editor. Using the UltraGrid
		/// example, the UltraGrid control implements an EmbeddableEditorOwnerBase-derived class which serves
		/// as the (primary) communication layer between the embeddable editor and the grid. This class provides the
		/// information that the editor needs to perform its tasks in a manner that is appropriate for the owner.
		/// For example, to obtain the formatting information the editor might require in order to know how to
		/// display its value, it calls the EmbeddableEditorOwnerBase class' <b>GetFormatInfo</b> method. The
		/// UltraGrid's implementation provides the value based on the corresponding column's <b>Format</b>
		/// property. If the column's Format property is not specifically set, the default owner's GetFormatInfo
		/// method is then called, giving the default (secondary) owner a chance to provide a format to use. If neither the
		/// grid nor the default owner provide a format, none is applied.
		/// </p></remarks>
		public RichTextEditor( EmbeddableEditorOwnerBase defaultOwner ) : base( defaultOwner )
		{
		}

		#endregion Constructors

		#region Abstract property/method overrides

			#region CanFocus
		/// <summary>
		/// Returns whether the editor can receive input focus while in edit mode.
		/// The <see cref="RichTextEditor"/> is a focusable editor, so we always return true.
		/// </summary>
		public override bool CanFocus
		{
			get{ return true; }
		}
			#endregion CanFocus

			#region Focused
		/// <summary>
		/// Returns whether the editor currently has input focus
		/// </summary>
		public override bool Focused
		{
			get
			{ 
				//	If we are not in edit mode, return false
				if ( ! this.IsInEditMode )
					return false;

				//	If we are in edit mode, return the Focused property
				//	of the RichTextBox control that we use for editing
				return this.RichTextBoxEditor.Focused; 
			}
		}
			#endregion Focused

			#region Focus
		/// <summary>
		/// Sets input focus to the editor.
		/// </summary>
		/// <returns>Returns true if the editor successfully took focus.</returns>
		public override bool Focus()
		{
			if ( ! this.IsInEditMode || ! this.RichTextBoxEditor.CanFocus )
				return false;

			if ( ! this.RichTextBoxEditor.Visible )
				this.RichTextBoxEditor.Show();

			return this.RichTextBoxEditor.Focus();
		}
			#endregion Focus

			#region CanEditType
		/// <summary>
		/// Returns whether the editor can edit the specified type.
		/// </summary>
		/// <param name="type">The type to evaluate.</param>
		/// <returns>A boolean indicating whether the type can be edited.</returns>
		public override bool CanEditType( System.Type type )
		{
			//	While we don't need to be this retrictive, we will limit
			//	this editor's useability to the string data type for the
			//	sake of simplicity.
			return type == typeof(string);
		}		
			#endregion CanEditType

			#region CanRenderType
		/// <summary>
		/// Returns whether the editor can render the specified type.
		/// </summary>
		/// <param name="type">The type to evaluate.</param>
		/// <returns>A boolean indicating whether the type can be edited.</returns>
		public override bool CanRenderType( System.Type type )
		{
			//	Since all objects expose a ToString method, return true
			//	unconditionally to indicate that the editor can render any type.
			return true;
		}
			#endregion CanRenderType

			#region GetEmbeddableElement
	
		/// <summary>
		/// Method that serves as a factory for the embeddable UIElements provided by this editor.
		/// </summary>
		/// <param name="parentElement">The element that will contain the embedded element.</param>
		/// <param name="owner">An object that derives from the EmbeddableEditorOwnerBase class.</param>
		/// <param name="ownerContext">Context information that is used to get the value to render via a call to the EmbeddableEditorOwnerBase.GetValue method.</param>
		/// <param name="includeEditElements">If true will add associated elements (e.g. editing elements like spin buttons).</param>
		/// <param name="reserveSpaceForEditElements">If true will reserve space for associated elements (e.g. editing elements like spin buttons).</param>
		/// <param name="drawOuterBorders">If true the element is expected to draw the outer borders</param>
		/// <param name="isToolTip">If true the element should draw as a tooltip, i.e., use SystemColors.Info as a BackColor, and not draw gradients.</param>
		/// <param name="previousElement">The element returned the prior time the parent element's PositionChildElements method was called.</param>
		/// <returns>An instance of the EmbeddableUIElementBase-derived class which represents this editor in the user interface.</returns>
		///	<seealso cref="RichTextEditorEmbeddableUIElement"/>
		public override EmbeddableUIElementBase GetEmbeddableElement(	UIElement parentElement,
																		EmbeddableEditorOwnerBase owner,
																		object ownerContext,
																		bool includeEditElements,
																		bool reserveSpaceForEditElements,
																		bool drawOuterBorders,
																		bool isToolTip,
																		EmbeddableUIElementBase previousElement )
		{
			RichTextEditorEmbeddableUIElement prevTextElement = null;

			if ( previousElement != null &&
				 previousElement.GetType() == typeof(RichTextEditorEmbeddableUIElement) )
				 prevTextElement = previousElement as RichTextEditorEmbeddableUIElement;

			if ( null != previousElement && this.ElementBeingEdited == previousElement )
			{
				previousElement.Initialize( owner,
											this,
											ownerContext,
											includeEditElements,
											reserveSpaceForEditElements,
											drawOuterBorders,
											isToolTip );

				return previousElement;
			}

			if ( prevTextElement != null &&
				 prevTextElement.Parent == parentElement &&
				 previousElement is RichTextEditorEmbeddableUIElement )
			{
				//	Call the Initialize method, to recreate the element without
				//	creating a new instance
				previousElement.Initialize( owner,
											this,
											ownerContext,
											includeEditElements,
											reserveSpaceForEditElements,
											drawOuterBorders,
											isToolTip );

				return previousElement;
			}

			return new RichTextEditorEmbeddableUIElement( parentElement,
												owner,
												this,
												ownerContext,
												includeEditElements,
												reserveSpaceForEditElements,
												drawOuterBorders,
												isToolTip );
		}
		

			#endregion	GetEmbeddableElement

			#region GetEmbeddableElementType
		/// <summary>
		/// Returns the type of the EmbeddableUIElementBase derived class that this editor uses as it's embeddable element.
		/// </summary>
		/// <returns>RichTextEditorEmbeddableUIElement as a type</returns>
		public override Type GetEmbeddableElementType( )
		{
			return typeof( RichTextEditorEmbeddableUIElement );
		}
			#endregion	GetEmbeddableElementType

			#region GetSize
		/// <summary>
		/// Used to determine the size preferred/required by the editor.
		/// </summary>
		/// <param name="sizeInfo">Structure that provides information regarding the size calculation including the owner for which the size calculation is occuring, whether to exclude edit elements, whether to include borders, etc.</param>
		/// <returns>The size needed to render the value by the editor based upon the specified options.</returns>
		protected override Size GetSize( ref EditorSizeInfo sizeInfo )
		{
			return RichTextEditor.IDEAL_SIZE;
		}
			#endregion GetSize

			#region IsInputKey
		/// <summary>
		/// Returns whether the specified key is used by the editor.
		/// </summary>
		/// <param name="keyData">The key to test</param>
		/// <returns>Returns whether the specified key is used by the editor.</returns>
		public override bool IsInputKey( Keys keyData )
		{
			return this.KeyActionMappings.IsKeyMapped( keyData, (long)this.CurrentState );
		}
			#endregion IsInputKey

			#region Clone
		/// <summary>
		/// Creates a copy of the embeddable editor with the specified default owner.
		/// </summary>
		/// <param name="defaultOwner">An instance of the default EmbeddableEditorOwnerBase-derived class from which to obtain owner-specific information</param>
		/// <returns>A copy of the editor</returns>
		public override EmbeddableEditorBase Clone(EmbeddableEditorOwnerBase defaultOwner)
		{
			RichTextEditor editor = new RichTextEditor(defaultOwner);
			editor.InitializeFrom(this);
			return editor;
		}
			#endregion	Clone

			#region CurrentEditText
		/// <summary>
		/// Returns the current text being edited without performing any validation.
		/// </summary>
		/// <remarks>
		/// <p class="body">The text returned from this property does <b>not</b> contain any RTF codes, only the text component of the current value.</p>
		/// <p class="note">Accessing this property when the editor is not in edit mode will throw an exception.</p>
		/// </remarks>
		public override string CurrentEditText
		{ 
			get
			{ 
				if ( ! this.IsInEditMode )
					throw new Exception( NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );
				
				return this.RichTextBoxEditor.Text;
			} 
		}

			#endregion CurrentEditText

			#region Value
		/// <summary>
		/// Gets/sets the editor's value.
		/// </summary>
		/// <remarks>
		/// <p class="body">
		/// Accessing the <b>Value</b> property when the <see cref="RichTextEditor"/> is not in edit mode will cause an exception to be thrown.
		/// </p>
		/// <p class="note">The <see cref="RichTextEditor"/> works with RTF (Rich Text Format) codes, and will expect the value being set to contain valid RTF codes. If the value being set is not of type string, an exception will be thrown; if the value being set is of type string, but does not contain valid RTF codes, the value will be accepted, but might not produce the desired result.</p>
		/// </remarks>
		/// <seealso cref="IsValid"/>
		/// <seealso cref="CurrentEditText"/>
		public override object Value
		{
			get
			{
				//	Throw an exception if we are not in edit mode
				if ( this.IsInEditMode == false )
					throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

				//	Throw an exception if the current value is not valid
				if ( this.IsValid == false )
					throw new Exception( RichTextEditor.CURRENT_VALUE_INVALID_EXCEPTION_MESSAGE );

				if ( this.RichTextBoxEditor.Text.Length == 0 )
					return null;

				return this.RichTextBoxEditor.Rtf;
			}

			set
			{
				//	Throw an exception if we are not in edit mode
				if ( this.IsInEditMode == false )
					throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );


				//	Throw an exception if the value being set is not of type string
				string stringValue = value as string;
				if ( stringValue == null )
					throw new Exception( RichTextEditor.VALUE_PROPERTY_WRONG_DATA_TYPE_EXCEPTION_MESSAGE );

				//	If the value being set consists of valid RTF codes, set the Rtf proeprty;
				//	otherwise, set the Text property.
				if ( this.RichTextBoxEditor.IsValidRTF(stringValue) )
					this.RichTextBoxEditor.Rtf = stringValue;
				else
					this.RichTextBoxEditor.Text = stringValue;
			}
		}
			#endregion Value

			#region IsValid
		/// <summary>
		/// Returns whether the editor's current value is valid.
		/// </summary>
		/// <p class="body">The <b>IsValid</b> property cannot be accessed when the editor is not in edit mode; doing so will cause an exception to be thrown.</p>
		/// <seealso cref="Value"/>
		public override bool IsValid
		{
			get
			{
				//	If we are not in edit mode, throw an exception
				if ( ! this.IsInEditMode )
					throw new Exception( NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

				//	If the owner's return from the 'IsNullable' method is false,
				//	and the edit portion of the control is left blank, return false
				//	to signify that the value is not valid.
				bool nullable = this.ElementBeingEdited.Owner.IsNullable( this.ElementBeingEdited.OwnerContext );
				if ( ! nullable && this.CurrentEditText.Length == 0 )
					return false;
				
				//	This editor defines no other criteria for validity, aside from nullability,
				//	so return true in all other circumstances.
				return true;
			}
		}
			#endregion IsValid

		#endregion Abstract property/method overrides

		#region Base class overrides

			#region OnBeforeEnterEditMode
		/// <summary>
		/// Called before edit mode has been entered. 
		/// </summary>
		/// <param name="cancel">Set to true to cancel the operation.</param>
		/// <remarks>The base class implementation raises the BeforeEnterEditMode event.</remarks>
		protected override void OnBeforeEnterEditMode( ref bool cancel )
		{
			//	Call the base class implementation, which raises the BeforeEnterEditMode event.
			base.OnBeforeEnterEditMode( ref cancel );

			if ( ! cancel )
			{
				if ( this.ElementBeingEdited != null )
				{
					//	Get the value from the owner; this is the value that the
					//	edit mode session will begin with.
					string ownerValue = this.ElementBeingEdited.Owner.GetValue( this.ElementBeingEdited.OwnerContext ) as string;
					
					//	Clear the EmbeddableRichTextBox's current value
					this.RichTextBoxEditor.Text = string.Empty;

					if ( ownerValue != null )
					{
						if ( this.RichTextBoxEditor.IsValidRTF(ownerValue) )
							this.RichTextBoxEditor.Rtf = ownerValue;
						else
							this.RichTextBoxEditor.Text = ownerValue;
					}

					//	Set the position of the RichTextBox control that we will
					//	use for the editing.
                    //Coverity Bug Fix CID 13023 
					RichTextEditorEmbeddableUIElement richTextEditorElement = this.ElementBeingEdited as RichTextEditorEmbeddableUIElement;
                    if (richTextEditorElement != null)
                    {
                        Rectangle rect = richTextEditorElement.EditArea;
                        this.RichTextBoxEditor.SetBounds(rect.Left, rect.Top, rect.Width, rect.Height);

                        //	Enter edit mode on the RichTextBox control.
                        this.RichTextBoxEditor.EnterEditMode(this.ElementBeingEdited as RichTextEditorEmbeddableUIElement);
                    }
				}
			}
		}
			#endregion	OnBeforeEnterEditMode

			#region	OnBeforeExitEditMode		
		/// <summary>
		/// Called before edit mode has been exited. 
		/// </summary>
		/// <param name="cancel">Set to true to cancel the operation.</param>
		/// <param name="forceExit">If true, edit mode is exited even if the BeforeExitEditMode event is canceled.</param>
		/// <param name="applyChanges">If true, any changes made while in edit mode will be applied.</param>
		protected override void OnBeforeExitEditMode( ref bool cancel, bool forceExit, bool applyChanges )
		{
			base.OnBeforeExitEditMode( ref cancel, forceExit, applyChanges );

			if ( ! cancel )
			{
				//	If the BeforeExitEditMode event was not canceled, take
				//	the EmbeddableRichTextBox out of edit mode.
				this.RichTextBoxEditor.ExitEditMode();
			}

		}
			#endregion	OnBeforeExitEditMode

			#region GetDisplayValue
		/// <summary>
		/// Provides editor-specific display value.
		/// </summary>
		/// <returns></returns>
		protected override string GetDisplayValue()
		{
			return this.CurrentEditText;
		}
			#endregion GetDisplayValue

			#region GetEditorValue
		/// <summary>
		/// Provides editor-specific editor value.
		/// </summary>
		/// <returns></returns>
		protected override object GetEditorValue()
		{
			return this.RichTextBoxEditor.Rtf;
		}
			#endregion GetEditorValue

		#endregion Base class overrides

		#region Internal properties

			#region RichTextBoxEditor
		/// <summary>
		/// Returns the <see cref="EmbeddableRichTextBox"/> instance that is used to realize
		/// editing capabilities for the element that is in edit mode.
		/// </summary>
		internal EmbeddableRichTextBox RichTextBoxEditor
		{
			get
			{
				//	Lazily create the control instance
				if ( this.richTextBoxEditor == null )
				{
					this.richTextBoxEditor = new EmbeddableRichTextBox();

					//	Set the BorderStyle to none, and let the element handle border drawing
					this.richTextBoxEditor.BorderStyle = System.Windows.Forms.BorderStyle.None;
				
				}

				return this.richTextBoxEditor;
			}
		}
			#endregion RichTextBoxEditor

			#region RichTextBoxRenderer
		/// <summary>
		/// Returns the <see cref="EmbeddableRichTextBox"/> instance that is used to render
		/// the value for elements that are not in edit mode.
		/// </summary>
		/// <remarks><p class="body">
		/// The <see cref="RichTextBoxRenderer"/> is used to draw into the embeddable element
		/// so that it assumes the appearance of a RichTextBox when it is not in edit mode.
		/// </p></remarks>
		internal EmbeddableRichTextBox RichTextBoxRenderer
		{
			get
			{
				//	Lazily create the control instance
				if ( this.richTextBoxRenderer == null )
				{
					this.richTextBoxRenderer = new EmbeddableRichTextBox();

					//	Set the BorderStyle to none, and let the element handle border drawing
					this.richTextBoxRenderer.BorderStyle = System.Windows.Forms.BorderStyle.None;					
				}

				return this.richTextBoxRenderer;
			}
		}
			#endregion RichTextBoxRenderer

			#region CurrentState
		/// <summary>
		/// Returns the current state of the editor. Used by the
		/// key-action mapping logic.
		/// </summary>
		internal RichTextEditorStates CurrentState
		{
			get
			{
				long state = 0;

				if ( this.IsInEditMode )
					state |= (long)RichTextEditorStates.InEditMode;

				return (RichTextEditorStates)state;
			}
		}
			#endregion CurrentState

		#endregion Internal properties

		#region Public properties

			#region KeyActionMappings
		/// <summary>
		/// Returns a collection of <see cref="RichTextEditorKeyActionMapping"/> objects
		/// which describe a key-action mapping for the <see cref="RichTextEditor"/>.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public RichTextEditorKeyActionMappings  KeyActionMappings
		{
			get
			{
				if ( null == this.keyActionMappings )
					this.keyActionMappings = new RichTextEditorKeyActionMappings();

				return this.keyActionMappings;
			}
		}
			#endregion KeyActionMappings

		#endregion Public properties

		#region Methods

			#region Extended functionality methods

			#region LoadFile
		/// <summary>
		/// Loads the contents of the specified RTF file into the edit portion.
		/// </summary>
		/// <remarks><p class="note"><b>Note: </b>Calling this method when the editor is not in edit mode will cause an exception to be thrown.</p>
		/// <param name="path">The path to the file to be loaded.</param>
		public void LoadFile( string path )
		{
			if ( ! this.IsInEditMode )
				throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

			this.RichTextBoxEditor.LoadFile( path );
		}
			#endregion LoadFile

			#region SaveFile
		/// <summary>
		/// Saves the current contents of the editor to the specified file as RTF (Rich Text Format).
		/// </summary>
		/// <remarks><p class="note"><b>Note: </b>Calling this method when the editor is not in edit mode will cause an exception to be thrown.</p>
		/// <param name="path">The path to the file to be loaded.</param>
		public void SaveFile( string path )
		{
			if ( ! this.IsInEditMode )
				throw new Exception( RichTextEditor.NOT_IN_EDIT_MODE_EXCEPTION_MESSAGE );

			this.RichTextBoxEditor.SaveFile( path );
		}
			#endregion SaveFile

			#endregion Extended functionality methods

			#region Protected method accessors

			#region InternalRaiseValueChanged
		/// <summary>
		/// Accessor for the protected RaiseValueChanged event.
		/// </summary>
		internal void InternalRaiseValueChanged()
		{
			this.RaiseValueChangedEvent();
		}
			#endregion InternalRaiseValueChanged

			#region InternalRaiseKeyDown
		/// <summary>
		/// Accessor for the protected RaiseKeyDown method.
		/// </summary>
		internal void InternalRaiseKeyDown( KeyEventArgs e )
		{
			this.RaiseKeyDownEvent( e );
		}
			#endregion InternalRaiseKeyDown

			#region InternalRaiseKeyPress
		/// <summary>
		/// Accessor for the protected RaiseKeyPress method.
		/// </summary>
		internal void InternalRaiseKeyPress( KeyPressEventArgs e )
		{
			this.RaiseKeyPressEvent( e );
		}
			#endregion InternalRaiseKeyPress

			#region InternalRaiseKeyUp
		/// <summary>
		/// Accessor for the protected RaiseKeyUp method.
		/// </summary>
		internal void InternalRaiseKeyUp( KeyEventArgs e )
		{
			this.RaiseKeyUpEvent( e );
		}
			#endregion InternalRaiseKeyUp

			#endregion Protected method accessors

			#region Helper routines

			#region PerformKeyAction
		/// <summary>
		/// Handles the specified keystroke by searching the <see cref="KeyActionMappings"/>
		/// collection for en entry that corresponds to the specified keystroke, and performing
		/// that action if one is found.
		/// </summary>
		/// <param name="keyData">The key data to evaluate</param>
		/// <returns>True if a corresponding action was executed; false otherwise.</returns>
		internal bool PerformKeyAction( Keys keyData )
		{
			//	Determine if the specified keystroke has an action mapped
			//	to it by calling the KeyActionMappingsBase class' IsKeyMapped
			//	method.
			long currentState = (long)this.CurrentState;
			if ( this.KeyActionMappings.IsKeyMapped( keyData, currentState ) )
			{
				RichTextEditorKeyActionMapping keyMapping = null;

				//	Build the SpecialKeys value based on whatever modifier
				//	keys are pressed; also, strip the modifier keys out of
				//	the keyData parameter.
				SpecialKeys specialKeys = (SpecialKeys)0;
				
				if ( (keyData & Keys.Alt) == Keys.Alt )
				{
					specialKeys |= SpecialKeys.Alt;
					keyData &= ~Keys.Alt;
				}

				if ( (keyData & Keys.Control) == Keys.Control )
				{
					specialKeys |= SpecialKeys.Ctrl;
					keyData &= ~Keys.Control;
				}

				if ( (keyData & Keys.Shift) == Keys.Shift )
				{
					specialKeys |= SpecialKeys.Shift;
					keyData &= ~Keys.Shift;
				}


				//	Use the KeyActionMappingsBase class' GetActionMappings method
				//	to get the actions that are mapped to the specified keystroke.
				KeyActionMappingBase[] keyMappings = this.KeyActionMappings.GetActionMappings( keyData, currentState, specialKeys );

				//	Iterate whatever actions are mapped for the specified keystroke and execute them
                //Coverity Bib Fix : CID 13106 
                if (keyMappings != null)
                {
                    for (int i = 0; i < keyMappings.Length; i++)
                    {
                        keyMapping = keyMappings[i] as RichTextEditorKeyActionMapping;
                        if (keyMapping != null)
                            return true;
                    }
                }
			}
			return false;
		}
			#endregion PerformKeyAction

			#endregion Helper routines

		#endregion Methods
	}
	#endregion RichTextEditor class

	#region RichTextEditorEmbeddableUIElement class
	/// <summary>
	/// EmbeddableUIElementBase-derived class that represents individual
	/// <see cref="RichTextEditor"/> owners in the user interface.
	/// </summary>
	/// <remarks>
	/// <p class="body">
	/// To understand how the embeddable element works with an owner
	/// such as the UltraGrid, it is helpful to use the following analogy:
	/// The EmbeddableEditorBase-derived editor (see <see cref="RichTextEditor"/>)
	/// typically applies to an entire column, while the EmbeddableUIElementBase-derived
	/// UIElement applies to one particular cell in that column. Only one of these
	/// elements can be in edit mode at any given time.
	/// </p>
	/// </remarks>
	/// <seealso cref="RichTextEditor"/>
	/// <seealso cref="EmbeddableRichTextBox"/>
	public class RichTextEditorEmbeddableUIElement : EmbeddableUIElementBase
	{
		#region Constructor

		/// <summary>
		/// Creates a new instance of the <see cref="RichTextEditorEmbeddableUIElement"/> class.
		/// </summary>
		/// <param name="parentElement">The parent element.</param>
		/// <param name="owner">The EmbeddableEditorOwnerBase-drived instance that provides information to the editor.</param>
		/// <param name="editor">The <see cref="RichTextEditor"/> instance to which this element will belong.</param>
		/// <param name="ownerContext">Contextual information used by the owner to identify the value.</param>
		/// <param name="includeEditElements">If true will add associated elements (e.g. editing elements like spin buttons).</param>
		/// <param name="reserveSpaceForEditElements">If true will reserve space for associated elements (e.g. editing elements like spin buttons).</param>
		/// <param name="drawOuterBorders">If true the element is expected to draw the outer borders</param>
		/// <param name="isToolTip">If true the element should draw as a tooltip, i.e., use SystemColors.Info as a BackColor, and not draw gradients.</param>
		/// <remarks><p class="note">Because the RichTextEditor does not contain edit elements, the <param name="includeEditElements"/> and <param name="reserveSpaceForEditElements"/> parameters are not applicable.</p></returns>
		public RichTextEditorEmbeddableUIElement( UIElement parentElement,
										EmbeddableEditorOwnerBase owner,
										EmbeddableEditorBase editor,
										object ownerContext,
										bool includeEditElements,
										bool reserveSpaceForEditElements,
										bool drawOuterBorders, 
										bool isToolTip ) : base( parentElement, owner, editor, ownerContext, includeEditElements, reserveSpaceForEditElements, drawOuterBorders, isToolTip )
		{
		}

		#endregion Constructor

		#region Overrides

			#region OnMouseDown
		/// <summary>
		/// Called when a mouse button is pressed while the cursor is positioned over the element.
		/// </summary>
		/// <param name="e">Mouse event arguments</param>
		/// <param name="adjustableArea">True if the mouse button was pressed while the cursor was positioned over the adjustable area</param>
		/// <param name="captureMouseForElement">If not null, contains a reference to the element that has captured the mouse</param>
		protected override bool OnMouseDown( MouseEventArgs e, bool adjustableArea, ref UIElement captureMouseForElement )
		{
			//	Raise the MouseDown event

			//	Create the event arguments
			EmbeddableMouseDownEventArgs eventArgs = new EmbeddableMouseDownEventArgs(	this,
																						false/*isButton*/,
																						e/*mouseArgs*/,
																						false/*eatMessage*/,
																						false/*notifyParentElement*/ );
						
			//	If disabled or in design mode, call the base class implementation
			bool disabled = this.Owner != null && ! this.Owner.IsEnabled( this.OwnerContext );
			bool designMode = this.Owner != null && this.Owner.DesignMode;
			if ( disabled || designMode )
				return base.OnMouseDown( e, adjustableArea, ref captureMouseForElement );

			//	Raise the editor's MouseDown event
			this.RaiseMouseDownEvent( eventArgs );
			bool retVal = eventArgs.EatMessage;
			
			//	If the (logical) left button was pressed, attempt to go into edit mode
			if ( e.Button == MouseButtons.Left )
			{
				RichTextEditor richTextEditor = this.Editor as RichTextEditor;

				//	Call the owner's EnterEditModeOnClick method to determine whether
				//	we should event try to go into edit mode in response to the MouseDown
				if ( ! this.IsInEditMode && this.Owner.EnterEditModeOnClick( this.OwnerContext ) )
				{
					richTextEditor.EnterEditMode( this );

					//	Set focus to the owning control if we are entering edit mode
					if ( this.IsInEditMode &&
						 ! richTextEditor.RichTextBoxEditor.Focused )
					{
						Control owningControl = this.Owner.GetControl( this.OwnerContext );
						if ( owningControl != null )
						{
							//	If the attempt to set focus on the owner fails, we must exit edit mode
							if ( ! owningControl.Focus() )
								richTextEditor.ExitEditMode( true, true );
						}
					}
				}

				//	If edit mode was cancelled then return the value
				//	of the EatMessage property of the event arguments.
				//	Note that we have to check the element's 'IsInEditMode'
				//	property (not the editor's), because if the BeforeExitEditMode
				//	event was canceled for some other element, the editor will still
				//	be in edit mode, but not on this element, which is what we are
				//	really interested in here.
				//
				if ( ! eventArgs.EmbeddableElement.IsInEditMode )
					return eventArgs.EatMessage;

				//	Return true or false depending on what the listener specified
				//	The return value here indicates whether to bypass default processing
				return	eventArgs.EatMessage;
			}

			//	Return the result of the call to the base class implementation
			//	for any other button but the left one.
			return retVal;

		}

		/// <summary>
		/// Internal accessor for the <see cref="OnMouseDown"/> method.
		/// </summary>
		/// <param name="e">Mouse event arguments</param>
		/// <param name="adjustableArea">True if the mouse button was pressed while the cursor was positioned over the adjustable area</param>
		/// <param name="captureMouseForElement">If not null, contains a reference to the element that has captured the mouse</param>
		internal bool InternalOnMouseDown( MouseEventArgs e, bool adjustableArea, ref UIElement captureMouseForElement )
		{
			return this.OnMouseDown( e, adjustableArea, ref captureMouseForElement );
		}
			#endregion OnMouseDown
		
			#region PositionChildElements
		/// <summary>
		/// Handles positioning of child UIElements.
		/// </summary>
		protected override void PositionChildElements()
		{
			UIElementsCollection oldElements = this.childElementsCollection;
			this.childElementsCollection = null;
			Rectangle workRect = this.RectInsideBorders;

			#region Create/extract the RichTextEditAreaUIElement

			RichTextEditAreaUIElement editAreaElement = RichTextEditorEmbeddableUIElement.ExtractExistingElement( oldElements, typeof(RichTextEditAreaUIElement), true ) as RichTextEditAreaUIElement;
			if ( editAreaElement != null )
				editAreaElement.Initialize( this );
			else
				editAreaElement = new RichTextEditAreaUIElement( this );

			#endregion Create/extract the RichTextEditAreaUIElement

			#region Image positioning

			//	Ask the owner to resolve the appearance and determine if there
			//	is an image to be displayed. If there is, we have to carve out
			//	space for it; where it goes is determined by the image alignment
			//	properties of the resolved appearance, and how big is determined
			//	by the EmbeddableEditorOwnerBase class' GetImageSize method.
			//
			AppearanceData appearance = new AppearanceData();
			AppearancePropFlags requestedProps =	AppearancePropFlags.Image		|
													AppearancePropFlags.ImageHAlign |
													AppearancePropFlags.ImageVAlign;

			this.Owner.ResolveAppearance( this.OwnerContext, ref appearance, ref requestedProps );

			//	If there was an image in the appearance, create an ImageUIElement
			//	for it and add it to the ChildElements collection.
			ImageUIElement imageElement = null;
			if ( appearance.Image != null )
			{
				//	Get the owner-provided ImageList, if any
				ImageList imageList = this.Owner.GetImageList( this.OwnerContext );

				//	Use the GetImage method to obtain the image, which handles
				//	extracting it from the ImageList if necessary
				Image image = appearance.GetImage( imageList );

				//	Get the image size...if the owner doesn't provide one,
				//	we will go with 16 X 16
				Size imageSize = Size.Empty;
				if ( ! this.Owner.GetSizeOfImages(this.OwnerContext, out imageSize) )
					imageSize = new Size(16, 16);
				
				Rectangle imageRect = workRect;
				imageRect.Width = imageSize.Width;
				imageRect.Height = imageSize.Height;
				
				bool adjustHeight = appearance.ImageHAlign == HAlign.Center;
				bool adjustWidth = appearance.ImageVAlign == VAlign.Middle;

				//	Horizontal positioning
				switch( appearance.ImageHAlign )
				{
					case HAlign.Default:
					case HAlign.Left:
					{
						workRect.X += imageSize.Width;
						workRect.Width -= imageSize.Width;
					}
					break;

					case HAlign.Right:
					{
						workRect.Width -= imageSize.Width;
						imageRect.X = workRect.Right;
					}
					break;

					case HAlign.Center:
					{
						imageRect.X += (workRect.Width / 2);
						imageRect.X -= (imageRect.Width / 2);
					}
					break;
				}

				//	Vertical positioning
				switch( appearance.ImageVAlign )
				{
					case VAlign.Default:
					case VAlign.Top:
					{
						if ( adjustHeight )
						{
							workRect.Y += imageSize.Height;
							workRect.Height -= imageSize.Height;
						}
					}
					break;

					case VAlign.Bottom:
					{
						imageRect.Y = workRect.Bottom - imageSize.Height;

						if ( adjustHeight )
							workRect.Height -= imageSize.Height;						
					}
					break;

					case VAlign.Middle:
					{
						imageRect.Y += (workRect.Height / 2);
						imageRect.Y -= (imageRect.Height / 2);
					}
					break;
				}

				//	Create the ImageUIElement and add it to the ChildElements collection.
				imageElement = new ImageUIElement( this, image );
				imageElement.Rect = imageRect;
			}

			#endregion Image positioning

			//	Set the position of the RichTextEditAreaUIElement, and add
			//	it to this element's ChildElements collection
			editAreaElement.Rect = workRect;
			this.ChildElements.Add( editAreaElement );

			//	We add the ImageUIElement after the RichTextEditAreaUIElement so
			//	that if the image alignment is middle center. the image will appear.
			if ( imageElement != null )
				this.ChildElements.Add( imageElement );


		}
			#endregion PositionChildElements

		#endregion Overrides

		#region Internal properties

			#region RichTextEditor
		/// <summary>
		/// Returns the <see cref="RichTextEditor"/> to which this <see cref="RichTextEditorEmbeddableUIElement"/> instance is associated.
		/// </summary>
		internal RichTextEditor RichTextEditor
		{
			get{ return this.Editor as RichTextEditor; }
		}
			#endregion RichTextEditor

			#region EditArea
		/// <summary>
		/// Returns a Rectangle that describes the area where the <see cref="EmbeddableRichTextBox"/> is to be displayed.
		/// </summary>
		internal Rectangle EditArea
		{
			get
			{
				RichTextEditAreaUIElement editAreaElement = this.GetDescendant( typeof(RichTextEditAreaUIElement) ) as RichTextEditAreaUIElement;
				if ( editAreaElement == null )
					return Rectangle.Empty;

				return editAreaElement.Rect;

			}
		}
			#endregion EditArea

		#endregion Internal properties

	}
	#endregion RichTextEditorEmbeddableUIElement class

	#region RichTextEditAreaUIElement class
	/// <summary>
	/// UIElement that displays an image of the <see cref="EmbeddableRichTextBox"/> control
	/// when its associated <see cref="RichTextEditorEmbeddableUIElement"/> is not in edit mode.
	/// </summary>
	public class RichTextEditAreaUIElement : UIElement
	{
		#region Member variables
		RichTextEditorEmbeddableUIElement richTextEditorEmbeddableUIElement = null;
		#endregion Member variables

		#region Constructor
		/// <summary>
		/// Creates a new instance of the <see cref="RichTextEditAreaUIElement"/> class.
		/// </summary>
		/// <param name="richTextEditorEmbeddableUIElement">The <see cref="RichTextEditorEmbeddableUIElement"/> to which this instance is associated.</param>
		public RichTextEditAreaUIElement( RichTextEditorEmbeddableUIElement richTextEditorEmbeddableUIElement )
		{
			this.Initialize( richTextEditorEmbeddableUIElement );
		}

		/// <summary>
		/// Initializes this instance for use by a new <see cref="RichTextEditorEmbeddableUIElement"/>.
		/// </summary>
		/// <param name="richTextEditorEmbeddableUIElement">The <see cref="RichTextEditorEmbeddableUIElement"/> to which this instance is associated.</param>
		public void Initialize( RichTextEditorEmbeddableUIElement richTextEditorEmbeddableUIElement )
		{
			this.richTextEditorEmbeddableUIElement = richTextEditorEmbeddableUIElement;
		}
		#endregion Constructor

		#region Overrides

			#region OnMouseDown
		/// <summary>
		/// Called when a mouse button is pressed while the cursor is positioned over the element.
		/// </summary>
		/// <param name="e">Mouse event arguments</param>
		/// <param name="adjustableArea">True if the mouse button was pressed while the cursor was positioned over the adjustable area</param>
		/// <param name="captureMouseForElement">If not null, contains a reference to the element that has captured the mouse</param>
		protected override bool OnMouseDown( MouseEventArgs e, bool adjustableArea, ref UIElement captureMouseForElement )
		{
			return this.richTextEditorEmbeddableUIElement.InternalOnMouseDown( e, adjustableArea, ref captureMouseForElement );
		}
			#endregion OnMouseDown

			#region DrawForeground
		/// <summary>
		/// Draws the element's foreground. Overridden for the purpose of
		/// drawing an image of the <see cref="EmbeddableRichTextBox"/> control
		/// when the element is not in edit mode.
		/// </summary>
		/// <param name="drawParams">The UIElementDrawParams struct that contains the drawing information.</param>
		protected override void DrawForeground(ref UIElementDrawParams drawParams)
		{
			RichTextEditorEmbeddableUIElement embeddableElement = this.richTextEditorEmbeddableUIElement;

			//	Nothing to draw when in edit mode, so return
			if ( embeddableElement.IsInEditMode )
				return;

			Rectangle rectInsideBorders = this.RectInsideBorders;
			Rectangle clipRect = this.ClipRect;
			EmbeddableRichTextBox rtfRenderer = embeddableElement.RichTextEditor.RichTextBoxRenderer;
			EmbeddableRichTextBox rtfEditor = embeddableElement.RichTextEditor.RichTextBoxEditor;

			//	Set the properties (of the EmbeddableRichTextBox that we use for rendering)
			//	that affect its visual appearance
			rtfRenderer.Size = new Size( rectInsideBorders.Width, rectInsideBorders.Height );
			rtfRenderer.Font = rtfEditor.Font;
			rtfRenderer.BackColor = rtfEditor.BackColor;
			rtfRenderer.ForeColor = rtfEditor.ForeColor;

			//	Get the value to render from the owner, and verify that it consists
			//	of  valid RTF codes. If it doesn't, but is of type string, we will
			//	display it as plain text. If the value is not of type string, we consider
			//	it to be null, and display the owner-provided null text, if any. Failing
			//	all those conditions, we display an empty string.
			object ownerValue = embeddableElement.Owner.GetValue( embeddableElement.OwnerContext );
			if ( ownerValue != null && ownerValue != DBNull.Value )
			{
				if ( rtfRenderer.IsValidRTF(ownerValue as string) == true )
					rtfRenderer.Rtf = ownerValue as string;
				else
					rtfRenderer.Text = ownerValue as string;
			}
			else
			{
				if ( embeddableElement.Owner.IsNullable(embeddableElement.OwnerContext) )
				{
					string nullText = string.Empty;
					if ( embeddableElement.Owner.GetNullText(embeddableElement.OwnerContext, out nullText) )
						rtfRenderer.Text = nullText;
					else
						rtfRenderer.Text = string.Empty;
				}
			}

			//	Draw the background...apparently InvokePaintBackground was broken in CLR2,
            //  so we must do this differently based on the environment.
            if (System.Environment.Version.Major == 1)
            {
                rtfRenderer.InternalInvokePaintBackground(drawParams.Graphics, rectInsideBorders);
            }
            else
            {
                using (SolidBrush brush = new SolidBrush(rtfRenderer.BackColor))
                {
                    drawParams.Graphics.FillRectangle(brush, rectInsideBorders);
                }
            }

			//	Create a new instance of the RichTextBoxRenderer class,
			//	which we use to render the foreground.
			RichTextBoxRenderer renderer = new RichTextBoxRenderer( rtfRenderer );
			Rectangle rect = rectInsideBorders;

			//	Tweak the position and width of the rectangle into which
			//	we render the RichTextBox to account for borders.
			rect.X += 1;
			rect.Width -= 1;

            // Due to optimizations made in 7.3, we need to offset the location of the rect in order to render
            // the content into the correct location.
            Point[] points = new Point[] { rect.Location };
            drawParams.Graphics.TransformPoints(System.Drawing.Drawing2D.CoordinateSpace.Page, System.Drawing.Drawing2D.CoordinateSpace.World, points);
            rect.Location = points[0];

			//	Render an image of the EmbeddableRichTextBox into this
			//	element's Graphics.
			renderer.Render( drawParams.Graphics, rect );
		}
			#endregion	DrawForeground

			#region DrawBackColor
		/// <summary>
		/// Draws the element's background. Overridden to prevent the background
		/// from being drawn, since we draw an image of the <see cref="EmbeddableRichTextBox"/>
		/// when the element is not in edit mode.
		/// </summary>
		protected override void DrawBackColor(ref UIElementDrawParams drawParams)
		{
		}
			#endregion DrawForeground

		#endregion Overrides
	}
	#endregion RichTextEditAreaUIElement class

}
