using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SharedLib;

namespace SpotfireIntegration
{
    public abstract class SFMenu
    {
        public const string MENU_ITEM_CONNECT = "Connect";
        public const string MENU_ITEM_DISCONNECT = "Disconnect";
        public const string MENU_ITEM_SFOPEN = "OpenAnalysis";
        public const string MENU_ITEM_SFREFRESH = "Refresh";
        public const string MENU_ITEM_AUTOREFRESH = "AutoRefresh";
        public const string MENU_ITEM_SFPROPS = "Properties";
        public const string MENU_ITEM_SFONCALLREFRESH = "OnCallRefresh";//CSBR - 150169
    }

    class SpotfireAddinMenuItem : IAddinMenuItem
    {
        string m_command, m_displayName;
        bool m_bCheckable, m_bSeparator;

        public SpotfireAddinMenuItem(string command, string displayName, 
            bool bCheckable, bool bSeparator)
        {
            m_command = command;
            m_displayName = displayName;
            m_bCheckable = bCheckable;
            m_bSeparator = bSeparator;
        }
        public string Command { get { return m_command; } }
        public string DisplayString { get { return m_displayName; } }
        public bool Checkable { get { return m_bCheckable; } }
        public bool Separator { get { return m_bSeparator; } }
    }
    class SpotfireAddinMenu : IAddinMenu
    {
        private List<IAddinMenuItem> m_items;

        public SpotfireAddinMenu()
        {
            m_items = new List<IAddinMenuItem>();
            m_items.Add(new SpotfireAddinMenuItem(SFMenu.MENU_ITEM_CONNECT, "Connect", false, false));
            m_items.Add(new SpotfireAddinMenuItem(SFMenu.MENU_ITEM_DISCONNECT, "Disconnect", false, false));
            //CSBR-151976 Here bCheckable parameter value is send as true to display the 'Open Analysis' at startup in the SF menu.
            m_items.Add(new SpotfireAddinMenuItem(SFMenu.MENU_ITEM_SFOPEN, "Open Analysis From Template...", true, false));
            m_items.Add(new SpotfireAddinMenuItem("---", "", false, true));
            m_items.Add(new SpotfireAddinMenuItem(SFMenu.MENU_ITEM_SFREFRESH, "Refresh", false, false));
            m_items.Add(new SpotfireAddinMenuItem(SFMenu.MENU_ITEM_AUTOREFRESH, "Auto-Refresh", true, false));
            m_items.Add(new SpotfireAddinMenuItem("---", "", false, true));
            m_items.Add(new SpotfireAddinMenuItem(SFMenu.MENU_ITEM_SFPROPS, "Preferences...", false, false));
        }
        public string Title { get { return "Spotfire"; } }
        public List<IAddinMenuItem> Items { get { return m_items; } }
    }
}
