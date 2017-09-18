using Nac.Common.Control;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nac.Wpf.Common.Control {
    public class NacWpfBlockCall : NacWpfBlockSeq {
        public NacWpfBlockCall(NacBlockCall nacBlockFuzzy) : base(nacBlockFuzzy) { }
        public new NacBlockCall Base { get { return base.Base as NacBlockCall; } }

        [DisplayName("Subroutine")]
        public new string Code { get { return Base.Code; } set { Base.Code = value; } }

    }
}
