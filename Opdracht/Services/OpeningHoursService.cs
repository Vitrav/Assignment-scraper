using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Opdracht.Models;
using ScrapySharp.Extensions;
using ScrapySharp.Network;

namespace Opdracht.Services
{
    public class OpeningHoursService : IOpeningHoursService
    {
        private readonly string _urlToScrape = "";
        private readonly List<string> _hours = new List<string>();
        private static ScrapingBrowser _browser = new ScrapingBrowser();
        private readonly HtmlNode _html;
        private List<OpeningHours> _openingHours;

        public OpeningHoursService(string urlToScrape)
        {
            _urlToScrape = urlToScrape;
            _html = GetHtml();
            _openingHours = GetAllOpeningsDetails();
    
        }

        public bool IsOpenToday()
        {

            return true;
            return _openingHours.Where(y => y.Date.ToString("d") == DateTime.Now.ToString("d") && y.Opened == true && DateTime.Now < y.ClosingHour).Any();
        }

        public OpeningHours OpeningHoursToday()
        {
            //Testing purposes
            return new OpeningHours()
            {
                Date = DateTime.Now,
                Day = "Maandag",
                Opened = true,
                OpeningHour = (DateTime?)new DateTime(2021, DateTime.Now.Month, DateTime.Now.Day, 10, 0, 0),
                ClosingHour = (DateTime?)new DateTime(2021, DateTime.Now.Month, DateTime.Now.Day, 20, 0, 0),
            };

            return _openingHours.Select(x => x).Where(y => y.Date.ToString("d") == DateTime.Now.ToString("d") && y.Opened == true).First();
        }
        public Double TimeBetweenDates(OpeningHours storeOpen)
        {
            return (storeOpen.OpeningHour - DateTime.Now).Value.TotalMinutes;
        }
    
        public Double TimeBetweenHours(OpeningHours current)
        {
            //return (DateTime.Parse(current.ClosingHour) - DateTime.Now).TotalMinutes;
            return (current.ClosingHour - DateTime.Now).Value.TotalMinutes;

        }
        public OpeningHours GetClosestToOpen()
        {
            return _openingHours.Where(x => x.Opened == true).First();
        }

        private List<OpeningHours> GetAllOpeningsDetails()
        {
            HtmlNode bothWeeksOpeningHours = _html.OwnerDocument.DocumentNode.SelectSingleNode($"//*[@id=\"openinghours\"]/div");
            return ParseHours(bothWeeksOpeningHours); 
        }

        private List<OpeningHours> ParseHours(HtmlNode overallData)
        {
            List<OpeningHours> openingHours = new List<OpeningHours>();
            List<string> days = new List<string>() { "maandag", "dinsdag", "woensdag", "donderdag", "vrijdag", "zaterdag", "zondag" };
            int tempLineCount = 0;
            string[] splitted = overallData.InnerHtml.Split('\n');
            foreach (var line in splitted)
            {
                tempLineCount += 1;
                //If line contains the date to parse
                if (line.ToString().Contains("<p>")) {
                    Queue<DateTime> weekSpan = ConvertToDate(line);
                    //42 lines between the date selection + the end of it
                    for (int x = 0; x < 42; x++)
                    {
                        if(days.Contains(splitted[tempLineCount + x]))
                        {
                            var times = splitted[tempLineCount + x + 3];
                            var startAndEndTime = times != "gesloten"? ConvertToHours(splitted[tempLineCount + x + 3]) : null;
                            var date = weekSpan.Dequeue();
                            openingHours.Add(new OpeningHours()
                            {
                                Date = date,
                                Day = splitted[tempLineCount + x],
                                Opened = times == "gesloten" ? false : true,
                                OpeningHour = startAndEndTime != null ? (DateTime?)new DateTime(date.Year, date.Month, date.Day, Convert.ToInt32(startAndEndTime[0].Substring(0, 2)), 0, 0) : null,
                                ClosingHour = startAndEndTime != null ? (DateTime?)new DateTime(date.Year, date.Month, date.Day, Convert.ToInt32(startAndEndTime[1].Substring(0, 2)), 0, 0) : null,
                        });                    
                        }
                    }
                }
            }
            return openingHours;
        }

        private List<string> ConvertToHours(string line)
        {
            List<string> hours = new List<string>();
            var cultureInfo = new CultureInfo("nl-NL");
            var splitted = line.Split();
            hours.Add(splitted[0]);
            hours.Add(splitted[2]);
            return hours;
        }

        private Queue<DateTime> ConvertToDate(string line)
        {
            Queue<DateTime> dates = new Queue<DateTime>();
            var cultureInfo = new CultureInfo("nl-NL");
            var splitted = line.Split();
            var startDate = DateTime.Parse($"{splitted[0].Replace("<p>","")}/{splitted[1]}/{DateTime.Now.Year}", cultureInfo);
            var endDate = DateTime.Parse($"{splitted[3]}/{splitted[1].Replace("</p>", "")}/{DateTime.Now.Year}", cultureInfo);

            for(var day = startDate.Date; day.Date <= endDate.Date; day = day.AddDays(1))
            {
                dates.Enqueue(day);
            }
            return dates;
 
        }

        private HtmlNode GetHtml()
        {
            WebPage result = _browser.NavigateToPage(new Uri(_urlToScrape));
            return result.Html;
        }


    }
}
