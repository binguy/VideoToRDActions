using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace VideoToRDActions
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Directory to your level files:");
            string PuthToLevel = Console.ReadLine().Replace("\"", "");
            List<string> LevelLines = File.ReadAllLines(PuthToLevel).ToList();
            var images = new List<string>();
            Console.WriteLine("Directory to your video file:");
            string PathToFile = Console.ReadLine().Replace("\"", "");//Video File
            if (!Directory.Exists("frames"))
            {
                Directory.CreateDirectory("frames");
            }
            Process prucc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "CMD.exe";
            startInfo.Arguments = $"/C ffmpeg -i \"{PathToFile}\" \"frames/out-%03d.jpg\"";
            prucc.StartInfo = startInfo;
            prucc.Start();
            prucc.WaitForExit();
            images = Directory.GetFiles("frames").ToList();
            string NormImage = "";
            var NormImageList = new List<string>();
            foreach(string image in images)
            {
                NormImage = image.Replace("frames\\", "");
                NormImageList.Add(NormImage);
            }
            string FinalImages = JsonConvert.SerializeObject(NormImageList).Replace(",", ", ");
            Console.WriteLine("FPS of video:");
            string fps = Console.ReadLine();
            Console.WriteLine("What rooms do you want to use? write something like: 0,1 or 1 or 1,2,3");
            string Roomus = JsonConvert.SerializeObject(Array.ConvertAll(Console.ReadLine().Split(','), int.Parse).ToList());
            Console.WriteLine("What bar and beat do you want to use? write something like: [BAR NUMBER],[BEAT NUMBER]. For example: 1,1 or 2,4");
            int[] barboot = Array.ConvertAll(Console.ReadLine().Split(','), int.Parse);
            string BackImages = "{ "+ $"\"bar\": {barboot[0]}, \"beat\": {barboot[1]}, \"y\": 0, \"type\": \"SetBackgroundColor\", \"rooms\": {Roomus}, \"backgroundType\": \"Image\", \"contentMode\": \"AspectFit\", \"color\": \"FFFFFFFF\", \"image\": {FinalImages}, \"fps\": {fps}, \"filter\": \"NearestNeighbor\", \"scrollX\": 0, \"scrollY\": 0"+" },";
            StringBuilder sb = new StringBuilder();
            bool CheckerForVibe = false;
            foreach(string line in LevelLines)
            {
                if(line.Contains("\"events\":"))
                {
                    CheckerForVibe = true;
                    sb.Append(line+"\n");
                }else if (CheckerForVibe && line.Contains("["))
                {
                    sb.Append(line + $"\n		{BackImages}\n");
                    CheckerForVibe = false;
                }
                else
                {
                    sb.Append(line+"\n");
                }
            }
            string directoryPrev = Path.GetDirectoryName(PuthToLevel);
            //Console.WriteLine(directoryPrev);
            using var sw = new StreamWriter(PuthToLevel.Replace(".rdlevel", "Video.rdlevel"));
            sw.WriteLine(sb.ToString());
            Console.WriteLine("\nData written to file " + PuthToLevel.Replace(".rdlevel", "Video.rdlevel"));
            foreach(string s in images)
            {
                string fileName = Path.GetFileName(s);
                string destFile = Path.Combine(directoryPrev, fileName);
                File.Copy(s, destFile, true);
            }
            foreach(string si in images)
            {
                File.Delete(si);
            }
            Directory.Delete("frames");
            Console.WriteLine("Press Any Key To Exit");
            Console.ReadKey();

        }
    }
}
