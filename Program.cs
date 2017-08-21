using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace MeasuresSavior
{
    class Program
    {
        private static List<FootballSet> listFS = new List<FootballSet>();
        public static void Main(string[] args)
        {
            while (true)
            {
                var menuSelect = MenuPopUp();
                Console.WriteLine();

                switch (menuSelect)
                {
                    case "1":  // Add a new set of related words.
                               // Complete
                        Case1();
                        break;

                    case "2":  // Paste in a new set of related words.
                               // Complete
                        Case2();
                        break;

                    case "3":  // Combine 2 word sets.
                               // Complete
                        Case3();
                        break;

                    case "4":  // Append 2 word sets.
                               // Complete
                        Case4();
                        break;

                    case "5":  // Add entries to a previously created set.
                               // Complete
                        Case5();
                        break;

                    case "6":  // Delete entries from a previously created set.
                               // Complete
                        Case6();
                        break;

                    case "7":  // Delete a set.
                               // Complete
                        Case7();
                        break;

                    case "8":  // Display a set.
                               // Complete
                        Case8();
                        break;

                    case "9":  // End the program
                               // Complete
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid input, try again.\n");
                        break;
                }
            }
        }
        public static String MenuPopUp()
        {
            Console.Write("Select from the following menu:"
                         + "\n\t1. Add a new set of related words."
                         + "\n\t2. Paste in a new set of related words."
                         + "\n\t3. Combine 2 word sets."
                         + "\n\t4. Append 2 word sets."
                         + "\n\t5. Add entries to a previously created set."
                         + "\n\t6. Delete entries from a previously created set."
                         + "\n\t7. Delete a set."
                         + "\n\t8. Display a set."
                         + "\n\t9. End the program."
                         + "\n>>Make your menu selection now:  ");
            String menuSelect = Console.ReadLine();
            return menuSelect;
        }
        // Add a new set of related words
        public static void Case1()
        {
            var fs = new FootballSet(GetValidName(SetName()));
            EntryLoop();
            fs.FootballSetList = CreateRelatedWordList(fs);
            listFS.Add(fs);
            SetIntroduction(fs);
        }
        // Paste in a new set of related words.
        public static void Case2()
        {
            var fs = GetPastedAliases(GetValidName(SetName()));
            listFS.Add(fs);
            SetIntroduction(fs);
        }
        // Create a phrasal set of 2 or more word lists.
        public static void Case3()
        {
            if (listFS.Any())
            {
                var fs = GetCombinedSet();
                listFS.Add(fs);
                SetIntroduction(fs);
            }
            else Console.WriteLine("You do not have any sets to combine.\n");
        }
        // Append 2 word sets.
        public static void Case4()
        {
            if (listFS.Any())
            {
                var appendFS = GetAppendSet();
                listFS.Add(appendFS);
                SetIntroduction(appendFS);
            }
            else Console.WriteLine("You do not have any sets to append.\n");
        }
        // Add entries to a previously created set.
        public static void Case5()
        {
            if (listFS.Any())
            {
                var fs = GetFootballSet(GetSetChoice());
                EntryLoop();
                CreateRelatedWordList(fs);
                SetIntroduction(fs);
            }
            else Console.WriteLine("You do not have any sets to add to.\n");
        }
        // Delete entries from a previously created set.
        public static void Case6()
        {
            if (listFS.Any())
            {
                var fs = GetFootballSet(GetSetChoice());
                DeleteOptions(fs);
                SetIntroduction(fs);
            }
            else Console.WriteLine("You do not have any sets to delete from.\n");
        }
        // Delete a set.
        public static void Case7()
        {
            if (listFS.Any())
            {
                DisplaySetOptions();
                if (listFS.Remove(GetFootballSet(GetSetChoice()))) Console.WriteLine("Your set has been deleted.");
                else Console.WriteLine("The set does not exist.");
            }
            else Console.WriteLine("You do not have any sets to delete.\n");
        }
        // Display a set.
        public static void Case8()
        {
            if (listFS.Any())
            {
                SetIntroduction(GetFootballSet(GetSetChoice()));
            }
            else Console.WriteLine("You do not have any sets to display.\n");
        }
        public static String SetName()
        {
            Console.Write(">>Enter a name for this set of words:  ");
            var setName = Console.ReadLine();
            return setName;
        }
        public static void EntryLoop()
        {
            Console.WriteLine("\nEnter a SINGULAR word and all of its BASIC variations, followed by enter." +
                "\nFor compound words, put an \"*\" where the break would be." +
                "\nFor example, \"touch*down\"" +
                "\nType \"!\" when finished for this word's variations.");
        }
        public static FootballSet GetAppendSet()
        {
            var listName1 = GetSetChoice();
            var fs1 = GetFootballSet(listName1);
            Console.Write(">>Now choose your set to append:  ");
            var listName2 = GetSetName("Set name");
            var fs2 = GetFootballSet(listName2);
            var listName = $"{listName1}+{listName2}";

            return AppendSets(listName, fs1, fs2);
        }
        public static FootballSet GetCombinedSet()
        {
            DisplaySetOptions();
            Console.Write("\n>>Option 1:  ");
            var listName1 = GetSetName("Option 1");
            Console.Write(">>Option 2:  ");
            var listName2 = GetSetName("Option 2");
            var combinedSetName = $"{listName1}&{listName2}";

            return CombineLists(combinedSetName, GetFootballSet(listName1), GetFootballSet(listName2));
        }
        public static List<Word> CreateRelatedWordList(FootballSet fs)
        {
            String latestEntry = "";
            var endList = false;

            while (!endList)
            {
                Console.Write("\n>>Next entry:  ");
                latestEntry = Console.ReadLine();
                if (!GetEndList(latestEntry))
                {
                    if (latestEntry != null)
                    {
                        if (latestEntry.Contains("*"))
                        {
                            AddCompoundVariations(fs, latestEntry);
                            PluralOptions(fs, latestEntry);
                        }
                        else
                        {
                            fs.Add(new Word(latestEntry));
                            PluralOptions(fs, latestEntry);
                        }
                    }
                    else throw new ArgumentNullException("The Word passed in is null");
                }
            }
            return fs.FootballSetList;
        }
        public static void PluralOptions(FootballSet fs, string latestEntry)
        {
            var invalidInput = true;
            String endingOption = "";

            while (invalidInput)
            {
                Console.Write("Choose the plural ending for this word:" +
                    "\n\tA)  +s" +
                    "\n\tB)  +es" +
                    "\n\tC)  Both plural endings" +
                    "\n\tD)  No plural needed" +
                    "\n>>Plural Option:  ");

                endingOption = Console.ReadLine().ToUpper();
                invalidInput = false;
                Word wordNoCompound = new Word(latestEntry);

                switch (endingOption)
                {
                    case "A":
                        if (latestEntry.Contains("*"))
                        {
                            AddCompoundVariations(fs, $"{latestEntry}s");
                        }
                        else
                        {
                            Word pluralSNoCompound = new Word($"{latestEntry}s");
                            fs.Add(pluralSNoCompound);
                        }
                        break;
                    case "B":
                        if (latestEntry.Contains("*"))
                        {
                            AddCompoundVariations(fs, $"{latestEntry}es");
                        }
                        else
                        {
                            Word pluralEsNoCompound = new Word($"{latestEntry}es");
                            fs.Add(pluralEsNoCompound);
                        }
                        break;
                    case "C":
                        if (latestEntry.Contains("*"))
                        {
                            AddCompoundVariations(fs, $"{latestEntry}s");
                            AddCompoundVariations(fs, $"{latestEntry}es");
                        }
                        else
                        {
                            Word pluralSNoCompound = new Word($"{latestEntry}s");
                            fs.Add(pluralSNoCompound);
                            Word pluralEsNoCompound = new Word($"{latestEntry}es");
                            fs.Add(pluralEsNoCompound);
                        }
                        break;
                    case "D":
                        break;
                    default:
                        invalidInput = true;
                        Console.WriteLine("\nInvalid input, try again.\n");
                        break;
                }
            }
        }
        public static void AddCompoundVariations(FootballSet fs, string uncompoundedWord)
        {
            Word word1 = new Word(uncompoundedWord.Replace("*", ""));
            fs.Add(word1);
            Word word2 = new Word(uncompoundedWord.Replace("*", " "));
            fs.Add(word2);
            Word word3 = new Word(uncompoundedWord.Replace("*", "-"));
            fs.Add(word3);
        }
        public static string GetSetName(string optionNumber)
        {
            var validName = false;
            var listName = "";

            while (!validName)
            {
                listName = Console.ReadLine();
                validName = GetSetExists(listName);
                if (!validName)
                {
                    Console.Write("That set does not exist. Try again." +
                        $"\n>>{optionNumber}:  ");
                }
            }
            return listName;
        }
        public static FootballSet GetPastedAliases(string setName)
        {
            var fs = new FootballSet(setName);
            var paste = GetPaste();
            ParsePaste(fs, paste);

            return fs;
        }
        public static string GetPaste()
        {
            Console.Write(">>Paste all the given aliases (include commas, quotes, and spaces):  ");
            var str = Console.ReadLine();
            if (str != null && str.Equals(""))
            {
                Console.WriteLine("\nYou did not enter anything. Try again.");
                GetPaste();
            }
            return str;
        }
        public static void ParsePaste(FootballSet fs, string paste)
        {
            string pattern = "\"[\\w- ]*\"";
            Regex r = new Regex(pattern);
            MatchCollection mc = r.Matches(paste);
            foreach (Match m in mc)
            {
                var parsedString = (m.Value.Substring(1, m.Value.Length - 2));
                Word parsedWord = new Word(parsedString);
                fs.Add(parsedWord);
            }
        }
        private static void DisplaySetOptions()
        {
            Console.WriteLine("You may choose from the following options:  ");
            foreach (var fs in listFS)
            {
                Console.Write("\"" + fs.ListName + "\"\t");
            }
        }
        private static bool GetSetExists(string listName)
        {
            var validName = false;
            foreach (var fs in listFS)
            {
                if (fs.ListName.Equals(listName, StringComparison.OrdinalIgnoreCase))
                {
                    validName = true;
                }
            }
            return validName;
        }
        public static FootballSet GetFootballSet(string listName)
        {
            FootballSet matchSet = null;
            foreach (var fs in listFS)
            {
                if (listName.Equals(fs.ListName, StringComparison.OrdinalIgnoreCase))
                {
                    matchSet = fs;
                }
            }
            return matchSet;
        }
        public static FootballSet CombineLists(string newListName, FootballSet set1, FootballSet set2)
        {
            FootballSet combinedSet = new FootballSet(newListName);
            List<Word> combinedList = new List<Word>();

            foreach (var word1 in set1.FootballSetList)
            {
                foreach (var word2 in set2.FootballSetList)
                {
                    Word combinedWord1 = new Word($"{word1.BasicWord} {word2.BasicWord}");
                    combinedList.Add(combinedWord1);

                    Word combinedWord2 =  new Word($"{word2.BasicWord} {word1.BasicWord}");
                    combinedList.Add(combinedWord2);
                }
            }
            combinedSet.FootballSetList = combinedList;
            return combinedSet;
        }
        public static FootballSet AppendSets(string listName, FootballSet set1, FootballSet set2)
        {
            var combinedSet = new FootballSet(listName);
            foreach (var word in set1.FootballSetList)
            {
                combinedSet.Add(word);
            }
            foreach (var word in set2.FootballSetList)
            {
                combinedSet.Add(word);
            }
            return combinedSet;
        }
        public static string GetSetChoice()
        {
            DisplaySetOptions();
            Console.Write("\n>>Set name:  ");
            var str = GetSetName("Set name");
            return str;
        }
        public static Boolean GetEndList(String latestEntry)
        {
            var endList = latestEntry.Equals("!", StringComparison.OrdinalIgnoreCase);

            return endList;
        }
        private static void SetIntroduction(FootballSet fs)
        {
            Console.WriteLine($"\nHere is your set, \"{fs.ListName}\":");
            fs.ToString(fs.FootballSetList);
            Console.WriteLine("\n\n");
        }
        private static void DeleteOptions(FootballSet fs)
        {
            /*Console.Write("\nSelect from the following menu:" +
                "\n\t1.  Delete any entries containing a specific string." +
                "\n\t2.  Delete one specific entry" +
                "\n>>Enter your choice:  ");
            var deleteOption = Console.ReadLine();
            switch (deleteOption)
            {
                case "1":   // Delete any entry with a certain string
                    DeleteEntriesWithString(fs);
                    break;
                case "2":   // Delete one specific entry
                    DeleteSpecificEntry(fs);
                    break;
                default:
                    Console.WriteLine("Invalid input, try again.\n");
                    DeleteOptions(fs);
                    break;
            }*/
            DeleteSpecificEntry(fs);
        }
        private static void DeleteEntriesWithString(FootballSet fs)
        {
            Console.Write("This is not case sensitive, and any entry containing this string will be deleted"
                + "\n\n>>Which entry or entries would you like to delete?:  ");
            var deleteString = Console.ReadLine();
            fs.RemoveGeneric(deleteString);
        }
        private static void DeleteSpecificEntry(FootballSet fs)
        {
            Console.Write(">>What entry would you like to delete? This is not case sensitive:  ");
            var deleteString = Console.ReadLine();
            fs.RemoveSpecific(deleteString);
        }
        private static string GetValidName(string enteredName)
        {
            if (!GetSetExists(enteredName))
            {
                return enteredName;
            }
            else
            {
                var validName = enteredName;
                var i = 1;
                while (true)
                {
                    validName = $"{enteredName}{i}";
                    if (!GetSetExists(validName))
                    {
                        Console.WriteLine($"That set already existed. Your new set name is {validName}.");
                        return validName;
                    }
                    i++;
                }
            }
        }
    }
    class FootballSet : Word
    {
        private List<Word> footballSetList;
        private string listName;

        public FootballSet( string myListName )
        {
            footballSetList = new List<Word>();
            listName = myListName;
        }
        public List<Word> FootballSetList
        {
            get { return footballSetList; }
            set { footballSetList = value; }
        }
        public string ListName
        {
            get { return listName; }
            set { listName = value; }
        }
        public void Add(Word word)
        {
            if (word != null)
            {
                footballSetList.Add(word);
            }
            else throw new NullReferenceException("Word passed into Add(Word word) is null");
        }
        public void RemoveGeneric(string deleteThis)
        {
            bool wasDeleted = false;
            for (Int32 i=0; i<footballSetList.Count; i++)
            {
                Word word = footballSetList[i];
                if (word.BasicWord.Contains(deleteThis))
                {
                    footballSetList.Remove(word);
                    wasDeleted = true;
                }
            }
            if (wasDeleted) Console.WriteLine($"All entries containing {deleteThis} have been deleted.");
            else Console.WriteLine("The entry does not exist in this set.");
        }
        public void RemoveSpecific(string deleteThis)
        {
            Word wordToRemove = null;
            foreach (var word in footballSetList)
            {
                if (word.BasicWord.Equals(deleteThis, StringComparison.OrdinalIgnoreCase))
                {
                    wordToRemove = word;
                }
            }
            if (wordToRemove != null)
            {
                footballSetList.Remove(wordToRemove);
                Console.WriteLine($"Your entry has been deleted.");
            }
            else Console.WriteLine("The entry does not exist in this set.");
        }
        /**
         * If has pluralEnding1, foreach word in wordList add plural form
         *      If hasPluralEnding2, foreach word in wordList add plural form
         */
        public void AddPluralVariations(FootballSet fs, Word singularWord)
        {   //if the word can be pluralized
            if (0 != String.Compare(singularWord.PluralEnding1, ""))
            {   foreach (var compoundAndOrSingularWord in footballSetList)
                {
                    Word pluralWord1 = new Word(compoundAndOrSingularWord + singularWord.PluralEnding1);
                    footballSetList.Add(pluralWord1);
                    //if the word can be pluralized a second way
                    if (0 != String.Compare(singularWord.PluralEnding2, ""))
                    {
                        Word pluralWord2 = new Word(compoundAndOrSingularWord + singularWord.PluralEnding2);
                        footballSetList.Add(pluralWord2);
                    }
                }
            }
        }
        public void ToString(List<Word> fs)
        {
            foreach (var word in fs)
            {
                Console.Write("\"" + word.BasicWord + "\", ");
            }
        }
    }
}
class Word
{
    private string basicWord = "";
    private string pluralEnding1 = "";
    private string pluralEnding2 = "";
    public Word()
    { }
    public Word(string myBasicWord, string myPluralEnding1, string myPluralEnding2)
    {
        basicWord = myBasicWord;
        pluralEnding1 = myPluralEnding1;
        pluralEnding2 = myPluralEnding2;
    }
    public Word(string pluralizedEntry)
    {
        basicWord = pluralizedEntry;
    }
    public string BasicWord
    {
        get { return basicWord; }
        set { basicWord = value; }
    }
    public string PluralEnding1
    {
        get { return pluralEnding1; }
        set { pluralEnding1 = value; }
    }
    public string PluralEnding2
    {
        get { return pluralEnding2; }
        set { pluralEnding2 = value; }
    }
}
