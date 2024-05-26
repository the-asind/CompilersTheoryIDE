using System;

namespace CompilersTheoryIDE.Model
{
    public class RecursiveDescentParser
    {
        private string _input;
        private int _position;

        public string Parse(string expression)
        {
            _input = expression;
            _position = 0;
            var result = Formula();
            
            if (_position < _input.Length)
            {
                result += "-ОШИБКА: Некорректный ввод, остались непроанализированные символы";
            }

            return result;
        }

        private string Formula()
        {
            var result = "";
            result += NumberOrFormula();

            while (_position < _input.Length && IsOperator(_input[_position]))
            {
                result += "-ЗНАК";
                result += Sign();
                result += NumberOrFormula();
            }

            return result;
        }

        private string NumberOrFormula()
        {
            if (_position >= _input.Length || _input[_position] != '(')
                return Number();
            
            _position++; // Пропускаем '('
            var result = Formula();
            if (_position < _input.Length && _input[_position] == ')')
            {
                _position++; // Пропускаем ')'
            }
            else
            {
                result += "-ОШИБКА: Несостыковка в скобках";
                return result;
            }

            return result;
        }

        private string Number()
        {
            var result = "-ЧИСЛО";
            while (_position < _input.Length && char.IsDigit(_input[_position]))
            {
                result += "-ЦИФРА";
                _position++;
            }

            if (result == "-ЧИСЛО")
            {
                result = "-ОШИБКА: Ожидалось число";
            }

            return result;
        }

        private string Sign()
        {
            if (_position >= _input.Length || !IsOperator(_input[_position]))
                return "-ОШИБКА: Ожидался оператор";

            var temp = _input[_position++].ToString();
            return temp == "-" ? "–" : temp;
        }

        private static bool IsOperator(char c) => 
            c is '+' or '-' or '*' or '/';
    }
}
