using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace FolderToGacIfDifferent
{
    class Program
    {
        private const string EXTDLL = ".dll";

        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Usage: " + System.AppDomain.CurrentDomain.FriendlyName + " <DLL_PATH>");
                    Console.WriteLine("Example: " + System.AppDomain.CurrentDomain.FriendlyName + " c:\\gac");
                    return;
                }

                if (!Program.IsUserAdministrator())
                {
                    Console.WriteLine("This application must be run as an Administrator.");
                    return;
                }

                System.EnterpriseServices.Internal.Publish p = new System.EnterpriseServices.Internal.Publish();

                DirectoryInfo inputFolder = new DirectoryInfo(args[0]);

                Console.WriteLine("Working folder " + inputFolder.FullName);

                foreach (FileInfo fileInput in inputFolder.EnumerateFiles("*" + EXTDLL))
                {
                    bool fileFound = false;

                    List<FileInfo> fileDestinationList = SearchForFileInGAC(fileInput);

                    foreach (FileInfo fileDestination in fileDestinationList)
                    {
                        if (Comparisons.FilesContentsAreEqual(fileInput, fileDestination))
                        {
                            fileFound = true;
                            break;
                        }
                    }

                    if (!fileFound && fileDestinationList.Count() > 0)
                    {
                        // We need to remove the file from the gac
                        Console.WriteLine("Removing " + fileInput + " from the GAC.");
                        p.GacRemove(fileInput.Name.Substring(0, fileInput.Name.Length - EXTDLL.Length));
                    }

                    if (!fileFound)
                    {
                        // We neeed to add the file to the gac
                        Console.WriteLine("Adding " + fileInput + " to the GAC.");
                        p.GacInstall(fileInput.FullName);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ouch!... " + e.Message);
                Console.WriteLine(e.StackTrace.ToString());
            }

        }

        private static List<FileInfo> SearchForFileInGAC(FileInfo fileInput)
        {
            List<FileInfo> fileInGac = new List<FileInfo>();
            string[] strGACFolders = ConfigurationManager.AppSettings["GACFOLDERS_COMMASEPARATED"].Split(new char[] { ',' });

            string fileInputNameWithoutExtension = fileInput.Name.Substring(0, fileInput.Name.Length - EXTDLL.Length);

            foreach(string oneGacFolder in strGACFolders)
            {
                DirectoryInfo directory = new DirectoryInfo(oneGacFolder + '\\' + fileInputNameWithoutExtension);
                if (directory.Exists)
                {
                    foreach (DirectoryInfo folderPerVersion in directory.EnumerateDirectories())
                    {
                        var listTemp = folderPerVersion.EnumerateFiles(fileInput.Name);
                        if (listTemp.Count() == 1)
                        {
                            fileInGac.Add(listTemp.First());
                        }
                        else if (listTemp.Count() > 1)
                        {
                            throw new ApplicationException("More than 1 file in this folder: " + folderPerVersion);
                        }
                    }
                }
            }
            return fileInGac;
        }
        
        public static bool IsUserAdministrator()
        {
            //bool value to hold our return value
            bool isAdmin;
            try
            {
                //get the currently logged in user
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException)
            {
                isAdmin = false;
            }
            catch (Exception)
            {
                isAdmin = false;
            }
            return isAdmin;
        }
    }
}
