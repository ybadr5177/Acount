using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.Size
{
    public class SizeGetByTypeSpecifications: BaseSpecification<DifferentSize>
    {
        public SizeGetByTypeSpecifications(SizeType Types) : base(/*DST => DST.Type == Types*/)
        {
            AddInclude(Ds => Ds.Dimensions);

        }
        public SizeGetByTypeSpecifications()
        {
            AddInclude(Ds => Ds.Dimensions);
        }
    }
}
