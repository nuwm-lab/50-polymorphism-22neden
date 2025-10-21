using System;

namespace OOP_Lab_Variant10
{
    // Базовий клас: дріб 1/(a * x)
    class Fraction
    {
        // Інкапсуляція: поля зроблені protected, доступні в похідному класі
        protected double a;

        public Fraction()
        {
            a = 1.0;
        }

        // Віртуальний метод для задання коефіцієнтів
        virtual public void SetCoefficients()
        {
            Console.Write("Введіть a (не нуль): ");
            a = ReadNonZeroDouble();
        }

        // Віртуальний метод виведення коефіцієнтів
        virtual public void Print()
        {
            Console.WriteLine($"Дріб: 1/(a * x), a = {a}");
        }

        // Віртуальний метод обчислення значення дробу у точці x
        virtual public double Evaluate(double x)
        {
            double denom = a * x;
            if (Math.Abs(denom) < 1e-12)
            {
                throw new DivideByZeroException("Знаменник дорівнює нулю при заданих a та x.");
            }
            return 1.0 / denom;
        }

        // Допоміжний метод для читання ненульового double
        protected double ReadNonZeroDouble()
        {
            while (true)
            {
                string s = Console.ReadLine();
                if (double.TryParse(s, out double val))
                {
                    if (Math.Abs(val) > 1e-12) return val;
                }
                Console.Write("Невірне значення. Введіть число, що не дорівнює нулю: ");
            }
        }
    }

    // Похідний клас: потрійний неперервний дріб 1/(a1*x + 1/(a2*x + 1/(a3*x)))
    class ContinuedFraction : Fraction
    {
        protected double a1, a2, a3;

        public ContinuedFraction()
        {
            a1 = a2 = a3 = 1.0;
        }

        // Перевизначаємо метод для задання коефіцієнтів
        public override void SetCoefficients()
        {
            Console.WriteLine("Введіть коефіцієнти a1, a2, a3 (кожен не повинен зробити знаменник нульовим):");
            Console.Write("a1 = "); a1 = ReadDoubleNotCreatingZeroDenominator();
            Console.Write("a2 = "); a2 = ReadDoubleNotCreatingZeroDenominator();
            Console.Write("a3 = "); a3 = ReadDoubleNotCreatingZeroDenominator();
        }

        public override void Print()
        {
            Console.WriteLine($"Тригонометричний підхідний дріб (ланцюжок): 1/(a1*x + 1/(a2*x + 1/(a3*x)))");
            Console.WriteLine($"a1 = {a1}, a2 = {a2}, a3 = {a3}");
        }

        // Обчислюємо значення дробу; перевіряємо, щоб проміжні знаменники не були нульовими
        public override double Evaluate(double x)
        {
            // Denominator inner-most: a3 * x
            double d3 = a3 * x;
            if (Math.Abs(d3) < 1e-12) throw new DivideByZeroException("Внутрішній знаменник a3 * x дорівнює нулю.");

            // Next: a2 * x + 1 / d3
            double d2 = a2 * x + 1.0 / d3;
            if (Math.Abs(d2) < 1e-12) throw new DivideByZeroException("Другий знаменник (a2*x + 1/(a3*x)) дорівнює нулю.");

            // Next: a1 * x + 1 / d2
            double d1 = a1 * x + 1.0 / d2;
            if (Math.Abs(d1) < 1e-12) throw new DivideByZeroException("Зовнішній знаменник (a1*x + 1/(...)) дорівнює нулю.");

            return 1.0 / d1;
        }

        // Читаємо число; тут просто уникаємо нуля для коефіцієнта,
        // але основну перевірку на нуль знаменника робимо в Evaluate.
        private double ReadDoubleNotCreatingZeroDenominator()
        {
            while (true)
            {
                string s = Console.ReadLine();
                if (double.TryParse(s, out double val))
                {
                    // не примушуємо val != 0, бо при x !=0 може бути прийнятно,
                    // але краще попередити невдале a == 0 (може зіпсувати вираз)
                    if (Math.Abs(val) > 1e-12) return val;
                }
                Console.Write("Невірне значення. Введіть число (не нуль): ");
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Лабораторна робота — варіант 10 (дріб та тригонометричний підхідний дріб)");
            Console.WriteLine("Виберіть режим роботи:");
            Console.WriteLine("1 — працювати з простим дробом 1/(a * x)");
            Console.WriteLine("2 — працювати з підрядним дробом 1/(a1*x + 1/(a2*x + 1/(a3*x)))");
            Console.Write("Ваш вибір (1 або 2): ");

            string choice = Console.ReadLine();
            Fraction obj = null;

            if (choice == "1")
            {
                obj = new Fraction();
            }
            else if (choice == "2")
            {
                obj = new ContinuedFraction();
            }
            else
            {
                Console.WriteLine("Невірний вибір. Програма завершується.");
                return;
            }

            // Викликаємо віртуальний метод через посилання на базовий клас
            obj.SetCoefficients();
            obj.Print();

            Console.Write("Введіть значення x для обчислення дробу: ");
            if (!double.TryParse(Console.ReadLine(), out double x))
            {
                Console.WriteLine("Некоректне значення x. Завершення.");
                return;
            }

            try
            {
                double result = obj.Evaluate(x);
                Console.WriteLine($"Значення дробу при x = {x} : {result}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Несподівана помилка: {ex.Message}");
            }
        }
    }
}

