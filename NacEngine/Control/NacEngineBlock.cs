using ExpressionEvaluator;
using Nac.Common;
using Nac.Common.Control;
using System;
using System.Linq;
using System.Runtime.Serialization;
using static NacEngineGlobal;
using Qualities = Nac.Common.Control.NacExecutionQuality;

namespace Nac.Engine.Control {
    [DataContract]
    public class NacEngineBlock : NacEngineObject {
        [DataMember]
        public new NacBlock Base { get { return base.Base as NacBlock; } set { base.Base = value; } }
        public NacEngineBlock(NacBlock block) : base(block) { }
        public NacExecutionStatus Status { get { return Base.Status; } set { Base.Status = value; } }
        public Qualities Quality { get { return Base.Quality; } set { Base.Quality = value; } }
        public bool Scheduled { get { return Base.Scheduled; } set { Base.Scheduled = value; } }
        public TimeSpan Countdown { get { return Base.Countdown; } set { Base.Countdown = value; } }
        public NacEngineRuntime Runtime { get { return G.Runtime(Path); } }
        public NacEngineDatabase Database { get { return G.Database(Path); } }

        //protected NacEngineDatabase Database { get { return G.Database(Path); } }
        protected virtual CompiledExpressionType ExpressionType { get { return CompiledExpressionType.StatementList; } }

        protected virtual bool CanExecute() {
            return Project.Run;
        }

        private CompiledExpression _exp;
        private CompiledExpression Exp {
            get {
                if (_exp == null) {
                    TypeRegistry typeRegistry = new TypeRegistry();
                    typeRegistry.RegisterType<DateTime>();
                    typeRegistry.RegisterType<TimeSpan>();
                    typeRegistry.RegisterSymbol("Math", new NacMath());
                    typeRegistry.RegisterSymbol("Block", this);
                    _exp = new CompiledExpression() { ExpressionType = ExpressionType, TypeRegistry = typeRegistry };
                }
                return _exp;
            }
        }

        public string Code { get { return Base.Code; } }

        private Delegate DynFunction;

        public virtual bool Execute() {
            if (!Scheduled || !CanExecute()) {
                //Quality = Qualities.Unknown;
                return true;
            }

            if (Code == null) { Result = null; Quality = Qualities.Empty; }
            else {
                if (Code != Exp.StringToParse || Quality == Qualities.Unknown) {
                    _exp = null;
                    if (Code != Exp.StringToParse) Exp.StringToParse = Code;
                    try { DynFunction = Compile(Exp); /*Exp.ScopeCompile();*/ }
                    catch (Exception x) when (G.Log(x, Exp)) { DynFunction = null; Quality = Qualities.BadCompilation; }
                }

                if (DynFunction != null) {
                    try {
                        Result = DynFunction.DynamicInvoke(Runtime);
                        Quality = Qualities.Good;
                    }
                    catch (Exception x) when (G.Log(x, DynFunction)) { Quality = Qualities.BadExecution; }
                }
            }
            return Quality == Qualities.Good;
        }

        protected virtual Delegate Compile(CompiledExpression expression) {
            var mi =
                expression.GetType()
                .GetMethods()
                .Where(
                    m => m.Name == "ScopeCompile"
                    && m.GetGenericArguments().Length == 1)
                .First();

            Type runtimeType = Runtime.GetType();
            var gen = mi.MakeGenericMethod(runtimeType);
            var compExpr = gen.Invoke(Exp, null);
            return compExpr as Delegate;
        }

        //public virtual dynamic Result { get; set; }
        public virtual object Result { get; set; }
    }
}
