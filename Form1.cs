using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Button = System.Windows.Forms.Button;


namespace calculator {
    public partial class Form1 : Form {
        private bool calculationDone = false;
        bool error = false;
        string memory = "";
        public Form1() {
            InitializeComponent();
            result.Text = "0";
            result.TextChanged += new EventHandler(Result_TextChanged);
            this.KeyPreview = true; // 允許表單偵測按鍵
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }
        private void Result_TextChanged(object sender, EventArgs e) {
            // 文字變更後，將光標移動到最後一個字元
            result.SelectionStart = result.Text.Length;
            result.SelectionLength = 0;  // 確保沒有選取文字
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            //deal with parentheses
            if (e.Shift && e.KeyCode == Keys.D9) {
                AppendToResult("(");  // 左括號
                e.SuppressKeyPress = true;  // 禁止輸入9
            }
            else if (e.Shift && e.KeyCode == Keys.D0) {
                AppendToResult(")");  // 右括號
                e.SuppressKeyPress = true;  // 禁止輸入0
            }
            else if (e.Shift && e.KeyCode == Keys.D6) {
                if (Isop(result.Text[result.Text.Length - 1])) { }
                else
                    AppendToResult("^");
                e.SuppressKeyPress = true;
            }
            else if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)) {
                string keyChar = (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) ? (e.KeyCode - Keys.D0).ToString() : (e.KeyCode - Keys.NumPad0).ToString();
                if (calculationDone) {
                    result.Text = keyChar;  // 新數字覆蓋結果
                    calculationDone = false;  // 重置運算
                    error = false;
                }
                else if (error) {
                    result.Clear();  // 清除錯誤訊息
                    result.Text = keyChar;  // 顯示新數字
                    error = false;  // 重置狀態
                }
                else 
                    AppendToResult(keyChar);
                
                e.SuppressKeyPress = true;
            }
            // 處理等號鍵
            else if (e.KeyCode == Keys.Enter) {
                CalculateResult(result.Text);
                e.SuppressKeyPress = true;  // 防止預設按鍵行為
            }
            else if (e.KeyCode == Keys.Back) {
                DeleteLastCharacter();
                e.SuppressKeyPress = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void Number_click(object sender, EventArgs e) {
            Button btn = sender as Button;
            if (calculationDone) {
                result.Text = btn.Text;  // 新的數字覆蓋結果
                calculationDone = false;  // 重置運算狀態
                result.Focus();
            }
            else if (error) {
                result.Clear();
                result.Text = btn.Text;
                error = false;
                result.Focus();
            }
            else {
                AppendToResult(btn.Text);
                result.Focus();
            }
        }
        private void AppendToResult(string text) {
            if (result.Text == "0")
                result.Clear();
            if (error) {
                result.Text = text;
                calculationDone = false;
                error = false;
            }
            if (calculationDone) {
                result.Text = text;  // 如果完成運算，覆蓋顯示框
                calculationDone = false;  // 重置運算完成狀態
            }
            else 
                result.Text += text;
        }
        private void Operation_click(object sender, EventArgs e) {
            Button btn = sender as Button;
            if (calculationDone)
                calculationDone = false;  // 如果運算剛結束，重置運算狀態
            if (Isop(result.Text[result.Text.Length - 1])) { }
            else
                AppendToResult(btn.Text);  // 繼續添加運算符
            result.Focus();
        }
        private void Equal_click(object sender, EventArgs e) {
            CalculateResult(result.Text);
            result.Focus();
        }
        private void Dot_click(object sender, EventArgs e) {
            AppendToResult(".");
            result.Focus();
        }
        private void LeftParen_click(object sender, EventArgs e) {
            AppendToResult("(");
            result.Focus();
        }
        private void RightParen_click(object sender, EventArgs e) {
            AppendToResult(")");
            result.Focus();
        }
        // 刪除單個字元
        private void Delete_click(object sender, EventArgs e) {
            DeleteLastCharacter();
            result.Focus();
        }
        // 刪除最後一個字元
        private void DeleteLastCharacter() {
            //del 1 digit if can
            if (result.Text.Length > 1)
                result.Text = result.Text.Remove(result.Text.Length - 1, 1);
            else if (result.Text.Length > 0)
                result.Text = "0";
        }
        // 刪除全部
        private void Clear_click(object sender, EventArgs e) {
            result.Clear();  // 清除顯示框
            result.Text = "0";  // 重置顯示為0
            lastResultLabel.Text = "";  // 清空顯示最後結果的 Label
            calculationDone = false;  // 重置運算狀態
            result.Focus();
        }
        //check expression
        private bool ValidateExpression(string expression) {
            // 檢查括號匹配、小數點是否合法
            int openParen = 0;
            bool hasDecimal = false;

            foreach (char c in expression) {
                if (char.IsDigit(c)) { }
                
                else if (c == '(') 
                    openParen++;
                else if (c == ')') {
                    openParen--;
                    if (openParen < 0) return false;
                }
                else if (c == '.' && !hasDecimal) 
                    hasDecimal = true;
                else if ("+-*/^".Contains(c)) 
                    hasDecimal = false;
                else 
                    return false; // 無效字符
            }
            return openParen == 0; // 括號匹配
        }
        private void CalculateResult(string expression) {
            try {
                if (ValidateExpression(expression)) {
                    var answer = Evaluatevalue();
                    result.Text = answer.ToString();
                    memory = result.Text; // 記憶本次結果
                    lastResultLabel.Text = result.Text;
                }
                else {
                    result.Text = "Error";
                    error = true;
                }
                calculationDone = false;  // 設置為運算完成狀態
            }
            catch (Exception) {
                result.Text = "Error";
                error = true;
            }
        }
        private double Evaluatevalue() { 
            Stack<double> operands = new Stack<double>();
            Stack<char> operators = new Stack<char>();
            int i = 0;
            while (i < result.Text.Length) {
                if (char.IsWhiteSpace(result.Text[i])) {
                    i++;
                    continue;
                }
                if (char.IsDigit(result.Text[i])) {
                    string s = "";
                    while (i < result.Text.Length && (char.IsDigit(result.Text[i]) || result.Text[i] == '.'))
                        s += result.Text[i++];

                    operands.Push(double.Parse(s));
                }
                else if (Isop(result.Text[i])) {
                    if (result.Text[i] == '-') {
                        //first char is '-' or previous char is operator is minus, considered as number
                        if (result.Text[i] == '-' && (i == 0 || result.Text[i-1] == '(' || Isop(result.Text[i-1]))) {
                            i++;
                            string s = "-";
                            while (i < result.Text.Length && (char.IsDigit(result.Text[i]) || result.Text[i] == '.'))
                                s += result.Text[i++];
                            operands.Push(double.Parse(s));
                        }
                        //else is substract, considered as operator
                        else {
                            while (operators.Count > 0 && Priority(operators.Peek()) >= Priority(result.Text[i])) 
                                operands.Push(Simplify(operands.Pop(), operands.Pop(), operators.Pop()));
                            operators.Push(result.Text[i++]);
                        }
                    }
                    else {//+,*,/,^
                        while (operators.Count > 0 && Priority(operators.Peek()) >= Priority(result.Text[i]))
                            operands.Push(Simplify(operands.Pop(), operands.Pop(), operators.Pop()));
                        operators.Push(result.Text[i++]);
                    }
                }
                else if (result.Text[i] =='(') 
                    operators.Push(result.Text[i++]);

                else if (result.Text[i] == ')') {
                    while(operators.Peek() != '(') 
                        operands.Push(Simplify(operands.Pop(),operands.Pop(),operators.Pop()));
                    
                    operators.Pop();
                    i++;
                }
            }
            while (operators.Count > 0)
                operands.Push(Simplify(operands.Pop(), operands.Pop(), operators.Pop()));

            return operands.Pop();
        }
        private int Priority(char c) {
            switch(c) {
                case '+':
                case '-':
                    return 1;
                case '*':
                case '/':
                    return 2;
                case '^':
                    return 3;
                default:
                    return 0;
            }
        }
        private double Simplify(double a,double b,char oper) {
            switch (oper) {
                case '+':
                    return b + a;
                case '-':
                    return b - a;
                case '*':
                    return b * a;
                case '/':
                    return b / a;
                case '^':
                    return Math.Pow(b, a);
                default:
                    return 0;
            }
        }
        private bool Isop(char c) {
            return c == '+' || c == '-' || c == '*' || c == '/' || c == '^';
        }
        private void Memory_click(object sender, EventArgs e) {
            AppendToResult(memory);
            result.Focus();
        }

        private void Power_click(object sender, EventArgs e) {
            AppendToResult("^");
            result.Focus();
        }
    }
}