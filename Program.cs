using System;
using System.Globalization;

namespace OOP_Lab_Var10
{
    // Інтерфейс для демонстрації поліморфізму
    public interface IFraction
    {
        void InitCoefficients();
        void Show();
        double Value(double x);
    }

    // Базовий клас: дріб 1 / (a * x)
    public class Fraction : IFraction
    {
        // Константа для перевірок на нуль
        private const double Epsilon = 1e-12;

        // Приватне поле з інкапсуляцією через protected властивість
        private double _a;
        protected double A
        {
            get => _a;
            set => _a = value;
        }

        // Конструктор за замовчуванням
        public Fraction() { }

        // Конструктор із параметром
        public Fraction(double a)
        {
            _a = a;
        }

        // Віртуальні методи
        public virtual void InitCoefficients()
        {
            A = ReadDoubleNotZero("Введіть коефіцієнт a: ");
        }

        public virtual void Show()
        {
            Console.WriteLine($"Тип: Простий дріб  1/(a*x), де a = {A}");
        }

        public virtual double Value(double x)
        {
            double denom = A * x;
            if (Math.Abs(denom) < Epsilon)
                throw new DivideByZeroException("Знаменник (a * x) дорівнює нулю.");
            return 1.0 / denom;
        }

        // Публічний метод для зчитування чисел (доступний з Program)
        public static double ReadDoubleFromConsole()
        {
            while (true)
            {
                string s = Console.ReadLine();
                if (double.TryParse(s?.Trim(), NumberStyles.Any, CultureInfo.InvariantCulture, out double val) ||
                    double.TryParse(s?.Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out val))
                {
                    return val;
                }
                Console.Write("Невірний формат. Спробуйте ще раз: ");
            }
        }

        // Зчитування ненульового числа
        public static double ReadDoubleNotZero(string prompt)
        {
            double val;
            do
            {
                Console.Write(prompt);
                val = ReadDoubleFromConsole();
                if (Math.Abs(val) < Epsilon)
                    Console.WriteLine("Значення не може бути 0. Повторіть ввод.");
            } while (Math.Abs(val) < Epsilon);
            return val;
        }
    }

    // Похідний клас: продовжений дріб 1 / (a1*x + 1/(a2*x + 1/(a3*x)))
    public class ContinuedFraction : Fraction
    {
        private const double Epsilon = 1e-12;

        private double _a1, _a2, _a3;

        protected double A1 { get => _a1; set => _a1 = value; }
        protected double A2 { get => _a2; set => _a2 = value; }
        protected double A3 { get => _a3; set => _a3 = value; }

        // Конструктори
        public ContinuedFraction() { }

        public ContinuedFraction(double a1, double a2, double a3)
        {
            _a1 = a1;
            _a2 = a2;
            _a3 = a3;
        }

        public override void InitCoefficients()
        {
            A1 = ReadDoubleNotZero("Введіть a1: ");
            A2 = ReadDoubleNotZero("Введіть a2: ");
            A3 = ReadDoubleNotZero("Введіть a3: ");
        }

        public override void Show()
        {
            Console.WriteLine("Тип: Продовжений дріб:");
            Console.WriteLine($"   1 / ( {A1}*x + 1/( {A2}*x + 1/( {A3}*x ) ) )");
        }

        public override double Value(double x)
        {
            double innerMost = A3 * x;
            if (Math.Abs(innerMost) < Epsilon)
                throw new DivideByZeroException("Помилка: a3 * x = 0.");

            double inner2 = A2 * x + 1.0 / innerMost;
            if (Math.Abs(inner2) < Epsilon)
                throw new DivideByZeroException("Помилка: a2*x + 1/(a3*x) = 0.");

            double inner1 = A1 * x + 1.0 / inner2;
            if (Math.Abs(inner1) < Epsilon)
                throw new DivideByZeroException("Помилка: a1*x + 1/(a2*x + 1/(a3*x)) = 0.");

            return 1.0 / inner1;
        }
    }

    // Головна програма
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("Лабораторна №5 — Поліморфізм. Варіант 10.");
            Console.WriteLine("1 — Простий дріб 1/(a*x)");
            Console.WriteLine("2 — Продовжений дріб 1/(a1*x + 1/(a2*x + 1/(a3*x)))");
            Console.WriteLine("3 — Тести / демонстрації");
            Console.WriteLine("0 — Вихід");

            while (true)
            {
                Console.Write("\nВаш вибір: ");
                string choice = Console.ReadLine()?.Trim();

                if (choice == "0") break;

                IFraction fraction;

                switch (choice)
                {
                    case "1":
                        fraction = new Fraction();
                        break;
                    case "2":
                        fraction = new ContinuedFraction();
                        break;
                    case "3":
                        RunTests();
                        continue;
                    default:
                        Console.WriteLine("Невірний вибір, спробуйте ще.");
                        continue;
                }

                fraction.InitCoefficients();
                fraction.Show();

                Console.Write("Введіть значення x: ");
                double x = Fraction.ReadDoubleFromConsole();

                try
                {
                    double value = fraction.Value(x);
                    Console.WriteLine($"Результат обчислення: {value}");
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine("Помилка обчислення: " + ex.Message);
                }
            }
        }

        // Демонстраційні тести
        private static void RunTests()
        {
            Console.WriteLine("\n=== Демонстраційні кейси ===");

            var f1 = new Fraction(2);
            var f2 = new ContinuedFraction(1, 2, 3);

            double[] xs = { 0.5, 1, 0.0001 };

            foreach (double x in xs)
            {
                try
                {
                    Console.WriteLine($"\nx = {x}");
                    Console.WriteLine($"Fraction(2): {f1.Value(x)}");
                    Console.WriteLine($"ContinuedFraction(1,2,3): {f2.Value(x)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"x={x}: {ex.Message}");
                }
            }
        }
    }
}

    }
}

