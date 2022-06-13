using System;
using System.IO;
using System.Collections.Generic;

namespace WorkwithDataBase
{
    class Program
    {
        static int menu_variants;

        public const string fpath = "DataBase.txt";
        public const string tempfile = "Temporary.txt";
        public const string tempfileforcopying = "Copying.txt";
        public static readonly string[] WorkerType = { "Директор", "НП", "Контроллер", "Рабочий" };
        static void Menu()
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

            bool recs_in_file = false;
            int number_of_records = 0;
            while (menu_variants != 0)
            {
                switch (menu_variants)
                {
                    case 1:
                        AddRecord(ref number_of_records);
                        recs_in_file = true;
                        break;
                    case 2:
                        if (!recs_in_file)
                            Console.WriteLine("В базе нет ни единого сотрудника, попробуйте добавить");
                        else
                        {
                            DeleteRecord(ref number_of_records);
                            if (number_of_records == 0)
                            {
                                Console.WriteLine("Удалена последняя запись");
                                recs_in_file = false;
                            }
                        }
                        break;
                    case 3:
                        if (!recs_in_file)
                            Console.WriteLine("В базе нет ни единого сотрудника для изменения данных, попробуйте добавить");
                        else
                            ChangeRecord(number_of_records);
                        break;
                    case 4:
                        if (!recs_in_file)
                        {
                            Console.WriteLine("В базе нет ни единой записи, попробуйте добавить");
                            Console.ReadLine();
                        }
                        else
                            OutputRecords();
                        break;
                    case 5:
                        SaveRecords();
                        break;
                    case 6:
                        RaWRecordsfromFile(ref number_of_records, ref recs_in_file);
                        break;
                    case 7:
                        if (!recs_in_file)
                        {
                            Console.WriteLine("В базе нет ни единого сотрудника для изменения данных, попробуйте добавить");
                            Console.ReadLine();
                        }
                        else
                            SelectionRecord();
                        break;
                    case 8:
                        if (!recs_in_file)
                            Console.WriteLine("В базе нет ни единого сотрудника для повышения, попробуйте добавить");
                        else
                            PromoteTypeinRecord();
                        break;
                    case 9:
                        Console.WriteLine($"Кол-во записей - {number_of_records}");
                        Console.ReadLine();
                        break;
                    default:
                        break;
                }
                Menu();
            }

            FileInfo fileinf = new FileInfo(tempfile);
            if (fileinf.Exists) fileinf.Delete();
        }

        static int DivHeadCounts(ref List<string> DHNames, string[] arrayreader)
        {

            string[] array;
            int DHcount = 0;
            foreach (var o in arrayreader)
            {
                array = o.Split(";");
                if (array[5] == WorkerType[1])
                {
                    DHcount++;
                    DHNames.Add(array[0]);
                }
            }
            return DHcount;
        }

