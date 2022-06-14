/*
Программа для учёта сотрудников на предприятии

У сотрудника есть основные атрибуты: Фамилия, Имя, Отчество, Дата рождения и пол, а также Должность и Доп.информация должности
Естественно у каждой должности индивидуальная доп.информация: Директор - Название предприятия; 
Начальник подразделения - Название подразделения; Контроллер - Название рабочей области; Рабочий - фамилия Начальника подразделения

Данная программа позволяет ввести учёт сотрудников на предприятии, а именно: Добавлять запись, Удалять запись, Изменять запись,
Вывод записей, Сохранения базы в отдельном файле, Ввод значений из файла в базу. 
Также введены дополнительные функции: Выборка по типу должности или по названию подразделения, также возможен вывод кол-ва записей в базе

Для завершения программы в меню работы введите 0
*/
using System;
using System.IO;
using System.Collections.Generic;

namespace WorkwithDataBase
{
    class Program
    {
        static int menu_variants;

        public const string tempfile = "Temporary.txt"; // Переменная имени временного файла
        public const string tempfileforcopying = "Copying.txt"; // Переменная имени временного для копирования файла
        public static readonly string[] WorkerType = { "Директор", "НП", "Контроллер", "Рабочий" }; // Массив должностей сотрудников
        static void Menu() // Функция вывода начального Меню работы базы
        {
            Console.Clear();
            Console.Write("Меню работы базы учёта персонала\n" +
                "--------------------------------\n" +
                "1.Добавить запись\n" +
                "2.Удалить запись\n" +
                "3.Изменить запись\n" +
                "4.Вывод записей\n" +
                "5.Сохранить данные\n" +
                "6.Ввести данные из файла\n" +
                "7.Выборка\n" +
                "8.Повысить сотрудника\n" +
                "9.Вывести кол-во записей\n" +
                "--------------------------------\n" +
                "Выберите вариант: ");
            menu_variants = Convert.ToInt32(Console.ReadLine());
            if (menu_variants > 9) Menu();
        }

        static void Main(string[] args)
        {

            Menu();

            bool recs_in_file = false; // Переменная, отвечающая за наличие хоть каких-то записей во временном файле. Изначально их нет
            int number_of_records = 0;

            while (menu_variants != 0)
            // Цикл постоянной работы программы, после выбора вырианта меню будет проработана соответсвующая функция и снова вызвано Меню
            {
                switch (menu_variants)
                {
                    case 1:
                        AddRecord(ref number_of_records);
                        recs_in_file = true; // Переменная = true, т.к. появляются записи во временном файле
                        break;
                    case 2:
                        if (!recs_in_file) // Проверка заполненности временного файла
                            Console.WriteLine("В базе нет ни единого сотрудника, попробуйте добавить");
                        else
                        {
                            DeleteRecord(ref number_of_records);
                            if (number_of_records == 0)
                            // Если кол-во записей после удаления в файле = 0, значит файл пустой, потому recs_in_file = false
                            {
                                Console.WriteLine("Удалена последняя запись");
                                recs_in_file = false;
                            }
                        }
                        Console.ReadLine(); // Задержка консоли
                        break;
                    case 3:
                        if (!recs_in_file) // Проверка на заполненность временного файла
                        {
                            Console.WriteLine("В базе нет ни единого сотрудника для изменения данных, попробуйте добавить");
                            Console.ReadLine(); // Задержка консоли
                        }
                        else
                            ChangeRecord(number_of_records);
                        break;
                    case 4:
                        if (!recs_in_file) // Проверка на заполненность временного файла
                        {
                            Console.WriteLine("В базе нет ни единой записи, попробуйте добавить");
                        }
                        else
                            OutputRecords();

                        Console.ReadLine(); // Задержка консоли
                        break;
                    case 5:
                        SaveRecords();
                        break;
                    case 6:
                        RaWRecordsfromFile(ref number_of_records, ref recs_in_file);
                        Console.ReadLine();
                        break;
                    case 7:
                        if (!recs_in_file) // Проверка на заполненность временного файла
                        {
                            Console.WriteLine("В базе нет ни единого сотрудника для изменения данных, попробуйте добавить");
                        }
                        else
                            SelectionRecord();

                        Console.ReadLine();
                        break;
                    case 8:
                        if (!recs_in_file) // Проверка на заполненность временного файла
                            Console.WriteLine("В базе нет ни единого сотрудника для повышения, попробуйте добавить");
                        else
                            PromoteTypeinRecord();

                        Console.ReadLine();
                        break;
                    case 9:
                        Console.WriteLine($"Кол-во записей - {number_of_records}"); // Вывод кол-ва записей в файле
                        Console.ReadLine(); // Задержка консоли
                        break;
                    default:
                        break;
                }
                Menu();
            }

            FileInfo fileinf = new FileInfo(tempfile);
            // После завершения работы удаляется временный файл хранения записей
            if (fileinf.Exists) fileinf.Delete();
        }

