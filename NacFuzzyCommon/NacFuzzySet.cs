using Accord.Fuzzy;
using Nac.Common.Fuzzy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Fuzzy.Common {
    public static class NacFuzzySet {
        private enum FuzzySetKind { Unknown, Trapezoidal, Triangular, LeftEdged, RightEdged };

        public static FuzzySet GetFuzzySet(this Nac.Common.Fuzzy.NacFuzzySet fuzzySet) {
            if (fuzzySet.MembershipFunction is NacTrapezoidalMembershipFunction) {
                var kind = FuzzySetKind.Unknown;
                var mf = fuzzySet.MembershipFunction as NacTrapezoidalMembershipFunction;

                if (mf.edge < 0) kind = FuzzySetKind.LeftEdged;
                else
                if (mf.edge > 0) kind = FuzzySetKind.RightEdged;
                else
                if (mf.m2 == mf.m3) kind = FuzzySetKind.Triangular;
                else kind = FuzzySetKind.Trapezoidal;

                IMembershipFunction mFun = null;

                switch (kind) {
                    case FuzzySetKind.LeftEdged:
                        mFun = new TrapezoidalFunction(mf.m1, mf.m2, mf.max, mf.min, TrapezoidalFunction.EdgeType.Left);
                        break;
                    case FuzzySetKind.RightEdged:
                        mFun = new TrapezoidalFunction(mf.m1, mf.m2, mf.max, mf.min, TrapezoidalFunction.EdgeType.Right);
                        break;
                    case FuzzySetKind.Triangular:
                        mFun = new TrapezoidalFunction(mf.m1, mf.m2, mf.m4, mf.max, mf.min);
                        break;
                    case FuzzySetKind.Trapezoidal:
                        mFun = new TrapezoidalFunction(mf.m1, mf.m2, mf.m3, mf.m4, mf.max, mf.min);
                        break;
                }
                if (mFun != null) return new FuzzySet(fuzzySet.Name, mFun);
            }
            return null;
        }
    }
}