        static void PromoteRec(int vars, ref string wtype, ref string addinf, string[] arrayreader)
        {
            switch (vars)
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

                    wtype = WorkerType[vars];
                    int i = 0;
                    List<string> ListforWorker = new List<string>();

                    if (DivHeadCounts(ref ListforWorker, arrayreader) > 0)
                    {
                        Console.WriteLine();
                        Console.WriteLine("На данный момент в базе числятся следующие Начальники подразделения:");
                        foreach (var o in ListforWorker)
                            Console.WriteLine($"{++i} - {o}");
                        Console.Write("Введите вариант из указанных выше: ");
                        int variant = Convert.ToInt32(Console.ReadLine());
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

        static void AddRecord(ref int addedRecords)
        {
            string first_name, second_name, patronymic, date_of_birth, gender, worker_type_str = "", addit_info_of_worker = "";
            int worker_type, add_another_record = 1;

            FileStream fstream = new FileStream(tempfile, FileMode.OpenOrCreate);
            StreamWriter rec = new StreamWriter(fstream);
            FileInfo finf = new FileInfo(tempfile);
            if (!finf.Exists)
            {
                finf.CopyTo(tempfileforcopying);
            }
            string[] array_to_funct = File.ReadAllLines(tempfileforcopying);
            fstream.Seek(0, SeekOrigin.End);

        RecordLoop:

            Console.Clear();

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


            PromoteRec(worker_type - 1, ref worker_type_str, ref addit_info_of_worker, array_to_funct);
            if (worker_type_str == "" && addit_info_of_worker == "") goto WorkTypeAdd;
            Console.Clear();

            rec.WriteLine(first_name + ";" + second_name + ";" + patronymic + ";" + date_of_birth + ";" + gender + ";" + WorkerType[worker_type - 1] + ";" + addit_info_of_worker);

            addedRecords++;

            Console.WriteLine("Хотите добавить ещё запись?(1 - Да, 2 - Нет):");
            add_another_record = Convert.ToInt32(Console.ReadLine());
            if (add_another_record == 1)
                goto RecordLoop;
            else
            {
                rec.Close();
                FileInfo finfforclose = new FileInfo(tempfileforcopying);
                finfforclose.Delete();
            }

        }

        static void DeleteRecord(ref int existingRecords)
        {
            FileInfo finf = new FileInfo(tempfile);

            if (finf.Exists)
            {
                finf.CopyTo(tempfileforcopying, true);

            RecNumDelete:

                Console.Write("Выберите нужную запись для удаления (от 1 до {0})\n" +
                    "(Если хотите отменить удаление введите 0): ", existingRecords);
                int id = Convert.ToInt32(Console.ReadLine());

                if (id == 0)
                    goto EndOfDelete;

                else if (id < 1 || id > existingRecords)
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
                    while (!sread.EndOfStream)
                    {
                        if (i + 1 == id)
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
                    sread.Close();
                    swrite.Close();
                    finf = new FileInfo(tempfileforcopying);
                    finf.Delete();
                    Console.ReadLine();
                    Console.Clear();
                }
            }
            else
                Console.WriteLine("Файла не существует, попробуйте добавить запись");
            EndOfDelete:;

        }

        static void ChangeRecord(int allRecords)
        {
            FileInfo finf = new FileInfo(tempfile);
            if (finf.Exists)
            {
                finf.CopyTo(tempfileforcopying, true);

            RecNumChange:
                Console.Write("Выберите запись для удаления (от 1 до {0})\n" +
                    "(Если хотите отменить удаление введите 0): ", allRecords);
                int id = Convert.ToInt32(Console.ReadLine());
                if (id == 0)
                    goto EndChange;
                if (id < 1 || id > allRecords)
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
                    string[] array_to_funct = File.ReadAllLines(tempfileforcopying);
                    while (!sread.EndOfStream)
                    {
                        if (i + 1 == id)
                        {
                            string first_name, second_name, patronymic, date_of_birth, gender, worker_type_str = "", addit_info_of_worker = "";
                            int worker_type;
                            string_for_copy = sread.ReadLine();
                            Console.Clear();

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

                            //int filepos = sread.BaseStream.Position();
                            PromoteRec(worker_type - 1, ref worker_type_str, ref addit_info_of_worker, array_to_funct);
                            if (worker_type_str == "" && addit_info_of_worker == "") goto WorkTypeChange;

                            //Console.Clear();

                            swrite.WriteLine(first_name + ";" + second_name + ";" + patronymic + ";" + date_of_birth + ";" + gender + ";" + WorkerType[worker_type - 1] + ";" + addit_info_of_worker);
                            i++;
                        }
                        else
                        {
                            string_for_copy = sread.ReadLine();
                            swrite.WriteLine(string_for_copy);
                            i++;
                        }
                    }
                    Console.WriteLine("Запись под номером {0} обновлена", id);
                    sread.Close();
                    swrite.Close();
                    Console.ReadLine();
                    Console.Clear();
                    finf = new FileInfo(tempfileforcopying);
                    finf.Delete();


                }

            }
            else
                Console.WriteLine("Файла не существует, попробуйте добавить запись");
            EndChange:;
        }

        static void OutputRecords()
        {
            StreamReader sreader = new StreamReader(tempfile);
            string[] array;
            string record;
            Console.Write(" -------------------------------------------------------------------------------------------------\n" +
                "|     Фамилия |         Имя |    Отчество |  Дата Рожд. |         Пол |   Должность |      Д.П.Д. |\n" +
                " -------------------------------------------------------------------------------------------------\n");
            while (!sreader.EndOfStream)
            {
                record = sreader.ReadLine();
                Console.Write("|");
                array = record.Split(';');
                for (int i = 0; i < 7; i++)
                {
                    Console.Write("{0,12} |", array[i]);
                }
                Console.WriteLine();
            }
            Console.Write(" -------------------------------------------------------------------------------------------------");
            Console.ReadLine();
            sreader.Close();
        }

        static void SaveRecords()
        {
            Console.WriteLine("Введите имя файла куда будете сохранять значения: ");
            string fname = Console.ReadLine();

            FileInfo finf = new FileInfo(fname);
        Create:
            if (finf.Exists)
            {

                string record;
                StreamReader sreader = new StreamReader(tempfile);
                StreamWriter swriter = new StreamWriter(fname);
                while (!sreader.EndOfStream)
                {
                    record = sreader.ReadLine();
                    swriter.WriteLine(record);
                }
                sreader.Close();
                swriter.Close();
                Console.WriteLine("Записи из временного файл {0} сохранены в файл {1}", tempfile, fname);
                Console.ReadLine();
            }
            else
            {
                Console.Write("Такого файла не существует, хотите создать файл с таким названием или отменить действие?\n" +
                    "(1 - Создать, 2 или любая другая цифра - Отменить): ");
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

        static void RaWRecordsfromFile(ref int num_recs_in_file, ref bool init)
        {
            num_recs_in_file = 0;
            bool errfile = false;
            Console.WriteLine("Введите название файла: ");
            string fname = Console.ReadLine();
            FileInfo finf = new FileInfo(fname);
            if (finf.Exists)
            {
                string record;
                StreamReader sreader = new StreamReader(fname);
                StreamWriter swriter = new StreamWriter(tempfile);
                while (!sreader.EndOfStream)
                {
                    record = sreader.ReadLine();
                    if (!errfile)
                    {
                        string[] array = record.Split(";");
                        if (array.Length != 7)
                        {
                            errfile = true;
                            Console.WriteLine("Вы открыли файл с неправильным шаблоном.\n " +
                                "Прошу вас открыть файл с правильным шаблоном записей либо создать новый файл с новым именем\n" +
                                "Для этого нужно вновь загрузить данные из файла, но указать новое имя файла");
                            Console.WriteLine();
                            goto EndOfRaW;
                        }
                    }
                    swriter.WriteLine(record);
                    num_recs_in_file++;
                }
                sreader.Close();
                swriter.Close();
                Console.WriteLine("Считано {0} записей из файла {1} и записано во временный файл {2}", num_recs_in_file, fname, tempfile);
                init = true;
            }
            else
            {
                Console.Write("Такого файла не существует, хотите создать файл с таким названием или отменить действие?\n" +
                    "(1 - Создать, 2 или любая другая цифра - Отменить): ");
                int arg = Convert.ToInt32(Console.ReadLine());
                if (arg == 1)
                    finf.Create();
                else
                    Console.WriteLine("Действие отменено");
            }
            Console.ReadLine();
        EndOfRaW:;
        }

        static void SelectionRecord()
        {
        VarsChange:
            Console.WriteLine("По какому атрибуту вы хотите сделать выборку?\n" +
                "1.По занимаемой должности\n" +
                "2.По подразделению\n" +
                "Выберите вариант: ");
            int vars = Convert.ToInt32(Console.ReadLine());
            string record, str;
            string[] array;

            StreamReader sreader = new StreamReader(tempfile);

            switch (vars)
            {
                case 1:
                    Console.Clear();
                    Console.WriteLine("Введите должность:");
                    str = Console.ReadLine();
                    if (str == "Начальник подразделения") str = "НП";
                    Console.Write(" -------------------------------------------------------------------------------------------------\n" +
                "|     Фамилия |         Имя |    Отчество |  Дата Рожд. |         Пол |   Должность |      Д.П.Д. |\n" +
                " -------------------------------------------------------------------------------------------------\n");
                    while (!sreader.EndOfStream)
                    {
                        record = sreader.ReadLine();
                        array = record.Split(';');
                        if (array[5] == str)
                        {
                            Console.Write("|");
                            for (int i = 0; i < 7; i++)
                            {
                                Console.Write("{0,12} |", array[i]);
                            }
                            Console.WriteLine();
                        }
                    }
                    Console.Write(" -------------------------------------------------------------------------------------------------");
                    break;
                case 2:
                    Console.Clear();
                    Console.Write("Введите подразделение:");
                    str = Console.ReadLine();
                    List<string>[] forarr = new List<string>[2];
                    for (int i = 0; i < 2; i++)
                    {
                        forarr[i] = new List<string>() { };
                    }
                    Console.Write(" -------------------------------------------------------------------------------------------------\n" +
                "|     Фамилия |         Имя |    Отчество |  Дата Рожд. |         Пол |   Должность |      Д.П.Д. |\n" +
                " -------------------------------------------------------------------------------------------------\n");
                    while (!sreader.EndOfStream)
                    {
                        record = sreader.ReadLine();
                        array = record.Split(';');
                        if (array[5] == WorkerType[3])
                            forarr[1].Add(record);
                        else if (array[6] == str)
                        {
                            //array = record.Split(';');
                            forarr[0].Add(array[0]);
                            Console.Write("|");
                            for (int i = 0; i < 7; i++)
                            {
                                Console.Write("{0,12} |", array[i]);
                            }
                            Console.WriteLine();
                        }

                    }
                    foreach (var o in forarr[0])
                    {
                        foreach (var c in forarr[1])
                        {
                            array = c.Split(";");
                            if (array[6] == o)
                            {
                                Console.Write("|");
                                for (int j = 0; j < 7; j++)
                                    Console.Write("{0,12} |", array[j]);
                                Console.WriteLine();
                            }
                        }
                    }
                    Console.Write(" -------------------------------------------------------------------------------------------------");
                    break;
                default:
                    Console.WriteLine("Вы выбрали неправильный вариант");
                    goto VarsChange;
                    break;
            }
            sreader.Close();
            Console.ReadLine();

        }

        static void PromoteTypeinRecord()
        {
            Console.WriteLine("Введите Фамилию сотрудника, которого желаете повысить: ");
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
                {
                    record = sreader.ReadLine();
                    sub = record.Split(";");
                    if (sub[0] == firstname)
                    {
                        for (int i = 0; i < 4; i++)
                        {
                            //Console.WriteLine($"Тип {WorkerType[i]}");
                            if (sub[5] == WorkerType[i])
                            {
                                //Console.WriteLine($"Работник типа {WorkerType[i]}");
                                if (i == 0)
                                    Console.WriteLine("Должность сотрудника с фамилией {0} - Директор, нет возможности повысить", sub[0]);
                                else
                                {
                                    Console.WriteLine();
                                    PromoteRec(i - 1, ref sub[5], ref sub[6], array_to_funct);
                                    //Console.WriteLine($"Работник типа {sub[5]} - {sub[6]}");
                                    record = String.Join(';', sub);
                                    wtype = i;
                                    promote = true;
                                    break;
                                }
                            }
                        }
                        if (promote)
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
            }
            else
                Console.WriteLine("Файла не существует, попробуйте добавить запись");
            Console.ReadLine();
        }
    }
}
