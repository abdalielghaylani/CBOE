/*  GREATIS FORM DESIGNER FOR .NET
 *  Private Designer Interface Implementation
 *  Copyright (C) 2004-2007 Greatis Software
 *  http://www.greatis.com/dotnet/formdes/
 *  http://www.greatis.com/bteam.html
 */

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Runtime.InteropServices;

using System.Diagnostics;

using System.IO;
using System.Collections.Specialized;
using System.Reflection;

using System.Runtime.Remoting;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Greatis
{
   namespace FormDesigner
   {
      #region TabOrderHelper
      // controls ordered: 
      // Ci, Cj i<j ==> 
      // Ci.TabIndex < Cj.TabIndex || (Ci.TabIndex == Cj.TabIndex && i<j)
      class TabOrderHelper
      {
         TabOrderHelper() { }

         // return the first control in the tab order (minimum of the tab index)
         public static Control First(Control.ControlCollection controls)
         {
            if (controls.Count == 0)
               return null;

            int index = controls[0].TabIndex;
            int item = 0;
            for (int i = 1; i < controls.Count; i++)
            {
               if (controls[i].TabIndex < index)
               {
                  item = i;
                  index = controls[i].TabIndex;
               }
            }
            return controls[item];
         }

         // return the last control in the tab order (maximum of the tab index)
         public static Control Last(Control.ControlCollection controls)
         {
            if (controls.Count == 0)
               return null;

            int index = controls[0].TabIndex;
            int item = 0;
            for (int i = 1; i < controls.Count; i++)
            {
               if (controls[i].TabIndex >= index)
               {
                  item = i;
                  index = controls[i].TabIndex;
               }
            }
            return controls[item];
         }

         // return minimum of the TabIndex in the ordered set, (TabOrdered)
         // where controls before the current, and the current itself is excluded
         public static Control Next(Control current, Control.ControlCollection controls)
         {
            int item = controls.IndexOf(current), findItem = -1;
            int index = current.TabIndex, curIndex = 0;
            bool init = false;

            for (int i = 0; i < controls.Count; i++)
            {
               if (controls[i].TabIndex < index)
                  continue;
               if (controls[i].TabIndex == index && i <= item)
                  continue;

               if (controls[i].TabIndex == index) // i >= item
               {
                  findItem = i;
                  break;
               }

               if (init == false)
               {
                  init = true;
                  curIndex = controls[i].TabIndex;
                  findItem = i;
                  continue;
               }

               if (controls[i].TabIndex < curIndex)
               {
                  curIndex = controls[i].TabIndex;
                  findItem = i;
               }
            }

            return (findItem < 0) ? First(controls) : controls[findItem];
         }

         // return maximum of the TabIndex in the ordered set, (TabOrdered)
         // where controls after the current, and the current itself is excluded
         public static Control Previous(Control current, Control.ControlCollection controls)
         {
            int item = controls.IndexOf(current), findItem = -1;
            int index = current.TabIndex, curIndex = 0;
            bool init = false;

            for (int i = 0; i < controls.Count; i++)
            {
               if (controls[i].TabIndex > index)
                  continue;
               if (controls[i].TabIndex == index && i >= item)
                  continue;

               if (controls[i].TabIndex == index)
               {
                  findItem = i;
                  break;
               }

               if (init == false)
               {
                  init = true;
                  curIndex = controls[i].TabIndex;
                  findItem = i;
                  continue;
               }

               if (controls[i].TabIndex > curIndex)
               {
                  curIndex = controls[i].TabIndex;
                  findItem = i;
               }
            }

            return (findItem < 0) ? Last(controls) : controls[findItem];
         }
      }
      #endregion

      #region Root Designer
      class RootDesigner : DocumentDesigner
      {
         public RootDesigner()
         {
         }
         protected override void Dispose(bool disposing)
         {
            EventFilter ef = (EventFilter)GetService(typeof(EventFilter));
            ef.KeyDown -= new KeyEventHandler(KeyListner);
            base.Dispose(disposing);
         }

         private void SelectNext(bool next)
         {
            ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
            if (ss == null)
               return;

            IDesignerHost dh = (IDesignerHost)GetService(typeof(IDesignerHost));
            Control ds = dh.RootComponent as Control;
            if (ds == null)
               return;

            Control control = ss.PrimarySelection as Control;
            if (control == null)
            {
               if (next == true)
                  control = TabOrderHelper.First(ds.Controls);
               else
               {
                  control = ds;
                  while (control.Controls.Count != 0)
                  {
                     control = TabOrderHelper.Last(control.Controls);
                     if (control is ContainerControl)
                        break;
                  }
               }
            }
            else
            {
               bool parentChange = false;
               Control parent = control.Parent;
               if (next == false)
               {
                  if (control.TabIndex == 0 && parent != ds)
                  {
                     control = parent;
                     parent = parent.Parent;
                  }
                  else
                  {
                     control = TabOrderHelper.Previous(control, parent.Controls);
                     while (control != null && control.Controls.Count != 0 &&
                        !(control is ContainerControl))
                     {
                        control = TabOrderHelper.Last(control.Controls);
                     }
                  }
               }
               else
               {
                  if (control.Controls.Count != 0 && !(control is ContainerControl))
                     control = TabOrderHelper.First(control.Controls);
                  else
                  {
                     while (control == TabOrderHelper.Last(parent.Controls) &&
                        parent != ds)
                     {
                        control = parent;
                        parent = parent.Parent;
                        parentChange = true;
                     }

                     if (control.Controls.Count == 0 || (control is ContainerControl) ||
                        parentChange == true)
                     {
                        control = TabOrderHelper.Next(control, parent.Controls);
                     }
                  }
               }
            }
            if (control != null)
            {
               Control[] selected = new Control[1] { control };
               ss.SetSelectedComponents(selected, SelectionTypes.Replace);
            }
         }

         private void KeyListner(object sender, KeyEventArgs e)
         {
            if (e.KeyValue == (int)Keys.Tab)
            {
               SelectNext((e.Modifiers & Keys.Shift) == 0);
            }
            else
            {
               Point amount;
               bool shiftByPixel = false;

               Designer designer = (Designer)GetService(typeof(Designer));
               if (designer != null && designer.SnapToGrid == false)
                  shiftByPixel = true;

               if ((e.Modifiers & Keys.Control) != 0)
                  shiftByPixel = !shiftByPixel;

               switch (e.KeyValue)
               {
                  case (int)Keys.Left:
                     amount = new Point((shiftByPixel == true) ? -1 : -GridSize.Width, 0);
                     break;

                  case (int)Keys.Right:
                     amount = new Point((shiftByPixel == true) ? 1 : GridSize.Width, 0);
                     break;

                  case (int)Keys.Up:
                     amount = new Point(0, (shiftByPixel == true) ? -1 : -GridSize.Height);
                     break;

                  case (int)Keys.Down:
                     amount = new Point(0, (shiftByPixel == true) ? 1 : GridSize.Height);
                     break;

                  default:
                     return;
               }

               if ((e.Modifiers & Keys.Shift) != 0)
                  ResizeSelection(amount);
               else
                  MoveSelection(amount);
            }
         }

         delegate void ActionDelegate(Control c, Point p);
         private void MoveOrResize(Point delta, ActionDelegate action)
         {
            ISelectionService ss = (ISelectionService)GetService(typeof(ISelectionService));
            if (ss == null)
               return;

            IComponentChangeService ics = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            ICollection selection = ss.GetSelectedComponents();
            if (selection.Count > 0)
            {
               Control primary = ss.PrimarySelection as Control;
               if (primary != null)
               {
                  foreach (object obj in selection)
                  {
                     Control control = obj as Control;
                     if (control != null && control.Parent == primary.Parent)
                     {
                        ics.OnComponentChanging(control, null);
                        action(control, delta);
                        ics.OnComponentChanged(control, null, null, null);
                     }
                  }
               }
            }
         }

         private void SetControlProperty(Control control, object value, string propertyName)
         {
            ISite site = control.Site;
            if (site == null)
               return;

            PropertyDescriptor md = TypeDescriptor.GetProperties(control)[propertyName];
            if (md != null)
            {
               IDesignerHost dh = (IDesignerHost)site.GetService(typeof(IDesignerHost));
               DesignerTransaction dt = dh.CreateTransaction("Change " + propertyName
                  + " " + control.Name);

               if (dt == null)
                  md.SetValue(control, value);
               else
               {
                  using (dt)
                  {
                     md.SetValue(control, value);
                     dt.Commit();
                  }
               }
            }
         }

         void ResizeControl(Control control, Point delta)
         {
            Size size = control.Size;
            size.Width += delta.X;
            size.Height += delta.Y;

            SetControlProperty(control, size, "Size");
         }

         void MoveControl(Control control, Point delta)
         {
            Point location = control.Location;
            location.X += delta.X;
            location.Y += delta.Y;

            SetControlProperty(control, location, "Location");
         }

         private void ResizeSelection(Point delta)
         {
            MoveOrResize(delta, new ActionDelegate(ResizeControl));
         }

         private void MoveSelection(Point delta)
         {
            MoveOrResize(delta, new ActionDelegate(MoveControl));
         }

         protected override void OnCreateHandle()
         {
            base.OnCreateHandle();

            EventFilter ef = (EventFilter)GetService(typeof(EventFilter));
            ef.KeyDown += new KeyEventHandler(KeyListner);
         }
      }
      #endregion

      #region Root Control
      [Designer(typeof(RootDesigner), typeof(IRootDesigner))]
      class RootControl : UserControl
      {
         private Control m_SavedParent;
         private Control m_DesignedControl;

         public RootControl()
         {
            // save parent for checking parent resizing
            m_SavedParent = this.Parent;
            if( m_SavedParent != null )
               m_SavedParent.SizeChanged += new EventHandler(OnParentResize);
         }

         protected override void Dispose(bool disposing)
         {
            if (m_SavedParent != null)
               m_SavedParent.SizeChanged -= new EventHandler(OnParentResize);

            base.Dispose(disposing);
         }

         protected override void OnLocationChanged(EventArgs e)
         {
            if (!AutoScroll && (Left != 0 || Top != 0))
            {
               Location = new Point(0, 0);
               ScrollableControl prnt = Parent as ScrollableControl;
               if (prnt != null && (prnt.AutoScrollPosition.X != 0 || prnt.AutoScrollPosition.Y != 0))
                  prnt.AutoScrollPosition = new Point(0, 0);
            }
            base.OnLocationChanged(e);
         }

         // adding DesignSurface into services
         override public ISite Site
         {
            get { return base.Site; }
            set { base.Site = value; }
         }

         protected override void OnParentChanged(EventArgs e)
         {
            if (m_SavedParent != null)
               m_SavedParent.SizeChanged -= new EventHandler(OnParentResize);

            m_SavedParent = Parent;
            if (Parent != null)
            {
               Parent.SizeChanged += new EventHandler(OnParentResize);
               // reset autoscroll for parent (it's a root view control)
               if (Parent is ScrollableControl)
                  ((ScrollableControl)Parent).AutoScroll = false;
            }

            base.OnParentChanged(e);
         }

         internal Control DesignedControl
         {
            set
            {
               if (m_DesignedControl != null)
               {
                  m_DesignedControl.BackColorChanged -= new EventHandler(FormBackColorChanged);
                  m_DesignedControl.BackgroundImageChanged -= new EventHandler(FormBackgroundImageChanged);
                  m_DesignedControl.BackgroundImageLayoutChanged -= new EventHandler(FormBackgroundImageLayoutChanged);
                  m_DesignedControl.FontChanged -= new EventHandler(FormFontChanged);
                  m_DesignedControl.ForeColorChanged -= new EventHandler(FormForeColorChanged);

                  // restore autoscroll
                  if (m_DesignedControl is ScrollableControl)
                     ((ScrollableControl)m_DesignedControl).AutoScroll = AutoScroll;
               }

               if (value != null)  // set properties from the form
               {
                  // check for transparent color
                  Control current = value;
                  while (current != null && current.BackColor == Color.Transparent)
                     current = current.Parent;

                  if (current != null)
                     BackColor = current.BackColor;
                  else
                     BackColor = SystemColors.Control;

                  BackgroundImage = value.BackgroundImage;
                  BackgroundImageLayout = value.BackgroundImageLayout;
                  Font = value.Font;
                  ForeColor = value.ForeColor;
                  

                  value.BackColorChanged += new EventHandler(FormBackColorChanged);
                  value.BackgroundImageChanged += new EventHandler(FormBackgroundImageChanged);
                  value.BackgroundImageLayoutChanged += new EventHandler(FormBackgroundImageLayoutChanged);
                  value.FontChanged += new EventHandler(FormFontChanged);
                  value.ForeColorChanged += new EventHandler(FormForeColorChanged);

                  // set autoscroll property to surface from designed control
                  if (value is ScrollableControl)
                  {
                     AutoScroll = ((ScrollableControl)value).AutoScroll;
                     ((ScrollableControl)value).AutoScroll = false;
                  }
               }

               m_DesignedControl = value;
            }
         }

         private void FormForeColorChanged(object sender, EventArgs e)
         {
            ForeColor = m_DesignedControl.ForeColor;
         }

         private void FormFontChanged(object sender, EventArgs e)
         {
            Font = m_DesignedControl.Font;
         }

         private void FormBackgroundImageChanged(object sender, EventArgs e)
         {
            BackgroundImage = m_DesignedControl.BackgroundImage;
         }

         private void FormBackgroundImageLayoutChanged(object sender, EventArgs e)
         {
            BackgroundImageLayout = m_DesignedControl.BackgroundImageLayout;
         }

         private void FormBackColorChanged(object sender, EventArgs e)
         {
            BackColor = m_DesignedControl.BackColor;
         }

         void OnParentResize(object o, EventArgs e)
         {
             SuspendLayout();

             ISelectionService ss = (ISelectionService)Site.GetService(typeof(ISelectionService));
             ICollection selected = ss.GetSelectedComponents();

             // //CSBR-118056: Controls are not woking in Edit Mode when CBV is maximized
             //if (selected.Count > 0)
             //    ss.SetSelectedComponents(null);

             Size = ((Control)o).ClientSize;
             Location = new Point(0, 0);

             if (selected.Count > 0)
                 ss.SetSelectedComponents(selected);

             ResumeLayout();
         }
      }
      #endregion
   }
}
