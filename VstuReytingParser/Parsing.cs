using HtmlAgilityPack;
using System.Net.Http.Headers;

namespace VstuReytingParser
{
    public class Parsing
    {            
        public async Task<string> ParseAllTable(string request)
        {
            // URL для отправки POST-запроса
            var url = "https://welcome.vstu.ru/acceptance/reyting/content.php";

            // Создание HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Установка заголовков запроса
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:127.0) Gecko/20100101 Firefox/127.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                client.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
                client.DefaultRequestHeaders.Add("Origin", "https://welcome.vstu.ru");
                client.DefaultRequestHeaders.Add("Referer", "https://welcome.vstu.ru/acceptance/reyting/");
                client.DefaultRequestHeaders.Add("Cookie", "PHPSESSID=to00cj8l7a8pu9vqh2mpaftgvd; BITRIX_SM_GUEST_ID=1863683; BITRIX_SM_LAST_VISIT=22.07.2024%2009%3A20%3A18; BITRIX_CONVERSION_CONTEXT_s1=%7B%22ID%22%3A1%2C%22EXPIRE%22%3A1721681940%2C%22UNIQUE%22%3A%5B%22conversion_visit_day%22%5D%7D; supportOnlineTalkID=n0PTJW6wtdzl9HMW0BlT0JRq7ODst39P; _ym_uid=1721628373809715348; _ym_d=1721628373; _ym_isad=2");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("Priority", "u=1");

                // Параметры POST-запроса
                var content = new StringContent(request, System.Text.Encoding.UTF8, "application/x-www-form-urlencoded");

