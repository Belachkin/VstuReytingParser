using VstuReytingParser;

namespace VstuParser
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            
            Console.Write("Введите снилс: ");
            string snils = Console.ReadLine();

            int id_levels = 0;
            int id_forms = 0;

            string forms = string.Empty;
            string levels = string.Empty;
            while (id_levels == 0)
            {
                while (id_forms == 0)
                {
                    Console.Write("Выберите форму обучения очная(1), очно-заочная(2), заочная(3): ");
                    forms = Console.ReadLine();
                    

                    if (forms == "1" || forms == "2" || forms == "3")
                    {
                        id_forms = Convert.ToInt32(forms);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error");
                        Console.ResetColor();
                    }
                }


                switch (id_forms)
                {
                    case 1:

                        Console.Write("Аспиратура(6) Бакалавр(3), Магистратура(4), Специалитет(5): ");
                        id_levels = Convert.ToInt32(Console.ReadLine());
                        break;
                    case 2:
                        Console.Write("Бакалавр(3), Магистратура(4):");
                        id_levels = Convert.ToInt32(Console.ReadLine());
                        break;
                    case 3:
                        Console.Write("Бакалавр(3), Магистратура(4), Специалитет(5): ");
                        id_levels = Convert.ToInt32(Console.ReadLine());
                        break;
                    default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Error");
                        Console.ResetColor();
                        break;

                }
            }

            string response = $"id_years=10&id_highschool=1&id_forms={id_forms}&id_levels%5B%5D={id_levels}";

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Поиск данных..");
            Console.ResetColor();         

            Parsing parsing = new Parsing();
            var allTableHtml = await parsing.ParseAllTable(response);
            var links = parsing.ExtractLinks(allTableHtml);
            var tableFacs = parsing.ExtractTableData(allTableHtml);
            var tableUser = await parsing.GetUserTable(links, snils);

            if(tableUser.Count <= 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Данные не обнаружены!");
                Console.ResetColor();

                return;
            }

            var currentFacTable = new List<List<string>> {
                new List<string>{"Факультет",  "План набора", "Подано на бюджет", "Подано на контракт Заявлений/Заключено"}
            };

            foreach ( var fac in tableFacs)
            {
                foreach ( var user in tableUser)
                {
                    if (user[12] == fac[0])
                    {                      
                        currentFacTable.Add(fac);                      
                    }
                }
            }

            

            CombineLists(currentFacTable, tableUser);

            Console.WriteLine("\n\n");
            ViewInfo.PrintTable(currentFacTable, 23);
            
            
        }

        public static void CombineLists(List<List<string>> facs, List<List<string>> users)
        {          
            facs.RemoveAll(fac => fac[2] == "-" && fac[3] == "-");

            foreach (var user in users)
            {
                var searchValue = user[12];

                var facIndex = facs.FindIndex(fac => fac[0] == searchValue);

                if (facIndex != -1)
                {
                    var userWithoutSearchValue = user.Take(12).ToList();
                    facs[facIndex].AddRange(userWithoutSearchValue);
                }
            }
        }
    }
}
