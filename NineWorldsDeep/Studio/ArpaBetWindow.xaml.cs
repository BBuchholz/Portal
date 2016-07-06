using NineWorldsDeep.Studio.ArpaBet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NineWorldsDeep.Studio
{
    /// <summary>
    /// Interaction logic for ArpaBetWindow.xaml
    /// </summary>
    public partial class ArpaBetWindow : Window
    {
        //indexes from beginning of word
        private Dictionary<char,
            List<ArpaBetPair>> mArpaBetLetters;
        
        //indexes from end of word
        private Dictionary<string,
            List<ArpaBetPair>> mArpaBetPhonemes;

        public ArpaBetWindow()
        {
            InitializeComponent();
        }

        private void EnsureDictionaries()
        {
            if (mArpaBetLetters == null || 
                mArpaBetPhonemes == null)
            {
                mArpaBetLetters =
                    new Dictionary<char, List<ArpaBetPair>>();

                mArpaBetPhonemes =
                    new Dictionary<string, List<ArpaBetPair>>();

                foreach (ArpaBetPair abp in
                    ArpaBetLoader.RetrieveArpaBet())
                {
                    //LETTERS
                    char letter = abp.Word.First();

                    if (!mArpaBetLetters.ContainsKey(letter))
                    {
                        mArpaBetLetters[letter] =
                            new List<ArpaBetPair>();
                    }

                    mArpaBetLetters[letter].Add(abp);

                    //PHONEMES
                    string phoneme = GetLastPhoneme(abp.StrippedArpaBetValue);

                    if (!mArpaBetPhonemes.ContainsKey(phoneme))
                    {
                        mArpaBetPhonemes[phoneme] =
                            new List<ArpaBetPair>();
                    }

                    mArpaBetPhonemes[phoneme].Add(abp);
                }
            }            
        }

        private IEnumerable<ArpaBetPair> GetLetter(string key)
        {
            EnsureDictionaries();

            if (!string.IsNullOrWhiteSpace(key))
            {
                char keyLetter = key.First();

                if (mArpaBetLetters.ContainsKey(keyLetter))
                {
                    return mArpaBetLetters[keyLetter];
                }
            }

            return new List<ArpaBetPair>();
        }

        private string GetLastPhoneme(string arpaBetWord)
        {
            return arpaBetWord.Split(
                    new char[] { ' ' }, 
                    StringSplitOptions.RemoveEmptyEntries)
                .Last();
        }

        private IEnumerable<ArpaBetPair> GetPhoneme(string key)
        {
            EnsureDictionaries();

            if (!string.IsNullOrWhiteSpace(key))
            {
                string keyPhoneme = GetLastPhoneme(key);

                if (mArpaBetPhonemes.ContainsKey(keyPhoneme))
                {
                    return mArpaBetPhonemes[keyPhoneme];
                }
            }
                        
            return new List<ArpaBetPair>();
        }

        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            foreach (DataGridColumn col in mDgdWordArpaBetValues.Columns)
            {
                ////change this value to filter readonly
                //if (!col.Header.Equals(""))
                //{
                //    col.IsReadOnly = true;
                //}

                //all read only
                col.IsReadOnly = true;
            }
        }

        private void CollectionsViewSource_Filter(object sender, FilterEventArgs e)
        {
            //some reference for datagrid grouping and sorting
            //https://msdn.microsoft.com/en-us/library/ff407126(v=vs.100).aspx
        }

        private void MenuItemLoadFromWeb_Click(object sender, RoutedEventArgs e)
        {
            mTxtWord.Text = "A";
            mTxtWord.SelectAll();
            ProcessWord();           
        }

        private void mTxtWord_KeyUp(object sender, KeyEventArgs e)
        {
            ProcessWord();
        }

        private void ProcessWord()
        {
            string partialWord = 
                mTxtWord.Text.ToUpper(); //ArpaBet all uppercase

            var filtered = 
                GetLetter(partialWord)
                .Where(x => x.Word.StartsWith(partialWord));

            mDgdWordArpaBetValues.ItemsSource = filtered;
        }

        private void ProcessArpaBetValue()
        {
            string partialWord =
                mTxtArpaBetValue.Text.ToUpper(); //ArpaBet all uppercase

            var filtered =
                GetPhoneme(partialWord)
                .Where(x => x.StrippedArpaBetValue.EndsWith(partialWord));

            mDgdWordArpaBetValues.ItemsSource = filtered;
        }

        private void mDgdWordArpaBetValues_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ArpaBetPair abp = (ArpaBetPair)mDgdWordArpaBetValues.SelectedItem;

            if(abp != null)
            {
                mTxtWord.Text = abp.Word;
                mTxtArpaBetValue.Text = abp.StrippedArpaBetValue;
                ProcessWord();
            }
        }

        private void mTxtArpaBetValue_KeyUp(object sender, KeyEventArgs e)
        {
            ProcessArpaBetValue();
        }
    }
}