        static int DivHeadCounts(ref List<string> DHNames, string[] arrayreader)
        // Функция возвращает кол-во Начальников Подразделения во временном файле
        {

            string[] array;
            // Массив значения атрибутов ([0] - Фамилия, [1] - Имя и т.д.)

            int DHcount = 0;

            foreach (var o in arrayreader)
            {
                array = o.Split(";"); // Разделение строки на подстроки и ввод в массив
                if (array[5] == WorkerType[1]) // Проверка на наличие должности Начальник подразделения в записи
                {
                    DHcount++;
                    DHNames.Add(array[0]);
                    // Список[0], в котором записываются все фамилии Начальников Подразделения
                }
            }
            return DHcount;
        }

        static void PromoteRec(int vars, ref string wtype, ref string addinf, string[] arrayreader)
        // Функция изменения/присуждения должности и доп.информации сотруднику
        {
            switch (vars) // Выбор должности сотрудника
            {
                case 0:
                    Console.Write("Введите название организации: ");
                    wtype = WorkerType[vars];
                    addinf = Console.ReadLine();
                    break;
                case 1:
                    Console.Write("Введите название подразделения: ");
                    wtype = WorkerType[vars];
                    addinf = Console.ReadLine();
                    break;
                case 2:
                    Console.Write("Введите рабочую область: ");
                    wtype = WorkerType[vars];
                    addinf = Console.ReadLine();
                    break;
                case 3:
                    // 3 - Рабочий, для него сделана отдельная функция, выводящая фамилии всех Начальников Подразделения
                    // Т.к. доп.информация для должности Работник - Фамилия начальника подразделения, то нужно впринципе проверять наличие начальников
                    wtype = WorkerType[vars];
                    int i = 0;
                    List<string> ListforWorker = new List<string>(); // Список фамилий Начальников подразделения

                    if (DivHeadCounts(ref ListforWorker, arrayreader) > 0)
                    // Условие, проверяющее наличие хоть какого-либо числящегося в базе Начальника Подразделения
                    {
                        Console.WriteLine();
                        Console.WriteLine("На данный момент в базе числятся следующие Начальники подразделения:");
                        // Вывод фамилий
                        foreach (var o in ListforWorker)
                            Console.WriteLine($"{++i} - {o}");
                        DSvars:
                        Console.Write("Введите вариант из указанных выше: ");
                        int variant = Convert.ToInt32(Console.ReadLine());
                        if (variant > i || variant < 1) // Проверка правильности ввода варианта из указанных выше
                        {
                            Console.WriteLine("Попробуйте вновь ввести вариант из указанных выше");
                            goto DSvars;
                        }
                        else
                            addinf = ListforWorker[i - 1];
                    }
                    else
                        Console.WriteLine("В базе на данный момент нет ни одного Начальника подразделения, просьба добавить");
                    break;
                default:
                    Console.WriteLine("Такой должности не существует\n");
                    break;
            }
        }