                // Отправка POST-запроса и получение ответа
                HttpResponseMessage response = await client.PostAsync(url, content);

                

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    // Чтение содержимого ответа
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(responseBody);
                    return responseBody;
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                    return string.Empty;
                }
            }
        }

        public async Task<string> ParseUserTable(string Url)
        {
            // URL для GET-запроса
            var url = $"https://welcome.vstu.ru{Url}";
            //Console.WriteLine(url);
            // Создание HttpClient
            using (HttpClient client = new HttpClient())
            {
                // Установка заголовков запроса
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:127.0) Gecko/20100101 Firefox/127.0");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xhtml+xml"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml", 0.9));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/avif"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("image/webp"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*", 0.8));
                client.DefaultRequestHeaders.Add("Accept-Language", "ru-RU,ru;q=0.8,en-US;q=0.5,en;q=0.3");
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br, zstd");
                client.DefaultRequestHeaders.Add("Referer", "https://welcome.vstu.ru/acceptance/reyting/");
                client.DefaultRequestHeaders.Add("Cookie", "PHPSESSID=to00cj8l7a8pu9vqh2mpaftgvd; BITRIX_SM_GUEST_ID=1863683; BITRIX_SM_LAST_VISIT=22.07.2024%2009%3A23%3A59; BITRIX_CONVERSION_CONTEXT_s1=%7B%22ID%22%3A1%2C%22EXPIRE%22%3A1721681940%2C%22UNIQUE%22%3A%5B%22conversion_visit_day%22%5D%7D; supportOnlineTalkID=n0PTJW6wtdzl9HMW0BlT0JRq7ODst39P; _ym_uid=1721628373809715348; _ym_d=1721628373; _ym_isad=2");
                client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "document");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "navigate");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-origin");
                client.DefaultRequestHeaders.Add("Sec-Fetch-User", "?1");
                client.DefaultRequestHeaders.Add("Priority", "u=1");

                // Отправка GET-запроса и получение ответа
                HttpResponseMessage response = await client.GetAsync(url);

                // Проверка успешности ответа
                if (response.IsSuccessStatusCode)
                {
                    // Чтение содержимого ответа
                    string responseBody = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(responseBody);

                    return responseBody;
                }
                else
                {
                    Console.WriteLine($"Ошибка: {response.StatusCode}");
                    return string.Empty;
                }
            }
        }

        public List<string> ExtractLinks(string html)
        {
            var links = new List<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Извлечение всех узлов <a> с атрибутом href
            var linkNodes = htmlDoc.DocumentNode.SelectNodes("//a[@href]");

            if (linkNodes != null)
            {
                foreach (var linkNode in linkNodes)
                {
                    // Получение значения href
                    var hrefValue = linkNode.GetAttributeValue("href", string.Empty);
                    if (!string.IsNullOrEmpty(hrefValue) && hrefValue.Contains("acceptance"))
                    {
                        links.Add(hrefValue);
                    }
                }
            }          
            return links;
        }

        public List<List<string>> ExtractTableData(string html)
        {
            var tableData = new List<List<string>>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            // Извлечение таблицы по классу
            var tableNode = htmlDoc.DocumentNode.SelectSingleNode("//table[@class='choice-form_result_table']");

            if (tableNode != null)
            {
                // Извлечение строк таблицы
                var rowNodes = tableNode.SelectNodes(".//tr");

                if (rowNodes != null)
                {
                    foreach (var rowNode in rowNodes)
                    {
                        var rowData = new List<string>();
                        // Извлечение ячеек в строке (как th, так и td)
                        var cellNodes = rowNode.SelectNodes(".//th|.//td");

                        if (cellNodes != null)
                        {
                            foreach (var cellNode in cellNodes)
                            {
                                if(cellNode != null)
                                {
                                    // Извлечение текста из ячейки
                                    var cellText = cellNode.InnerText.Trim();
                                    if(cellText == string.Empty || cellText == "")
                                    {
                                        cellText = "-";
                                    }
                                    rowData.Add(cellText);
                                    
                                    
                                }
                                
                            }
                        }

                        tableData.Add(rowData);
                    }
                }
            }

            return tableData;
        }

        public static List<string> ExtractUserTableData(string html, string snils)
        {
            var tableData = new List<List<string>>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var h1Node = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='unit-75']/h1");
            string h1Text = h1Node != null ? h1Node.InnerText.Trim() : "H1 not found";

            var rows = htmlDoc.DocumentNode.SelectNodes("//table[@class='choice-form_result_table']//tr");
            
            if (rows != null)
            {
                foreach (var row in rows)
                {
                    var rowData = new List<string>();
                    var cells = row.SelectNodes("th|td");

                    if (cells != null)
                    {
                        rowData.AddRange(cells.Select(cell => cell.InnerText.Trim()));
                        tableData.Add(rowData);
                    }
                }
            }

            List<string> userdata = new List<string>();
            
            foreach (var items in tableData)
            {
                foreach(var item in items)
                {
                    
                    if (item == snils)
                    {
                        int index = tableData.IndexOf(items);
                        userdata = items;
                        userdata[0] = $"[{index}] {userdata[0]}";
                        userdata.Add(h1Text);
                        
                                                                                             
                        break;
                    }
                        
                }
            }

            return userdata;
        }

        public async Task<List<List<string>>> GetUserTable(List<string> links, string snils)
        {
            int cout = 0;
            List<List<string>> UserData = new List<List<string>>();
            foreach(var link in links)
            {
                if(cout != 5)
                {
                    var data = ExtractUserTableData(await ParseUserTable(link), snils);

                    if (data.Count != 0)
                    {
                        data[0] += $" {link}";
                        UserData.Add(data);
                        cout++;
                        Console.SetCursorPosition(0, Console.CursorTop);

                        Console.Write($"Найдено совпадений ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        
                        Console.Write($"[{cout}/5]");
                        Console.ResetColor();
                    }
                    
                }
                else
                {
                    break;
                }
                

                
            }

            List<List<string>> table = new List<List<string>>
            {
                new List<string>{ "Рейтинг для зачисления", "УИД", "Документ", "Сумма баллов", "-", "-", "-", "-", "Баллы ИД", "Преим. право", "Приоритет", "Заявление", "Факультет" }
            };
                       
            foreach (var items in UserData)
            {
                if (items.Count != 0)
                {
                    table.Add(items);

                }

            }

            return table;

            
        }

        
    }
}
