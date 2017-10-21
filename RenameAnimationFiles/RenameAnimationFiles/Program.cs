using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameAnimationFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter path of Skin Folder containing the animation files:");
            string path = Console.ReadLine();

            DirectoryInfo info = new DirectoryInfo(path);
            DirectoryInfo[] sub = info.GetDirectories();

            foreach (DirectoryInfo item in sub.ToList())
            {
                int i = 0;
                foreach (FileInfo file in item.GetFiles("*.png").ToList())
                {
                    string numberOfImage;
                    if (i < 10) numberOfImage = "0" + i;
                    else numberOfImage = i.ToString();

                    string destination = item.FullName + "\\" + item.Name + "_" + numberOfImage + ".png";

                    File.Move(file.FullName, destination);
                    i++;
                }

                i = 0;
                foreach (FileInfo file in item.GetFiles("*.meta").ToList())
                {
                    string numberOfImage;
                    if (i < 10) numberOfImage = "0" + i;
                    else numberOfImage = i.ToString();

                    string destination = item.FullName + "\\" + item.Name + "_" + numberOfImage + ".meta";

                    File.Move(file.FullName, destination);
                    i++;
                }
            }
        }
    }
}
