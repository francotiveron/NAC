using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExpressionEvaluator;
using System.Dynamic;

namespace ExprEvalTest {
    class Program {
        class Scope {
            public int a;
            public void Work() {  a += 5; }
        }
        static void Main(string[] args) {
            MyTest2();
            //MyTest1();
            //Dynamictest();

        }

        private static void MyTest2() {
            RTDB rtdb = new RTDB();
            rtdb.Add(new Type1() { Val = 1 }, "obj1");
            rtdb.Add(new Type1() { Val = 2 }, "obj2");
            rtdb.Add(new Type1() { Val = 3 }, "obj3");
            rtdb.Add(new Type1() { Val = 4 }, "obj4");

            var expression = @"obj2 + obj4";
            var exp = new CompiledExpression(expression);
            var func = exp.ScopeCompile<RTDB>();
            int b = (int)func(rtdb);
        }

        static void MyTest1() {
            Scope scope = new Scope() { a = 1 };
            var expression = @"Work(); a+2;";
            //var expression = @"a + 2";
            var exp = new CompiledExpression<int>(expression) { TypeRegistry = new TypeRegistry() };
            exp.ExpressionType = CompiledExpressionType.StatementList;
            var func = exp.ScopeCompile<Scope>();
            int b = func(scope);
            b = func(scope);
        }
        public static void Dynamictest() {
            var x = "test";
            var flag = x.Contains("te");

            var expression = "var x = \"test\";\n\n";
            expression = expression + "if(null!=x) x= \"rest\"; ";
            var ce1 = new CompiledExpression() { StringToParse = expression, ExpressionType = CompiledExpressionType.StatementList };
            var result = ce1.Eval();
        }
    }

    class Type1 {
        public int Val{ get; set; }
    }
    class RTDB : DynamicObject {
        private Dictionary<string, Type1> _data;
        public RTDB() {
            _data = new Dictionary<string, Type1>();
        }
        public Type1 this[string id] {
            get {
                return _data[id];
            }
            set {
                _data[id] = value;
            }
        }
        public void Add(Type1 element, string id) {
            _data.Add(id, element);
        }
        #region Dynamic
        public override bool TryGetMember(GetMemberBinder binder, out object result) {
            Type1 v = _data[binder.Name];
            result = v.Val;
            return true;
        }
        public override bool TrySetMember(SetMemberBinder binder, object value) {
            return true;
        }
        #endregion
    }
}
