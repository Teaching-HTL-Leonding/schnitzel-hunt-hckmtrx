#region Constants
const string MENUCARDS_DIRECTORY = "menucards";
const string DISH = "Schnitzel";
#endregion

#region Main Program
{
    var files = Directory.GetFiles(MENUCARDS_DIRECTORY);
    LoopThroughFiles(files);
}
#endregion

#region Methods
void LoopThroughFiles(string[] files)
{
    foreach (var file in files)
    {
        var lines = File.ReadAllLines(file);

        if (string.Join("", lines).Contains(DISH))
        {
            var fileName = Path.GetFileName(file);
            Console.WriteLine(fileName.Substring(0, fileName.LastIndexOf('.')));

            PrintDishes(lines);
        }
    }
}

void PrintDishes(string[] dishes)
{
    foreach (var dish in dishes)
    {
        if (dish.Contains(DISH))
        {
            Console.WriteLine($"\t{dish.Substring(0, dish.LastIndexOf(':'))}");
        }
    }
}
#endregion
