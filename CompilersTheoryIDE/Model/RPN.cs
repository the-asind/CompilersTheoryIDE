using System;
using System.Collections.Generic;

namespace CompilersTheoryIDE.Model;

public class PolishNotationCalculator
{
    public string Output = "";
    public double Result;
    public List<string> Errors = new();

    public double Calculate(string input)
    {
        Output = GetExpression(input);
        if (Errors.Count == 0)
            Result = Counting(Output);
        else
            Result = 0;
        return Result;
    }

    private string GetExpression(string input)
    {
        var output = string.Empty;
        var operatorStack = new Stack<char>();

        for (var i = 0; i < input.Length; i++)
        {
            if (IsSeparator(input[i]))
                continue;

            if (char.IsDigit(input[i]))
            {
                while (!IsSeparator(input[i]) && !IsOperator(input[i]) && !IsUnexpectedSymbol(input[i]))
                {
                    output += input[i];
                    i++;

                    if (i == input.Length) break;
                }

                output += " ";
                i--;
            }

            if (IsOperator(input[i]))
                switch (input[i])
                {
                    case '(':
                        operatorStack.Push(input[i]);
                        break;
                    case ')':
                    {
                        var foundOpeningBracket = false;
                        while (operatorStack.Count > 0 && operatorStack.Peek() != '(')
                            output += operatorStack.Pop() + " ";

                        if (operatorStack.Count > 0 && operatorStack.Peek() == '(')
                        {
                            operatorStack.Pop();
                            foundOpeningBracket = true;
                        }

                        if (!foundOpeningBracket)
                            Errors.Add("Ошибка: Отсутствует открывающая скобка для закрывающей скобки");

                        break;
                    }
                    default:
                    {
                        if (operatorStack.Count > 0)
                            if (GetPriority(input[i]) <= GetPriority(operatorStack.Peek()))
                                output += operatorStack.Pop() + " ";

                        operatorStack.Push(char.Parse(input[i].ToString()));
                        break;
                    }
                }

            if (IsUnexpectedSymbol(input[i]))
                Errors.Add("Ошибка: Непредвиденный символ " + input[i]);
        }

        while (operatorStack.Count > 0)
            if (operatorStack.Peek() == '(')
            {
                Errors.Add("Ошибка: Отсутствует закрывающая скобка");
                operatorStack.Pop();
            }
            else
            {
                output += operatorStack.Pop() + " ";
            }

        return output;
    }

    private double Counting(string input)
    {
        double result = 0;
        var temp = new Stack<double>();

        for (var i = 0; i < input.Length; i++)
            if (char.IsDigit(input[i]))
            {
                var a = string.Empty;

                while (!IsSeparator(input[i]) && !IsOperator(input[i]) && !IsUnexpectedSymbol(input[i]))
                {
                    a += input[i];
                    i++;
                    if (i == input.Length) break;
                }

                temp.Push(double.Parse(a));
                i--;
            }
            else if (IsOperator(input[i]))
            {
                if (temp.Count < 2)
                    Errors.Add("Ошибка: Недостаточно операндов для данного оператора");

                var a = temp.Pop();
                var b = temp.Pop();

                result = input[i] switch
                {
                    '+' => b + a,
                    '-' => b - a,
                    '*' => b * a,
                    '/' => b / a,
                    '^' => double.Parse(Math.Pow(double.Parse(b.ToString()), double.Parse(a.ToString())).ToString()),
                    _ => result
                };
                temp.Push(result);
            }

        return temp.Peek();
    }

    private static bool IsSeparator(char c)
    {
        return " =".Contains(c);
    }

    private static bool IsOperator(char с)
    {
        return "+-/*^()".Contains(с);
    }

    private static bool IsUnexpectedSymbol(char c)
    {
        return !IsOperator(c) && !IsSeparator(c) && !char.IsDigit(c);
    }

    private static byte GetPriority(char s)
    {
        return s switch
        {
            '(' => 0,
            ')' => 1,
            '+' => 2,
            '-' => 3,
            '*' => 4,
            '/' => 4,
            '^' => 5,
            _ => 6
        };
    }

    public string GetNormalExpression(string rpnExpression)
    {
        var stack = new Stack<string>();
        var tokens = rpnExpression.Split(' ');

        foreach (var token in tokens)
            if (IsOperator(token[0]))
            {
                if (stack.Count < 2)
                {
                    Errors.Add("Ошибка: Недостаточно операндов для данного оператора");
                    return "";
                }

                var operand2 = stack.Pop();
                var operand1 = stack.Pop();
                var expression = $"({operand1} {token} {operand2})";
                stack.Push(expression);
            }
            else
            {
                stack.Push(token);
            }

        if (stack.Count == 1) return stack.Pop();
        Errors.Add("Ошибка: Неверное количество операндов в обратной польской нотации");
        return stack.ToString();

    }
}