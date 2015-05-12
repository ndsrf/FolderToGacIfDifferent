using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices.Internal;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace FolderToGacIfDifferent
{
    internal class Program
    {
        private const int EXTENSIONLENGHT = 4; // ".exe" or ".dll"

        private static void Main(string[] args)
        {
            try
            {
                bool uninstallMode = false;
                string[] searchPatterns;

                if (args.Length == 0)
                {
                    Console.WriteLine("Usage: " + AppDomain.CurrentDomain.FriendlyName + "<DLL_PATH>  [/u] [Comma separated patterns including extensions]");
                    Console.WriteLine("Example: " + AppDomain.CurrentDomain.FriendlyName + " c:\\gac");
                    Console.WriteLine("Example: " + AppDomain.CurrentDomain.FriendlyName + " c:\\gac /u Sequel*.dll,Sequel*.exe");
                    Console.WriteLine("By default it will install all the *.exe and *.dll files from the specified folder.");

                }
                else if (!Program.IsUserAdministrator())
                {
                    Console.WriteLine("This application must be run as an Administrator.");
                }
                else
                {
                    if (args.Length > 1)
                    {
                        uninstallMode = "/u".Equals(args[1], StringComparison.CurrentCultureIgnoreCase);
                    }

                    Publish publish = new Publish();
                    DirectoryInfo directoryInfo = new DirectoryInfo(args[0]);
                    Console.WriteLine("Working folder " + directoryInfo.FullName);
                    if (args.Length > 2)
                    {
                        searchPatterns = args[2].Split(',');
                    }
                    else
                    {
                        searchPatterns = new string[] { "*.dll", "*.exe" };
                    }
                    var fileList = MyDirectory.GetFiles(directoryInfo.FullName, searchPatterns, System.IO.SearchOption.TopDirectoryOnly);
                    foreach (string fileName in fileList)
                    {
                        FileInfo fileInfo = new FileInfo(fileName);

                        if (uninstallMode)
                        {
                            Console.WriteLine("Removing " + (object)fileInfo + " from the GAC.");
                            publish.GacRemove(fileInfo.FullName);
                        }
                        else
                        {
                            bool flag = false;
                            List<FileInfo> list = Program.SearchForFileInGAC(fileInfo);
                            foreach (FileInfo fileInfo2 in list)
                            {
                                if (Comparisons.FilesContentsAreEqual(fileInfo, fileInfo2))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag && Enumerable.Count<FileInfo>((IEnumerable<FileInfo>)list) > 0)
                            {
                                Console.WriteLine("Removing " + (object)fileInfo + " from the GAC.");
                                publish.GacRemove(fileInfo.FullName);
                            }
                            if (!flag)
                            {
                                Console.WriteLine("Adding " + (object)fileInfo + " to the GAC.");
                                publish.GacInstall(fileInfo.FullName);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ouch!... " + ex.Message);
                Console.WriteLine(ex.StackTrace.ToString());
            }
        }

        private static List<FileInfo> SearchForFileInGAC(FileInfo fileInput)
        {
            List<FileInfo> list = new List<FileInfo>();
            string[] strArray;
            try
            {
                strArray = ConfigurationManager.AppSettings["GACFOLDERS_COMMASEPARATED"].Split(',');
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot read from config file the GACFOLDERS_COMMASEPARATED setting...", ex);
            }
            string str = fileInput.Name.Substring(0, fileInput.Name.Length - EXTENSIONLENGHT);
            foreach (object obj in strArray)
            {
                DirectoryInfo directoryInfo1 = new DirectoryInfo((string)obj + (object)'\\' + str);
                if (directoryInfo1.Exists)
                {
                    foreach (DirectoryInfo directoryInfo2 in directoryInfo1.EnumerateDirectories())
                    {
                        IEnumerable<FileInfo> source = directoryInfo2.EnumerateFiles(fileInput.Name);
                        if (Enumerable.Count<FileInfo>(source) == 1)
                            list.Add(Enumerable.First<FileInfo>(source));
                        else if (Enumerable.Count<FileInfo>(source) > 1)
                            throw new ApplicationException("More than 1 file in this folder: " + (object)directoryInfo2);
                    }
                }
            }
            return list;
        }

        public static bool IsUserAdministrator()
        {
            try
            {
                return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
