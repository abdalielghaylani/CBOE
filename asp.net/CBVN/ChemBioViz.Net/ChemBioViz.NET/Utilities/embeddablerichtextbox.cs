#region Using directives

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

#endregion Using directives

namespace Infragistics.Win.EmbeddableEditors
{

    #region EmbeddableRichTextBox class
    /// <summary>
    /// RichTextBox-derived class used by the <see cref="RichTextEditor"/>
    /// to realize the in-place editing functionality.
    /// </summary>
    /// <remarks>
    /// <p class="body">
    /// The <b>EmbeddableRichTextBox</b> derives from the RichTextBox control, with little value added. Its primary purpose
    /// is to provide in-place editing functionality for the <see cref="RichTextEditor"/>. When a <see cref="RichTextEditorEmbeddableUIElement"/>
    /// is placed into edit mode, an <b>EmbeddableRichTextBox</b> is positioned over that element, and behaves exactly as a standard
    /// RichTextBox control, for the duration of the edit mode session. When the edit mode session ends, an image of the <b>EmbeddableRichTextBox</b>
    /// is rendered into the embeddable element's foreground.
    /// </p></remarks>
    /// <seealso cref="RichTextEditor"/>
    /// <seealso cref="RichTextEditorEmbeddableUIElement"/>
    public class EmbeddableRichTextBox : RichTextBox
    {
        #region Member variables

        private RichTextEditorEmbeddableUIElement richTextEditorUIElement = null;
        private bool suspendEventFiring = true;

        #endregion Member variables

        #region Constructor
        /// <summary>
        /// (Internal) Creates a new instance of the <see cref="EmbeddableRichTextBox"/> control.
        /// </summary>
        /// <remarks><p class="note">The constructor is marked internal because the EmbeddableRichTextBox
        /// is not designed for use outside the realm of embeddable editors.</p></remarks>
        internal EmbeddableRichTextBox()
        {
        }
        #endregion Constructor

        #region Methods

        #region Control-class overrides
        //
        //	The following section contains overridden implementations of
        //	virtual Control-class methods, which we use to communicate information
        //	to the EmbeddableEditorOwnerBase-derived owner.
        //
        #region OnTextChanged
        /// <summary>
        /// Raises the TextChanged event. Overridden for the purpose
        /// of firing the associated <see cref="RichTextEditor"/>'s
        /// <b>ValueChanged</b> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnTextChanged(EventArgs e)
        {
            //	If we are in edit mode, and we are not suspending event
            //	firing, raise the editor's ValueChanged event in response
            //	to the EmbeddableRichTextBox's TextChanged event.
            if (this.IsInEditMode && this.suspendEventFiring == false)
            {
                RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
                if (editor != null) //Coverity Bug Fix CID 13017 
                    editor.InternalRaiseValueChanged();
            }

            base.OnTextChanged(e);
        }
        #endregion OnTextChanged

        #region OnKeyDown
        /// <summary>
        /// Raises the KeyDown event. Overridden for the purpose
        /// of firing the associated <see cref="RichTextEditor"/>'s
        /// <b>KeyDown</b> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (this.IsInEditMode)
            {
                RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
                //Coverity Bug Fix CID :13014 
                if (editor != null)
                {
                    editor.InternalRaiseKeyDown(e);

                    //	If the owner marks the keystroke handled, we return
                    //	without calling the base implementation. This effectively
                    //	gives the owner the first chance to handle the key. An example
                    //	of this is when there exists a KeyActionMapping for the key; in
                    //	that case the owner will perform the action, and set the Handled
                    //	property of the event arguments, signifying to the editor that it
                    //	should bypass its default processing for that key.
                    if (e.Handled)
                        return;

                    //	Pass the keystroke to the editor, which might have a special
                    //	action defined for it. If it does, the action will be performed
                    //	and the editor will return true to signify that it handled the
                    //	keystroke.                                        
                    if (editor.PerformKeyAction(e.KeyData))
                        return;
                }

            }

            base.OnKeyDown(e);
        }
        #endregion OnKeyDown

