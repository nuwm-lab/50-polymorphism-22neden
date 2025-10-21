using System;
using System.Globalization;

namespace OOP_Lab_Var10
{
    // Інтерфейс для демонстрації поліморфізму
    interface IFraction
    {
        void Show();
        double Value(double x);
    }

    // Базовий клас: дріб 1 / (a * x)
    class Fraction : IFraction
    {
        protected const double Epsilon = 1e-12;
        private double _a;

        protected double A
        {
            get => _a;
            set => _a = value;
        }

        // Конструктор за замовчуванням
        public Fraction()
        {
            _a = 1.0; // Значення за замовчуванням
        }

        // Конструктор з параметром
        public Fraction(double a)
        {
            _a = a;
        }

        public virtual void Show()
        {
            Console.WriteLine($"Тип: Простий дріб 1/(a*x), a = {A}");
        }

        /// <summary>
        /// Обчислює значення дробу 1/(a*x).
        /// </summary>
        /// <exception cref="DivideByZeroException">Викидається, якщо a*x ≈ 0</exception>
        public virtual double Value(double x)
        {
            double denom = A * x;
            if (Math.Abs(denom) < Epsilon)
            {
                throw new DivideByZeroException($"Знаменник (a * x) дорівнює нулю: {A} * {x} ≈ 0");
            }
            return 1.0 / denom;
        }
    }

    // Похідний клас: триступеневий ланцюговий дріб
    // 1 / (a1*x + 1/(a2*x + 1/(a3*x)))
    class ContinuedFraction : Fraction
    {
        private double _a1, _a2, _a3;

        public double A1 
        { 
            get => _a1; 
            set => _a1 = value; 
        }
        
        public double A2 
        { 
            get => _a2; 
            set => _a2 = value; 
        }
        
        public double A3 
        { 
            get => _a3; 
            set => _a3 = value; 
        }

        // Конструктор за замовчуванням
        public ContinuedFraction() : base()
        {
            _a1 = 1.0;
            _a2 = 1.0;
            _a3 = 1.0;
        }

        // Конструктор з параметрами
        public ContinuedFraction(double a1, double a2, double a3) : base()
        {
            _a1 = a1;
            _a2 = a2;
            _a3 = a3;
        }

        public override void Show()
        {
            Console.WriteLine("Тип: Триступеневий ланцюговий дріб");
            Console.WriteLine($"Формула: 1 / ({A1}*x + 1/({A2}*x + 1/({A3}*x)))");
            Console.WriteLine($"Коефіцієнти: a1 = {A1}, a2 = {A2}, a3 = {A3}");
        }

        /// <summary>
        /// Обчислює значення триступеневого ланцюгового дробу.
        /// </summary>
        /// <exception cref="DivideByZeroException">Викидається при діленні на нуль на будь-якому рівні</exception>
        public override double Value(double x)
        {
            // Обчислюємо знизу вгору
            double innerMost = A3 * x;
            if (Math.Abs(innerMost) < Epsilon)
                throw new DivideByZeroException($"Внутрішній знаменник a3*x = 0: {A3} * {x} ≈ 0");

            double level2 = A2 * x + 1.0 / innerMost;
            if (Math.Abs(level2) < Epsilon)
                throw new DivideByZeroException($"Середній знаменник a2*x + 1/(a3*x) ≈ 0");

            double level1 = A1 * x + 1.0 / level2;
            if (Math.Abs(level1) < Epsilon)
                throw new DivideByZeroException($"Зовнішній знаменник a1*x + 1/(a2*x + 1/(a3*x)) ≈ 0");

            return 1.0 / level1;
        }
    }

    // Допоміжний клас для роботи з консоллю (відокремлення вводу/виводу)
    static class ConsoleHelper
    {
        /// <summary>
        /// Зчитує число типу double з консолі.
        /// Підтримує як крапку, так і кому як десятковий роздільник.
        /// </summary>
        public static double ReadDouble(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                string input = Console.ReadLine()?.Trim();
                
                if (string.IsNullOrEmpty(input))
                {
                    Console.WriteLine("Порожній ввід. Спробуйте ще раз.");
                    continue;
                }

                // Нормалізуємо роздільник: заміна коми на крапку
                string normalized = input.Replace(',', '.');
                
                if (double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                {
                    return value;
                }
                
                Console.WriteLine("Невірний формат числа. Спробуйте ще раз.");
            }
        }

