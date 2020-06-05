using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using System.Xml;
using HtmlAgilityPack;
using Interfaces;
using Models;

namespace Application.Done
{
    public class Integration : IWebSiteIntegration
    {
        public WebPage WebPage { get; }
        public IDumpsRepository DumpsRepository { get; }

        public IEqualityComparer<Entry> EntriesComparer { get; }
        public Integration() { }
        public Integration(IDumpsRepository dumpsRepository,
            IEqualityComparer<Entry> equalityComparer)
        {
            DumpsRepository = dumpsRepository;
            EntriesComparer = equalityComparer;
            WebPage = new WebPage
            {
                Url = "http://nportal.pl/mieszkania/?page=270",
                Name = "Mitula WebSite Integration",
                WebPageFeatures = new WebPageFeatures
                {
                    HomeSale = false,
                    HomeRental = false,
                    HouseSale = false,
                    HouseRental = false
                }
            };
        }

        public Dump GenerateDump()
        {

            return new Dump
            {

                DateTime = DateTime.Now,
                WebPage = WebPage,
                Entries = GetEntries(WebPage)

            };
        }

        public List<Entry> GetEntries(WebPage webpage)
        {
            List<Entry> entries = new List<Entry>();
            // ładowanie pierwszej strony
            var htmlDoc = LoadWebPage(webpage.Url);
            var adslist = htmlDoc.DocumentNode.SelectNodes("//div[@id='listContainer']/*/div[@class='slist']/*/div[@class='slr_left']/h2/a");
            
            
            //przechodzenie po stronach
            var nextPage = htmlDoc.DocumentNode.SelectNodes("//*/footer/*/div[@class='paginator clearfix']/a[@class='navigate next']").First().Attributes["href"].Value;
            do
             {
            // przetwarzanie strony, dodawanie ogłoszeń ze strony
            foreach (var node in adslist)
                    entries.Add(GetEntry(node.Attributes["href"].Value));

                // ładowanie następnej strony
                htmlDoc = LoadWebPage(nextPage);
                adslist = htmlDoc.DocumentNode.SelectNodes("//div[@id='listContainer']/*/div[@class='slist']/*/div[@class='slr_left']/h2/a");
                try { nextPage = htmlDoc.DocumentNode.SelectNodes("//*/footer/*/div[@class='paginator clearfix']/a[@class='navigate next']").First().Attributes["href"].Value; }
                catch { nextPage = null; }
            } while (nextPage != null) ;

            return entries;
        }

        public List<Entry> GetEntriesPage(WebPage webpage)
        {
            List<Entry> entries = new List<Entry>();
            // ładowanie pierwszej strony
            var htmlDoc = LoadWebPage(webpage.Url);
            var adslist = htmlDoc.DocumentNode.SelectNodes("//div[@id='listContainer']/*/div[@class='slist']/*/div[@class='slr_left']/h2/a");


            //przechodzenie po stronach
            var nextPage = htmlDoc.DocumentNode.SelectNodes("//*/footer/*/div[@class='paginator clearfix']/a[@class='navigate next']").First().Attributes["href"].Value;
            
                // przetwarzanie strony, dodawanie ogłoszeń ze strony
                foreach (var node in adslist)
                    entries.Add(GetEntry(node.Attributes["href"].Value));
            

            return entries;
        }

        public Entry GetEntry(string url)
        {
            var page = LoadWebPage(url);
            return new Entry
            {
                OfferDetails = new OfferDetails
                {
                    Url = url,
                    CreationDateTime = getCreationDateTime(page),
                    OfferKind = GetOfferKind(url),
                    SellerContact = new SellerContact
                    {
                        //Email = getEmail(page),
                        Telephone = getTelephone(page),
                        Name = getName(page),
                        
                    },
                    IsStillValid = true
                },
                PropertyPrice = new PropertyPrice
                {
                    TotalGrossPrice = getTotalGrossPrice(page),
                    PricePerMeter = getPricePerMeter(page),
                    //ResidentalRent = ,
                },
                PropertyDetails = new PropertyDetails
                {
                    Area = GetArea(page),
                    NumberOfRooms = getNumberOfRooms(page),
                    FloorNumber = getFloorNumber(page),
                    YearOfConstruction = getYearOfConstruction(page),
                },
                PropertyAddress = new PropertyAddress
                {
                    City = GetCity(page),
                    //District = ,
                    StreetName = getStreetName(page),
                    //DetailedAddress = ,
                },
                PropertyFeatures = new PropertyFeatures
                {
                    GardenArea = getGardenArea(page),
                    Balconies = getBalconies(page),
                    BasementArea = getBasementArea(page),
                    OutdoorParkingPlaces = getOutdoorParkingPlaces(page),
                    IndoorParkingPlaces = getIndoorParkingPlaces(page),
                },
                RawDescription = "Kup Teraz!",
            };
        }

