using CarvedRock.Api.GraphQL.Types;
using CarvedRock.Api.Repositories;
using GraphQL;
using GraphQL.Types;

namespace CarvedRock.Api.GraphQL
{
    public class CarvedRockQuery: ObjectGraphType
    {
        public CarvedRockQuery(ProductRepository productRepository, ProductReviewRepository reviewRepository)
        {
            Field<ListGraphType<ProductType>>(
                "products", //nazov fieldu , vyuzitelne pri definovany query
                resolve: context => productRepository.GetAll() //definuje ako maju byt data resolvovane --> GetAll() vracia Task ktory nie je potrebne await, riesenie GraphQL (nuget) sa o toto postara
            );

            Field<ProductType>(
                "product",
                arguments: new QueryArguments(new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id"}), //custom argument pre ziskanie konkretneho zaznamu
                resolve: context =>
                {
                    context.Errors.Add(new ExecutionError("Error message"));
                    var id = context.GetArgument<int>("id");
                    return productRepository.GetOne(id);
                }
            );

            Field<ListGraphType<ProductReviewType>>(
                "reviews",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "productId" }
                ),
                resolve: context =>
                {
                    var id = context.GetArgument<int>("productId");
                    return reviewRepository.GetForProduct(id);
                }
            );
        }
    }
}