        static void AddRecord(ref int addedRecords) // Функция добавления новой записи во временный файл
        {
            string first_name, second_name, patronymic, date_of_birth, gender, worker_type_str = "", addit_info_of_worker = "";
            int worker_type, add_another_record = 1;

            FileStream fstream = new FileStream(tempfile, FileMode.OpenOrCreate); // Создание потока fstream
            StreamWriter rec = new StreamWriter(fstream); // Открываем для записи временный файл

            FileInfo finf = new FileInfo(tempfile);
            // Открываем файл для копирования его в другой файл. Если такого файла нет, создаём его.
            finf.CopyTo(tempfileforcopying, true);

            string[] array_to_funct_promote = File.ReadAllLines(tempfileforcopying); // Записываем в массив все строки из скопированного файла
            fstream.Seek(0, SeekOrigin.End); // Переводим указатель на начало файла

        RecordLoop: // Метка для повторного ввода записи

            Console.Clear();

            // Изменение временных переменных для добавления записи в файл
            Console.Write("Введите фамилию: ");
            first_name = Console.ReadLine();

            Console.Write("Введите имя: ");
            second_name = Console.ReadLine();

            Console.Write("Введите отчество: ");
            patronymic = Console.ReadLine();

            Console.Write("Введите дату рождения: ");
            date_of_birth = Console.ReadLine();

            Console.Write("Введите пол: ");
            gender = Console.ReadLine();

        WorkTypeAdd:
            Console.Write("Должности\n" +
                "1.Директор\n" +
                "2.Начальник подразделения\n" +
                "3.Контроллер\n" +
                "4.Рабочий\n" +
                "Выберите соответственную должность (Введите число):");
            worker_type = Convert.ToInt32(Console.ReadLine());


            PromoteRec(worker_type - 1, ref worker_type_str, ref addit_info_of_worker, array_to_funct_promote);
            if (worker_type_str == "" && addit_info_of_worker == "") goto WorkTypeAdd;
            // Переходим обратно к вводу вариантов если введены неправильные/ошибочные значения
            Console.Clear();

            rec.WriteLine(first_name + ";" + second_name + ";" + patronymic + ";" + date_of_birth + ";" + gender + ";" + WorkerType[worker_type - 1] + ";" + addit_info_of_worker);
            // Полученные данные соединяем в одну строку и в временный файл

            addedRecords++;

            Console.WriteLine("Хотите добавить ещё запись?(1 - Да, 2 - Нет):"); // Повторный ввод записи
            add_another_record = Convert.ToInt32(Console.ReadLine());
            if (add_another_record == 1)
                goto RecordLoop;
            else
            {
                rec.Close(); // Закрываем файл "для записи"
                FileInfo finfforclose = new FileInfo(tempfileforcopying); // Удаляем временный для копирования файл, дальше он не нужен
                finfforclose.Delete();
            }

        }

        static void DeleteRecord(ref int existingRecords) // Функция удаления записи
        {
            FileInfo finf = new FileInfo(tempfile);

            if (finf.Exists) // Проверка существования временного файла
            {
                finf.CopyTo(tempfileforcopying, true); // Копирование временного файла в временный для копирования файл

            RecNumDelete:

                Console.Write("Выберите нужную запись для удаления (от 1 до {0})\n" + // Выбор записи для удаления
                    "(Если хотите отменить удаление введите 0): ", existingRecords);
                int id = Convert.ToInt32(Console.ReadLine());

                if (id == 0)
                    goto EndOfDelete;

                else if (id < 1 || id > existingRecords) // Проверка правильности ввода номера записи
                {
                    Console.WriteLine("Ошибка ввода номера записи, попробуйте вновь");
                    goto RecNumDelete;
                }
                else
                {
                    int i = 0;
                    string string_for_copy;

                    StreamReader sread = new StreamReader(tempfileforcopying);
                    StreamWriter swrite = new StreamWriter(tempfile);
                    // Открываю два файла, один - временный для записи, другой - временный для копирования для чтения
                    // Удаление реализовано через перебор всех строк из "для копирования" файла для нахождения нужной
                    // Строка считывает - Проверяется - Если та что искали: Изменение в ней данных и ввод в временный файли; Если та что не искали: Ввод в временный файл;

                    while (!sread.EndOfStream)
                    {
                        if (i + 1 == id) // Проверка нахождения нужной записи
                        {
                            string_for_copy = sread.ReadLine();
                            i++;
                        }
                        else
                        {
                            string_for_copy = sread.ReadLine();
                            swrite.WriteLine(string_for_copy);
                            i++;
                        }
                    }
                    Console.WriteLine("Запись под номером {0} удалена", id);
                    existingRecords--;
                    sread.Close(); // Закрытия файла для чтения
                    swrite.Close(); // Закрытие файла для записи
                    finf = new FileInfo(tempfileforcopying); // Удаления файла для копирования, т.к. он больше нам не нужне
                    finf.Delete();
                    Console.ReadLine(); // Задержка консоли
                    Console.Clear(); // Очищение консоли
                }
            }
            else
                Console.WriteLine("Файла не существует, попробуйте добавить запись");
            EndOfDelete:;

        }

