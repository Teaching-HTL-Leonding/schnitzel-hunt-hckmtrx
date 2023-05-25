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
    LoopThroughFiles(files);
}
#endregion

#region Methods
void LoopThroughFiles(string[] files)
{
    var cheapestDishesOnMenus = new (string RestaurantName, decimal Price)[files.Length];

    for (int i = 0; i < files.Length; i++)
    {
        var file = files[i];

        var lines = File.ReadAllLines(file);
        var text = string.Join('\n', lines);

        var restaurantName = Path.GetFileNameWithoutExtension(file);

        if (text.Contains(DISH_TO_FIND))
        {
            Console.WriteLine(restaurantName);
            PrintDishes(lines);
        }

        if (text.Contains(CHEAPEST_DISH))
        {
            cheapestDishesOnMenus[i] = (restaurantName, GetPriceOfDish(CHEAPEST_DISH, lines));
        }
    }

    var orderedDishes = cheapestDishesOnMenus
        .Where(dish => dish.Price != 0)
        .OrderBy(dish => dish.Price);

    var cheapestDish = orderedDishes.First();
    var expensiveDish = orderedDishes.Last();

    Console.WriteLine($"\n{cheapestDish.RestaurantName}, {cheapestDish.Price}{CURRENCY}");
    Console.WriteLine($"{expensiveDish.RestaurantName}, {expensiveDish.Price}{CURRENCY}");
}

void PrintDishes(string[] dishes)
{
    foreach (var dish in dishes)
    {
        if (dish.Contains(DISH_TO_FIND))
        {
            Console.WriteLine($"\t{dish.Substring(0, dish.LastIndexOf(PRICE_DELIMITER))}");
        }
    }
}

decimal GetPriceOfDish(string dish, string[] lines)
{
    foreach (var line in lines)
    {
        if (line.Contains(dish))
        {
            var price = decimal.Parse(
                line[
                    (line.LastIndexOf(PRICE_DELIMITER) + 2)..
                    line.LastIndexOf(CURRENCY)
                ]
            );

            return price;
        }
    }

    throw new Exception($"dish: {dish} not found");
}
#endregion
