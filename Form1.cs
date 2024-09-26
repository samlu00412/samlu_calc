using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NCalc;
using Expression = NCalc.Expression;


namespace calculator {
    public partial class Form1 : Form {
        private bool calculationDone = false;
        bool error = false;
        public Form1() {
            InitializeComponent();
            result.Text = "0";
            result.TextChanged += new EventHandler(result_TextChanged);
            this.KeyPreview = true; // 允許表單偵測按鍵
            this.KeyDown += new KeyEventHandler(Form1_KeyDown);
        }
        private void result_TextChanged(object sender, EventArgs e) {
            // 每次文字變更後，將光標移動到最後一個字元
            result.SelectionStart = result.Text.Length;
            result.SelectionLength = 0;  // 確保沒有選取任何文字
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            //deal with parentheses
            if (e.Shift && e.KeyCode == Keys.D9) {
                AppendToResult("(");  // 左括號
                e.SuppressKeyPress = true;  // 禁止輸入數字9
            }
            else if (e.Shift && e.KeyCode == Keys.D0) {
                AppendToResult(")");  // 右括號
                e.SuppressKeyPress = true;  // 禁止輸入數字0
            }
            else if ((e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)) {
                string keyChar = (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9)? (e.KeyCode - Keys.D0).ToString() : (e.KeyCode - Keys.NumPad0).ToString();
                if (calculationDone) {
                    result.Text = keyChar;  // 新的數字覆蓋結果
                    calculationDone = false;  // 重置運算狀態
                    error = false;
                }
                else if (error) {
                    result.Clear();  // 清除錯誤訊息
                    result.Text = keyChar;  // 顯示新的數字
                    error = false;  // 重置錯誤狀態
                }
                else {
                    AppendToResult(keyChar);  // 否則繼續添加數字
                }
                e.SuppressKeyPress = true;  // 防止預設的按鍵行為
            }
            // 處理等號鍵
            else if (e.KeyCode == Keys.Enter) {
                CalculateResult(result.Text);
                e.SuppressKeyPress = true;  // 防止預設的按鍵行為
            }
            else if(e.KeyCode == Keys.Back) {
                DeleteLastCharacter();
                e.SuppressKeyPress= true;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {

        }

        private void Number_click(object sender, EventArgs e) {
            Button btn = sender as Button;
            if (calculationDone) {
                result.Text = btn.Text;  // 新的數字覆蓋結果
                calculationDone = false;  // 重置運算狀態
            }
            else if (error) {
                result.Clear();
                result.Text = btn.Text;
                error = false;
            }
            else
                AppendToResult(btn.Text);  // 否則繼續添加數字
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
                result.Text = text;  // 如果剛完成運算，覆蓋顯示框
                calculationDone = false;  // 重置運算完成狀態
            }
            else
                result.Text += text;  // 否則繼續追加
        }
        private void Operation_click(object sender, EventArgs e) {
            Button btn = sender as Button;
            if (calculationDone)
                calculationDone = false;  // 如果運算剛結束，重置運算狀態

            AppendToResult(" " + btn.Text + " ");  // 繼續添加運算符
        }
        private void Equal_click(object sender, EventArgs e) {
            CalculateResult(result.Text);
        }
        
        // 小數點按鈕點擊
        private void Dot_click(object sender, EventArgs e) {
            AppendToResult(".");
        }

        // 左括號按鈕點擊
        private void LeftParen_click(object sender, EventArgs e) {
            AppendToResult("(");
        }
        private void RightParen_click(object sender, EventArgs e) {
            AppendToResult(")");
        }

        // 刪除單個字元
        private void Delete_click(object sender, EventArgs e) {
            DeleteLastCharacter();
        }

        // 刪除最後一個字元的功能
        private void DeleteLastCharacter() {
            //del 1 digit if can
            if (result.Text.Length > 1)
                result.Text = result.Text.Remove(result.Text.Length - 1, 1);
            else if (result.Text.Length > 0)
                result.Text = "0";
        }
        // 刪除全部內容
        private void Clear_click(object sender, EventArgs e) {
            result.Clear();  // 清除顯示框
            result.Text = "0";  // 重置顯示為0
            lastResultLabel.Text = "";  // 清空顯示最後結果的 Label
            calculationDone = false;  // 重置運算狀態
        }
        // 計算表達式結果
        private void CalculateResult(string expression) {
            try {
                Expression ncalcExpression = new Expression(expression);
                object answer = Convert.ToInt64(ncalcExpression.Evaluate());
                double temp = Convert.ToDouble(answer);
                temp = Math.Round(temp, 5);
                // 顯示結果
                result.Text = temp.ToString();
                lastResultLabel.Text = "ans: " + temp.ToString();
                calculationDone = false;  // 設置為運算完成狀態
            }
            catch (Exception) {
                calculationDone = true;
                result.Text = "Error";
                error = true;
            }
        }
    }
}
