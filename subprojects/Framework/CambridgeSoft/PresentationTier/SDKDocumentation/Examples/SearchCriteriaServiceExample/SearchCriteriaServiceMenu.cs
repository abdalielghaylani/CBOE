using System;
using System.Collections.Generic;
using System.Text;
using SearchCriteriaServiceExample.BLL;
using CambridgeSoft.COE.Framework.COESearchCriteriaService;
using CambridgeSoft.COE.Framework;

namespace SearchCriteriaServiceExample
{
    public class SearchCriteriaServiceMenu
    {
        private SearchCriteriaService _searchCriteriaService = new SearchCriteriaService("CSSADMIN", "COEDB", 0, 0);

        public SearchCriteriaServiceMenu() { }

        public void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to Search Criteria Service Example\n");
            Console.WriteLine("Select the option\n");
            Console.WriteLine("1 - Get all recent search criterias");
            Console.WriteLine("2 - Get some recent search criterias");
            Console.WriteLine("3 - Get saved search criterias");
            Console.WriteLine("4 - Save a recent search criteria");
            Console.WriteLine("5 - EXIT\n");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    Console.Clear();
                    Console.WriteLine("Recent SearchCriteria List\n");
                    Console.WriteLine("ID\t NAME\t DATECREATED\n");
                    foreach (COESearchCriteriaBO searchcriteria in this._searchCriteriaService.RecentSearchCriteriaList)
                    {
                        Console.WriteLine(searchcriteria.ID.ToString() + "\t" + searchcriteria.Name + "\t" + searchcriteria.DateCreated.ToString());
                    }
                    Console.WriteLine("\nPress Enter to continue");
                    Console.ReadLine();
                    MainMenu();
                    break;
                case "2":
                    Console.Clear();
                    Console.WriteLine("Select quantity to retrieve:\n");
                    string quantity = Console.ReadLine();
                    string byFromGroup = string.Empty;

                    while (byFromGroup != "Y" && byFromGroup != "N")
                    {
                        Console.WriteLine("Get by FormGroup? y/n\n");
                        byFromGroup = Console.ReadLine();
                        byFromGroup = byFromGroup.ToUpper();
                        if (byFromGroup != "Y" && byFromGroup != "N")
                        {
                            Console.Clear();
                            Console.WriteLine("Selected Option is not correct\n");
                            Console.WriteLine("Press Enter to continue...\n");
                            Console.ReadLine();
                            Console.Clear();
                        }
                    }

                    if (byFromGroup == "Y")
                    {
                        _searchCriteriaService.GetSomeRecentSearchCriteria(Int32.Parse(quantity), true);

                    }
                    else
                    {
                        _searchCriteriaService.GetSomeRecentSearchCriteria(Int32.Parse(quantity), false);
                    }
                    Console.Clear();
                    Console.WriteLine("Recent SearchCriteria List\n");
                    Console.WriteLine("ID\t NAME\t DATECREATED\n");
                    foreach (COESearchCriteriaBO searchcriteria in this._searchCriteriaService.RecentSearchCriteriaList)
                    {
                        Console.WriteLine(searchcriteria.ID.ToString() + "\t" + searchcriteria.Name + "\t" + searchcriteria.DateCreated.ToString());
                    }
                    Console.WriteLine("\nPress Enter to continue");
                    Console.ReadLine();
                    MainMenu();
                    break;
                case "3":
                    Console.Clear();
                    string byFromGroup2 = string.Empty;

                    while (byFromGroup2 != "Y" && byFromGroup2 != "N")
                    {
                        Console.WriteLine("Get by FormGroup? y/n\n");
                        byFromGroup2 = Console.ReadLine();
                        byFromGroup2 = byFromGroup2.ToUpper();
                        if (byFromGroup2 != "Y" && byFromGroup2 != "N")
                        {
                            Console.Clear();
                            Console.WriteLine("Selected Option is not correct\n");
                            Console.WriteLine("Press Enter to continue...\n");
                            Console.ReadLine();
                            Console.Clear();
                        }
                    }

                    if (byFromGroup2 == "Y")
                    {
                        _searchCriteriaService.GetSavedSearchCriteriaByFormGroup();

                    }
                    Console.WriteLine("Saved SearchCriteria List\n");
                    Console.WriteLine("ID\t NAME\t DATECREATED\n");
                    foreach (COESearchCriteriaBO searchcriteria in this._searchCriteriaService.RecentSearchCriteriaList)
                    {
                        Console.WriteLine(searchcriteria.ID.ToString() + "\t" + searchcriteria.Name + "\t" + searchcriteria.DateCreated.ToString());
                    }
                    Console.WriteLine("\nPress Enter to continue");
                    Console.ReadLine();
                    MainMenu();
                    break;
                case "4":
                    Console.Clear();
                    Console.WriteLine("Recent SearchCriteria List\n");
                    Console.WriteLine("ID\t NAME\t DATECREATED\n");
                    foreach (COESearchCriteriaBO searchcriteria in this._searchCriteriaService.RecentSearchCriteriaList)
                    {
                        Console.WriteLine(searchcriteria.ID.ToString() + "\t" + searchcriteria.Name + "\t" + searchcriteria.DateCreated.ToString());
                    }

                    Console.WriteLine("\nSelect Recent Search Criteria ID\n");
                    string id = Console.ReadLine();
                    bool exist=false;
                    foreach (COESearchCriteriaBO searchCri in _searchCriteriaService.RecentSearchCriteriaList) 
                    {
                        if (searchCri.ID == Int32.Parse(id))
                            exist = true;
                    }
                    if (exist)
                    {
                        COESearchCriteriaBO bo = (COESearchCriteriaBO.Get(SearchCriteriaType.TEMP, Int32.Parse(id))).Save();

                        Console.Clear();

                        Console.WriteLine("The saved SearchCriteria ID is = " + bo.ID.ToString() + "\n");
                    }
                    else 
                    {
                        Console.WriteLine("\nSearchCriteria doesn´t exist\n");
                    }
                    Console.WriteLine("\nPress Enter to continue");
                    Console.ReadLine();
                    MainMenu();
                    break;
                case "5":
                    break;
                default:
                    MainMenu();
                    break;
            }
        }

    }
}
