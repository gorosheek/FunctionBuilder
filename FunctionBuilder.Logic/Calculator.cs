using System;
using System.Collections.Generic;
using System.Text;

namespace FunctionBuilder.Logic
{
    public class Calculator
    {
        private object[] RPNobj { get; set; }
        public double X { get; private set; }
        public double Y { get; private set; }
        public Calculator(string expression, double x)
        {
            X = x;
            RPNobj = GetRPN(expression.Replace("x", X.ToString()));
            Y = CalculateRPN();
        }
        private object[] GetRPN(string exp)
        {
            List<object> expression = ParseExpression(exp);
            return ParseToRPN(expression);
        }
        private List<object> ParseExpression(string expression)
        {
            List<object> objExpression = new List<object>();
            for (int i = 0; i < expression.Length; i++) 
            {
                if (CheckDigit(expression[i].ToString()) || 
                    "-".Contains(expression[i]) && CheckDigit(expression[i + 1].ToString()))
                {
                    StringBuilder strBuilder = new StringBuilder().Append(expression[i]);
                    while (CheckDigit(expression[i + 1].ToString()) || expression[i + 1] == ',')
                    {
                        i++;
                        strBuilder.Append(expression[i]);
                    }
                    objExpression.Add(double.Parse(strBuilder.ToString()));
                }
                else if (CheckBrakets(expression[i].ToString()) || CheckStartAndEnd(expression[i].ToString()))
                {
                    objExpression.Add(expression[i]);
                }
                else if ("+-*/^".Contains(expression[i])) 
                {
                    Operations operation = ChooseOperation(expression[i]);
                    objExpression.Add(operation);
                }
                else
                    continue;
            }
            return objExpression;
        }
        private bool CheckDigit(string param)
        {
            if (double.TryParse(param, out _))
                return true;
            else
                return false;
        }
        private bool CheckBrakets(string param)
        {
            if (param == "(" || param == ")")
                return true;
            else
                return false;
        }
        private bool CheckStartAndEnd(string param)
        {
            if (param == "|")
                return true;
            else
                return false;
        }
        private Operations ChooseOperation(object operation)
        {
            Operations op = null;
            switch (operation.ToString())
            {
                case ("+"):
                    op = new Plus();
                    break;
                case ("-"):
                    op = new Minus();
                    break;
                case ("*"):
                    op = new Mult();
                    break;
                case ("/"):
                    op = new Dev();
                    break;
                case ("^"):
                    op = new Degree();
                    break;
            }
            return op;
        }
        private object[] ParseToRPN(List<object> expression)
        {
            Stack<object> outputStr = new Stack<object>();
            Stack<object> stack = new Stack<object>();
            int i = 0;
            while (true)
            {
                if (i == 0)
                {
                    stack.Push(expression[i]);
                    i++;
                }
                else if (expression[i] is double)
                {
                    outputStr.Push(expression[i]);
                    i++;
                }
                else if (expression[i] is Operations)
                {
                    switch ((expression[i] as Operations).Priority)
                    {
                        case (1):
                            stack.Push(expression[i]);
                            i++;
                            break;
                        case (2):
                            if (!(stack.Peek() is Operations) || (stack.Peek() as Operations).Priority == 3)
                            {
                                stack.Push(expression[i]);
                                i++;
                            }
                            else
                            {
                                outputStr.Push(stack.Pop());
                            }
                            break;
                        case (3):
                            if (stack.Peek() is Operations)
                            {
                                outputStr.Push(stack.Pop());
                            }
                            else
                            {
                                stack.Push(expression[i]);
                                i++;
                            }
                            break;
                    }
                }

                else if (expression[i].ToString() == "(")
                {
                    stack.Push(expression[i]);
                    i++;
                }
                else if (expression[i].ToString() == ")")
                {
                    if (stack.Peek().ToString() != "(")
                    {
                        outputStr.Push(stack.Pop());
                    }
                    else
                    {
                        stack.Pop();
                        i++;
                    }
                }
                else if (CheckStartAndEnd(expression[i].ToString()) && (stack.Count != 0))
                {
                    outputStr.Push(stack.Pop());
                }
                else
                {
                    break;
                }

            }
            return outputStr.ToArray();
        }
        private double CalculateRPN()
        {
            Stack<double> answer = new Stack<double>();
            for (int i = RPNobj.Length - 1; i >= 0; i--)
            {
                if (RPNobj[i] is double)
                {
                    answer.Push(Convert.ToDouble(RPNobj[i]));
                }
                else if (RPNobj[i] is Operations)
                {
                    double[] nums = { answer.Pop(), answer.Pop() };
                    answer.Push((RPNobj[i] as Operations).Calculate(nums));
                }
            }
            return answer.Pop();
        }
    }
}