        #region OnKeyUp
        /// <summary>
        /// Raises the KeyUp event. Overridden for the purpose
        /// of firing the associated <see cref="RichTextEditor"/>'s
        /// <b>KeyUp</b> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (this.IsInEditMode)
            {
                RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
                if (editor != null) //Coverity Bug Fix CID : 13016 
                {
                    editor.InternalRaiseKeyUp(e);

                    //	If the owner marks the keystroke handled, we return
                    //	without calling the base implementation. This effectively
                    //	gives the owner the first chance to handle the key. An example
                    //	of this is when there exists a KeyActionMapping for the key; in
                    //	that case the owner will perform the action, and set the Handled
                    //	property of the event arguments, signifying to the editor that it
                    //	should bypass its default processing for that key.
                    if (e.Handled)
                        return;
                }
            }

            base.OnKeyUp(e);
        }
        #endregion OnKeyDown

        #region OnKeyPress
        /// <summary>
        /// Raises the KeyUp event. Overridden for the purpose
        /// of firing the associated <see cref="RichTextEditor"/>'s
        /// <b>KeyPress</b> event.
        /// </summary>
        /// <param name="e">Event arguments.</param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (this.IsInEditMode)
            {
                RichTextEditor editor = this.richTextEditorUIElement.Editor as RichTextEditor;
                //Coverity Bug Fix CID :13015 
                if (editor != null)
                {
                    editor.InternalRaiseKeyPress(e);

                    //	If the owner marks the keystroke handled, we return
                    //	without calling the base implementation. This effectively
                    //	gives the owner the first chance to handle the key. An example
                    //	of this is when there exists a KeyActionMapping for the key; in
                    //	that case the owner will perform the action, and set the Handled
                    //	property of the event arguments, signifying to the editor that it
                    //	(the editor) should bypass its default processing for that key.
                    if (e.Handled)
                        return;
                }
            }

            base.OnKeyPress(e);
        }
        #endregion OnKeyPress

        #region IsInputKey
        /// <summary>
        /// Returns whether the specified key is a regular input key or a
        /// special key that requires preprocessing.
        /// </summary>
        /// <param name="keyData">The key to be evaluated</param>
        protected override bool IsInputKey(Keys keyData)
        {
            //	If we are not in edit mode, return the base class implementation's value
            if (!this.IsInEditMode)
                return base.IsInputKey(keyData);

            //	First, we call the base class implementation; if it returns true, we return true also.
            //	We need to do this so we take keys like the right and left arrow keys, which are
            //	standard navigational keys for a TextBox, but for which the owner might not have
            //	any mappings defined.
            //
            //	Second, we call the owner's IsKeyMapped method; if it returns true, we return true also.
            //	An example of this is the UltraGrid's TAB key handling; By default, this method would
            //	return false for the TAB key, but we want to return true if the grid does, so that the
            //	OnKeyDown method gets called, which we pass along to the grid, which in turn reacts
            //	based on the value of the TabNavigation property.
            //
            //	Finally, we call the editor's IsInputKey method, which makes the final determination
            //	as to whether the key is used.
            //
            if (base.IsInputKey(keyData))
                return true;

            if (this.richTextEditorUIElement.Owner.IsKeyMapped(keyData, this.richTextEditorUIElement))
                return true;

            return this.richTextEditorUIElement.Editor.IsInputKey(keyData);
        }

        /// <summary>
        /// Accessor for the protected IsInputKey method
        /// </summary>
        /// <param name="keyData">The key to test</param>
        /// <returns>A boolean indicating whether the key is used by the control.</returns>
        internal protected bool InternalIsInputKey(Keys keyData)
        {
            return this.IsInputKey(keyData);
        }

        #endregion IsInputKey

        #endregion Control-class overrides

        #region Helper methods

        #region EnterEditMode
        /// <summary>
        /// Places this <see cref="EmbeddableRichTextBox"/> instance into edit mode on the the specified <see cref="RichTextEditorEmbeddableUIElement"/>.
        /// </summary>
        /// <param name="richTextEditorUIElement">The <see cref="RichTextEditorEmbeddableUIElement"/> to which this instance is to be attached.</param>
        /// <returns>A boolean indicating whether the control was successfully taken into edit mode.</returns>
        public bool EnterEditMode(RichTextEditorEmbeddableUIElement richTextEditorUIElement)
        {
            //	If the specified RichTextEditorEmbeddableUIElement is null, throw an exception
            if (richTextEditorUIElement == null)
                throw new ArgumentNullException();

            //	Set the RichTextEditorEmbeddableUIElement member variable so we have
            //	a reference to the embeddable element during the edit mode
            //	session. Note that from this embeddable element, we can get
            //	a reference to the owner and the editor.
            this.richTextEditorUIElement = richTextEditorUIElement;

            //	For the properties that are not marked virtual, we cannot override
            //	them and return the owner-provided value, so we have to set them.
            this.WordWrap = this.richTextEditorUIElement.Owner.WrapText(this.richTextEditorUIElement.OwnerContext);
            this.ReadOnly = this.richTextEditorUIElement.Owner.IsReadOnly(this.richTextEditorUIElement.OwnerContext);
            this.Multiline = this.richTextEditorUIElement.Owner.IsMultiLine(this.richTextEditorUIElement.OwnerContext);

            //	Determine which, if any, scrollbars we should display
            System.Windows.Forms.ScrollBars scrollbars = this.richTextEditorUIElement.Owner.GetTextBoxScrollBars(this.richTextEditorUIElement.OwnerContext);

            //	Since the owner's GetTextBoxScrollBars method returns a value of type
            //	System.Windows.Forms.ScrollBars, and the RichTextBox's ScrollBars
            //	property is of type RichTextBoxScrollBars, we need to translate
            //	the returned value into the appropriate RichTextBoxScrollBars value.
            RichTextBoxScrollBars propertyVal = RichTextBoxScrollBars.None;
            switch (scrollbars)
            {
                case System.Windows.Forms.ScrollBars.Both: { propertyVal = RichTextBoxScrollBars.Both; } break;
                case System.Windows.Forms.ScrollBars.Horizontal: { propertyVal = RichTextBoxScrollBars.Horizontal; } break;
                case System.Windows.Forms.ScrollBars.None: { propertyVal = RichTextBoxScrollBars.None; } break;
                case System.Windows.Forms.ScrollBars.Vertical: { propertyVal = RichTextBoxScrollBars.Vertical; } break;
            }
            this.ScrollBars = propertyVal;

            //	Ensure visibility
            if (this.Visible == false)
                this.Visible = true;

            //	Parent this EmbeddableRichTextBox to the owner-provided control
            Control owningControl = this.richTextEditorUIElement.Owner.GetControl(this.richTextEditorUIElement.OwnerContext);
            if (owningControl != null)
                owningControl.Controls.Add(this);

            //	Now that we are fully in edit mode, allow events to fire
            this.suspendEventFiring = false;

            //	Take focus, and return false if we were unable to
            return this.Focus();
        }
        #endregion EnterEditMode

        #region ExitEditMode
        /// <summary>
        /// Takes this <see cref="EmbeddableRichTextBox"/> instance out of edit mode.
        /// </summary>
        public void ExitEditMode()
        {
            //	Since we are exiting edit mode, suspend event firing
            this.suspendEventFiring = true;

            //	Clear the value
            this.Text = string.Empty;

            //	Set focus to the owner-provided control
            Control owningControl = this.richTextEditorUIElement.Owner.GetControl(this.richTextEditorUIElement.OwnerContext);
            if (owningControl != null)
                owningControl.Focus();

            //	Hide the EmbeddableRichTextBox
            this.Visible = false;

            //	Nullify the RichTextEditorEmbeddableUIElement member variable
            this.richTextEditorUIElement = null;
        }
        #endregion ExitEditMode

        #region IsValidRTF
        /// <summary>
        /// Returns whether the specified value constitutes a valid RTF (Rich Text Format) string.
        /// </summary>
        /// <param name="rtf">The RTF value to test.</param>
        /// <returns>A boolean indicating whether the specified value constitutes a valid RTF (Rich Text Format) string.</returns>
        public bool IsValidRTF(string rtf)
        {
            string previousValue = this.Rtf;
            try
            {
                this.Rtf = rtf;
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                this.Rtf = previousValue;
            }
        }
        #endregion IsValidRTF

        #region InternalInvokePaintBackground
        /// <summary>
        /// Accessor for the protected InvokePaintBackground method.
        /// </summary>
        /// <param name="g">The Graphics object onto which the control's background will be rendered.</param>
        internal void InternalInvokePaintBackground(Graphics g, Rectangle rect)
        {
            //Coverity bug Fix CID 13152 
            using (PaintEventArgs e = new PaintEventArgs(g, rect))
            {
                this.InvokePaintBackground(this, e);
            }
        }
        #endregion InternalInvokePaintBackground

        #endregion Helper methods

        #endregion Methods

        #region Public Properties

        //	For the properties that correspond directly to EmbeddableEditorOwnerBase
        //	class methods, we override them and return the owner-provided value. Note
        //	that some of these properties are not virtual, and as such we have to set
        //	them when we enter edit mode.

        #region BackColor
        /// <summary>
        /// Override the BackColor property so we can return the value
        /// obtained via the EmbeddableEditorOwnerBase-derived owner's
        /// ResolveAppearance method.
        /// </summary>
        public override Color BackColor
        {
            get
            {
                //	If not in edit mode, return the base class value.
                if (this.IsInEditMode == false)
                    return base.BackColor;

                //	Call the owner's ResolveAppearance method; if the owner does not
                //	resolve the appearance, we will return the base class value; otherwise
                //	we return the value provided by the owner.
                AppearancePropFlags requestedProps = AppearancePropFlags.BackColor;
                AppearanceData appearance = new AppearanceData();
                bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(this.richTextEditorUIElement.OwnerContext,
                                                                                        ref appearance,
                                                                                        ref requestedProps);

                if (!resolved)
                    return base.BackColor;

                Color backColor = appearance.BackColor;
                if (backColor.A < 255)
                    backColor = Color.FromArgb(255, backColor);

                return backColor;
            }

            //	Do nothing; we want this property to be read-only,
            //	and always return the value as dictated by the owner.
            set { }
        }
        #endregion BackColor

        #region ForeColor
        /// <summary>
        /// Override the ForeColor property so we can return the value
        /// obtained via the EmbeddableEditorOwnerBase-derived owner's
        /// ResolveAppearance method.
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                //	If not in edit mode, return the base class value.
                if (this.IsInEditMode == false)
                    return base.ForeColor;

                //	Call the owner's ResolveAppearance method; if the owner does not
                //	resolve the appearance, we will return the base class value; otherwise
                //	we return the value provided by the owner.
                AppearancePropFlags requestedProps = AppearancePropFlags.ForeColor;
                AppearanceData appearance = new AppearanceData();
                bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(this.richTextEditorUIElement.OwnerContext,
                                                                                        ref appearance,
                                                                                        ref requestedProps);

                if (!resolved)
                    return base.ForeColor;

                Color foreColor = appearance.ForeColor;
                if (foreColor.A < 255)
                    foreColor = Color.FromArgb(255, foreColor);

                return foreColor;
            }

            //	Do nothing; we want this property to be read-only,
            //	and always return the value as dictated by the owner.
            set { }
        }
        #endregion ForeColor

        #region Cursor
        /// <summary>
        /// Override the Cursor property so we can return the value
        /// obtained via the EmbeddableEditorOwnerBase-derived owner's
        /// ResolveAppearance method.
        /// </summary>
        public override Cursor Cursor
        {
            get
            {
                //	If not in edit mode, return the base class value.
                if (this.IsInEditMode == false)
                    return base.Cursor;

                //	Call the owner's ResolveAppearance method; if the owner does not
                //	resolve the appearance, we will return the base class value; otherwise
                //	we return the value provided by the owner.
                AppearancePropFlags requestedProps = AppearancePropFlags.Cursor;
                AppearanceData appearance = new AppearanceData();
                bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(this.richTextEditorUIElement.OwnerContext,
                                                                                        ref appearance,
                                                                                        ref requestedProps);

                if (!resolved || appearance.Cursor == null)
                    return base.Cursor;

                return appearance.Cursor;
            }

            set { base.Cursor = value; }
        }
        #endregion Cursor

        #region Font
        /// <summary>
        /// Override the Font property so we can return the value
        /// obtained via the EmbeddableEditorOwnerBase-derived owner's
        /// ResolveAppearance method.
        /// </summary>
        public override Font Font
        {
            get
            {
                //	If not in edit mode, return the base class value.
                if (this.IsInEditMode == false)
                    return base.Font;

                //	Call the owner's ResolveAppearance method; if the owner does not
                //	resolve the appearance, we will return the base class value; otherwise
                //	we return the value provided by the owner.
                AppearancePropFlags requestedProps = AppearancePropFlags.FontData;
                AppearanceData appearance = new AppearanceData();
                bool resolved = this.richTextEditorUIElement.Owner.ResolveAppearance(this.richTextEditorUIElement.OwnerContext,
                                                                                        ref appearance,
                                                                                        ref requestedProps);

                if (!resolved)
                    return base.Font;

                Font createdFont = appearance.CreateFont(base.Font);
                if (createdFont != null)
                    return createdFont;
                else
                    return base.Font;
            }

            set { }
        }
        #endregion Font

        #region MaxLength
        /// <summary>
        /// Override the MaxLength property so we can return the value
        /// specified by the EmbeddableEditorOwnerBase-derived owner.
        /// </summary>
        public override int MaxLength
        {
            get
            {
                //	If not in edit mode, return the base class value.
                if (this.IsInEditMode == false)
                    return base.MaxLength;

                //	Try to get the value from the owner; if they return a meaningful value,
                //	that is what we will use for the control; otherwise, return the value of
                //	the control's MaxLength property.
                int maxLength = base.MaxLength;
                if (this.richTextEditorUIElement.Owner.GetMaxLength(this.richTextEditorUIElement.OwnerContext, out maxLength))
                    return maxLength;
                else
                    return base.MaxLength;
            }

            //	Do nothing; we want this property to be read-only,
            //	and always return the value as dictated by the owner.
            set { }
        }
        #endregion MaxLength

        #endregion Public Properties

        #region Private properties

        #region IsInEditMode
        /// <summary>
        /// Returns whether this <see cref="EmbeddableRichTextBox"/> instance is currently in edit mode.
        /// </summary>
        private bool IsInEditMode
        {
            get
            {
                return this.richTextEditorUIElement != null &&
                        this.richTextEditorUIElement.IsInEditMode;
            }
        }
        #endregion IsInEditMode

        #endregion Private properties
    }
    #endregion EmbeddableRichTextBox class

    #region RichTextBoxRenderer class
    /// <summary>
    /// Class used to render the contents of a <see cref="System.Windows.Forms.RichTextBox"/>
    /// </summary>
    public class RichTextBoxRenderer
    {
        #region Constants

        private const int WM_USER = 0x400;
        private const int EM_FORMATRANGE = WM_USER + 57;
        private const int EM_DISPLAYBAND = WM_USER + 51;
        private const float twipsPerInch = 1440f;

        #endregion	Constants

        #region Member Variables

        private RichTextBox richTextBox;
        private int currentCharacter;
        private int nextCharacterToRender;
        private int textLength;
        private float pixelsPerInchX = 96;
        private float pixelsPerInchY = 96;

        #endregion	Member Variables

        #region Constructor
        /// <summary>
        /// Creates a new instance of the <see cref="RichTextBoxRenderer"/> class
        /// </summary>
        /// <param name="richTextBox">The <see cref="System.Windows.Forms.RichTextBox"/> control to be rendered.</param>
        public RichTextBoxRenderer(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;
            this.textLength = this.richTextBox.Text.Length;
        }
        #endregion	Constructor

        #region Properties

        #region RichTextBox
        /// <summary>
        /// Returns the <see cref="System.Windows.Forms.RichTextBox"/> that will be rendered.
        /// </summary>
        public RichTextBox RichTextBox
        {
            get { return this.richTextBox; }
        }
        #endregion	RichTextBox

        #endregion	Properties

        #region Methods

        #region Render
        /// <summary>
        /// Renders an image of the <see cref="RichTextBox"/> onto the specified Graphics object.
        /// </summary>
        /// <param name="g">The Graphics object onto which the control's image is to be rendered.</param>
        /// <param name="rect">The Rectangle describing the location of the image.</param>
        public void Render(Graphics g, Rectangle rect)
        {
            //	If nothing to render, return
            if (this.richTextBox == null)
                return;

            //	Get the resolution from the Graphics object
            this.pixelsPerInchX = g.DpiX;
            this.pixelsPerInchY = g.DpiY;

            //	Initialize the currentCharacter / nextCharacterToRender / textLength members
            this.currentCharacter = this.nextCharacterToRender = 0;
            this.textLength = this.richTextBox.Text.Length;

            int savedDcState = 0;
            IntPtr clipRegionHandle = IntPtr.Zero;
            IntPtr hdc = IntPtr.Zero;
            try
            {
                // Set the clip region on the graphics object so that the RichTextBox will
                // not render outside of the bounds of the rect
                g.SetClip(rect);

                // Get a handle to the clip region since GetHdc on the graphic object will
                // not take the clip region into account.
                clipRegionHandle = g.Clip.GetHrgn(g);

                // Get the handle to the graphic object
                hdc = g.GetHdc();

                // Save the state of the graphics object so that we can restore it later
                savedDcState = NativeWindowMethods.SaveDC(hdc);

                // Set the clip region on the hdc that the RichTextBox will render to
                NativeWindowMethods.SelectClipRgn(hdc, clipRegionHandle);

                //	Continue until all text is processed
                while (this.nextCharacterToRender < this.textLength)
                {
                    this.nextCharacterToRender = this.currentCharacter;
                    this.currentCharacter = this.RenderingHelper(hdc, g, rect);
                }
            }
            finally
            {
                if (hdc != IntPtr.Zero && savedDcState != 0)
                    NativeWindowMethods.RestoreDC(hdc, savedDcState);

                g.ReleaseHdc(hdc);

                g.ResetClip();

                if (clipRegionHandle != IntPtr.Zero)
                    NativeWindowMethods.DeleteObject(clipRegionHandle);
            }

        }
        #endregion Render

        #region RenderingHelper
        /// <summary>
        /// Renders the contents of the <see cref="RichTextBox"/> onto the specified Graphics object.
        /// </summary>
        /// <param name="g">The Graphics object onto which the <see cref="RichTextBox"/>'s contents will be rendered.</param>
        private int RenderingHelper(IntPtr hdc, Graphics g, Rectangle rect)
        {
            CHARRANGE cr = new CHARRANGE();
            cr.cpMin = this.currentCharacter;
            cr.cpMax = this.textLength;


            RECT rc = new RECT();
            rc.left = rect.Left;
            rc.top = rect.Top;
            rc.right = rect.Right;
            rc.bottom = rect.Bottom;

            float scalingFactorX = this.pixelsPerInchX;
            float scalingFactorY = this.pixelsPerInchY;
            rc.left = (int)(rect.Left / scalingFactorX * twipsPerInch);
            rc.top = (int)(rect.Top / scalingFactorY * twipsPerInch);
            rc.right = (int)(rect.Right / scalingFactorX * twipsPerInch);

            //	Make the rect's height larger than needed, to prevent
            //	the rendering from wrapping aound to the top in the case
            //	when the content does not fully fit into the cell's bounds
            rc.bottom = (int)(short.MaxValue / scalingFactorY * twipsPerInch);

            RECT rcPage = new RECT();

            IntPtr hwnd = this.richTextBox.Handle;
            HandleRef handleRefHwnd = new HandleRef(this.richTextBox, hwnd);
            HandleRef handleRefHdc = new HandleRef(g, hdc);

            FORMATRANGE fr = new FORMATRANGE();
            fr.hdc = hdc;
            fr.hdcTarget = hdc;
            fr.chrg = cr;
            fr.rc = rc;
            fr.rcPage = rcPage;

            IntPtr nextCharacterToRender = SendMessage(handleRefHwnd, EM_FORMATRANGE, IntPtr.Zero, ref fr);

            SendMessage(handleRefHwnd, EM_DISPLAYBAND, IntPtr.Zero, ref fr.rc);

            SendMessage(handleRefHwnd, EM_FORMATRANGE, IntPtr.Zero, IntPtr.Zero);

            return nextCharacterToRender.ToInt32();
        }
        #endregion	RenderingHelper

        #endregion	Methods

        #region APIs

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, ref FORMATRANGE range);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, ref RECT rect);

        #endregion	APIs

        #region Nested Types

        #region RECT
        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion	RECT

        #region CHARRANGE
        [StructLayout(LayoutKind.Sequential)]
        internal struct CHARRANGE
        {
            public int cpMin;
            public int cpMax;
        }
        #endregion	CHARRANGE

        #region FORMATRANGE
        [StructLayout(LayoutKind.Sequential)]
        internal struct FORMATRANGE
        {
            public IntPtr hdc;
            public IntPtr hdcTarget;
            public RECT rc;
            public RECT rcPage;
            public CHARRANGE chrg;
        }
        #endregion	FORMATRANGE

        #endregion	Nested Types
    }
    #endregion RichTextBoxRenderer class

}

