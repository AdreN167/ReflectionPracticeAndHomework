using System;
using System.Collections.Generic;
using System.Reflection;

namespace SQLTablesGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Введите путь к файлу: ");
            var path = Console.ReadLine();

            Console.WriteLine();

            if (path != null)
            {
                Assembly resorceBuild;

                try
                {
                    resorceBuild = Assembly.LoadFile(path);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    return;
                }
                
                var dictionary = new Dictionary<string, string>() 
                {
                    {"System.Int32", "int"},
                    {"System.Double", "float"},
                    {"System.String", "nvarchar(max)"},
                    {"System.DateTime", "date"},
                    {"System.Boolean", "byte"},
                    {"System.Char", "nvarchar(1)"}
                };

                foreach (var type in resorceBuild.GetTypes())
                {
                    string resultQuery = "";

                    if (type.IsClass && !type.IsAbstract && type.GetProperties().Length >= 2)
                    {
                        int iteration = 0;
                        string tableName = $"{type.Name}s";

                        if (type.Name[type.Name.Length - 1] == 'y')
                        {
                            tableName = $"{type.Name.Substring(0, type.Name.Length - 1)}ies";
                        }

                        resultQuery += $"create table {tableName} (\n";

                        foreach (var property in type.GetProperties())
                        {
                            if (dictionary.TryGetValue(property.PropertyType.ToString(), out string valueType))
                            {
                                resultQuery += $"\t{property.Name} {valueType} ";

                                if (property.PropertyType.ToString() == "System.String")
                                {
                                    resultQuery += "null";
                                }
                                else
                                {
                                    resultQuery += "not null";
                                }

                                if (iteration == 0)
                                {
                                    resultQuery += " primary key,\n";
                                }
                                else if (iteration < type.GetProperties().Length - 1)
                                {
                                    resultQuery += ",\n";
                                }
                                else
                                {
                                    resultQuery += "\n";
                                }
                            }
                            iteration++;
                        }

                        resultQuery += ");\n";
                    }

                    Console.WriteLine(resultQuery);
                }
            }
            else
            {
                Console.WriteLine("Некорректный путь!");
            }
        }
    }
}
