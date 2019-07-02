using Moonmile.Redmine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Diagnostics;

namespace RmClient.Lib
{
    public class RedmineEntities
    {
        public RedmineEntities()
        {

            var provider = new RedmineQueryProvider();
            this.Projects = new Query<Project>(provider);
            this.Issues = new Query<Issue>(provider);
        }
        public virtual Query<Project> Projects { get; set; }
        public virtual Query<Issue> Issues { get; set; }
    }

    public static class RedmineQueryProviderExtensions
    {
        public static void Add<T>( this Query<T> q, T item )
        {
            ((RedmineQueryProvider)q.Provider).AddItem(item);
        }
        public static void Remove<T>(this Query<T> q, T item)
        {
            ((RedmineQueryProvider)q.Provider).RemoveItem(item);
        }
        public static void Update<T>(this Query<T> q, T item)
        {
            ((RedmineQueryProvider)q.Provider).UpdateItem(item);
        }
    }



    public class RedmineQueryProvider : QueryProvider
    {
        /// <summary>
        /// データ検索
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public override object Execute(Expression expression)
        {
            Debug.WriteLine("called RedmineQueryProvider::Execute");

            if ( expression.Type.GetGenericArguments().Count() == 1 ) {
                var t = expression.Type.GetGenericArguments()[0];
                if ( t == typeof(Project))
                    return new List<Project>();
                if (t == typeof(Issue))
                    return new List<Issue>();
            }

            return null;
        }

        /// <summary>
        /// データの追加
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void AddItem<T>( T item ) 
        {
            if ( item.GetType() == typeof(Project))
            {

            }
        }
        public void RemoveItem<T>(T item)
        {

        }
        public void UpdateItem<T>(T item)
        {

        }


        public override string GetQueryText(Expression expression)
        {
            Debug.WriteLine("called RedmineQueryProvider::GetQueryText");
            return "";
        }

        public static IQueryable<T> CreateQueryable<T>()
        {
            return new Query<T>(new RedmineQueryProvider());
        }
    }



    public class Query<T> :
        IQueryable<T>, IQueryable,
        IEnumerable<T>, IEnumerable,
        IOrderedQueryable<T>, IOrderedQueryable
    {
        QueryProvider provider;
        Expression expression;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="expression"></param>
        public Query(QueryProvider provider, Expression expression = null )
        {
            if (expression == null)
            {
                expression = Expression.Constant(this);
            }
            this.provider = provider;
            this.expression = expression;
        }

        public Type ElementType => typeof(T);
        public Expression Expression => this.expression;
        public IQueryProvider Provider => this.provider;

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)this.provider.Execute(this.expression)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.provider.Execute(this.expression)).GetEnumerator();
        }

        public override string ToString()
        {
            return this.provider.GetQueryText(this.expression);
        }
    }
    public abstract class QueryProvider : IQueryProvider
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        protected QueryProvider()
        {

        }


        public IQueryable CreateQuery(Expression expression)
        {
            Type elementType = TypeSystem.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new Query<TElement>(this, expression);
        }

        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {
            return (TResult)this.Execute(expression);
        }

        public abstract string GetQueryText(Expression expression);
        public abstract object Execute(Expression expression);

    }

    /// <summary>
    /// ヘルパー関数
    /// </summary>
    internal static class TypeSystem
    {
        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) return seqType;
            return ienum.GetGenericArguments()[0];
        }
        private static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
                return null;
            if (seqType.IsArray)
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }
            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces != null && ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null) return ienum;
                }
            }
            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }
            return null;
        }
    }

}
