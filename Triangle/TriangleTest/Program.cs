using System.Diagnostics;

namespace MyApp
{
internal class Program
{
    static async Task Main(string[] args)
    {
        string path = args[0];

        using (StreamReader reader = new StreamReader(path))
        {
            string ? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (line == "")
                {
                    Console.WriteLine();
                    continue;
                }
                string[] subs = line.Split(' ');

                bool isSingle = true;
                string intendedResponse = subs[^1];
                if (subs[^1] == "треугольник" || subs[^1] == "ошибка")
                {
                    isSingle = false;
                    intendedResponse = subs[^2] + ' ' + subs[^1];
                }

                var psi = new ProcessStartInfo { FileName = @"../../../../Triangle/bin/Debug/net6.0/Triangle.exe",
                                                 UseShellExecute = false, RedirectStandardOutput = true };

                int n = 0;
                while (n < subs.Length - (isSingle ? 1 : 2))
                {
                    psi.ArgumentList.Add(subs[n]);
                    n++;
                }

                using var process = Process.Start(psi);
                using StreamReader output = process!.StandardOutput;

                string data = output.ReadLine() ?? "";
                if (data == intendedResponse)
                {
                    Console.WriteLine("sucсess;");
                    continue;
                }
                Console.WriteLine("error;");
            }
        }
    }
}
}
