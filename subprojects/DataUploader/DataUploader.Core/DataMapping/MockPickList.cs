using System.Collections.Generic;

namespace CambridgeSoft.COE.DataLoader.Core.DataMapping
{
    /// <summary>
    /// Serves as the temporary mock PickList business object, before we really implement the
    /// functions to translate value to id.
    /// </summary>
    public class MockPickList
    {
        public int PickListID = 0;
        public string Description = string.Empty;
        public Dictionary<int, string> Items = null;

        public MockPickList()
        {
            Items = new Dictionary<int, string>();
        }
    }

    public class MockPickListDomain
    {
        private static Dictionary<int, MockPickList> _pickLists = null;

        static MockPickListDomain()
        {
            // Ideally, the picklist domain should contain multiple picklists and be responsible for
            // initializing, caching and retrieving any requested picklist.
            // Here we construct a couple of ones for mocking purpose.
            _pickLists = new Dictionary<int, MockPickList>();

            MockPickList pickList = new MockPickList();
            
            pickList.PickListID = 1;
            pickList.Description = "Scientists";
            pickList.Items.Add(1, "Andy");
            pickList.Items.Add(2, "Terry");
            pickList.Items.Add(3, "Robert");

            _pickLists.Add(pickList.PickListID, pickList);

            pickList.PickListID = 2;
            pickList.Description = "Projects";
            pickList.Items.Add(1, "Mercury");
            pickList.Items.Add(2, "Venus");
            pickList.Items.Add(3, "Mars");

            _pickLists.Add(pickList.PickListID, pickList);
        }

        public static int GetIdByValue(int pickListId, string value)
        {
            int id = -1;

            foreach (KeyValuePair<int, string> kvp in _pickLists[pickListId].Items)
            {
                if (string.Compare(kvp.Value, value, true) == 0)
                {
                    return kvp.Key;
                }
            }

            return id;
        }
    }
}
