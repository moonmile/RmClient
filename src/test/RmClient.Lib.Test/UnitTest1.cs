using System;
using Xunit;
using System.Linq;


namespace RmClient.Lib.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var ent = new RedmineEntities();
            var items = 
                from t in ent.Projects
                where t.Identifier == "projname"
                where t.IsPublic == true
                select t ;
            var lst = items.ToList();

            Assert.NotNull(lst);
            Assert.Empty(lst);

        }
    }
}
