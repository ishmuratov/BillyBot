using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace PictureSearchExample
{
    public static class FileWorker
    {
        public static void SaveToFile(IData _data, string _filePath)
        {
            if (_data == null)
            {
                return;
            }

            try
            {
                using (var fs = new FileInfo(_filePath).Create())
                {
                    var formatter = new BinaryFormatter();
                    formatter.Serialize(fs, _data);
                }
            }
            catch (Exception _ex)
            {
                Logger.IOLog($"Ошибка. Файл {_filePath} не может быть сохранён. " + _ex.Message);
            }
        }

        public static IData LoadFromFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                return null;
            }

            try
            {
                using (var fs = new FileInfo(filePath).Open(FileMode.Open))
                {
                    var formatter = new BinaryFormatter();
                    return formatter.Deserialize(fs) as IData;
                }
            }
            catch (Exception _ex)
            {
                Logger.IOLog($"Ошибка файла {filePath} : файл повреждён. " + _ex.Message);
            }

            return null;
        }

        public static void WriteToFile(string _data, string _filePath)
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(_filePath, true))
                {
                    sw.WriteLine(_data);
                }
            }
            catch (Exception _ex)
            {
                Logger.IOLog($"Ошибка. Файл {_filePath} не может быть сохранён. " + _ex.Message);
            }
        }

        public static string ReadFromTXTFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Logger.IOLog($"Ошибка файла {filePath} : файл не найден! ");
                return string.Empty;
            }
            else
            {
                return File.ReadAllText(filePath);
            }
        }
    }
}