        public string getStreetName(HtmlDocument page)
        {
            string street;
            street = page.DocumentNode.SelectSingleNode(".//*/div[@id='description']").InnerText;
            street = street.Remove(0, street.IndexOf("ul.")+3).TrimStart();
            street = street.Remove(street.IndexOf(" "));

            return street;
        }

        public PolishCity GetCity(HtmlDocument page)
        {
            PolishCity ecity;
            string city = "";
            List<string> cities = new List<string>();

            try
            {
                city = page.DocumentNode.SelectSingleNode(".//*/h1[@id='location']").InnerText;
                cities = city.Replace("\n", String.Empty).Split(',').ToList();
                foreach (var c in cities)
                {
                    city = RemoveDiacritics( c.ToUpper().Trim());
                    if (Enum.TryParse(city, out ecity))
                        return ecity;
                    foreach (var d in cities)
                    {
                        city = city + "_" + RemoveDiacritics(c.ToUpper().Trim());
                        if (Enum.TryParse(city, out ecity))
                            return ecity;
                    }
                }
            }
            catch
            {
                ;
            }

            return 0;
        }

        public decimal getBasementArea(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Piwnica:")
                        {
                                if (child.NextSibling.NextSibling.InnerText.IndexOf("TAK")>=0)
                                {
                                    return 1;
                                }
                                else
                                {
                                    area = child.NextSibling.NextSibling.InnerText;
                                    area = area.Replace("\n", String.Empty).Replace(" m²", "").Trim();
                                    var areaa = decimal.Parse(area);
                                    return areaa;
                                }
                        }
                        else
                            return 0;

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public int getOutdoorParkingPlaces(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Miejsca parkingowe:" && child.NextSibling.NextSibling.InnerText.IndexOf("postojowe") >= 0)
                        {
                            foreach (var child2 in row.SelectNodes("th|td"))
                                if (child.InnerText == "Liczba miejsc parkingowych:")
                                {
                                    area = child.NextSibling.NextSibling.InnerText;
                                    area = area.Replace("\n", String.Empty).Trim();
                                    var areaa = int.Parse(area);
                                    return areaa;
                                }
                                else
                                    return 1;
                        }
                        else
                            return 0;

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public int getIndoorParkingPlaces(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Miejsca parkingowe:" && child.NextSibling.NextSibling.InnerText.IndexOf("podziemny")>=0)
                        {
                            foreach (var child2 in row.SelectNodes("th|td"))
                                if (child.InnerText == "Liczba miejsc parkingowych:")
                                {
                                    area = child.NextSibling.NextSibling.InnerText;
                                    area = area.Replace("\n", String.Empty).Trim();
                                    var areaa = int.Parse(area);
                                    return areaa;
                                }
                                else
                                    return 1;
                        }
                        else
                            return 0;

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public int getBalconies(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Balkon:")
                        {
                            area = child.NextSibling.NextSibling.InnerText;
                            if( area.IndexOf("TAK")>=0 )
                                return 1;
                            else
                            return 0;
                        }

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public int getYearOfConstruction(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Rok budowy:")
                        {
                            area = child.NextSibling.NextSibling.InnerText;
                            area = area.Replace("\n", String.Empty).Trim();
                            var areaa = int.Parse(area);
                            return areaa;
                        }

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public int getFloorNumber(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Piętro:")
                        {
                            area = child.NextSibling.NextSibling.InnerText;
                            area = area.Replace("\n", String.Empty).Trim();
                            var areaa = int.Parse(area);
                            return areaa;
                        }

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public int getNumberOfRooms(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Liczba pokoi:")
                        {
                            area = child.NextSibling.NextSibling.InnerText;
                            area = area.Replace("\n", String.Empty).Trim();
                            var areaa = int.Parse(area);
                            return areaa;
                        }

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public decimal getGardenArea(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText.IndexOf( "gródek")>=0 || child.InnerText.IndexOf("gród") >= 0)
                        {
                            if (child.NextSibling.NextSibling.InnerText.IndexOf("TAK") >= 0)
                                return 1;
                            else
                            area = child.NextSibling.NextSibling.InnerText;
                            area = area.Replace("\n", String.Empty).Replace(" m²", "").Trim();
                            var areaa = decimal.Parse(area);
                            return areaa;
                        }

                }
            }
            catch
            {
                return 0;
            }

            return 0;

        }


        public decimal GetArea(HtmlDocument page)
        {
            string area = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td"))
                        if (child.InnerText == "Powierzchnia mieszkalna:")
                        {
                            area = child.NextSibling.NextSibling.InnerText;
                            area = area.Replace("\n", String.Empty).Replace(" m²", "").Trim();
                            var areaa = decimal.Parse(area);
                            return areaa;
                        }

                }
            }
            catch
            {
                area = "0";
            }

            return 0;

        }

        public decimal getPricePerMeter(HtmlDocument page)
        {
            string price = "1,0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach (var child in row.SelectNodes("th|td")) 
                    if (child.InnerText == "Cena za m&sup2;:")
                    {
                        price = child.NextSibling.NextSibling.InnerText;
                        price = price.Replace("\n", String.Empty).Replace(" zł/m&#178;", "").Trim();
                        var pricedec = decimal.Parse(price);
                        return pricedec;
                    }

                }
            }
            catch
            {
                price = "0";
            }

            return 0;

        }