        static void ChangeRecord(int allRecords) // Функция изменения данных записи
        {
            FileInfo finf = new FileInfo(tempfile);

            if (finf.Exists) // Проверка существования временного файла
            {
                finf.CopyTo(tempfileforcopying, true); // Копирование временного файла в временный для копирования файл

            RecNumChange:
                Console.Write("Выберите запись для изменения (от 1 до {0})\n" + // Выбор записи для изменения
                    "(Если хотите отменить удаление введите 0): ", allRecords);
                int id = Convert.ToInt32(Console.ReadLine());

                if (id == 0) // Если ввели 0
                    goto EndChange;

                if (id < 1 || id > allRecords) // Проверка правильности ввода варианта
                {
                    Console.WriteLine("Ошибка ввода номера записи, попробуйте вновь");
                    goto RecNumChange;
                }
                else
                {
                    int i = 0;
                    string string_for_copy;

                    StreamReader sread = new StreamReader(tempfileforcopying);
                    StreamWriter swrite = new StreamWriter(tempfile);
                    // Реализация та же, что и в функции ChangeRecord. Логика та же

                    string[] array_to_funct = File.ReadAllLines(tempfileforcopying);
                    while (!sread.EndOfStream)
                    {
                        if (i + 1 == id) // Проверка нахождения нужной записи
                        {
                            string first_name, second_name, patronymic, date_of_birth, gender, worker_type_str = "", addit_info_of_worker = "";
                            int worker_type;
                            string_for_copy = sread.ReadLine(); // Чтение строки из файла
                            Console.Clear();

                            // Изменения временных значений для последующего ввода в запись
                            Console.Write("Введите фамилию: ");
                            first_name = Console.ReadLine();

                            Console.Write("Введите имя: ");
                            second_name = Console.ReadLine();

                            Console.Write("Введите отчество: ");
                            patronymic = Console.ReadLine();

                            Console.Write("Введите дату рождения: ");
                            date_of_birth = Console.ReadLine();

                            Console.Write("Введите пол: ");
                            gender = Console.ReadLine();

                        WorkTypeChange:
                            Console.Write("Должности\n" +
                                "1.Директор\n" +
                                "2.Начальник подразделения\n" +
                                "3.Контроллер\n" +
                                "4.Рабочий\n" +
                                "Выберите соответственную должность (Введите число):");
                            worker_type = Convert.ToInt32(Console.ReadLine());

                            PromoteRec(worker_type - 1, ref worker_type_str, ref addit_info_of_worker, array_to_funct);
                            if (worker_type_str == "" && addit_info_of_worker == "") goto WorkTypeChange;
                            // Если введены ошибочные значения в функцию, то возвращаемся в выбор должности

                            swrite.WriteLine(first_name + ";" + second_name + ";" + patronymic + ";" + date_of_birth + ";" + gender + ";" + WorkerType[worker_type - 1] + ";" + addit_info_of_worker);
                            // Ввод записи в файл
                            i++;
                        }
                        else
                        {
                            string_for_copy = sread.ReadLine(); // Чтение строки из файла
                            swrite.WriteLine(string_for_copy); // Ввод записи в файл
                            i++;
                        }
                    }
                    Console.WriteLine("Запись под номером {0} обновлена", id);
                    sread.Close();
                    swrite.Close();
                    // Закрытие файлов для записи и чтения

                    Console.ReadLine(); // Задержка консоли
                    Console.Clear(); // Очищение консоли

                    finf = new FileInfo(tempfileforcopying); // Удаление файла для копирования
                    finf.Delete();


                }

            }
            else
                Console.WriteLine("Файла не существует, попробуйте добавить запись");
            EndChange:;
        }