        /// <summary>
        /// Зчитує ненульове число.
        /// </summary>
        public static double ReadNonZeroDouble(string prompt)
        {
            while (true)
            {
                double value = ReadDouble(prompt);
                if (Math.Abs(value) >= Fraction.Epsilon)
                {
                    return value;
                }
                Console.WriteLine("Значення не може бути нульовим. Спробуйте ще раз.");
            }
        }
    }

    // Клас для unit-тестів
    static class FractionTests
    {
        public static void RunAllTests()
        {
            Console.WriteLine("=== Запуск unit-тестів ===\n");
            
            int passed = 0;
            int total = 0;

            // Тест 1: Простий дріб з нормальними значеннями
            total++;
            try
            {
                var f = new Fraction(2);
                double result = f.Value(3);
                double expected = 1.0 / (2 * 3);
                if (Math.Abs(result - expected) < 1e-10)
                {
                    Console.WriteLine("✓ Тест 1 пройдено: Fraction(2).Value(3) = " + result);
                    passed++;
                }
                else
                {
                    Console.WriteLine($"✗ Тест 1 провалено: очікувалось {expected}, отримано {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тест 1 провалено: {ex.Message}");
            }

            // Тест 2: Простий дріб - ділення на нуль
            total++;
            try
            {
                var f = new Fraction(0);
                f.Value(5);
                Console.WriteLine("✗ Тест 2 провалено: очікувалось DivideByZeroException");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("✓ Тест 2 пройдено: DivideByZeroException при a=0");
                passed++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тест 2 провалено: неочікуваний виняток {ex.GetType().Name}");
            }

            // Тест 3: Простий дріб - x=0
            total++;
            try
            {
                var f = new Fraction(2);
                f.Value(0);
                Console.WriteLine("✗ Тест 3 провалено: очікувалось DivideByZeroException");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("✓ Тест 3 пройдено: DivideByZeroException при x=0");
                passed++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тест 3 провалено: неочікуваний виняток {ex.GetType().Name}");
            }

            // Тест 4: Ланцюговий дріб з нормальними значеннями
            total++;
            try
            {
                var cf = new ContinuedFraction(1, 2, 3);
                double result = cf.Value(1);
                // 1 / (1*1 + 1/(2*1 + 1/(3*1))) = 1 / (1 + 1/(2 + 1/3)) = 1 / (1 + 1/(7/3)) = 1 / (1 + 3/7) = 1 / (10/7) = 0.7
                double expected = 0.7;
                if (Math.Abs(result - expected) < 1e-10)
                {
                    Console.WriteLine($"✓ Тест 4 пройдено: ContinuedFraction(1,2,3).Value(1) = {result}");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"✗ Тест 4 провалено: очікувалось {expected}, отримано {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тест 4 провалено: {ex.Message}");
            }

            // Тест 5: Ланцюговий дріб - x=0
            total++;
            try
            {
                var cf = new ContinuedFraction(1, 2, 3);
                cf.Value(0);
                Console.WriteLine("✗ Тест 5 провалено: очікувалось DivideByZeroException");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("✓ Тест 5 пройдено: DivideByZeroException при x=0");
                passed++;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тест 5 провалено: неочікуваний виняток {ex.GetType().Name}");
            }

            // Тест 6: Від'ємні коефіцієнти
            total++;
            try
            {
                var cf = new ContinuedFraction(-1, -2, -3);
                double result = cf.Value(1);
                double expected = -0.7; // Аналогічно тесту 4, але зі зміненим знаком
                if (Math.Abs(result - expected) < 1e-10)
                {
                    Console.WriteLine($"✓ Тест 6 пройдено: ContinuedFraction(-1,-2,-3).Value(1) = {result}");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"✗ Тест 6 провалено: очікувалось {expected}, отримано {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тест 6 провалено: {ex.Message}");
            }

            // Тест 7: Малі значення x
            total++;
            try
            {
                var f = new Fraction(1);
                double result = f.Value(0.001);
                double expected = 1000.0;
                if (Math.Abs(result - expected) < 1e-8)
                {
                    Console.WriteLine($"✓ Тест 7 пройдено: Fraction(1).Value(0.001) = {result}");
                    passed++;
                }
                else
                {
                    Console.WriteLine($"✗ Тест 7 провалено: очікувалось {expected}, отримано {result}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Тест 7 провалено: {ex.Message}");
            }

            Console.WriteLine($"\n=== Результати тестування: {passed}/{total} пройдено ===\n");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("Лабораторна робота №5 (варіант 10)");
            Console.WriteLine("Поліморфізм та віртуальні методи\n");
            
            // Запуск unit-тестів
            FractionTests.RunAllTests();

            // Демонстрація роботи з конструкторами
            Console.WriteLine("=== Демонстрація роботи з об'єктами ===\n");
            RunDemo();

            // Інтерактивний режим
            Console.WriteLine("\n=== Інтерактивний режим ===");
            RunInteractiveMode();
        }

        static void RunDemo()
        {
            try
            {
                Console.WriteLine("1. Простий дріб з a = 2:");
                IFraction f1 = new Fraction(2);
                f1.Show();
                Console.WriteLine($"   Значення при x = 3: {f1.Value(3):F6}\n");

                Console.WriteLine("2. Ланцюговий дріб з a1=1, a2=2, a3=3:");
                IFraction f2 = new ContinuedFraction(1, 2, 3);
                f2.Show();
                Console.WriteLine($"   Значення при x = 1: {f2.Value(1):F6}\n");

                Console.WriteLine("3. Тест крайнього випадку (малий x):");
                IFraction f3 = new Fraction(1);
                double smallX = 0.001;
                Console.WriteLine($"   Простий дріб 1/(1*{smallX}): {f3.Value(smallX):F6}\n");

                Console.WriteLine("4. Демонстрація обробки помилок:");
                try
                {
                    IFraction f4 = new Fraction(0);
                    f4.Show();
                    Console.Write("   Спроба обчислити Value(5)... ");
                    f4.Value(5);
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"Помилка (очікувана): {ex.Message}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Несподівана помилка: {ex.Message}");
            }
        }

        static void RunInteractiveMode()
        {
            Console.WriteLine("Оберіть режим роботи:");
            Console.WriteLine("  1 - Простий дріб 1/(a*x)");
            Console.WriteLine("  2 - Триступеневий ланцюговий дріб");
            Console.WriteLine("  0 - Вихід");

            while (true)
            {
                Console.Write("\nВаш вибір (0/1/2): ");
                string choice = Console.ReadLine()?.Trim();

                if (choice == "0")
                {
                    Console.WriteLine("Вихід з програми. До побачення!");
                    return;
                }

                IFraction fraction = null;

                try
                {
                    if (choice == "1")
                    {
                        double a = ConsoleHelper.ReadDouble("Введіть коефіцієнт a: ");
                        fraction = new Fraction(a);
                    }
                    else if (choice == "2")
                    {
                        double a1 = ConsoleHelper.ReadDouble("Введіть коефіцієнт a1: ");
                        double a2 = ConsoleHelper.ReadDouble("Введіть коефіцієнт a2: ");
                        double a3 = ConsoleHelper.ReadDouble("Введіть коефіцієнт a3: ");
                        fraction = new ContinuedFraction(a1, a2, a3);
                    }
                    else
                    {
                        Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                        continue;
                    }

                    Console.WriteLine();
                    fraction.Show();

                    double x = ConsoleHelper.ReadDouble("\nВведіть точку x для обчислення: ");

                    double result = fraction.Value(x);
                    Console.WriteLine($"\nРезультат: f({x}) = {result:F10}");
                }
                catch (DivideByZeroException ex)
                {
                    Console.WriteLine($"\n⚠ Помилка обчислення: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n⚠ Несподівана помилка: {ex.GetType().Name} - {ex.Message}");
                }

                Console.WriteLine("\n" + new string('-', 60));
            }
        }
    }
}

