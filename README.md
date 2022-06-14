# Программа учёта персонала некого предприятия
# Дубенский А.Ю.

Инструкция работы программы
После запуска программы в консоль выводиться меню работы программы
Меню имеет следующие пункты:
1. Добавить запись
2. Удалить запись
3. Изменить запись
4. Вывод данных
5. Сохранить данные
6. Ввод данных из файла
7. Выборка
8. Повышение должности
9. Вывод кол-ва записей

Некоторый пункты не будут работать после первого запуска программы, т.к. наша база изначально не заполнена записями.
Работа с "Базой персонала" реализована через создаваемый временный файл Temporary.txt, после запуска естественно файл не создан.
Сначала нужно заполнить наш временный файл хоть какими-то записями. Для этого используйте пункты 1 или 6.
В случае 1 пункта всё понятно: После выбора данного варианта вас просят заполнить основные атрибуты: ФИО, Дата рождения, Пол, Должность и Доп.информация должности.
В случае 6 пункта вас просят ввести имя постоянного файла. (Такой файл можно самому создать, либо после Сохранения данных ввести новое имя (Прим. NewBase.txt)).
Также до добавления или после вам доступен пункт 9 - Вывод кол-ва записей. При выборе данного пункта в консоль выводится кол-во всех существующих записей во временном файле
После заполнения базы записями вам доступны следующие пункты: 2, 3, 4, 7, 8.
Если выбран 2 пункт, вас просят ввести номер записи. После выбора записи соответствующая запись удаляется из временного файла.
Номер записи можно узнать, если вы используете пункт 4 - Вывод. После вывод в консоль выводится удобная наглядная таблица существующей бд.
В случае выбора 3 пункта - просят номер записи. После выбора записи вас просят перезаполнить значения записи - От фамилии до должности. Введёные данные перезаписываются.
В случае выбора 7 пункта - просят ввести вариант выборки: по должности или по подразделению. 
При выборе пункта "по должности" просят ввести название должности, и после ввода должности выводится выборка по должности соотвественно
При выборе пункта "по подразделению" просят ввести название соответственного подразделения и выводят выборку.
В случае выбора 8 пункта - просят ввести фамилию сотрудника. После данного сотрудника повышают в должности. Выше должности Директора повысить невозможно.
Для сохранения данных используйте 5 - Сохранение данных. При выборе этого пункта просят ввести название файла, куда будут сохранены записи.
Во всех файлах строки/записи имеют следующий шаблон - Фамилия;Имя;Отчество;Дата рождения;Пол;Должность;Доп.информация должности

ДЛЯ ЗАВЕРШЕНИЯ ПРОГРАММЫ В ОБЩЕМ МЕНЮ ВВЕДИТЕ 0

P.S. В папке WorkWithDataBase добавлен файл DataBase.txt. Просьба перекинуть данный файл в файл bin\Debug\"Версия .NET"\
Нужно добавить в папку, где создаются файлы Temporary.txt и Copying.txt. Если добавите, сможет с помощью 6 пункта ввести уже заполненную БД для наглядной работы