        static void OutputRecords() // Функция вывода записей временного файла в консоль
        {
            StreamReader sreader = new StreamReader(tempfile);

            string[] array;
            string record;

            Console.Write(" -------------------------------------------------------------------------------------------------\n" + // Вывод атрибутов записи
                "| Запись |     Фамилия |         Имя |    Отчество |  Дата Рожд. |         Пол |   Должность |      Д.П.Д. |\n" +
                " -------------------------------------------------------------------------------------------------\n");
            int numrec = 1;
            while (!sreader.EndOfStream)
            {
                record = sreader.ReadLine();
                Console.Write("|{0,7} |", numrec++); // Вывод номера записи

                array = record.Split(';'); // Разделение записи на подстроки и запись в массив
                for (int i = 0; i < 7; i++)
                {
                    Console.Write("{0,12} |", array[i]); // Вывод значений атрибутов записи
                }
                Console.WriteLine();
            }
            Console.Write(" -------------------------------------------------------------------------------------------------");
            sreader.Close();
            Console.ReadLine(); // Задержка консоли
        }

        static void SaveRecords() // Функция сохранения всех записей в файл. Из временного в постоянный
        {
            Console.WriteLine("Введите имя файла куда будете сохранять значения: ");
            string fname = Console.ReadLine();
            // Строка ввода имени постоянного файла

            FileInfo finf = new FileInfo(fname);
        Create:
            if (finf.Exists) // Проверка существования введённого постоянного файла
            {

                string record;
                StreamReader sreader = new StreamReader(tempfile);
                StreamWriter swriter = new StreamWriter(fname);
                // Открытие файлов для чтения и записей

                while (!sreader.EndOfStream) // Ввод всех записей из временного файла в постоянный
                {
                    record = sreader.ReadLine();
                    swriter.WriteLine(record);
                }
                sreader.Close();
                swriter.Close();
                // Закрытие файлов для чтения и записи

                Console.WriteLine("Записи из временного файл {0} сохранены в файл {1}", tempfile, fname);
                Console.ReadLine(); // Задержка консоли
            }
            else
            {
                Console.Write("Такого файла не существует, хотите создать файл с таким названием или отменить действие?\n" +
                    "(1 - Создать, 2 или любая другая цифра - Отменить): ");
                // Если файла с таким именем не существует, то предлагается выбор - Создать или Отменить действие
                // После создания начинается запись из временного файла в новосозданный
                int arg = Convert.ToInt32(Console.ReadLine());
                if (arg == 1)
                {
                    finf.Create();
                    goto Create;
                }
                else
                    Console.WriteLine("Действие отменено");
                Console.ReadLine();
            }
        }

