using Lusteni.Osmismerky;
using Lusteni.Sudoku;
using Osmismerky;
using System;
using System.IO;

namespace Lusteni
{
    public enum SudokuType
    {
        Numeric = 0,
        Alphabet = 1,
        Unknown = 2
    }

    internal static class Tools
    {
        internal static string ForwardChSubstitution(string input)
        {
            return AppConfig.IsCZElanguage ? input.Replace(Const.Char_CH, Const.Char_CH_Substitution) : input;
        }

        internal static string ReverseChSubstitution(string input)
        {
            return AppConfig.IsCZElanguage ? input.Replace(Const.Char_CH_Substitution, Const.Char_CH) : input;
        }
        internal static string RemoveSpaces(string input)
        {
            return input.Replace(Const.Char_Space, string.Empty);
        }

        internal static string TryGetSolutionDirectoryInfo(string currentPath = null)
        {
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\.."));
        }

      

    }
}
