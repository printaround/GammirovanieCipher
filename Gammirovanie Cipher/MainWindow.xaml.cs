using System;
using System.IO;
using System.Media;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Gammirovanie01
{
    public partial class MainWindow : Window
    {
        private char[] symbols = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890 .,<>?/|':;-_=+()*&^%$#@!{}~".ToCharArray();

        private short power;    //Значение N
        private string key;

        public MainWindow()
        {
            InitializeComponent();
            power = Convert.ToInt16(symbols.Length);
            key = RandomKey();
            StringInRTB(Convert.ToString(key),keyText);
        }
        private void GenerateKeyBtn(object sender, RoutedEventArgs e)
        {
            SoundPlay();

            StringInRTB(RandomKey(), keyText);
        }
        private void SaveDecTextBtn(object sender, RoutedEventArgs e)
        {
            SoundPlay();

            string file_name = @"MyData\enc_dec_text.txt";
            WriteToFile(file_name, outputText);
        }
        private void LoadTextBtn(object sender, RoutedEventArgs e)
        {
            SoundPlay();

            string file_name = @"MyData\enc_dec_text.txt";
            ReadFromFile(file_name, inputText);
        }
        private void SaveKeyBtn(object sender, RoutedEventArgs e)
        {
            SoundPlay();

            string file_name = @"MyData\key_text.txt";
            WriteToFile(file_name, keyText);
        }
        private void LoadKeyBtn(object sender, RoutedEventArgs e)
        {
            SoundPlay();

            string file_name = @"MyData\key_text.txt";
            ReadFromFile(file_name, keyText);
        }
        private void EncBtn(object sender, RoutedEventArgs e)
        {
            SoundPlay();

            string message = StringFromRTB(inputText);

            CheckKey(); //Проверка ключа на корректность

            message = Encryption(message);
            StringInRTB(message, outputText);
        }
        private void DecBtn(object sender, RoutedEventArgs e)
        {
            SoundPlay();

            string message = StringFromRTB(inputText);

            CheckKey(); //Проверка ключа на корректность

            message = Decryption(message);
            StringInRTB(message, outputText);
        }

        //-----------------------------------------------------------------------------------------------------------------------------------
        private string Encryption(string mess)
        {
            string result = "";
            int keyword_index = 0;

            for(int i = 0; i < mess.Length; i++)
            {
                int c = (Array.IndexOf(symbols, mess[i]) + Array.IndexOf(symbols, key[keyword_index])) % power;
                result += symbols[c];
                keyword_index++;
            }

            //Костыль для удаления двух последних элементов
            result = result.Remove(result.Length - 1);
            result = result.Remove(result.Length - 1);
            return result;
        }

        private string Decryption(string mess)
        {
            string result = "";
            int keyword_index = 0;

            for (int i = 0; i < mess.Length; i++)
            {
                int p = (Array.IndexOf(symbols, mess[i]) + power - Array.IndexOf(symbols, key[keyword_index])) % power;
                result += symbols[p];
                keyword_index++;
            }

            //Костыль для удаления двух последних элементов
            result = result.Remove(result.Length - 1);
            result = result.Remove(result.Length - 1);
            return result;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        private string RandomKey()
        {
            Random rand = new Random();
            string result = "";

            for (int i = 0; i < (power+1); i++)
                result += symbols[rand.Next(0, power)];

            return result;
        }
        private void CheckKey()   //Проверка ключа на корректность
        {
            key = StringFromRTB(keyText);
            if (key.Length < power)
            {
                key = RandomKey();
                StringInRTB(key, keyText);
            }
        }
        private string StringFromRTB(RichTextBox rtb)   //Получени строки из RichTextBox
        {
            TextRange textRange = new TextRange(
                rtb.Document.ContentStart,
                rtb.Document.ContentEnd
            );
            return textRange.Text;
        }
        private void StringInRTB(string received_str, RichTextBox rtb)   //Вывод строки в RichTextBox
        {
            FlowDocument objFdoc = new FlowDocument();
            Paragraph objPara1 = new Paragraph();
            objPara1.Inlines.Add(new Run(received_str));
            objFdoc.Blocks.Add(objPara1);
            rtb.Document = objFdoc;
        }
        //-----------------------------------------------------------------------------------------------------------------------------------
        private void WriteToFile(string gettingFileName, RichTextBox rtb)
        {
            try
            {
                StreamWriter sw = new StreamWriter(gettingFileName);
                sw.WriteLine(StringFromRTB(rtb));
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }
        private string ReadFromFile(string gettingFileName, RichTextBox rtb)
        {
            string file_data = "none";
            try
            {
                StreamReader sr = new StreamReader(gettingFileName);
                file_data = sr.ReadToEnd();
                sr.Close();
                StringInRTB(file_data, rtb);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
            return file_data;
        }
        private void SoundPlay()
        {
            try
            {
                SoundPlayer sp = new SoundPlayer();
                sp.SoundLocation = @"MyData\Sounds\sound_btn.wav";
                sp.Load();
                sp.Play();
            }
            catch (Exception er)
            {
                Console.WriteLine("Exception: " + er.Message);
            }
        }
    }
}