        static void RaWRecordsfromFile(ref int num_recs_in_file, ref bool init) // Функция ввода записей из постоянного файла во временный
        {
            num_recs_in_file = 0;
            bool errfile = false;
            // Переменная для проверки постоянного файла на правильность введённых строк. Изначально переменная false
            // В файлы вводится определённый шаблон строк - Значения атрибутов, разделённый знаком ";";

            Console.WriteLine("Введите название файла: ");
            string fname = Console.ReadLine();

            FileInfo finf = new FileInfo(fname);

        RaWfromFile:
            if (finf.Exists) // Аналогичная проверка в функции SaveRecords
            {

                string record;
                StreamReader sreader = new StreamReader(fname);
                StreamWriter swriter = new StreamWriter(tempfile);
                while (!sreader.EndOfStream)
                {
                    record = sreader.ReadLine();
                    if (!errfile) // Проверка правильности строк в постоянном файле
                    {
                        string[] array = record.Split(";");
                        if (array.Length != 7) // Если значений в строке 7 (Кол-во атрибутов), то строки в правильном шаблоне, если нет - выполняется условие
                        {
                            errfile = true;
                            Console.WriteLine("Вы открыли файл с неправильным шаблоном.\n " +
                                "Прошу вас открыть файл с правильным шаблоном записей либо создать новый файл с новым именем\n" +
                                "Для этого нужно вновь загрузить данные из файла, но указать новое имя файла");
                            Console.WriteLine();
                            goto EndOfRaW;
                        }
                    }
                    swriter.WriteLine(record); // Запись строк из постоянного во временный
                    num_recs_in_file++;
                }
                sreader.Close();
                swriter.Close();
                // Закрытие файлов для записи и чтения
                Console.WriteLine("Считано {0} записей из файла {1} и записано во временный файл {2}", num_recs_in_file, fname, tempfile);

                if (num_recs_in_file == 0)
                    // Переменная init служит для проверки "Введены ли записи во временный файл".
                    // Если переменная = 0, значит что из файла было прочитано 0 записей, а значит и во временный файл перенесено 0 записей, временный пуст
                    init = false;
                else
                    init = true;
            }
            else
            {
                Console.Write("Такого файла не существует, хотите создать файл с таким названием или отменить действие?\n" +
                    "(1 - Создать, 2 или любая другая цифра - Отменить): ");
                // Аналогичная функция создания постоянного файла в функции SaveRecords
                int arg = Convert.ToInt32(Console.ReadLine());
                if (arg == 1)
                {
                    finf.Create();
                    errfile = true;
                    goto RaWfromFile;
                }
                else
                    Console.WriteLine("Действие отменено");
            }
        EndOfRaW:;
            Console.ReadLine(); // Задержка
        }

        static void SelectionRecord()
        {
        VarsChange:
            Console.WriteLine("По какому атрибуту вы хотите сделать выборку?\n" + // Выбор выборки по варианту
                "1.По занимаемой должности\n" +
                "2.По подразделению\n" +
                "Выберите вариант: ");
            int vars = Convert.ToInt32(Console.ReadLine());

            string str, record;
            string[] array;
            int numrec;
            StreamReader sreader = new StreamReader(tempfile);

            switch (vars) // Выбор варианта выборки
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Введите должность:"); // Ввод должности
                    str = Console.ReadLine();
                    if (str == "Начальник подразделения") str = "НП";

                    Console.Write(" -------------------------------------------------------------------------------------------------\n" +
                "| Запись |     Фамилия |         Имя |    Отчество |  Дата Рожд. |         Пол |   Должность |      Д.П.Д. |\n" +
                " -------------------------------------------------------------------------------------------------\n");
                    numrec = 1;
                    while (!sreader.EndOfStream)
                    {
                        // Аналогичный перебор как и в функция Записи и Сохранения. 
                        array = sreader.ReadLine().Split(";");
                        if (array[5] == str) // Проверка нужной должности в записи
                        {
                            Console.Write("|{0,7} |", numrec++); // Вывод номера записи
                            for (int i = 0; i < 7; i++)
                            {
                                Console.Write("{0,12} |", array[i]); // Вывод записи
                            }
                            Console.WriteLine();
                        }
                    }
                    Console.Write(" -------------------------------------------------------------------------------------------------");
                    break;
                case 2:
                    Console.Clear();
                    Console.Write("Введите подразделение:"); // Ввод подразделения
                    str = Console.ReadLine();

                    string fnofsv = ""; // Фамилия Начальника подразделения
                    List<string> forarr = new List<string>();
                    // Массив списков, служащий для нахождения фамилий Начальников подразделения и Рабочих
                    // Т.к. у Начальников Подразделения в доп.инф-ии введено название подразделения, то нахождение не составит труда
                    // После Начальников Подразделения я вывожу рабочих, т.к. Рабочий подчиняется Начальнику подразделения, а Начальник служит на подразделении


                    Console.Write(" -------------------------------------------------------------------------------------------------\n" +
                "| Запись |     Фамилия |         Имя |    Отчество |  Дата Рожд. |         Пол |   Должность |      Д.П.Д. |\n" +
                " -------------------------------------------------------------------------------------------------\n");
                    numrec = 1; // Переменная номера записи

                    while (!sreader.EndOfStream)
                    {
                        record = sreader.ReadLine();
                        array = record.Split(';');
                        // Считывается строка с файла, разделяется на значения и значения сохраняются в массиве

                        if (array[5] == WorkerType[3])
                            forarr.Add(record); // В forarr[1] сохраняется запись сотрудника Рабочего

                        else if (array[6] == str)
                        {
                            fnofsv = array[0]; // В переменную сохраняется фамилия НП
                            Console.Write("|{0,7} |", numrec++);
                            for (int i = 0; i < 7; i++)
                            {
                                Console.Write("{0,12} |", array[i]); // Вывод записи начальника подразделения
                            }
                            Console.WriteLine();
                        }

                    }
                    foreach (var o in forarr) // Вывод Рабочих, у кого в доп.информации указан нужный Начальник подразделения
                    {
                        array = o.Split(";");
                        if (array[6] == fnofsv)
                        {
                            Console.Write("|{0,7} |", numrec++);
                            for (int j = 0; j < 7; j++)
                                Console.Write("{0,12} |", array[j]);
                            Console.WriteLine();
                        }
                    }
                    Console.Write(" -------------------------------------------------------------------------------------------------");
                    break;
                default:
                    Console.WriteLine("Вы выбрали неправильный вариант");
                    goto VarsChange;
                    break;
            }
            sreader.Close(); // Закрытие файла для чтения
            Console.ReadLine(); // Задержка консоли

        }

