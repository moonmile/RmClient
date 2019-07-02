using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using System.Linq;
using Moonmile.Redmine.Model;

namespace RmClient.Lib.Test.Consolex
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var ent = new RedmineEntities();
            var items =
                from t in ent.Projects
                where t.Identifier == "projname"
                where t.IsPublic == true
                select t;
            /*
            var items = ent.Projects
                .Where(t => t.Identifier == "projname" && t.IsPublic == true);
                */
            items.Expression.Dump();


            var items2 =
                from p in ent.Projects
                join i in ent.Issues on p.Id equals i.Project.Id
                where p.IsPublic == true && p.Identifier == "project_name"
                where i.Priority.Name == "高め"
                select i;

            items2.Expression.Dump();

            var lst = items2.ToList();



            var issue = new Issue();

            ent.Issues.Add(issue);
            ent.Issues.Update(issue);


            
            Console.ReadKey();



        }
    }

    static class ExpressionExtentions
    {
        public static void Dump( this Expression e, int indent = 0 )
        {
            var space = new string(' ', indent*2);
            Console.WriteLine(space + $"NodeType: {e.NodeType} {e.GetType().Name}");
            if ( e is MethodCallExpression )
            {
                var ee = (MethodCallExpression)e;
                Console.WriteLine(space + $" MethodCallExpression");
                foreach ( var it in ee.Arguments )
                {
                    it.Dump(indent+1);
                }
            }
            else if ( e is UnaryExpression )
            {
                var ee = (UnaryExpression)e;
                Console.WriteLine(space + $" UnaryExpression");
                ee.Operand.Dump(indent + 1);
            }
            else if ( e is LambdaExpression )
            {
                var ee = (LambdaExpression)e;
                Console.WriteLine(space + $" LambdaExpression");
                ee.Body.Dump(indent + 1);
            }
            else if (e is BinaryExpression)
            {
                var ee = (BinaryExpression)e;
                Console.WriteLine(space + $" BinaryExpression");
                ee.Left.Dump(indent + 1);
                ee.Right.Dump(indent + 1);
            }
            else if ( e is MemberExpression )
            {
                var ee = (MemberExpression)e;
                Console.WriteLine(space + $" MemberExpression {ee.Expression.Type.Name} \"{ee.Member.Name}\"");
            }
            else if ( e is ConstantExpression )
            {
                var ee = (ConstantExpression)e;
                Console.WriteLine(space + $" ConstantExpression \"{ee.Value}\"");
            }
            else
            {
                Console.WriteLine(space +  $" other");
            }
        }
    }

    /*
NodeType: Call MethodCallExpression2
 MethodCallExpression
 NodeType: Constant ConstantExpression
  ConstantExpression
 NodeType: Quote UnaryExpression
  UnaryExpression
  NodeType: Lambda Expression1`1
   LambdaExpression
   NodeType: AndAlso LogicalBinaryExpression
    BinaryExpression
    NodeType: Equal MethodBinaryExpression
     BinaryExpression
     NodeType: MemberAccess PropertyExpression
      MemberExpression Identifier
     NodeType: Constant ConstantExpression
      ConstantExpression projname
    NodeType: Equal LogicalBinaryExpression
     BinaryExpression
     NodeType: MemberAccess PropertyExpression
      MemberExpression IsPublic
     NodeType: Constant ConstantExpression
      ConstantExpression True
    
*/


    static class EnumerableExtensions
    {
        // コレクションの各要素に対して、指定された処理をインデックス付きで実行
        public static void ForEach<TItem>(this IEnumerable<TItem> collection, Action<TItem, int> action)
        {
            int index = 0;
            foreach (var item in collection)
                action(item, index++);
        }
    }

    // Expression の中身をダンプ (ラムダ式、二項演算式、単項演算式以外は簡易表示)
    static class ExpressionViewer
    {
        // Expression の中を (再帰的に) 表示
        public static void Show(this Expression expression, int level = 0)
        {
            if (expression as LambdaExpression != null)
                // ラムダ式のときは詳細に表示
                ShowLambdaExpression((LambdaExpression)expression, level);
            else if (expression as BinaryExpression != null)
                // 二項演算のときは詳細に表示
                ShowBinaryExpression((BinaryExpression)expression, level);
            else if (expression as UnaryExpression != null)
                // 単項演算のときは詳細に表示
                ShowUnaryExpression((UnaryExpression)expression, level);
            else if (expression != null)
                // それ以外も沢山あるが、今回は省略してベース部分だけ表示
                ShowExpressionBase(expression, level);
        }

        // Expression のベース部分を表示
        static void ShowExpressionBase(Expression expression, int level)
        {
            ShowText(string.Format("☆Expression: {0}", expression), level);
            ShowText(string.Format("ノードタイプ: {0}", expression.NodeType), level + 1);
        }

        // LambdaExpression (ラムダ式) の中を (再帰的に) 表示
        static void ShowLambdaExpression(LambdaExpression expression, int level)
        {
            ShowExpressionBase(expression, level);
            ShowText(string.Format("名前: {0}", expression.Name), level + 1);
            ShowText(string.Format("戻り値の型: {0}", expression.ReturnType), level + 1);
            ShowParameterExpressions(expression.Parameters, level + 1); // 引数のコレクション
            ShowText(string.Format("本体: {0}", expression.Body), level + 1);
            expression.Body.Show(level + 2); // 本体を再帰的に表示
        }

        // BinaryExpression (二項演算式) の中を (再帰的に) 表示
        static void ShowBinaryExpression(BinaryExpression expression, int level)
        {
            ShowExpressionBase(expression, level);
            ShowText(string.Format("型: {0}", expression.Type), level + 1);
            ShowText(string.Format("左オペランド: {0}", expression.Left), level + 1);
            expression.Left.Show(level + 2); // 左オペランドを再帰的に表示
            ShowText(string.Format("右オペランド: {0}", expression.Right), level + 1);
            expression.Right.Show(level + 2); // 右オペランドを再帰的に表示
        }

        // UnaryExpression (単項演算式) の中を (再帰的に) 表示
        static void ShowUnaryExpression(UnaryExpression expression, int level)
        {
            ShowExpressionBase(expression, level);
            ShowText(string.Format("型: {0}", expression.Type), level + 1);
            ShowText(string.Format("オペランド: {0}", expression.Operand), level + 1);
            expression.Operand.Show(level + 2); // オペランドを再帰的に表示
        }

        // 引数の式のコレクションを表示
        static void ShowParameterExpressions(ReadOnlyCollection<ParameterExpression> parameterExpressions, int level)
        {
            ShowText("引数群", level);
            if (parameterExpressions == null || parameterExpressions.Count == 0)
                ShowText("引数なし", level);
            else
                parameterExpressions.ForEach((parameterExpression, index) => ShowParameterExpression(parameterExpression, index, level + 1));
        }

        // 引数の式の中を表示
        static void ShowParameterExpression(ParameterExpression parameterExpression, int index, int level)
        {
            ShowText(string.Format("引数{0}", index + 1), level + 1);
            ShowExpressionBase(parameterExpression, level + 1);
            ShowText(string.Format("引数の型: {1}, 引数の名前: {2}", parameterExpression.NodeType, parameterExpression.Type, parameterExpression.Name), level + 2);
        }

        // 文字列をレベルに応じてインデント付で表示
        static void ShowText(string itemText, int level)
        {
            Console.WriteLine("{0}{1}", Indent(level), itemText);
        }

        // インデントの為の文字列を生成
        static string Indent(int level)
        {
            return level == 0 ? "" : new string(' ', (level - 1) * 4 + 1) + "|-- ";
        }
    }
}


