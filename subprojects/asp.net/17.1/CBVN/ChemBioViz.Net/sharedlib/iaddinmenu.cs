using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace SharedLib
{
    public interface ICBVAddin
    {
        void Init(object form);                     // just passes form to addin; signature method which identifies cbv addin
        void Execute();                             // take default addin action
        void ExecuteWithString(string s);           // take other addin action; s might be filename, keyword, etc.
        void Deinit();                              // break connection between form and addin
        string GetDescription();                    // return one-line addin description for user info

        IAddinMenu GetMenu();                       // return menu definition
        bool HandleMenuCommand(string menuCmd);     // process user click on command
        bool IsEnabled(string menuCmd);             // return true if menu item should be enabled
        bool IsChecked(string menuCmd);             // return true if menu item is checkable and should be checked
        string GetMenuImagePath();                  // state-dependent image for main menu; return as temporary pathname

        string GetSettings();                       // return string of prop values; format is private to the addin 
        void SetSettings(string s);                 // load string into props
    }
    //---------------------------------------------------------------------
    public interface IAddinMenuItem
    {
        string Command { get; }                     // string to be passed back on click
        string DisplayString { get; }               // string to be displayed on menu
        bool Checkable { get; }                     // return true if menu item is to be checkable (state button)
        bool Separator { get; }                     // return true if item is separator; if true, other members are ignored
    }
    //---------------------------------------------------------------------
    public interface IAddinMenu
    {
        string Title { get; }                       // title to be displayed on main menu bar
        List<IAddinMenuItem> Items { get; }         // the menu items
    }
    //---------------------------------------------------------------------
}
