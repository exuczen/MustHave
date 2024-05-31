using System;
using System.IO;
using UnityEngine;

namespace MustHave.Utils
{
    public struct FileUtils
    {
        public static void TryCopyFile(string sourceFilePath, string destFilePath, bool deleteExisting = true)
        {
            if (File.Exists(sourceFilePath))
            {
                try
                {
                    if (File.Exists(destFilePath))
                    {
                        if (deleteExisting)
                        {
                            File.Delete(destFilePath);
                        }
                        else
                        {
                            Debug.LogWarning($"File exists: {destFilePath}");
                            return;
                        }
                    }
                    File.Copy(sourceFilePath, destFilePath);
                }
                catch (Exception)
                {
                    throw;
                }
                Debug.Log($"Succesfully copied file: {sourceFilePath} to {destFilePath}");
            }
            else
            {
                Debug.LogWarning($"File does not exist: {sourceFilePath}");
            }
        }

        public static void TryMoveFile(string sourceFilePath, string destFilePath, bool deleteExisting = true)
        {
            if (File.Exists(sourceFilePath))
            {
                try
                {
                    if (File.Exists(destFilePath))
                    {
                        if (deleteExisting)
                        {
                            File.Delete(destFilePath);
                        }
                        else
                        {
                            Debug.LogWarning($"File exists: {destFilePath}");
                            return;
                        }
                    }
                    File.Move(sourceFilePath, destFilePath);
                }
                catch (Exception)
                {
                    throw;
                }
                Debug.Log($"Succesfully moved file: {sourceFilePath} to {destFilePath}");
            }
            else
            {
                Debug.LogWarning($"File does not exist: {sourceFilePath}");
            }
        }

        public static void TryMoveDirectory(string sourcePath, string destPath, bool deleteExisting = true)
        {
            if (Directory.Exists(sourcePath))
            {
                try
                {
                    if (Directory.Exists(destPath))
                    {
                        if (deleteExisting)
                        {
                            Directory.Delete(destPath);
                        }
                        else
                        {
                            Debug.LogWarning($"Directory exists: {destPath}");
                            return;
                        }
                    }
                    Directory.Move(sourcePath, destPath);
                }
                catch (Exception)
                {
                    throw;
                }
                Debug.Log($"Succesfully moved diredtory: {sourcePath} to {destPath}");
            }
            else
            {
                Debug.LogWarning($"Directory does not exist: {sourcePath}");
            }
        }
    }
}
