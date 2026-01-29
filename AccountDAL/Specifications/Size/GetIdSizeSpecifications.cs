using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.Size
{
    public class GetIdSizeSpecifications: BaseSpecification<DifferentSize>
    {
        public GetIdSizeSpecifications(string label, SizeType type):base(s => s.Label == label /*&& s.Type == type*/)
        {

        }
        public GetIdSizeSpecifications(decimal width, decimal height, decimal depth) : base
           (
            //s => s.Type == SizeType.Dimension &&
            //        s.Dimensions != null &&
            //        s.Dimensions.Width == width &&
            //        s.Dimensions.Height == height &&
            //        s.Dimensions.Depth == depth
            )
        {
            AddInclude(s => s.Dimensions);
        }
        public GetIdSizeSpecifications(int id):base(s => s.Id == id) 
        {
            AddInclude(s => s.Dimensions);
        }
    }
}
