using GraphQL.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarvedRock.Api.GraphQL.Types
{
    public class ProductReviewInputType : InputObjectGraphType
    {
        public ProductReviewInputType() //vytvorenie input type
        {
            Name = "reviewInput";
            Field<NonNullGraphType<StringGraphType>>("title");
            Field<StringGraphType>("review");
            Field<NonNullGraphType<IdGraphType>>("productId");
        }
    }
}
