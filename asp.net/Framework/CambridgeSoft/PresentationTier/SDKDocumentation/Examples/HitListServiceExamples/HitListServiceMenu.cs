using System;
using System.Collections.Generic;
using System.Text;
using CambridgeSoft.COE.Framework.COEHitListService;
using CambridgeSoft.COE.Framework;

namespace HitListServiceExamples
{
    public class HitListServiceMenu
    {
        HitListService _hitLists = new HitListService();

        public void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Welcome to HitList Service Example Application...\n");
            Console.WriteLine("Select your option...\n");
            Console.WriteLine("1 - Show TEMP HitLists");
            Console.WriteLine("2 - Show SAVED HitList");
            Console.WriteLine("3 - Performing HitList Operations");
            Console.WriteLine("4 - Mark HitLists");
            Console.WriteLine("5 - Exit\n");

            string option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    ShowHitLists(HitListType.TEMP);
                    Console.WriteLine("Press enter to return...");
                    Console.ReadLine();
                    MainMenu();
                    break;
                case "2":
                    ShowHitLists(HitListType.SAVED);
                    Console.WriteLine("Press enter to return...");
                    Console.ReadLine();
                    MainMenu();
                    break;
                case "3":
                    OperationsMenu();
                    break;
                case "4":
                    MainMenu();
                    break;
                case "5":
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Incorrect Option");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    MainMenu();
                    break;
            }

        }

        public void ShowHitLists(HitListType hitListType)
        {
            Console.Clear();

            switch (hitListType)
            {
                case HitListType.TEMP:

                    Console.WriteLine("TEMP HitLists\n");

                    Console.WriteLine("HITLIST\tID\tNAME\t\tDESCRIPTION\t\tNUMBER OF HITS\tTYPE\n");

                    for (int i = 0; i <= 10; i++)
                    {
                        Console.WriteLine(i + "\t" + _hitLists.TempHitListList[i].ID + "\t" + _hitLists.TempHitListList[i].Name + "\t\t" + _hitLists.TempHitListList[i].Description + "\t\t" + _hitLists.TempHitListList[i].NumHits.ToString() + "\t\t" + _hitLists.TempHitListList[i].HitListType);
                    }
                    Console.WriteLine("\n");
                    break;
                case HitListType.SAVED:

                    Console.WriteLine("SAVED HitLists\n");

                    Console.WriteLine("HITLIST\tID\tNAME\tDESCRIPTION\tNUMBER OF HITS\tTYPE\n");

                    for (int i = 0; i <= 10; i++)
                    {
                        Console.WriteLine(i + "\t" + _hitLists.SavedHitListList[i].ID + "\t" + _hitLists.SavedHitListList[i].Name + "\t" + _hitLists.SavedHitListList[i].Description + "\t\t" + _hitLists.SavedHitListList[i].NumHits.ToString() + "\t\t" + _hitLists.SavedHitListList[i].HitListType);
                    }
                    Console.WriteLine("\n");
                    break;
                default:
                    Console.WriteLine("The HitListType is not correct\n");
                    Console.WriteLine("Press enter to return...");
                    Console.ReadLine();
                    break;
            }
        }

        public void OperationsMenu()
        {
            Console.Clear();
            Console.WriteLine("HitLists Operations Menu...\n");
            Console.WriteLine("Select your option...\n");
            Console.WriteLine("1 - Intersect HitLists");
            Console.WriteLine("2 - Subtract HitLists");
            Console.WriteLine("3 - Subtract a list of ids to a hitlist");
            Console.WriteLine("4 - Unite HitLists");
            Console.WriteLine("5 - Return\n");

            string operation = Console.ReadLine();
            HitListType typeSelected = HitListType.SAVED;
            string hitListIndex1 = string.Empty;

            if(operation != "5") {
                Console.WriteLine("Select HitList Types (T)=TEMP - (S)=SAVED\n");

                string type = Console.ReadLine();

                switch(type.ToUpper()) {
                    case "T":
                        typeSelected = HitListType.TEMP;
                        break;
                    case "S":
                        typeSelected = HitListType.SAVED;
                        break;
                    default:
                        Console.WriteLine("The selected type is not correct\n");
                        Console.WriteLine("Press enter to continue...");
                        Console.ReadLine();
                        OperationsMenu();
                        break;
                }
                ShowHitLists(typeSelected);

                Console.WriteLine("Select first HitList index\n");
                hitListIndex1 = Console.ReadLine();
            }
            
            switch (operation)
            {
                case "1":
                    Console.WriteLine("Select second HitList index\n");
                    string hitListIndex2 = Console.ReadLine();
                    _hitLists.IntersecHitLists(typeSelected, Convert.ToInt32(hitListIndex1), Convert.ToInt32(hitListIndex2));
                    Console.WriteLine("The HitLists was intersected\n");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                    break;
                case "2":
                    Console.WriteLine("Select second HitList index\n");
                    hitListIndex2 = Console.ReadLine();
                    _hitLists.SubtractHitLists(typeSelected, Convert.ToInt32(hitListIndex1), Convert.ToInt32(hitListIndex2));
                    Console.WriteLine("The HitLists was subtracted\n");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    break;
                case "3":
                    Console.WriteLine("Write a list of ids to be excluded separated by comma\n");
                    int[] idsToExclude = Array.ConvertAll<string, int>(Console.ReadLine().Split(','), delegate(string s) {
                        return int.Parse(s);
                    });
                    _hitLists.SubtractHitLists(typeSelected, Convert.ToInt32(hitListIndex1), idsToExclude);
                    Console.WriteLine("The HitLists was subtracted\n");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    break;
                case "4":
                    Console.WriteLine("Select second HitList index\n");
                    hitListIndex2 = Console.ReadLine();
                    _hitLists.UniteHitLists(typeSelected, Convert.ToInt32(hitListIndex1), Convert.ToInt32(hitListIndex2));
                    Console.WriteLine("The HitLists was united\n");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadLine();
                    break;
                case "5":
                    MainMenu();
                    break;
                default:
                    Console.WriteLine("The selected option is not correct\n");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    OperationsMenu();
                    break;
            }
        }

        public void MarkHitListsMenu()
        {
            Console.Clear();
            Console.WriteLine("Mark HitList Menu\n");
            Console.WriteLine("Select HitList Types (T)=TEMP - (S)=SAVED\n");
            string type = Console.ReadLine();
            switch (type.ToUpper())
            {
                case "T":
                    ShowHitLists(HitListType.TEMP);
                    break;
                case "S":
                    ShowHitLists(HitListType.SAVED);
                    break;
                default:
                    Console.WriteLine("The selected type is not correct\n");
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                    MarkHitListsMenu();
                    break;
            }
            Console.WriteLine("Select HitList index\n");
            string hitListIndex = Console.ReadLine();
            Console.Clear();
            Console.WriteLine("The mark HitsIDs are (34,123,45,56,78)\n");
            int[] hitsToBeMarked ={ 34, 123, 45, 56, 78 };
            COEHitListBO markedHitList;
            switch (type.ToUpper())
            {

                case "T":
                    markedHitList = _hitLists.MarkHits(HitListType.TEMP, Convert.ToInt32(hitListIndex), hitsToBeMarked);
                    break;
                case "S":
                    markedHitList = _hitLists.MarkHits(HitListType.SAVED, Convert.ToInt32(hitListIndex), hitsToBeMarked);
                    break;
                default:
                    throw new Exception("HitList Type is not correct");
            }
            Console.WriteLine("Marked HitList Info...\n");
            Console.WriteLine("ID\tNAME\t\tNUMBER OF HITS\n");
            Console.WriteLine(markedHitList.ID.ToString() +"\t"+ markedHitList.Name+"\t\t"+markedHitList.NumHits+"\n");
            Console.WriteLine("Press enter to continue...");
            Console.ReadLine();
            MainMenu();
        }
    }
}
