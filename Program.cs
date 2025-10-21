using System;
using System.Globalization;

namespace OOP_Lab_Var10
{
    // Інтерфейс для демонстрації поліморфізму
    interface IFraction
    {
        void InitCoefficients();
        void Show();
        double Value(double x);
    }

    // Базовий клас: дріб 1 / (a * x)
    class Fraction : IFraction
    {
        private const double Epsilon = 1e-12;
        private double _a;

        protected double A
        {
            get => _a;
            set => _a = value;
        }

        // Конструктор за замовчуванням
        public Fraction()
        {
        }

        // Конструктор з параметром
        public Fraction(double a)
        {
            _a = a;
        }

        public virtual void InitCoefficients()
        {
            Console.Write("Введіть коефіцієнт a (може бути 0, але тоді буде ділення на нуль): ");
            A = ReadDoubleFromConsole();
        }

        public virtual void Show()
        {
            Console.WriteLine($"Тип: Простий дріб  1/(a*x). a = {A}");
        }

        public virtual double Value(double x)
        {
            double denom = A * x;
            if (Math.Abs(denom) < Epsilon)
            {
                throw new DivideByZeroException("Знаменник (a * x) дорівнює нулю.");
            }
            return 1.0 / denom;
        }

        /// <summary>
        /// Зчитує число типу double з консолі (допускає 0).
        /// </summary>
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

        /// <summary>
        /// Зчитує число, що не дорівнює 0.
        /// </summary>
        protected static double ReadDoubleNotZero(string prompt)
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

    // Похідний клас: триступеневий ланцюговий дріб
    class ContinuedFraction : Fraction
    {
        private const double Epsilon = 1e-12;
        private double _a1, _a2, _a3;

        protected double A1 { get => _a1; set => _a1 = value; }
        protected double A2 { get => _a2; set => _a2 = value; }
        protected double A3 { get => _a3; set => _a3 = value; }

        // Конструктор за замовчуванням
        public ContinuedFraction()
        {
        }

        // Конструктор з параметрами
        public ContinuedFraction(double a1, double a2, double a3) : base()
        {
            if (Math.Abs(a1) < Epsilon || Math.Abs(a2) < Epsilon || Math.Abs(a3) < Epsilon)
                throw new ArgumentException("Коефіцієнти a1, a2, a3 не можуть бути нульовими.");
            
            _a1 = a1;
            _a2 = a2;
            _a3 = a3;
        }

        public override void InitCoefficients()
        {
            A1 = ReadDoubleNotZero("Введіть a1 (не може бути 0): ");
            A2 = ReadDoubleNotZero("Введіть a2 (не може бути 0): ");
            A3 = ReadDoubleNotZero("Введіть a3 (не може бути 0): ");
        }

        public override void Show()
        {
            Console.WriteLine("Тип: Триступеневий ланцюговий дріб:");
            Console.WriteLine($"   1 / ( {A1}*x + 1/( {A2}*x + 1/( {A3}*x ) ) )");
            Console.WriteLine($"Коефіцієнти: a1 = {A1}, a2 = {A2}, a3 = {A3}");
        }

        public override double Value(double x)
        {
            double innerMost = A3 * x;
            if (Math.Abs(innerMost) < Epsilon)
                throw new DivideByZeroException("Неможливо обчислити: a3 * x = 0 (внутрішній знаменник).");

            double inner2 = A2 * x + 1.0 / innerMost;
            if (Math.Abs(inner2) < Epsilon)
                throw new DivideByZeroException("Неможливо обчислити: a2*x + 1/(a3*x) = 0 (середній знаменник).");

            double inner1 = A1 * x + 1.0 / inner2;
            if (Math.Abs(inner1) < Epsilon)
                throw new DivideByZeroException("Неможливо обчислити: a1*x + 1/(a2*x + 1/(a3*x)) = 0 (зовнішній знаменник).");

            return 1.0 / inner1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Лабораторна (варіант 10). Поліморфізм та віртуальні методи.");
            
            // Демонстрація роботи з конструкторами
            Console.WriteLine("\n=== Демонстрація конструкторів ===");
            RunConstructorDemo();

            // Інтерактивний режим
            Console.WriteLine("\n=== Інтерактивний режим ===");
            RunInteractiveMode();
        }

        static void RunConstructorDemo()
        {
            try
            {
                Console.WriteLine("\n1. Простий дріб з a = 2:");
                IFraction f1 = new Fraction(2);
                f1.Show();
                Console.WriteLine($"   Значення при x = 3: {f1.Value(3)}");

                Console.WriteLine("\n2. Ланцюговий дріб з a1=1, a2=2, a3=3:");
                IFraction f2 = new ContinuedFraction(1, 2, 3);
                f2.Show();
                Console.WriteLine($"   Значення при x = 1: {f2.Value(1)}");

                Console.WriteLine("\n3. Тест крайнього випадку (x близький до 0):");
                IFraction f3 = new Fraction(1);
                double smallX = 0.001;
                Console.WriteLine($"   Значення при x = {smallX}: {f3.Value(smallX)}");

                Console.WriteLine("\n4. Тест на ділення на нуль:");
                try
                {
                    IFraction f4 = new Fraction(0);
                    f4.Value(5);
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"   Очікувана помилка: {ex.Message}");
                }
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Помилка обчислення: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Помилка аргументу: {ex.Message}");
            }
        }

        static void RunInteractiveMode()
        {
            Console.WriteLine("Оберіть режим роботи:");
            Console.WriteLine("  1 - Простий дріб 1/(a * x)");
            Console.WriteLine("  2 - Триступеневий ланцюговий дріб 1/(a1*x + 1/(a2*x + 1/(a3*x)))");
            Console.WriteLine("  0 - Вихід");

            while (true)
            {
                Console.Write("\nВаш вибір (0/1/2): ");
                string choice = Console.ReadLine()?.Trim();

                if (choice == "0")
                {
                    Console.WriteLine("Вихід. Успіхів!");
                    return;
                }

                IFraction fraction = null;

                if (choice == "1")
                    fraction = new Fraction();
                else if (choice == "2")
                    fraction = new ContinuedFraction();
                else
                {
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                    continue;
                }

                fraction.InitCoefficients();
                fraction.Show();

                Console.Write("Введіть точку x, в якій обчислити значення дробу: ");
                double x = Fraction.ReadDoubleFromConsole();

                try
                {
                    double result = fraction.Value(x);
                    Console.WriteLine($"Значення дробу в x = {x} : {result}");
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"Помилка обчислення: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Несподівана помилка: {ex.GetType().Name} - {ex.Message}");
                }

                Console.WriteLine("\n--- Готово. Можна повторити або вийти ---");
            }
        }
    }
}

