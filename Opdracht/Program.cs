using System;
using Opdracht.Services;
namespace Opdracht
{
    class Program
    {
        static void Main(string[] args)
        {
            OpeningHoursService bijenkorf = new OpeningHoursService("https://www.debijenkorf.nl/rotterdam");
            //Console.WriteLine(bijenkorfOpeningstijden.IsOpenToday());
            PrintStatus();



            void PrintStatus()
            {
                bool open = true;
                switch (bijenkorf.IsOpenToday())
                {
                    case true:
                        var current = bijenkorf.OpeningHoursToday();
                        Console.WriteLine($"De Bijenkorf is vandaag geopend tussen: {current.OpeningHour} and {current.ClosingHour}");
                        while (open)
                        {
                          
                            TimeSpan ts = TimeSpan.FromMinutes(bijenkorf.TimeBetweenHours(current));
                            Console.WriteLine($"U heeft nog: {$"{ts.Hours} uur en {ts.Minutes} minuten"} om te shoppen");
                            ClearLine();
                        }
                        
                        break;
                    case false:
                        var closest = bijenkorf.GetClosestToOpen();
                        ;
                        Console.WriteLine($"De Bijenkorf is vandaag gesloten. U zult moeten wachten tot:  {closest.OpeningHour} uur");
                        while (open)
                        {
                            TimeSpan ts = TimeSpan.FromMinutes(bijenkorf.TimeBetweenDates(closest));
                            Console.WriteLine($"Dit duurt nog: {$"{ts.Days} dagen en {ts.Hours} uur en {ts.Minutes} minuten"}");
                            System.Threading.Thread.Sleep(1);
                            ClearLine();
                        }
                        break; 
                }
                
            }

            static void ClearLine()
            {
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }

        }

       


    }
}
