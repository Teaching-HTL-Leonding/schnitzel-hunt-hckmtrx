Console.OutputEncoding = System.Text.Encoding.Default;

#region Constants
const string MENUCARDS_DIRECTORY = "menucards";
const string DISH_TO_FIND = "Schnitzel";
const string CHEAPEST_DISH = "Seitan Schnitzel";
const char PRICE_DELIMITER = ':';
const char CURRENCY = '€';
#endregion

#region Main Program
{
    var files = Directory.GetFiles(MENUCARDS_DIRECTORY);

    Console.WriteLine("WHERE TO GET SCHNITZEL?");
    Console.WriteLine("=======================\n");

    LoopThroughFiles(files);
}
#endregion

#region Methods
void LoopThroughFiles(string[] files)
{
    var dishPrices = new (string RestaurantName, decimal Price)[files.Length];
    var cheapestMenus = new (string RestaurantName, string DishName, decimal DishPrice)[3];

    for (int i = 0; i < files.Length; i++)
    {
        var file = files[i];
        var restaurantName = Path.GetFileNameWithoutExtension(file);

        var lines = File.ReadAllLines(file);
        var text = string.Join('\n', lines);

        if (text.Contains(DISH_TO_FIND))
        {
            Console.WriteLine(restaurantName);


            var cheapDishes = AnalyzeAndPrintDishes(lines, cheapestMenus);

            for (int j = 0; j < cheapDishes.Length; j++)
            {
                if (cheapDishes[j].DishPrice != 0 && (cheapestMenus[j].DishPrice == 0 || cheapDishes[j].DishPrice < cheapestMenus[j].DishPrice))
                {
                    cheapestMenus[j] = (
                        restaurantName,
                        cheapDishes[j].DishName,
                        cheapDishes[j].DishPrice
                    );
                }
            }
        }

        if (text.Contains(CHEAPEST_DISH))
        {
            var cheapestDishIndex = Array.IndexOf(
                lines,
                lines.Where(dish => dish.Contains(CHEAPEST_DISH)).First()
            );

            dishPrices[i] = (restaurantName, GetPriceOfDish(lines[cheapestDishIndex]));
        }
    }

    PrintCheapAndExpensiveDish(dishPrices);
    PrintCheapMenu(cheapestMenus);
}

(string DishName, decimal DishPrice)[] AnalyzeAndPrintDishes(string[] lines, (string RestaurantName, string DishName, decimal DishPrice)[] cheapMenus)
{
    var courseIndex = -1;
    var cheapDishes = new (string, decimal)[3];

    for (int i = 0; i < lines.Length; i++)
    {
        var line = lines[i];

        if (line.Length > 0 && line.All(character => char.IsUpper(character) || character == ' ')) { courseIndex++; }
        else if (line.Contains(DISH_TO_FIND))
        {
            var dishName = line.Substring(0, line.LastIndexOf(PRICE_DELIMITER));
            var dishPrice = GetPriceOfDish(line);

            if (cheapMenus[courseIndex].DishPrice == 0 || dishPrice < cheapMenus[courseIndex].DishPrice)
            {
                cheapDishes[courseIndex] = (dishName, dishPrice);
            }

            Console.WriteLine($"\t{dishName}");
        }
    }

    return cheapDishes;
}

void PrintCheapAndExpensiveDish((string RestaurantName, decimal Price)[] dishPrices)
{
    var orderedDishes = dishPrices
        .Where(dish => dish.Price != 0)
        .OrderBy(dish => dish.Price);

    var cheapestDish = orderedDishes.First();
    var expensiveDish = orderedDishes.Last();


    Console.WriteLine("\n\nWHERE TO GET THE CHEAPEST SEITAN SCHNITZEL?");
    Console.WriteLine("===========================================\n");
    Console.WriteLine($"{cheapestDish.RestaurantName}, {cheapestDish.Price}{CURRENCY}");

    Console.WriteLine("\n\nWHERE TO GET THE MOST EXPENSIVE SEITAN SCHNITZEL?");
    Console.WriteLine("=================================================\n");
    Console.WriteLine($"{expensiveDish.RestaurantName}, {expensiveDish.Price}{CURRENCY}");
}

void PrintCheapMenu((string RestaurantName, string DishName, decimal DishPrice)[] feast)
{
    Console.WriteLine("\n\nWHERE TO GET THE CHEAPEST SCHNITZEL FEAST?");
    Console.WriteLine("==========================================\n");

    for (int i = 0; i < feast.Length; i++)
    {
        var course = feast[i];
        var courseOutput = i switch
        {
            0 => "Appetizers",
            1 => "Main Dish",
            _ => "Dessert"
        };

        Console.WriteLine($"{courseOutput}: {course.RestaurantName}, {course.DishName}, {course.DishPrice}{CURRENCY}");
    }
}

decimal GetPriceOfDish(string dish)
{
    return decimal.Parse(
        dish[
            (dish.LastIndexOf(PRICE_DELIMITER) + 2)..dish.LastIndexOf(CURRENCY)
        ]
    );
}
#endregion