        static void PromoteTypeinRecord() // Функция повышения сотрудника
        {
            Console.WriteLine("Введите Фамилию сотрудника, которого желаете повысить: "); // Ввод фамилии сотрудника
            string firstname = Console.ReadLine();

            string record;
            string[] sub;
            int wtype = 0;

            FileInfo finf = new FileInfo(tempfile);
            if (finf.Exists)
            {
                finf.CopyTo(tempfileforcopying, true);

                StreamReader sreader = new StreamReader(tempfileforcopying);
                StreamWriter swrite = new StreamWriter(tempfile);
                string[] array_to_funct = File.ReadAllLines(tempfileforcopying);
                bool promote = false;

                while (!sreader.EndOfStream)
                // Аналогичная перезапись из одного файла в другой как в функциях Сохранения и Ввода из файла
                {
                    record = sreader.ReadLine();
                    sub = record.Split(";");

                    if (sub[0] == firstname)
                    // Перебор на нахождения сотрудника с нужной фамилией
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            if (sub[5] == WorkerType[i]) // Проверка должности сотрудника
                            {
                                if (i == 0) // Если должность Директор, то повышение невозможно
                                    Console.WriteLine("Должность сотрудника с фамилией {0} - Директор, нет возможности повысить", sub[0]);
                                else
                                {
                                    Console.WriteLine();
                                    PromoteRec(i - 1, ref sub[5], ref sub[6], array_to_funct); // Присваивается новая должность
                                    record = String.Join(';', sub);
                                    wtype = i;
                                    promote = true;
                                    break;
                                }
                            }
                        }
                        if (promote) // Если сотрудник повышен - выводится сообщение, если нет - значит файл перебрал все значения и не нашёл фамилию
                            Console.WriteLine("Сотрудник {0} повышен с {1} до {2}", firstname, WorkerType[wtype], WorkerType[wtype - 1]);
                        else
                            Console.WriteLine("Сотрудник с такой фамилией не повышен");
                    }
                    swrite.WriteLine(record);
                }
                sreader.Close();
                swrite.Close();
                finf = new FileInfo(tempfileforcopying);
                finf.Delete();
                // Закрытие файлов для записи и чтения и удаления временного для копирования файла
            }
            else
                Console.WriteLine("Файла не существует, попробуйте добавить запись");
            Console.ReadLine();
        }
    }
}

