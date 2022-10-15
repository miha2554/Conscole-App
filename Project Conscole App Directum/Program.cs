namespace Test
{
    internal class Program
    {
        public const string dateTimeFormat = "dd-MM-yyyy HH:mm:ss";
        public const string dateFormat = "dd-MM-yyyy";
        private const string helpMessage = @"add    - Создать новую встречу
remove - Удалить одну из встреч
list   - Показать все встречи
edit   - Редактировать встречу
export - Экспортировать в файл
exit   - Выйти из программы";

        public static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Введите help, что бы увидеть все комманды.");
            while (true)
            {
                string command = ReadStr("> ").Trim().ToLower();
                switch (command)
                {
                    case "help":
                        Console.WriteLine(helpMessage);
                        break;
                    case "add":
                        Console.Clear();
                        Console.WriteLine("Создание встречи.");

                        string name = ReadStr("Введите название встречи: ");
                        DateTimeOffset start = ReadDate(
                            $"Введите начало встречи (формат: {dateTimeFormat}): ",
                            dateTimeFormat
                        );
                        DateTimeOffset end = ReadDate(
                            $"Введите конец встречи (формат: {dateTimeFormat}): ",
                            dateTimeFormat
                        );

                        if (start.ToUnixTimeSeconds() >= end.ToUnixTimeSeconds())
                        {
                            Console.WriteLine("Конец происходит раньше, чем начало?!?!😳 \n");
                            break;
                        }

                        uint notificationTime = ReadUInt("За сколько минут вас предупредить о вcтрече: ");

                        try
                        {
                            MeetingsList.AddToList(new Meeting(name, start, end, notificationTime));
                        } catch(ArgumentException)
                        {
                            Console.WriteLine("Данная встреча пересекается с другой встречей.");
                            break;
                        } catch(OverflowException)
                        {
                            Console.WriteLine("Время встречи уже прошло");
                            break;
                        }

                        Console.WriteLine("Встреча создана.\n");

                        break;
                    case "edit":
                        if (MeetingsList.meetings.Count == 0)
                        {
                            Console.WriteLine("Список встреч пуст.\n");
                            break;
                        }

                        WriteList();

                        int index = Convert.ToInt32(ReadUInt("Введите индекс встречи: "));
                        if (index > MeetingsList.meetings.Count)
                        {
                            Console.WriteLine("Неверный индекс.");
                            break;
                        }

                        name = ReadStr("Введите название встречи: ");
                        start = ReadDate(
                            $"Введите начало встречи (формат: {dateTimeFormat}): ",
                            dateTimeFormat
                        );
                        end = ReadDate(
                            $"Введите конец встречи (формат: {dateTimeFormat}): ",
                            dateTimeFormat
                        );
                        if (start.ToUnixTimeSeconds() >= end.ToUnixTimeSeconds())
                        {
                            Console.WriteLine("Конец происходит раньше, чем начало?!?!😳 \n");
                            break;
                        }

                        notificationTime = ReadUInt("За сколько минут вас предупредить о встрече: ");

                        MeetingsList.meetings[index].name = name;
                        MeetingsList.meetings[index].start = start;
                        MeetingsList.meetings[index].end = end;
                        MeetingsList.meetings[index].notificationTime = notificationTime;



                        Console.WriteLine("Встреча изменена.\n");

                        break;
                    case "list":
                        if (MeetingsList.meetings.Count == 0)
                        {
                            Console.WriteLine("Список встреч пуст.\n");
                            break;
                        }
                        WriteList();
                        break;
                    case "remove":
                        if (MeetingsList.meetings.Count == 0)
                        {
                            Console.WriteLine("Список встреч пуст.\n");
                            break;
                        }

                        WriteList();

                        index = Convert.ToInt32(ReadUInt("Введите индекс встречи: "));
                        if (index < 0 || index > MeetingsList.meetings.Count)
                        {
                            Console.WriteLine("Неверный индекс.");
                            break;
                        }

                        MeetingsList.RemoveByIndex(index);
                        Console.WriteLine("Встреча удалена.");
                        break;
                    case "export":
                        string path = ReadStr("Введите путь для экспорта (c:/path/to/file.json): ");
                        if (!IsValidPath(path))
                        {
                            Console.WriteLine("Указанный путь невозможен.");
                            break;
                        }

                        DateTimeOffset exportDate = ReadDate("Введите дату: ", dateFormat);

                        MeetingsList.ExportToFile(path, exportDate);
                        break;
                    case "exit":
                        System.Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine($"Команда {command} не найдена.");
                        Console.WriteLine("Введите help, что бы увидеть все комманды.");
                        break;
                }
            }
        }

        private static string ReadStr(string msg)
        {
            while (true)
            {
                Console.Write(msg);
                string? str = Console.ReadLine();
                if (str != null)
                {
                    return str;
                }
                else
                {
                    Console.WriteLine("Ошибка чтения строки, повторите ввод");
                }
            }
        }

        private static uint ReadUInt(string msg) {
            while (true)
            {
                try
                {
                    return UInt32.Parse(ReadStr(msg));
                } catch (FormatException)
                {
                    Console.WriteLine("Введите число");
                } catch (System.OverflowException)
                {
                    Console.WriteLine("Вы ввели число вне принимаемого диапазона");
                }
            }
        }


        public static DateTimeOffset ReadDate(string msg, string format)
        {
            while (true)
            {
                try
                {
                    return DateTimeOffset.ParseExact(
                        ReadStr(msg),
                        format,
                        System.Globalization.CultureInfo.InvariantCulture
                    );
                }
                catch (FormatException)
                {
                    Console.WriteLine("Неправильный формат ввода, попробуйте снова.");
                }

            }
        }

        private static void WriteList()
        {
            foreach (var (value, i) in MeetingsList.meetings.Select((value, i) => (value, i)))
            {
                Console.WriteLine($"{i} - {value.name}. Начало: {value.start.ToString(dateTimeFormat)}, конец: {value.end.ToString(dateTimeFormat)} ");
            }
            Console.Write("\n");
        }
        private static bool IsValidPath(string path, bool allowRelativePaths = false)
        {
            bool isValid = true;

            try
            {
                string fullPath = Path.GetFullPath(path);

                if (allowRelativePaths)
                {
                    isValid = Path.IsPathRooted(path);
                }
                else
                {
                    string root = Path.GetPathRoot(path)!;
                    isValid = string.IsNullOrEmpty(root.Trim(new char[] { '\\', '/' })) == false;
                }
            }
            catch (Exception)
            {
                isValid = false;
            }

            return isValid;
        }

    }   
}
