/*
    1. You need to create a Math game containing the 4 basic operations
    2. The divisions should result on INTEGERS ONLY and dividends should go from 0 to 100. Example: Your app shouldn't present the division 7/2 to the user, since it doesn't result in an integer.
    3. Users should be presented with a menu to choose an operation
    4. You should record previous games in a List and there should be an option in the menu for the user to visualize a history of previous games.
*/


using System.Diagnostics;
using WillCodeBitsMathGame;

MathGameLogic mathGame = new MathGameLogic();
Random random = new Random();

int firstNumber;
int secondNumber;
int userMenuSelection;
int score = 0;
bool gameOver = false;

DifficultyLevel difficultyLevel = DifficultyLevel.Easy;

while (!gameOver)
{
    userMenuSelection = GetUserMenuSelection(mathGame);

    firstNumber = random.Next(1, 101);
    secondNumber = random.Next(1, 101);

    switch (userMenuSelection)
    {

        case 1:
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '+', difficultyLevel);
            break;
        case 2:
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '-', difficultyLevel);
            break;
        case 3:
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '*', difficultyLevel);
            break;
        case 4:
            while (firstNumber % secondNumber != 0)
            {
                firstNumber = random.Next(1, 101);
                secondNumber = random.Next(1, 101);
            }
            score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '+', difficultyLevel);
            break;
        case 5:
            int numberOfQuestions = 99;
            Console.WriteLine("Please enter the number of question you want to attempt.");
            while (!int.TryParse(Console.ReadLine(), out numberOfQuestions))
            {
                Console.WriteLine("Please enter the number of questions you want to attempt as an integer number");
            }
            while (numberOfQuestions > 0)
            {
                int randomOperation = random.Next(1, 5);

                if (randomOperation == 1)
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '+', difficultyLevel);
                }
                else if (randomOperation == 2)
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '-', difficultyLevel);
                }
                else if (randomOperation == 3)
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '*', difficultyLevel);
                }
                else
                {
                    firstNumber = random.Next(1, 101);
                    secondNumber = random.Next(1, 101);
                    while (firstNumber % secondNumber != 0)
                    {
                        firstNumber = random.Next(1, 101);
                        secondNumber = random.Next(1, 101);
                    }
                    score += await PerformOperation(mathGame, firstNumber, secondNumber, score, '+', difficultyLevel);
                }
                numberOfQuestions--;
            }
            break;
        case 6:
            Console.WriteLine("GAME HISTORY: \n");
            foreach (var operation in mathGame.GameHistory)
            {
                Console.WriteLine($"{operation}");
            }
            break;
        case 7:
            difficultyLevel = ChangeDifficulty();
            DifficultyLevel difficultyEnum = (DifficultyLevel)difficultyLevel;
            Enum.IsDefined(typeof(DifficultyLevel), difficultyEnum);
            Console.WriteLine($"Your new difficulty level: {difficultyLevel}");
            break;

        case 8:
            gameOver = true;
            Console.WriteLine($"Your final score is: {score}");
            break;
    }
}

static DifficultyLevel ChangeDifficulty()
{
    int userSelection = 0;

    Console.WriteLine("Please enter a difficulty level");
    Console.WriteLine("1. Easy");
    Console.WriteLine("2. Medium");
    Console.WriteLine("3. Hard");

    while (!int.TryParse(Console.ReadLine(), out userSelection) || (userSelection < 1 || userSelection > 3))
    {
        Console.WriteLine("Please enter a valid option 1-3");
    }

    return userSelection switch
    {
        1 => DifficultyLevel.Easy,
        2 => DifficultyLevel.Medium,
        3 => DifficultyLevel.Hard,
        _ => DifficultyLevel.Easy,
    };
}

static void DisplayMathGameQuestion(int firstNumber, int secondNumber, char operation)
{
    Console.WriteLine($"{firstNumber} {operation} {secondNumber} = ??");
}

static int GetUserMenuSelection(MathGameLogic mathGame)
{
    int selection = -1;
    mathGame.ShowMenu();
    while (selection < 1 || selection > 8)
    {
        while (!int.TryParse(Console.ReadLine(), out selection))
        {
            Console.WriteLine("Please enter a valid option 1-8");
        }

        if (!(selection >= 1 && selection <= 8))
        {
            Console.WriteLine("Please enter a valid option 1-8");
        }
    }

    return selection;
}


static async Task<int?> GetUserResponse(DifficultyLevel difficulty)
{
    int response = 0;
    int timeout = (int)difficulty;

    Stopwatch stopwatch = new Stopwatch();
    stopwatch.Start();

    Task<string?> getUserInputTask = Task.Run(() => Console.ReadLine());

    try
    {
        string? result = await Task.WhenAny(getUserInputTask, Task.Delay(timeout * 1000)) == getUserInputTask ? getUserInputTask.Result : null;

        stopwatch.Stop();

        if (result != null && int.TryParse(result, out response))
        {
            Console.WriteLine($"Time taken to answer: {stopwatch.Elapsed.ToString(@"m\::ss\.fff")}");
            return response;
        }

        else
        {
            throw new OperationCanceledException();
        }
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Time is up!");
        return null;
    }
}

static int ValidateResult(int result, int? userResponse, int score)
{
    if (result == userResponse)
    {
        Console.WriteLine("You answer correctly; You earned 5 point");
        score += 5;
    }
    else
    {
        Console.WriteLine("Try again!");
        Console.WriteLine($"Correct answer is: {result}");
    }
    return score;
}

static async Task<int> PerformOperation(MathGameLogic mathGame, int firstNumber, int secondNumber, int score, char operation, DifficultyLevel difficulty)
{
    int result;
    int? userResponse;
    DisplayMathGameQuestion(firstNumber, secondNumber, operation);
    result = mathGame.MathOperation(firstNumber, secondNumber, operation);
    userResponse = await GetUserResponse(difficulty);
    score += ValidateResult(result, userResponse, score);
    return score;

}
public enum DifficultyLevel
{
    Easy = 45,
    Medium = 30,
    Hard = 15
};

