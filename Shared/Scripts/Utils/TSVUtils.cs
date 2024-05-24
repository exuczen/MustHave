#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine.Assertions;
#endif

public struct TSVUtils
{
#if UNITY_EDITOR
    public const string TSVFolderPath = @"Assets/Editor/Data/TSV/";

    private const char CellSeparator = '\t';
    static readonly string[] CellDataSeparator = { ", " };

    public static List<string[]> LoadData(string fileName, int startIndex = 1)
    {
        var result = new List<string[]>();
        var lines = Regex.Split(File.ReadAllText(TSVFolderPath + fileName), "\r|\n|\r\n");
        for (int i = startIndex; i < lines.Length; i++)
        {
            if (lines[i].Length > 0)
            {
                //Assert.IsFalse(lines[i].Contains("\""));
                result.Add(lines[i].Split(CellSeparator));
            }
        }
        return result;
    }

    public static string[] SplitCellData(string cellData)
    {
        return cellData.Split(CellDataSeparator, StringSplitOptions.RemoveEmptyEntries);
    }


    public static T ParseEnum<T>(string data)
    {
        return (T)Enum.Parse(typeof(T), data);
    }
#endif
}
