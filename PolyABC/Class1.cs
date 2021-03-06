﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyABC
{
    public class Crypt
    {
        private static char[] _arrayAlphabet = new char[29];
        private static char[,] _arrayVigenere = new char[27, 27];
        private static List<int> _listeKey = new List<int>();
        private static List<int> _listeKlarText = new List<int>();
        private static List<string> _listeTempKey = new List<string>();
        private static List<string> _listeCodierterText = new List<string>();

        private static string _kodierterText = string.Empty;
        private static string _deKodierterText = string.Empty;
        private static string _errorLog = string.Empty;

        private static void Clear()
        {
            _listeKey.Clear();
            _listeKlarText.Clear();
            _listeTempKey.Clear();
        }

        private static void ArrayAlphabetErstellen()
        {
            char chrLetterTemp = 'A';
            _arrayAlphabet[0] = '[';
            for (int i = 1; i <= 26; i++)
            {

                _arrayAlphabet[i] = chrLetterTemp;
                if (chrLetterTemp == '[')
                {
                    chrLetterTemp = 'A';
                }
                chrLetterTemp++;
            }
            _arrayAlphabet[27] = ']';
            _arrayAlphabet[28] = ' ';
        }

        private static void ArrayVigenereErstellen()
        {
            //Hier wird die Tabelle erstellt, aus der sich nachher die verschlüsselten Buchstaben ergeben
            char chrLetter = 'A';
            for (int j = 0; j <= 26; j++)
            {
                char chrLetterTemp = chrLetter;
                for (int i = 0; i <= 26; i++)
                {

                    if (i != 26)
                    {
                        _arrayVigenere[j, i] = chrLetterTemp;
                        chrLetterTemp++;
                        if (chrLetterTemp == '[')
                        {
                            chrLetterTemp = 'A';
                        }
                    }
                    else
                    {
                        _arrayVigenere[j, i] = ' ';
                    }
                }
                chrLetter++;
            }
        }

        public static string errorLog
        {
            get { return _errorLog; }
            set { _errorLog = value; }
        }

        private static string[] SplitIntoWords(string strToSplit)
        {
            string[] strTrennen = strToSplit.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            return strTrennen;
        }
        /// <summary>
        /// Kodieren
        /// </summary>
        /// <param name="Text">Der zu verschlüsselnde Text</param>
        /// <param name="Key">Der Schüssel, mit dem verschlüsselt werden soll</param>
        public static void Kodieren(string Text, string Key)
        {
            string strCodiert = string.Empty;
            string strKlarText = Text.ToUpper();
            string strKey = Key.ToUpper();

            ArrayAlphabetErstellen();
            ArrayVigenereErstellen();

            Clear();
            //Der Key darf nur aus zusammenhängenden Zeichen, also ohne Leerzeichen, bestehen
            string[] strTrennen = strKey.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (strTrennen.Length > 0)
            {
                if (TestString(strTrennen[0].ToString()) && TestString(strKlarText))
                {
                    strTrennen[0] = strKlarText;
                    foreach (string wort in strTrennen)
                    {
                        //Bei einem neuen Durchgang, müssen die beiden Listen gelöscht werden, damit das berreits verabeitete nicht nocheinmal verarbeitet wird
                        Clear();
                        TextToNumber(wort, strKey);
                        DoCodieren();
                    }
                    foreach (string codiertesWort in _listeCodierterText)
                    {
                        strCodiert += " " + codiertesWort;
                    }
                    _listeCodierterText.Clear();
                }
            }
            else
            {
                errorLog = "Der Key darf keine Leerzeichen enthalten!";
            }
            KodierterText = strCodiert;
        }

        private static bool TestString(string text)
        {
            char[] arrayChar = text.ToCharArray();
            foreach (char item in arrayChar)
            {
                if (!_arrayAlphabet.Contains(item))
                {
                    errorLog = "Es sind nur Buchstaben erlaubt. Umlaute werden noch nicht unterstützt!";
                    return false;
                }
            }
            return true;
        }

        public static string KodierterText
        {
            get { return _kodierterText; }
            set { _kodierterText = value; }
        }
        /// <summary>
        /// DeKodieren
        /// </summary>
        /// <param name="Text">Der zu entschlüsselnde Text</param>
        /// <param name="Key">Der Schlüssel, mit dem entschlüsselt werden soll</param>
        public static void Dekodieren(string Text, string Key)
        {
            Clear();
            ArrayVigenereErstellen();
            ArrayAlphabetErstellen();

            string strKey = Key.ToUpper();
            string strCodiert = Text.ToUpper().Trim(); //Übergabewert muss hier rein
            if (strCodiert != "" && strKey != "")
            {
                if (TestString(strCodiert) && TestString(strKey))
                {
                    int intCounterSpacesRemoved = 0;
                    while (strCodiert[0].ToString() == " ")
                    {
                        intCounterSpacesRemoved++;
                        strCodiert = strCodiert.Remove(0, 1);
                    }
                    if (intCounterSpacesRemoved > 0)
                    {

                    }

                    string decodiert = string.Empty;
                    TextToNumber(strCodiert, strKey);

                    for (int i = 0; i <= _listeKlarText.Count - 1; i++)
                    {
                        if (_listeKlarText[i] >= 0) _listeKlarText[i] = _listeKlarText[i] + 65;
                    }

                    int intLengthOfText = 0;

                    do
                    {
                        foreach (int x in _listeKey)
                        {
                            if (intLengthOfText <= _listeKlarText.Count - 1)
                            {
                                for (int count = 0; count <= 26; count++)
                                {
                                    int test = _arrayVigenere[x, count];
                                    if (test == _listeKlarText[intLengthOfText] || test == 32)
                                    {
                                        if (test == 32)
                                        {
                                            decodiert += _arrayAlphabet[count + 2];
                                        }
                                        else
                                        {
                                            decodiert += _arrayAlphabet[count + 1];
                                        }

                                        break;
                                    }
                                }
                                intLengthOfText++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    } while (intLengthOfText <= _listeKlarText.Count - 1);
                    DeKodierterText = decodiert;
                }
            }
            else
            {
                errorLog = "Kodierter Text oder Key ist leer!";
            }
        }

        public static string DeKodierterText
        {
            get { return _deKodierterText; }
            set { _deKodierterText = value; }
        }

        private static void DoCodieren()
        {
            _listeTempKey.Clear();
            string strCodierterText = "";

            int i = 0;
            //Für jedes char in listeKlarText wird ein Element aus der listeKey in die listeTempKey geschrieben, 
            //wenn die listeKey zuende ist, wird von vorne angefangen.
            foreach (char element in _listeKlarText)
            {
                if (i < _listeKey.Count)
                {

                    _listeTempKey.Add(_listeKey.ElementAt(i).ToString());
                    i++;
                    //Das else löst aus, wenn i = der Anzahl der Elemente in listeKey ist, d.h. der Key zum verschlüsseln ist am Ende angelangt
                    //und es muss wieder von vorne begonnen werden
                }
                else
                {
                    i = 0;
                    _listeTempKey.Add(_listeKey.ElementAt(i).ToString());
                    i++;
                }

            }//Ende foreach
            i = 0;
            //Jetzt habe ich den entgültigen Key zum Verschlüsseln in der listeTempKey.
            //Nun kommt es zum eigentlichen Verschlüsseln.
            //Für jedes Element in der listeTempKey, wird ein Element mit dem gleichen Index aus der listeKlarText genommen. Dabei ist das Element aus der
            //listeTempKey die Nummer der Row in arrayVigenere und das Element aus der listeKlarText die Nummer der Column.
            //Daraus ergibt sich dann der verschlüsselte Buchstabe, welche dann in strCodierterText geschreiben wird.
            //Das wird jetzt mit jedem Element der listeTempKey(und damit auch der listeKlarText) gemacht.
            foreach (string element in _listeTempKey)
            {
                if (_listeKlarText.ElementAt(i) > -1)
                {
                    strCodierterText += _arrayVigenere[Convert.ToInt16(_listeTempKey.ElementAt(i)), Convert.ToInt16(_listeKlarText.ElementAt(i))].ToString();
                }
                else
                {
                    strCodierterText += " ";
                }
                i++;
            }
            //Wenn alle Elemente durch sind, wird das Ergebis in die TextBox txtCodiert geschrieben
            _listeCodierterText.Add(strCodierterText);
            string strFertigerText = strCodierterText;
        }

        private static void TextToNumber(string wort, string strKey)
        {
            //Jeden Char in eine Zahl umwandeln
            //Jeden Char im Key in die listeKey schreiben
            int intBuchstabe = 0;
            foreach (char c in strKey)
            {
                intBuchstabe = c - 65;
                _listeKey.Add(intBuchstabe);
            }
            //Jeden Char im KlarText in die listeKlarText schreiben
            intBuchstabe = 0;
            foreach (char c in wort)
            {
                intBuchstabe = c - 65;
                if (intBuchstabe == -33) intBuchstabe = 26;
                _listeKlarText.Add(intBuchstabe);
            }
        }
    }
}
