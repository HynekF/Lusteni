// See https://aka.ms/new-console-template for more information
using System.Text;

Console.WriteLine("Hello, World!");

namespace Sudoku
{
    internal class SudokuCore
    {

        public List<byte[]> Data;

        public int Dimension
        {
            get
            {
                int _dimension = 0;
                if (Data != null)
                {
                    _dimension = Data.Count;
                }
                return _dimension;
            }
        }

        public void LoadSudokuCore(string fileFullPath)
        {
            using (StreamReader file = new StreamReader(fileFullPath))
            {
                string ln;                

                while ((ln = file.ReadLine()) != null)
                {                    
                    Data.Add(Encoding.UTF8.GetBytes(ln));                    
                }
                file.Close();
            }
            
        }

        public bool IsResultFinished
        {
            get 
            { 
                return !Data.Any(array => array.Contains((byte)0)); 
            }
        }
    }
}