        public decimal getTotalGrossPrice(HtmlDocument page)
        {
            string price = "1.0";

            try
            {
                foreach (var row in page.DocumentNode.SelectNodes(".//*/table[@id='paramsBasic']/tr"))
                {
                    foreach(var child in row.SelectNodes("th|td"))
                    if (child.InnerText == "Cena:")
                    {
                        price = child.NextSibling.NextSibling.InnerText;
                        price = price.Replace("\n", String.Empty).Replace("&nbsp;zł", "").Trim();

                        return decimal.Parse(price);
                    }

                }
            }
            catch
            {
                price = "0";
            }

            return 0;
        }


        public DateTime getCreationDateTime(HtmlDocument page)
        {
            string stime;
            try {stime = page.DocumentNode.SelectNodes("//*/div[@id='lastModfied']").First().InnerHtml; }
            catch { stime = ""; return DateTime.Now; }//jak inaczej nie przypisać daty?
            stime = stime.Remove(0, 22);
            stime = stime.Replace('-','/');
            var time = DateTime.ParseExact(stime, "yyyy/MM/dd HH:mm:ss", null);
            return time;
        }

        public OfferKind GetOfferKind(string url)
        {
            OfferKind offer;
            if (url.IndexOf("sprzedaz") > 0)
                offer =  OfferKind.SALE;
            else //do poprawy
                offer =  OfferKind.RENTAL;
            return offer;
        }

        public string getTelephone(HtmlDocument page)
        {
            string telephone;

            try
            {
                telephone = page.DocumentNode.SelectNodes("//*/section[@id='sidebar']/div/section/div/*/span[@class='hidden']").First().InnerHtml;
            }
            catch
            {
                telephone = "";
            }
            return telephone;
        }

        public string getName(HtmlDocument page)
        {
            string name;

            try
            {
                name = page.DocumentNode.SelectNodes("//*/div[@class='agentName']").First().InnerHtml;
                name = name.Replace("\n", String.Empty).TrimEnd().TrimStart();
            }
            catch
            {
                name = "";
            }
            return name;
        }


        public HtmlDocument LoadWebPage(string url)
        {
            var client = new WebClient();
            string text = client.DownloadString(url);
            var htmlDoc = new HtmlDocument();
            //Console.WriteLine(text.ToString());
            htmlDoc.LoadHtml(text);
            //Console.WriteLine(htmlDoc.ToString()); Wyświetlenie całej strony

            return htmlDoc;
        }

        public string RemoveDiacritics(string text)
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();
            foreach (char letter in arrayText)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(letter) != UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            return sbReturn.ToString();
        }
    }
}
        
